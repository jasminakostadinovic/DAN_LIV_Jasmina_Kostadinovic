﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAN_LIV_Jasmina_Kostadinovic.Models
{
    abstract class MotorVehicle
    {
        public double EngineDisplacement  { get; protected set; }
        public int Weight { get; protected set; }
        public string Category { get; protected set; }
        public string EngineType { get; protected set; }
        public string Color { get; protected set; }
        public int EngineNo { get; protected set; }
      
        protected Array colors = Enum.GetValues(typeof(Colors));

        internal void GenerateRandomColor()
        {
            Color = colors.GetValue(Program.random.Next(colors.Length)).ToString();
        }

        internal virtual void Start() { }
        internal virtual void FailedStop() { }
    }
}
