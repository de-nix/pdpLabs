using System;
using System.Collections.Generic;
using System.Threading;

namespace producer_consumer
{


    public class Main
    {
        public static void produce() {
            while (!(vector1.Count == 0))
            {
                int x1 = vector1.Dequeue();
                int x2 = vector2.Dequeue();
                while (product != 0) { Thread.Sleep(100); }
                m.WaitOne();
                product = x1 * x2;
                Console.WriteLine("Producer produced " + product);
                m.ReleaseMutex();

            }
    
        }
        public static void consume() {

            while (!(vector1.Count == 0))
            {
               
                while (product == 0) { Thread.Sleep(100); }
                m.WaitOne();
                vectorialProduct += product;
                Console.WriteLine("Consumer consumed " + product+" in the final result " + vectorialProduct);

                product = 0;

                m.ReleaseMutex();

            }

        }
        public static int product = 0;
        public static int vectorialProduct = 0;
        public static Mutex m = new Mutex();
        static Queue<int> vector1 = new Queue<int>();
        static Queue<int> vector2 = new Queue<int>();
        public static void main() {

            vector1.Enqueue(5);
            vector1.Enqueue(-3);
            vector1.Enqueue(4);
            vector2.Enqueue(2);
            vector2.Enqueue(1);
            vector2.Enqueue(-2);

            Thread producer = new Thread(produce);
            Thread consumer = new Thread(consume);

            producer.Start();
            consumer.Start();

            producer.Join();
            consumer.Join();
            Console.WriteLine(vectorialProduct);
        
        }

    }
}
