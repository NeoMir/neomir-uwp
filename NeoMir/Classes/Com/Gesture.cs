﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NeoMir.Classes.Com
{
    public class Gesture
    {
        public string Name { get; private set; }
        public bool IsConsumed { get; set; }

        public Gesture(string name)
        {
            Name = name;
            IsConsumed = false;
        }

    }
}