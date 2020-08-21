using System;
using System.Threading;
using System.Threading.Tasks;

namespace DAN_LIV_Jasmina_Kostadinovic.Models
{
    class Car : MotorVehicle
    {
        #region Fields
        CancellationTokenSource cancellationToken = new CancellationTokenSource();
        CancellationTokenSource cancellationTokenForSemaphore = new CancellationTokenSource();
        private bool traficLightSignalGreen;
        private int minimumAmountOfFuel = 15;
        #endregion

        #region Constructors
        public Car(string manufacturer) : base()
        {
            Manufacturer = manufacturer;
            GenerateRandomColor();
            TankVolume = Program.random.Next(50, 90);
            RemainingFuel = TankVolume;
            Thread = new Thread(Start);
            Thread.Name = Manufacturer + " " + RegistrationNo;
            FuelConsumption = Program.random.Next(1, 5);
        }
        #endregion

        #region Properties
        public string RegistrationNo { get; protected set; }
        public int DoorsCount { get; protected set; }
        public int TankVolume { get; protected set; }
        public string TransmissionType { get; protected set; }
        public string Manufacturer { get; protected set; }
        public int TrafficLicenseNo { get; protected set; }
        public int RemainingFuel { get; protected set; }
        public int FuelConsumption { get; protected set; }
        public Thread Thread { get; protected set; }
        #endregion

        #region Methods
        private void Refuel()
        {
            Console.WriteLine($"The car {Color} {Thread.Name} has stoped to refuel.");
            Program.semaphore.Wait();
            Console.WriteLine($"The car {Color} {Thread.Name} is refilling...");
            RemainingFuel = TankVolume;
            Console.WriteLine(RemainingFuel + "" + Thread.Name);
            Program.semaphore.Release();
            Console.WriteLine($"The car {Color} {Thread.Name} has left the gas station.");
            Program.autoReset.Set();
        }

        private void SemaphorSimulation()
        {
            if (Program.random.Next(0, 2) == 1)
                traficLightSignalGreen = true;
            while (true)
            {
                Thread.Sleep(2000);
                if (traficLightSignalGreen == true)
                    traficLightSignalGreen = false;
                else
                    traficLightSignalGreen = true;
            }
        }
        private void FuelConsumptionProgress()
        {
            while (RemainingFuel > 0)
            {
                Thread.Sleep(1000);
                RemainingFuel -= FuelConsumption;
                if (IsRanOutOfGasoline())                
                    cancellationToken.Cancel();                
            }
        }

        private bool ShouldRefuel()
        {
            return RemainingFuel < minimumAmountOfFuel;

        }
        internal void Repaint(string color)
        {
            Color = color;
            GenerateRegistrationNo();
        }

        protected void GenerateRegistrationNo()
        {
            RegistrationNo = Program.random.Next(1000000, 10000000).ToString();
        }

        internal override async void Start()
        {
            await StartRace();
            Program.countdown.Signal();
        }
        private async Task StartRace()
        {
            try
            {
                Console.WriteLine($"The car {Color} {Thread.Name} has start the car race.");

                //fuel consumption simulation
                var fuelConsumption = Task.Run(() => FuelConsumptionProgress(), cancellationToken.Token);
                //semaphore simulation
                var senaphore = Task.Run(() => SemaphorSimulation(), cancellationToken.Token);

                //the first section of the race
                await Task.Delay(10000, cancellationToken.Token);

                //checking if the car has run out of the fuel
                if (!cancellationToken.IsCancellationRequested)
                {
                    
                    if (!traficLightSignalGreen)
                    {
                        Console.WriteLine("The traffic light is red.");
                        Thread.Sleep(2000);
                    }
                    Console.WriteLine($"The car {Color} {Thread.Name} has passed the trafic light.");

                    //we dont need semaphore anymore
                    cancellationTokenForSemaphore.Cancel();

                    //the section before gas station
                    await Task.Delay(3000, cancellationToken.Token);
                
                    //checking if the car has run out of the fuel
                    if (!cancellationToken.IsCancellationRequested)
                    {
                        if (ShouldRefuel())
                            await Task.Run(() => Refuel());

                        //the last section of the race
                        await Task.Delay(7000, cancellationToken.Token);

                        if (!cancellationToken.IsCancellationRequested)
                        {
                            await Task.Run(() => SuccesfullyStop());
                            return;
                        }                          
                    }
                }
                await Task.Run(() => FailedStop());              
            }
            catch (TaskCanceledException)
            {
                //ignoring the TaskCanceledException               
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }

        }

        private async void SuccesfullyStop()
        {
            cancellationToken.Cancel();
            Console.WriteLine($"The car {Color} {Thread.Name} has successfully finished the car race.");
            await Task.Run(() => GetRankingOfTheRedCar());
        }

        private void GetRankingOfTheRedCar()
        {
            if (Color == Colors.Red.ToString())
            {
                lock (Program.locker)
                {
                    if (Program.counter == 0)
                        Program.firstPosition = $"{Color} {Thread.Name}";
                    else
                        Program.secondPosition = $"{Color} {Thread.Name}";
                    Program.counter++;
                }
            }
        }
        internal async override void FailedStop()
        {
          Console.WriteLine($"The car {Color} {Thread.Name} ran out of gasoline and has been disqualified from the race.");           
        }

        protected bool IsRanOutOfGasoline()
        {
            return RemainingFuel <= 0;
        }
        #endregion
    }
}
