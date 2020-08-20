﻿using DAN_LIV_Jasmina_Kostadinovic.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace DAN_LIV_Jasmina_Kostadinovic
{
  
    class Program
    {
        public static CountdownEvent countdown = new CountdownEvent(3);
        public static object locker = new object();
        public static SemaphoreSlim semaphore = new SemaphoreSlim(1);
        public static AutoResetEvent autoReset = new AutoResetEvent(false);
        public static Stopwatch timerBeforeTraficLight = new Stopwatch();
        public static Stopwatch timerAfterTraficLight = new Stopwatch();
        public static int counter;
        public static string firstPosition;
        public static string secondPosition;
        static void Main(string[] args)
        {
            var car1 = new Car("BMW");
            var car2 = new Car("Audi");
            var car3 = new Car("Golf");
            car3.Repaint(Colors.Orange.ToString());

            car1.Repaint(Colors.Red.ToString());
            car2.Repaint(Colors.Red.ToString());

            var cars = new List<Car>(3);
            cars.Add(car1);
            cars.Add(car2);
            cars.Add(car3);
            for (int i = 0; i < cars.Count; i++)
            {
                cars[i].Thread.Start();
            }
            countdown.Wait();
            Console.WriteLine("The car race is over.");
            if (firstPosition != null)
                Console.WriteLine($"The winner is: {firstPosition}");
            if (secondPosition != null)
                Console.WriteLine($"The second position: {secondPosition}");

            Console.ReadLine();
        }
    }
}
