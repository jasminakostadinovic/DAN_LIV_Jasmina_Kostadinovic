using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAN_LIV_Jasmina_Kostadinovic.Models
{
    class Truck : MotorVehicle
    {
        public double loadCapacity { get; protected set; }
        public double Height { get; protected set; }
        public int SeatsCount { get; protected set; }

        internal void Load() { }
        internal void Unload() { }
    }
}
