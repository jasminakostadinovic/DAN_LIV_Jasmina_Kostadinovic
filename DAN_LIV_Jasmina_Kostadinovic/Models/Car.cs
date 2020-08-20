using System;
using System.ComponentModel;
using System.Threading;
using System.Threading.Tasks;

namespace DAN_LIV_Jasmina_Kostadinovic.Models
{
    class Car : MotorVehicle
    {
        CancellationTokenSource cancellationToken = new CancellationTokenSource();
        CancellationTokenSource cancellationTokenForSemaphore = new CancellationTokenSource();
        public Car(string manufacturer) : base()
        {
            Manufacturer = manufacturer;
            GenerateRandomColor();
            TankVolume = random.Next(40, 45);
            RemainingGasoline = TankVolume;
            Thread = new Thread(Start);
            Thread.Name = Manufacturer + " " + RegistrationNo;
            workerGasolineConsumption.DoWork += GasolineConsumptionProgress;
            workerSemaphore.DoWork += SemaphorSimulation;
            //workerRefuel.DoWork += Refuel;
            GasolineConsumption = random.Next(35, 40);
        }

        private void Refuel()
        {
            Console.WriteLine($"The car {Color} {Thread.Name} has stoped to refuel.");
            Program.semaphore.Wait();
            Console.WriteLine($"The car {Color} {Thread.Name} is refilling...");
            Program.semaphore.Release();
            Console.WriteLine($"The car {Color} {Thread.Name} has left the gas station.");
            Program.autoReset.Set();

        }

        private void SemaphorSimulation(object sender, DoWorkEventArgs e)
        {
            while (!cancellationTokenForSemaphore.IsCancellationRequested)
            {
                Thread.Sleep(2000);
                if (TraficLightSignalGreen == true)
                    TraficLightSignalGreen = false;
                else
                    TraficLightSignalGreen = true;
            }
        }



        private void GasolineConsumptionProgress(object sender, DoWorkEventArgs e)
        {
            while (RemainingGasoline > 0 || !cancellationToken.IsCancellationRequested)
            {
                Thread.Sleep(1000);
                RemainingGasoline -= GasolineConsumption;
                if (IsRanOutOfGasoline())
                {
                    cancellationToken.Cancel();
                    Stop();
                }

            }
        }

        private bool ShouldRefuel()
        {
            return RemainingGasoline < 15;

        }

        public string RegistrationNo { get; protected set; }
        public int DoorsCount { get; protected set; }
        public int TankVolume { get; protected set; }
        public string TransmissionType { get; protected set; }
        public string Manufacturer { get; protected set; }
        public int TrafficLicenseNo { get; protected set; }

        public int RemainingGasoline { get; protected set; }
        public int GasolineConsumption { get; protected set; }
        private bool TraficLightSignalGreen;

        public Thread Thread { get; protected set; }
        private BackgroundWorker workerGasolineConsumption = new BackgroundWorker();
        private BackgroundWorker workerSemaphore = new BackgroundWorker();


        internal void Repaint(string color)
        {
            Color = color;
            GenerateRegistrationNo();
        }

        protected void GenerateRegistrationNo()
        {
            RegistrationNo = random.Next(1000000, 10000000).ToString();
        }


        internal async override void Start()
        {
            try
            {
                Console.WriteLine($"The car {Color} {Thread.Name} has start the car race.");

                workerGasolineConsumption.RunWorkerAsync();
                workerSemaphore.RunWorkerAsync();
                await Task.Delay(10000, cancellationToken.Token);

                if (!cancellationToken.IsCancellationRequested)
                {
                    if (!TraficLightSignalGreen)
                        Thread.Sleep(2000);
                    Console.WriteLine($"The car {Color} {Thread.Name} has passed the trafic light.");
                    cancellationTokenForSemaphore.Cancel();
                    await Task.Delay(3000, cancellationToken.Token);

                    if (!cancellationToken.IsCancellationRequested)
                    {
                        if (ShouldRefuel())
                            await Task.Run(() => Refuel());

                        await Task.Delay(7000, cancellationToken.Token);
                    }
                }
                Stop();
                Program.countdown.Signal();
            }
            catch (TaskCanceledException)
            {

            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }

        }

        private void GetPlace()
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
        internal async override void Stop()
        {
            if (cancellationToken.IsCancellationRequested)
            {
                Console.WriteLine($"The car {Color} {Thread.Name} ran out of gasoline and has been disqualified from the race.");
                return;
            }
            cancellationToken.Cancel();
            Console.WriteLine($"The car {Color} {Thread.Name} has finished the car race.");
            await Task.Run(() => GetPlace());
        }

        protected bool IsRanOutOfGasoline()
        {
            return RemainingGasoline <= 0;
        }
    }
}
