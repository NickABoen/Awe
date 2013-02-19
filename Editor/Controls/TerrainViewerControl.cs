#region File Description
//-----------------------------------------------------------------------------
// ModelViewerControl.cs
//
// Microsoft XNA Community Game Platform
// Copyright (C) Microsoft Corporation. All rights reserved.
//-----------------------------------------------------------------------------
#endregion

#region Using Statements
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Windows.Forms;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
#endregion

namespace AweEditor
{
    class Instance
    {
        float size;
        Matrix transform;

        public Matrix Transform { get { return transform; } }

        public Instance()
        {
            Random random = new Random();

            size = 0.5f;

            Matrix scale, movement;

            Matrix.CreateScale(size, out scale);
            Matrix.CreateTranslation(random.Next(100), random.Next(100), random.Next(100), out movement);

            Matrix.Multiply(ref scale, ref movement, out transform);
        }
    }

    /// <summary>
    /// Example control inherits from GraphicsDeviceControl, and displays
    /// a voxel terrain. The main form class is responsible for loading
    /// the terrain: this control just displays it.
    /// </summary>
    class TerrainViewerControl : GraphicsDeviceControl
    {
        /// <summary>
        /// Gets or sets the current voxel terrain.
        /// </summary>
        public VoxelTerrain VoxelTerrain
        {
            get { return voxelTerrain; }
            set { voxelTerrain = value; }
        }

        List<Model> modelList;
        List<Vector3> transformList;

        List<Instance> instances;
        Matrix[] instanceTransforms;
        Model instanceModel;
        Matrix[] instanceModelBones;
        DynamicVertexBuffer instanceVertexBuffer;

        static VertexDeclaration instanceVertexDeclaration = new VertexDeclaration
        (
            new VertexElement(0, VertexElementFormat.Vector4, VertexElementUsage.BlendWeight, 0),
            new VertexElement(16, VertexElementFormat.Vector4, VertexElementUsage.BlendWeight, 1),
            new VertexElement(32, VertexElementFormat.Vector4, VertexElementUsage.BlendWeight, 2),
            new VertexElement(48, VertexElementFormat.Vector4, VertexElementUsage.BlendWeight, 3)
        );

        public Model Model
        {
            get
            {
                return instanceModel;
            }

            set
            {
                instanceModel = value;
                if (instanceModel != null)
                {
                    MeasureModel();
                    eyePosition = modelCenter;
                    MakeModel();

                    if (instances == null)
                    {
                        instances = new List<Instance>();

                        for (int i = 0; i < 10; i++)
                            instances.Add(new Instance());
                    }

                    if (instanceModelBones == null)
                    {
                        instanceModelBones = new Matrix[instanceModel.Bones.Count];
                        instanceModel.CopyAbsoluteBoneTransformsTo(instanceModelBones);
                    }
                }
            }
        }

        VoxelTerrain voxelTerrain;

        Stopwatch timer;


        Matrix[] boneTransforms;
        Vector3 modelCenter;
        float modelRadius;

        /// <summary>
        /// Initializes the control.
        /// </summary>
        protected override void Initialize()
        {
            timer = Stopwatch.StartNew();

            // Hook the idle event to constantly redraw our animation.
            Application.Idle += delegate { Invalidate(); };
        }

        Vector3 eyePosition;

        /// <summary>
        /// Draws the control.
        /// </summary>
        protected override void Draw()
        {
            // Clear to the default control background color.
            Color backColor = new Color(BackColor.R, BackColor.G, BackColor.B);

            GraphicsDevice.Clear(backColor);

            if (voxelTerrain != null)
            {
                if (instanceModel != null)
                {
                    // Compute camera matrices.
                    float rotation = (float)timer.Elapsed.TotalSeconds;
                    //rotation = 0;
                    eyePosition.Z = 10;// 10;
                    eyePosition.Y = 10;// 5;
                    eyePosition.X = 10;
                    //Debug.WriteLine(String.Format("eyeposition = ({0},{1})", eyePosition.Z, eyePosition.Y));

                    float aspectRatio = GraphicsDevice.Viewport.AspectRatio;

                    float nearClip = modelRadius / 100;
                    float farClip = modelRadius * 100;

                    Matrix world = Matrix.CreateRotationY(0);
                    Matrix view = Matrix.CreateLookAt(eyePosition, new Vector3 (0,0,-10), Vector3.Up);
                    Matrix projection = Matrix.CreatePerspectiveFieldOfView(1, aspectRatio,
                                                                        nearClip, farClip);
                    
                    // Draw the model.

                    int x = 0;
                    foreach (Model model in modelList)
                    {
                        foreach (ModelMesh mesh in model.Meshes)
                        {
                            foreach (BasicEffect effect in mesh.Effects)
                            {
                                Matrix test = Matrix.CreateTranslation(transformList[x]);
                                Matrix temp, rotMat, finished;

                                Matrix.Multiply(ref test, ref boneTransforms[mesh.ParentBone.Index], out temp);
                                Matrix.CreateRotationY(rotation, out rotMat);
                                Matrix.Multiply(ref temp, ref rotMat, out finished);

                                effect.World = finished;
                                //effect.World = boneTransforms[mesh.ParentBone.Index] * world;
                                effect.View = view;
                                effect.Projection = projection;

                                effect.EnableDefaultLighting();
                                effect.PreferPerPixelLighting = true;
                                effect.SpecularPower = 16;
                            }

                            mesh.Draw();
                        }
                        x++;
                    }
                    
                    /*Matrix view = Matrix.CreateLookAt(new Vector3(0, 0, 15), Vector3.Zero, Vector3.Up);
                    Matrix projection = Matrix.CreatePerspectiveFieldOfView(MathHelper.PiOver4, GraphicsDevice.Viewport.AspectRatio, 1, 100);

                    GraphicsDevice.BlendState = BlendState.Opaque;
                    GraphicsDevice.DepthStencilState = DepthStencilState.Default;

                    Array.Resize(ref instanceTransforms, instances.Count);

                    for (int i = 0; i < instances.Count; i++)
                        instanceTransforms[i] = instances[i].Transform;

                    DrawModelHardwareInstancing(instanceModel, instanceModelBones, instanceTransforms, view, projection);*/
                }
            }
        }

        private void DrawModelHardwareInstancing(Model model, Matrix[] modelBones, Matrix[] instances, Matrix view, Matrix projection)
        {
            if (instances.Length == 0)
                return;

            if ((instanceVertexBuffer == null) || (instances.Length > instanceVertexBuffer.VertexCount))
            {
                if (instanceVertexBuffer != null)
                    instanceVertexBuffer.Dispose();

                instanceVertexBuffer = new DynamicVertexBuffer(GraphicsDevice, instanceVertexDeclaration, instances.Length, BufferUsage.WriteOnly);
            }

            instanceVertexBuffer.SetData(instances, 0, instances.Length, SetDataOptions.Discard);

            foreach (ModelMesh mesh in model.Meshes)
            {
                foreach (ModelMeshPart meshPart in mesh.MeshParts)
                {
                    GraphicsDevice.SetVertexBuffers(
                        new VertexBufferBinding(meshPart.VertexBuffer, meshPart.VertexOffset, 0),
                        new VertexBufferBinding(instanceVertexBuffer, 0, 1)
                    );

                    Effect effect = meshPart.Effect;

                    effect.CurrentTechnique = effect.Techniques["HardwareInstancing"];

                    effect.Parameters["World"].SetValue(modelBones[mesh.ParentBone.Index]);
                    effect.Parameters["View"].SetValue(view);
                    effect.Parameters["Projection"].SetValue(projection);

                    foreach (EffectPass pass in effect.CurrentTechnique.Passes)
                    {
                        pass.Apply();

                        GraphicsDevice.DrawInstancedPrimitives(PrimitiveType.TriangleList, 0, 0,
                            meshPart.NumVertices, meshPart.StartIndex, meshPart.PrimitiveCount, instances.Length);
                    }
                }
            }
        }

        void MeasureModel()
        {
            // Look up the absolute bone transforms for this model.
            boneTransforms = new Matrix[instanceModel.Bones.Count];

            instanceModel.CopyAbsoluteBoneTransformsTo(boneTransforms);

            // Compute an (approximate) model center position by
            // averaging the center of each mesh bounding sphere.
            modelCenter = Vector3.Zero;

            foreach (ModelMesh mesh in instanceModel.Meshes)
            {
                BoundingSphere meshBounds = mesh.BoundingSphere;
                Matrix transform = boneTransforms[mesh.ParentBone.Index];
                Vector3 meshCenter = Vector3.Transform(meshBounds.Center, transform);

                modelCenter += meshCenter;
            }

            modelCenter /= instanceModel.Meshes.Count;

            // Now we know the center point, we can compute the model radius
            // by examining the radius of each mesh bounding sphere.
            modelRadius = 0;

            foreach (ModelMesh mesh in instanceModel.Meshes)
            {
                BoundingSphere meshBounds = mesh.BoundingSphere;
                Matrix transform = boneTransforms[mesh.ParentBone.Index];
                Vector3 meshCenter = Vector3.Transform(meshBounds.Center, transform);

                float transformScale = transform.Forward.Length();

                float meshRadius = (meshCenter - modelCenter).Length() +
                                   (meshBounds.Radius * transformScale);

                modelRadius = Math.Max(modelRadius, meshRadius);
            }
        }

        void MakeModel()
        {
            transformList = voxelTerrain.GetTransformations();

            Matrix transform = Matrix.CreateTranslation(-8, -64, -8);

            for (int x = 0; x < transformList.Count; x++)
            {
                transformList[x] = Vector3.Transform(transformList[x], transform);
            }

            modelList = new List<Model>();

            for (int i = 0; i < transformList.Count; i++)
            {
                modelList.Add(instanceModel);
            }

        }
    }
}
