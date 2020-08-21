namespace DAN_LIV_Jasmina_Kostadinovic.Models
{
    class Tractor : MotorVehicle
    {
        public Tractor(int engineNo)
        {
            EngineNo = engineNo;
        }

        public double WheelSize { get; set; }
        public int Wheelbase { get; set; }
        public string PoweredBy { get; set; }

    }
}
