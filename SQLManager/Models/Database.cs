﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SQLManager.Models
{
    class Database
    {
        public Database()
        {

        }
        public Database(string name)
        {
            Name = name;
        }

        public string Name { get; set; }
        public override string ToString() => Name;
    }
}
