using System;

namespace DAN_LIV_Jasmina_Kostadinovic.Models
{
    abstract class MotorVehicle
    {
        protected Array colors = Enum.GetValues(typeof(Colors));
        #region Constructors
        protected MotorVehicle(double engineDisplacement, int weight, string category, string engineType, string color, int engineNo, Array colors)
        {
            EngineDisplacement = engineDisplacement;
            Weight = weight;
            Category = category;
            EngineType = engineType;
            Color = color;
            EngineNo = engineNo;
            this.colors = colors;
        }

        protected MotorVehicle()
        {
        }
        #endregion

        #region Properties
        public double EngineDisplacement { get; protected set; }
        public int Weight { get; protected set; }
        public string Category { get; protected set; }
        public string EngineType { get; protected set; }
        public string Color { get; protected set; }
        public int EngineNo { get; protected set; }
        #endregion

        #region Methods
        internal void GenerateRandomColor()
        {
            Color = colors.GetValue(Program.random.Next(colors.Length)).ToString();
            EngineNo = Program.random.Next(10000, 100000);
        }

        internal virtual void Start() { }
        internal virtual void Stop() { }
        #endregion   
    }
}
