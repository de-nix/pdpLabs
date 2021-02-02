using System;
using System.Threading;
using System.Threading.Tasks;
using MPI;

namespace lab8_DSM
{
    class MainProgram
    {
        static void Main(string[] args)
        {
            using (new MPI.Environment(ref args))
            {
                DSM dsm = new DSM();

                if (Communicator.world.Rank == 0)
                {

                    Thread thread = new Thread(listener);
                    thread.Start(dsm); 

                    bool exit = false;
                    dsm.subscribe("a"); 
                    dsm.subscribe("b"); 

                    while (!exit)
                    {
                        Console.WriteLine("1. Set value");
                        Console.WriteLine("2. Update value");
                        Console.WriteLine("0. Exit");
                  
                        displayDSM(dsm);
                        int answer;
                        int.TryParse(Console.ReadLine(), out answer);

                        if (answer == 0)
                        {
                            dsm.close(); 
                            exit = true;
                        }else if (answer == 1)
                        {
                            Console.WriteLine(" Enter variable to be set = ");
                            string var = Console.ReadLine();

                            Console.WriteLine("Enter value = ");
                            int val;
                            int.TryParse(Console.ReadLine(), out val);

                            dsm.update(var, val);
                           
                        }else if (answer == 2)
                        {
                            Console.WriteLine("Enter variable to be updated = ");
                            string var = Console.ReadLine();

                            Console.WriteLine("Enter old value = ");
                            int val;
                            int.TryParse(Console.ReadLine(), out val);

                            Console.WriteLine("Enter new value = ");
                            int newVal;
                            int.TryParse(Console.ReadLine(), out newVal);
                            dsm.replace(var, val, newVal); 
                        }
                    }
                }
                else if (Communicator.world.Rank == 1)
                {
                    Thread thread = new Thread(listener);
                    thread.Start(dsm);
                    dsm.subscribe("a");
                    thread.Join();
                }
                else if (Communicator.world.Rank == 2)
                {
                    Thread thread = new Thread(listener);
                    thread.Start(dsm);
                    dsm.subscribe("b");
                    thread.Join(); 
                }
            }
        }
        static void listener(Object obj)
        {
            DSM dsm = (DSM)obj;

            while (true)
            {
                Msg msg = Communicator.world.Receive<Msg>(Communicator.anySource, Communicator.anyTag);
                if (msg.exit) break;
                if (msg.type == "update")
                {
                    dsm.setVariable(msg.var, msg.rankVal);
                }

                if (msg.type == "subscribe")
                {
                    dsm.forwardSubscribe(msg.var, msg.rankVal);
                }

            }
        }
        static void displayDSM(DSM dsm)
        {
            Console.Write(" a= " + dsm.a + " b= " + dsm.b + "\n");
            foreach (string var in dsm.subscribers.Keys)
            {
                Console.Write(var + " [ ");
                foreach (int rank in dsm.subscribers[var])
                {
                    Console.Write(rank + " ");
                }
                Console.Write("] ");
            }
            Console.WriteLine();
        }
    }
}
