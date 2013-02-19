﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AweEditor
{
    public class TAG_List : Tag
    {
        List<Tag> data { get; set; }

        TagType type { get; set; }

        public TAG_List(string _name, int _type)
        {
            this.data = new List<Tag>();
            this.name = _name;
            this.type = (TagType)_type;
            this.tagType = TagType.TAG_List;
        }

        public void AddItem(Tag item)
        {
            data.Add(item);
        }
    }
}