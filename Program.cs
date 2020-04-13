using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Threading;

namespace Parallel_processing
{
    class Program
    {
        static void Main(string[] args)
        {
            Stopwatch time_counter = new Stopwatch();
            Random rnd = new Random();
            List<Thread> threads = new List<Thread>();
            EventWaitHandle wait_handle = new EventWaitHandle(false, EventResetMode.ManualReset);

            int matrix_size = 300;
            int repeat = 100;
            long time = 0;
            string timeToFile = "";
            
            double[,] addendum = new double[matrix_size, matrix_size];
            double[,] _addendum = new double[matrix_size, matrix_size];
            double[,] amount = new double[matrix_size, matrix_size];

            for (int i = 0; i < matrix_size; i++)
            {
                for (int j = 0; j < matrix_size; j++)
                {
                    addendum[i, j] = rnd.NextDouble() * 10;
                    _addendum[i, j] = rnd.NextDouble() * 10;
                }
            }

            Console.Write("Threads count: ");
            int.TryParse(Console.ReadLine(), out int threads_count);

            
            for (int k = 0; k < repeat; k++)
            {
                amount = new double[matrix_size, matrix_size];
                for (int t = 0; t < threads_count; t++)
                {
                    Thread thread = new Thread(() => 
                    {
                        wait_handle.WaitOne();
                        for (int i = t; i < matrix_size; i += threads_count)
                        {
                            for (int j = 0; j < matrix_size; j++)
                            {
                                for (int l = 0; l < matrix_size; l++)
                                {
                                    amount[i, j] += addendum[i, l] * _addendum[l, j];
                                }
                            }
                        }
                    });
                    threads.Add(thread);
                    thread.Start();
                }

                time_counter.Restart();
                wait_handle.Set();
                for (int i = 0; i < threads_count; i++)
                {
                    threads[i].Join();
                }
                wait_handle.Reset();
                time_counter.Stop();
                time += time_counter.ElapsedMilliseconds;
                timeToFile += (time_counter.ElapsedMilliseconds / 1000.0).ToString() + Environment.NewLine;
            }
            Console.WriteLine($"threads: {threads_count}{Environment.NewLine}time: {time / repeat}");
            File.WriteAllText("results.txt", timeToFile);
            Process.Start("notepad", "results.txt");
            Console.ReadKey();
        }
    }
}
