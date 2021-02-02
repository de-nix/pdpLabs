using System;
using MPI;
using System.Threading;
using System.Threading.Tasks;
using System.Linq;
using Environment = MPI.Environment;
using System.Collections.Generic;

namespace mpiTest
{
    class Program
    {


        //Write a parallellized and distributed version of the QuickSort algorithm using MPI.

        //Write a parallel algorithm that computes the product of two big numbers.

        //Write a parallel or distributed program that counts the number of subsets of k out of N that satisfy a given property. You have a function (bool pred(vector <int> const& v)) that verifies if a given subset satisfies the property. Your program shall call that function once for each subset of k elements and count the number of times it returns true.

        //Write a parallel program that computes the prime numbers up to N. it is assumed to have the list of pimes up to sqrt(N), and will check each of the other numbers if it is divisible with a number from the initial list


        static List<int> primes1(int maxN, int nrProcs, List<int> primesTo) {


            int begin = primesTo.Last() + 1;
            if (nrProcs == 1) {
                nrProcs = 2;
            }
            int step = (maxN - begin) / (nrProcs - 1);
            int end = begin + step;
            if (end > maxN) end = maxN;
            List<int> result = new List<int>();
            result.AddRange(primesTo);

            for (int rank = 1; rank < nrProcs; rank++)
            {
                Communicator.world.Send<List<int>>(primesTo, rank, 0);
                Communicator.world.Send<int>( begin,rank, 1);
                Communicator.world.Send<int>( end-1,rank, 2);
                begin = end;
                end += step;
                if (end > maxN) end = maxN;
            }
            for (int rank = 1;rank<nrProcs;rank++) {
                var x = Communicator.world.Receive<List<int>>(rank, 0);
                result.AddRange(x);
            }
            return result;
        }

        static void worker1(int myId, int nrProcs)
        {
            List<int> primesTo = Communicator.world.Receive<List<int>>(0, 0);
            int begin = Communicator.world.Receive<int>(0, 1);
            int end = Communicator.world.Receive<int>(0, 2);
            List<int> result = new List<int>();
            bool isPrime = true;
            for (int i = begin; i <= end; i++) {
                isPrime = true;
                foreach (int prime in primesTo) {
                    if (i % prime == 0) { isPrime = false; break; };
                }
                if (isPrime)
                    result.Add(i);
            }
            Communicator.world.Send(result,0,0);

        }


        static void sortArray(List<int> numbers, int processors) {

            int begin = 0;
            int step = (numbers.Count + Communicator.world.Size - 2) / (Communicator.world.Size - 1);
            int end = begin+step;
            if (end > numbers.Count) end = numbers.Count;
            for(int j=1;j<processors;j++) {
                List<int> temp = new List<int>();
                for (int i = begin; i < end; i++) {
                    temp.Add(numbers[i]);
                }
                Communicator.world.Send(temp, j, 0);
                begin = end;
                end = begin + step;
                if (end > numbers.Count) end = numbers.Count;
            }

           
            var result = Communicator.world.Receive<List<int>>(1, 0);
            for (int j = 2; j < processors; j++)
            {
                var sortedList2 = Communicator.world.Receive<List<int>>( j, 0);
                result = merge(result, sortedList2);
            }

            foreach (var x in result) {
                Console.WriteLine(x);
            }


        }

        private static void sort()
        {
            List<int> tempList = Communicator.world.Receive<List<int>>(0, 0);
            tempList.Sort();

            Console.WriteLine(Communicator.world.Rank + ":" + "raahdvjkabfehwuefgk/n");
            Communicator.world.Send(tempList, 0, 0);

        }


        private static List<int> merge(List<int> result, List<int> sortedList2)
        {
            List<int> final = new List<int>();
            int x1 = 0;
            int x2 = 0;
            while (x1 < result.Count && x2 < sortedList2.Count) {

                if (result[x1] < sortedList2[x2])
                {
                    final.Add(result[x1]);
                    x1++;
                }
                else {
                    final.Add(sortedList2[x2]);
                    x2++;
                }
            }

            while (x1 < result.Count) {
                final.Add(result[x1]);
                x1++;
            }
            while (x2 < sortedList2.Count)
            {
                final.Add(sortedList2[x2]);
                x2++;
            }
            return final;
        }

        static void Main(string[] args)
        {
            using (new Environment(ref args))
            {
                var comm = Communicator.world;

                if (comm.Rank == 0)
                {
                    primes(new List<int> { 1, 2, 3,4 }, new List<int> { 1, 2, 3 ,4}, comm.Size);
                }
                else
                {
                    worker(comm.Rank,comm.Size);

                }
            }
        }

        private static void combinationsWorker()
        {
            int v1 = Communicator.world.Receive<int>(0, 1);
            int v2 = Communicator.world.Receive<int>(0, 2);
           
            List<int> temp = Communicator.world.Receive<List<int>>(0, 0);
            List<List<int>> result = new List<List<int>>();
            List<int> comb = new List<int>();

            for(int jj =0; jj < temp.Count; jj++) {

                Console.WriteLine(temp[jj] + " --=-- " + Communicator.world.Rank + "\n");
                int num = temp[jj];
                comb = new List<int>();
                comb.Add(num);
                int tempNum = num;
                while (comb.Count != v2 && comb.Count!=0 && tempNum != v1) {
                    comb.Add(++tempNum);

                  
                    if (comb.Count == v2) {
                        var t = new List<int>();
                        t.AddRange(comb);
                        result.Add(t);
                        int ooldV = comb.Last();
                        comb.RemoveAt(comb.Count - 1);
                        int oldV = comb.Last();
                        if (ooldV == v1 && comb.Count > 0)
                        {
                            comb.RemoveAt(comb.Count - 1);
                            comb.Add(++oldV);
                        }
                        int plus = 0;
                        if (tempNum == v1)
                        {
                            while (comb.Count > 1 && comb.Last() == v1-plus )
                            {
                                plus++;
                                comb.RemoveAt(comb.Count - 1);
                                if (comb.Count > 0)
                                {
                                    comb.RemoveAt(comb.Count - 1);
                                    comb.Add(++oldV);
                                }
                            }
               
                            tempNum = comb.Last();
                        }
                       
                    }
                    if (comb.Count == 1) break;

                }

                foreach (var i in result)
                {
                    String x = " { ";
                    foreach (var II in i)
                    {
                        x += II +", " ;
                    }
                    Console.WriteLine(x+ " } " + Communicator.world.Rank +"\n");
                }
                Communicator.world.Send(result, 0, 0);
            
            }
        }

        private static void combinationsMByK(int v1, int v2, int size)
        {

            int begin = 1;
            int v = v1 - v2 + 1;
            int step = (v + size - 2) / (size - 1);
            int end = step + begin;
            if (end > v) end = v;

            for (int rank = 1; rank < size; rank++) {
                List<int> temp = new List<int>();

                Console.WriteLine("begin:" + begin + ", and end : " + end + "\n");

                if (begin == end) {
                    temp.Add(begin);
                    Communicator.world.Send(v1, rank, 1);
                    Communicator.world.Send(v2, rank, 2);
                    Communicator.world.Send(temp, rank, 0);
                    break;
                }

                for (int y = begin; y < end; y++) {
                    temp.Add(y);
                }
                Communicator.world.Send(v1, rank, 1);
                Communicator.world.Send(v2, rank, 2);
                Communicator.world.Send(temp, rank, 0);
                begin = end;
                end = step + begin;
                if (end > v) end = v;
            }

            List<List<int>> finalSet = new List<List<int>>();
            for (int rank = 1; rank < size; rank++) {
                if (rank > begin) {
                    break;
                }
                var res = Communicator.world.Receive<List<List<int>>>(rank, 0);
                finalSet.AddRange(res);
            
            }
            foreach (List<int> combination in finalSet) {
                foreach (var number in combination) {
                    Console.WriteLine(number);
                }
                Console.WriteLine("\n");
            }
          
        }

        private static List<int> primes(List<int> a, List<int> b, int nrProcs) {
            List<int> result = new List<int>();
            int begin = 0;
            int totSize = a.Count + b.Count -1;
            int step = (totSize+ nrProcs - 2) / (nrProcs-1);
            int end = 0;
            for (int i = 1; i < nrProcs; i++) {

                end = begin + step;
                if (end >= totSize) end = totSize;

                Console.WriteLine("begin: " + begin + " end: " + end);
                if (begin == end) {
                    Communicator.world.Send(begin, i, 1);
                    Communicator.world.Send(end+1, i, 2);
                    Communicator.world.Send(a, i, 3);
                    Communicator.world.Send(b, i, 4);
                    break;


                }
                Communicator.world.Send(begin, i, 1);
                Communicator.world.Send(end, i, 2);
                Communicator.world.Send(a, i, 3);
                Communicator.world.Send(b, i, 4);

                begin = end;

            }
            for (int i = 1; i < nrProcs; i++)
            {

                var temp = Communicator.world.Receive<List<int>>( i, 0);
                result.AddRange(temp);
                if (result.Count == totSize) break;

            }

            foreach (var t in result) {
                Console.WriteLine(t);
            }
            return result;

        }

        static void worker(int myId, int nrProcs) {
            int beginK = Communicator.world.Receive<int>(0, 1);
            int endK = Communicator.world.Receive<int>(0, 2);
            List<int> aVector = Communicator.world.Receive<List<int>>(0, 3);
            List<int> bVector = Communicator.world.Receive<List<int>>(0, 4);
            List<int> temp = new List<int>();

            for (int k = beginK; k < endK; k++) {
                int sum = 0;
                for (int i = 0; i < aVector.Count; i++)
                {
                    if (i <= k && k-i <aVector.Count)
                    {
                        sum += aVector[i] * bVector[k - i];
                    }
                }
                temp.Add(sum);
            }

            String x = "";
            foreach (var c in temp) {
                x += c + " ";
            }
            Console.WriteLine(x + " rank" + myId);

            Communicator.world.Send(temp, 0, 0);

        }

    }
}
