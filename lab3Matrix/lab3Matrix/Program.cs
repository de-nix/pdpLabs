using Microsoft.VisualBasic.CompilerServices;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;
using System.Threading;

namespace lab3Matrix
{
    class Program
    {

        static void computeProduct(int i, int j, int n) {
            int sum = 0;
            for (int k = 0; k < n; k++) {
                sum += matrix1[i][k] * matrix2[k][j];
            }
            matrixResult[i][j] = sum;
        }

        static void tasker1(Object threadIndex) {
            int index = (int)threadIndex;
            int threads = noThreads;
            int startPosition = (size * size / threads) * index;

            int endPosition = 0;
            if (index == threads - 1)
            {
                endPosition = size * size;
            }
            else {
                endPosition = (size * size / threads) * (index + 1);

            }
            for (int i = startPosition; i < endPosition; i++) {

                computeProduct(i / size, i % size, size);
                
            }
        }

        static void tasker2(Object threadIndex)
        {
            int index = (int)threadIndex;
            int threads = noThreads;
            int startPosition = (size * size / threads) * index;
            int endPosition = 0;
            if (index == threads - 1)
            {
                endPosition = size * size;
            }
            else
            {
                endPosition = (size * size / threads) * (index + 1);

            }
            for (int i = startPosition; i < endPosition; i++)
            {

                computeProduct(i % size, i / size, size);
            }
        }

        static void tasker3(Object threadIndex)
        {

            int index = (int)threadIndex;
            int threads = noThreads;
            int startI = index / size;
            int startJ = index / size;
            int j = 0;
            for (int i = startI; i < size; i++)
            {
                for (j = startJ; j < size; j = j + threads)
                {
                    computeProduct(i, j, size);
                }
                startJ = j - size;
            }
        }
        static void printMatrix(List<List<int>> matrix){
            StringBuilder res = new StringBuilder();
            foreach (List<int> row in matrix) {
                foreach (int x in row) {
                    res.Append(" "+ x +" ");
                }
                res.Append("\n");
            }
            Console.WriteLine(res.ToString());
        }

        static List<List<int>> matrix1 = new List<List<int>>();
        static List<List<int>> matrix2 = new List<List<int>>();
        static List<List<int>> matrixResult = new List<List<int>>();
        static int size=0;
        static int noThreads = 0;
        static void Main(string[] args)
        {
            Console.WriteLine("Give the size of the matrix\n");
            int n = int.Parse(Console.ReadLine());
            Random random = new Random();
            for (int i = 0; i < n; i++)
            {
                matrix1.Add(new List<int>());
                matrix2.Add(new List<int>());
                matrixResult.Add(new List<int>());
                for (int j = 0; j < n; j++) {
                    matrix1[i].Add(random.Next(0,15)-7);
                    matrix2[i].Add(random.Next(0, 15)-7);
                    matrixResult[i].Add(0);
                }
            }
            size = n;
            List<Thread> threads = new List<Thread>();
            Console.WriteLine("Give the nb of threads\n");
            noThreads = int.Parse(Console.ReadLine());

            DateTime start,end;
            start = DateTime.Now;
            for (int i = 0; i < noThreads; i++)
            {
                threads.Add(new Thread(tasker1));
            }
            for (int i = 0; i < noThreads; i++)
            {
                threads[i].Start(i);
            }
            for (int i = 0; i < noThreads; i++)
            {
                threads[i].Join();
            }
            end = DateTime.Now;
            Console.WriteLine("threads completed tasker1 in " + (end - start));
            threads.Clear();


            var events1 = new List<ManualResetEvent>();
            start = DateTime.Now;

            for (int i = 0; i < noThreads; i++)
            {
                var resetEvent = new ManualResetEvent(false);
                int a = new int();
                a = i;
                ThreadPool.QueueUserWorkItem(arg => {
                    
                    tasker1(a);
                    resetEvent.Set();
                });
                events1.Add(resetEvent);
            }
            WaitHandle.WaitAll(events1.ToArray());
            end = DateTime.Now;
            Console.WriteLine("thread pool completed tasker1 in " + (end - start));


            start = DateTime.Now;
            for (int i = 0; i < noThreads; i++)
            {
                threads.Add(new Thread(tasker2));
            }
            for (int i = 0; i < noThreads; i++)
            {
                threads[i].Start(i);
            }
            for (int i = 0; i < noThreads; i++)
            {
                threads[i].Join();
            }
            end = DateTime.Now;
            Console.WriteLine("threads completed tasker2 in " + (end - start));
            threads.Clear();

            var events2 = new List<ManualResetEvent>();
            start = DateTime.Now;


            for (int i = 0; i < noThreads; i++)
            {
                var resetEvent = new ManualResetEvent(false);
                int a = new int();
                a = i;
                ThreadPool.QueueUserWorkItem(arg => {

                    tasker2(a);
                    resetEvent.Set();
                });
                events2.Add(resetEvent);
            }
            WaitHandle.WaitAll(events2.ToArray());
            end = DateTime.Now;
            Console.WriteLine("thread pool completed tasker2 in " + (end - start));

            //printMatrix(matrix1);
            //Console.WriteLine("\n");
            //printMatrix(matrix2);
            //Console.WriteLine("\n");

            start = DateTime.Now;
            for (int i = 0; i < noThreads; i++)
            {
                threads.Add(new Thread(tasker3));
            }
            for (int i = 0; i < noThreads; i++)
            {
                threads[i].Start(i);
            }
            for (int i = 0; i < noThreads; i++)
            {
                threads[i].Join();
            }
            end = DateTime.Now;
            Console.WriteLine("threads completed tasker3 in " + (end - start));
            threads.Clear();

            //printMatrix(matrixResult);
            //Console.WriteLine("\n");


            start = DateTime.Now;
            var events3 = new List<ManualResetEvent>();


            for (int i = 0; i < noThreads; i++)
            {
                var resetEvent = new ManualResetEvent(false);
                int a = new int();
                a = i;
                ThreadPool.QueueUserWorkItem(arg => {

                    tasker3(a);
                    resetEvent.Set();
                });
                events3.Add(resetEvent);
            }
            WaitHandle.WaitAll(events3.ToArray());
            end = DateTime.Now;
            Console.WriteLine("thread pool completed tasker1 in " + (end - start));
            //Console.WriteLine(matrixResult);
            //Console.WriteLine("\n");
        }
    }
}
