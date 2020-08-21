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
