using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.IO.Compression;

using System.Windows.Forms;
using System.Net;

using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace AweEditor
{
    /// <summary>
    /// BlockType corresponds to Minecraft Bock IDs for ease of import
    /// (see http://www.minecraftwiki.net/wiki/Data_values#Block_IDs)
    /// </summary>
    public enum BlockType
    {
        #region 159 Block Types

        Air = 0,
        Stone = 1,
        Grass = 2,
        Dirt = 3,
        Cobblestone = 4,
        WoodenPlanks = 5,
        Saplings = 6,
        Bedrock = 7,
        Water = 8,
        Stationary_Water = 9,
        Lava = 10,
        Stationary_Lava = 11,
        Sand = 12,
        Gravel = 13,
        Gold_Ore = 14,
        Iron_Ore = 15,
        Coal_Ore = 16,
        Wood = 17,
        Leaves = 18,
        Sponge = 19,
        Glass = 20,
        Lapis_Lazuli_Ore = 21,
        Lapis_Lazuli_Block = 22,
        Dispenser = 23,
        Sandstone = 24,
        Note_Block = 25,
        Bed = 26,
        Powered_Rail = 27,
        Detector_Rail = 28,
        Sticky_Piston = 29,
        Cobweb = 30,
        Tall_Grass = 31,
        Dead_Bush = 32,
        Piston = 33,
        Piston_Extentsion = 34,
        Wool = 35,
        Block_Moved_By_Piston = 36,
        Dandelion = 37,
        Rose = 38,
        Brown_Mushroom = 39,
        Red_Mushroom = 40,
        Block_Of_Gold = 41,
        Block_Of_Iron = 42,
        Double_Slabs = 43,
        Slabs = 44,
        Bricks = 45,
        TNT = 46,
        Bookshelf = 47,
        Moss_Stone = 48,
        Obsidian = 49,
        Torch = 50,
        Fire = 51,
        Monster_Spawner = 52,
        Oak_Wood_Stairs = 53,
        Chest = 54,
        Redstone_Wire = 55,
        Diamond_Ore = 56,
        Block_Of_Diamond = 57,
        Crafting_Table = 58,
        Wheat_Seeds = 59,
        Farmland = 60,
        Furnace = 61,
        Burning_Furnace = 62,
        Sign_Post = 63,
        Wooden_Door = 64,
        Ladders = 65,
        Rails = 66,
        Cobblestone_Stairs = 67,
        Wall_Sign = 68,
        Lever = 69,
        Stone_Pressure_Plate = 70,
        Iron_Door = 71,
        Wooden_Pressure_Plate = 72,
        Redstone_Ore = 73,
        Glowing_Redstone_Ore = 74,
        Redstone_Torch_Inactive = 75,
        Redstone_Torch_Active = 76,
        Stone_Button = 77,
        Snow = 78,
        Ice = 79,
        Snow_Block = 80,
        Cactus = 81,
        Clay_Block = 82,
        Sugar_Cane = 83,
        Jukebox = 84,
        Fence = 85,
        Pumpkin = 86,
        Netherrack = 87,
        Soul_Sand = 88,
        Glowstone_Block = 89,
        Nether_Portal = 90,
        Jack_O_Lantern = 91,
        Cake_Block = 92,
        Redstone_Repeater_Inactive = 93,
        Redstone_Repeater_Active = 94,
        Locked_Chest = 95,
        Trapdoor = 96,
        Monster_Egg = 97,
        Stone_Bricks = 98,
        Huge_Brown_Mushroom = 99,
        Huge_Red_Mushroom = 100,
        Iron_Bars = 101,
        Glass_Pane = 102,
        Melon = 103,
        Pumpkin_Stem = 104,
        Melon_Stem = 105,
        Vines = 106,
        Fence_Gate = 107,
        Brick_Stairs = 108,
        Stone_Brick_Stairs = 109,
        Mycelium = 110,
        Lily_Pad = 111,
        Nether_Brick = 112,
        Nether_Brick_Fence = 113,
        Nether_Brick_Stairs = 114,
        Nether_Wart = 115,
        Enchantment_Table = 116,
        Brewing_Stand = 117,
        Cauldron = 118,
        End_Portal = 119,
        End_Portal_Frame = 120,
        End_Stone = 121,
        Dragon_Egg = 122,
        Redstone_Lamp_Inactive = 123,
        Redstone_Lamp_Active = 124,
        Wooden_Double_Slab = 125,
        Wooden_Slab = 126,
        Cocoa_Pod = 127,
        Sandstone_Stairs = 128,
        Emerald_Ore = 129,
        Ender_Chest = 130,
        Tripwire_Hook = 131,
        Tripwire = 132,
        Block_Of_Emerald = 133,
        Spruce_Wood_Stairs = 134,
        Birch_Wood_Stairs = 135,
        Jungle_Wood_Stairs = 136,
        Command_Block = 137,
        Beacon = 138,
        Cobblestone_Wall = 139,
        Flower_Pot = 140,
        Carrots = 141,
        Potatoes = 142,
        Wooden_Button = 143,
        Mob_Heads = 144,
        Anvil = 145,
        Trapped_Chest = 146,
        Weighted_Pressure_Plate_Light = 147,
        Weighted_Pressure_Plate_Heavy = 148,
        Redstone_Comparator_Inactive = 149,
        Redstone_Comparator_Active = 150,
        Daylight_Sensor = 151,
        Block_Of_Redstone = 152,
        Nether_Quartz_Ore = 153,
        Hopper = 154,
        Block_Of_Quartz = 155,
        Quartz_Stairs = 156,
        Activator_Rail = 157,
        Dropper = 158

        #endregion
    }

    public class Block
    {
        public Vector3 Position { get; set; }
        public BlockType BlockID { get; set; }

        public Block(int xPos, int yPos, int zPos)
        {
            Position = new Vector3(xPos, yPos, zPos);
            BlockID = BlockType.Air;
        }
    }

    public class Chunk
    {
        public Vector2 Position { get; set; }
        public Block[, ,] Blocks { get; set; }

        public int Ymax { get; set; }

        public Chunk(int xPos, int zPos)
        {
            Position = new Vector2(xPos, zPos);
            Blocks = new Block[256, 16, 16];
            Ymax = 128;
            for (int y = 0; y < 256; y++)
            {
                for (int z = 0; z < 16; z++)
                {
                    for (int x = 0; x < 16; x++)
                    {
                        Blocks[y, z, x] = new Block(x, y, z);
                    }
                }
            }
        }
    }

    /// <summary>
    /// A class to represent voxel terrain
    /// </summary>
    public class VoxelTerrain
    {

        public static Model instancedModel;
        public static Matrix[] instancedModelBones;

        //private Chunk[,] Region;
        private Chunk Region;

        // TODO: Complete class
        public VoxelTerrain()
        {
            //Region = 32x32 chunks
            //Chunk = 16x256x16 blocks

            //Most array structures in minecraft are ordered YZX

            ///TODO: use classes for chunk and block instead of structs

            Region = new Chunk(0, 0);
            /*Region = new Chunk[32, 32];

            for (int xPos = 0; xPos < 32; xPos++)
            {
                for (int zPos = 0; zPos < 32; zPos++)
                {
                    Region[xPos, zPos] = new Chunk(xPos, zPos);
                }
            }*/

        }

        public int ConvertToLocalCoordinates(int chunkCoord, int blockCoord)
        {
            return (16 * (chunkCoord - 1)) + blockCoord;
        }

        public void ConvertChunkToBlocks(int chunkX, int chunkZ, byte[] blockIDs, int yMax)
        {
            //TODO: Add dynamic loading

            //Region[chunkX, chunkZ].Position = new Vector2(chunkX, chunkZ);
            Region.Position = new Vector2(chunkX, chunkZ);

            for (int y = 0; y < Region.Ymax; y++)
            {
                for (int z = 0; z < 16; z++)
                {
                    for (int x = 0; x < 16; x++)
                    {
                        /*
                        Region[chunkX, chunkZ].Blocks[y, z, x].Ymax = yMax;
                        Region[chunkX, chunkZ].Blocks[y, z, x].BlockID = (BlockType)blockIDs[(yMax * y) + (z * 16) + x];
                        Region[chunkX, chunkZ].Blocks[y, z, x].Position = new Vector3(x, y, z);
                        */

                        Region.Blocks[y, z, x].BlockID = (BlockType)blockIDs[(Region.Ymax * y) + (z * 16) + x];
                        Region.Blocks[y, z, x].Position = new Vector3(x, y, z);
                    }
                }
            }

        }

        public List<Vector3> GetTransformations()
        {
            List<Vector3> newList = new List<Vector3>();

            /*foreach (Block b in Region.Blocks)
            {
                if (b.BlockID != BlockType.Air)
                    newList.Add(b.Position);
            }*/

            for (int y = 0; y < Region.Ymax; y++)
            {
                for (int z = 0; z < 16; z++)
                {
                    for (int x = 0; x < 16; x++)
                    {
                        if (Region.Blocks[y, x, z].BlockID != BlockType.Air)
                            newList.Add(Region.Blocks[y, x, z].Position);
                    }
                }
            }


            return newList;
        }

    }
}
