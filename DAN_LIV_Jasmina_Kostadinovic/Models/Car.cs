﻿using System;
using System.ComponentModel;
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
        private BackgroundWorker workerFuelConsumption = new BackgroundWorker();
        private BackgroundWorker workerSemaphore = new BackgroundWorker();
        private int minimumAmountOfFuel = 15;
        #endregion

        #region Constructors
        public Car(string manufacturer) : base()
        {
            Manufacturer = manufacturer;
            GenerateRandomColor();
            TankVolume = random.Next(40, 70);
            RemainingFuel = TankVolume;
            Thread = new Thread(Start);
            Thread.Name = Manufacturer + " " + RegistrationNo;
            workerFuelConsumption.DoWork += FuelConsumptionProgress;
            workerSemaphore.DoWork += SemaphorSimulation;
            FuelConsumption = random.Next(1, 10);
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
            Program.semaphore.Release();
            Console.WriteLine($"The car {Color} {Thread.Name} has left the gas station.");
            Program.autoReset.Set();
        }

        private void SemaphorSimulation(object sender, DoWorkEventArgs e)
        {
            while (!cancellationTokenForSemaphore.IsCancellationRequested)
            {
                Thread.Sleep(2000);
                if (traficLightSignalGreen == true)
                    traficLightSignalGreen = false;
                else
                    traficLightSignalGreen = true;
            }
        }
        private void FuelConsumptionProgress(object sender, DoWorkEventArgs e)
        {
            while (RemainingFuel > 0 || !cancellationToken.IsCancellationRequested)
            {
                Thread.Sleep(1000);
                RemainingFuel -= FuelConsumption;
                if (IsRanOutOfGasoline())
                {
                    cancellationToken.Cancel();
                    Stop();
                }

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
            RegistrationNo = random.Next(1000000, 10000000).ToString();
        }


        internal async override void Start()
        {
            try
            {
                Console.WriteLine($"The car {Color} {Thread.Name} has start the car race.");

                workerFuelConsumption.RunWorkerAsync();
                workerSemaphore.RunWorkerAsync();

                //the first section of the race
                await Task.Delay(10000, cancellationToken.Token);

                //checking if the car has run out of the fuel
                if (!cancellationToken.IsCancellationRequested)
                {
                    if (!traficLightSignalGreen)
                        Thread.Sleep(2000);
                    Console.WriteLine($"The car {Color} {Thread.Name} has passed the trafic light.");
                    cancellationTokenForSemaphore.Cancel();
                    await Task.Delay(3000, cancellationToken.Token);

                    //checking if the car has run out of the fuel
                    if (!cancellationToken.IsCancellationRequested)
                    {
                        if (ShouldRefuel())
                            await Task.Run(() => Refuel());

                        //the last section of the race
                        await Task.Delay(7000, cancellationToken.Token);
                    }
                }
                Stop();
                Program.countdown.Signal();
            }
            catch (TaskCanceledException)
            {
                //ignoring the TaskCanceledException
                Program.countdown.Signal();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }

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
        internal async override void Stop()
        {
            if (cancellationToken.IsCancellationRequested)
            {
                Console.WriteLine($"The car {Color} {Thread.Name} ran out of gasoline and has been disqualified from the race.");
                return;
            }
            cancellationToken.Cancel();
            Console.WriteLine($"The car {Color} {Thread.Name} has successfully finished the car race.");
            await Task.Run(() => GetRankingOfTheRedCar());
        }

        protected bool IsRanOutOfGasoline()
        {
            return RemainingFuel <= 0;
        }
        #endregion
    }
}
