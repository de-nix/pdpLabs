using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using MPI;
using System.Linq;
//using OpenCLTemplate;
using Environment = MPI.Environment;

namespace lab5
{
    [Serializable]
    class Polynomial
    {
        int[] coefficients;
        int capacity;
        public Polynomial(int size, bool empty = false)
        {
            capacity = size;
            coefficients = new int[capacity];
            for (int i = 0; i < size; i++)
            {
                if (empty) coefficients[i]=0;
                else coefficients[i] = new Random().Next(10);
            }
        }
        public String printPolynomial() {
            StringBuilder builder = new StringBuilder();
            for (int i = capacity - 1; i >= 0; i--) {
                if (coefficients[i] != 0) { 
                    builder.Append(" + ").Append(coefficients[i]);
                if(i!=0)builder.Append("X^").Append(i);}
            }
            builder.Append("\n");
            return builder.ToString();
        }
        public static Polynomial seqProduct(Polynomial p1, Polynomial P2)
        {
            Polynomial result = new Polynomial(p1.capacity + P2.capacity-1, true);
            for (int i = 0; i < p1.capacity; i++)
            {
                for (int j = 0; j < P2.capacity; j++)
                {
                    int temp = p1.coefficients[i] * P2.coefficients[j];
                    result.coefficients[i + j] += temp;
                }
            }
            return result;
        }

        public static Polynomial parallelProductImproved(Polynomial p1, Polynomial p2)
        {

            Polynomial result = new Polynomial(p1.capacity + p2.capacity - 1, true);
            Parallel.For(0, p1.capacity, i => {
                for (int j = 0; j < p2.capacity; ++j)
                    Interlocked.Add(ref result.coefficients[i + j], p1.coefficients[i] * p2.coefficients[j]);
            });
            return result;
        }


        public static Polynomial parallelKaratsuba(Polynomial p1, Polynomial p2,int noThreads)
        {
            int minLevel = (int)Math.Log(noThreads, 3);
            int noUsedThreads = (int)Math.Pow(3, minLevel);
            Polynomial[] finals = new Polynomial[noUsedThreads]; 
            Thread[] threads = new Thread[noUsedThreads];

            void actKaratsuba(Polynomial x, Polynomial y, int level,int branchThreads, int offset)
            {

                if (level == minLevel) {
                    threads[offset]= new Thread(() => {finals[offset] = karatsuba(x, y); });
                    return;
                }

                int n = Math.Max(x.capacity, y.capacity);
                double m = Math.Ceiling(1.0 * n / 2.0);
                var x_ = x.split((int)m);
                var y_ = y.split((int)m);
                var xL = x_.Item1;
                var xR = x_.Item2;
                var yL = y_.Item1;
                var yR = y_.Item2;
                actKaratsuba(xR, yR, level + 1,branchThreads/3,offset);
                actKaratsuba(xL, yL, level + 1, branchThreads / 3,offset + branchThreads / 3);
                actKaratsuba(add(xR, xL), add(yR, yL),level+1, branchThreads / 3, offset + 2*branchThreads / 3);

            }

            Polynomial getKaratsuba(Polynomial x, Polynomial y, int level, int branchThreads, int offset)
            {
                if (level == minLevel)
                {
                    return finals[offset];
                }

                if (x.capacity < 3 || y.capacity < 3)
                    return seqProduct(x, y);
                int n = Math.Max(x.capacity, y.capacity);
                double m = Math.Ceiling(1.0 * n / 2.0);
                var x_ = x.split((int)m);
                var y_ = y.split((int)m);
                var xL = x_.Item1;
                var xR = x_.Item2;
                var yL = y_.Item1;
                var yR = y_.Item2;

                var a = getKaratsuba(xR, yR, level + 1, branchThreads / 3, offset);
                var d = getKaratsuba(xL, yL, level + 1, branchThreads / 3, offset + branchThreads / 3);
                var e = subtract(subtract(getKaratsuba(add(xR, xL), add(yR, yL), level + 1, branchThreads / 3, offset + 2 * branchThreads / 3), a), d);

 
                a.power((int)m * 2);
                e.power((int)m);
                return add(add(a, e), d);
            }
            actKaratsuba(p1, p2, 0, noUsedThreads, 0);
            foreach (Thread t in threads)
            {
                t.Start();
            }
            foreach (Thread t in threads)
            {
                t.Join();
            }

            Polynomial result = new Polynomial(p1.capacity + p2.capacity - 1, true);
            return getKaratsuba(p1,p2,0,noUsedThreads,0);
           
        }

     

        public static Polynomial add(Polynomial p1, Polynomial P2)
        {
            Polynomial result = new Polynomial(Math.Max(p1.capacity, P2.capacity), true);
            for (int i = 0; i < Math.Min(p1.capacity, P2.capacity); i++)
            {
                int temp = p1.coefficients[i] + P2.coefficients[i];
                result.coefficients[i] += temp;
            }
            for (int i = Math.Min(p1.capacity, P2.capacity); i < Math.Max(p1.capacity, P2.capacity); i++) {
                if (i >= p1.capacity) result.coefficients[i] = P2.coefficients[i];
                else result.coefficients[i] = p1.coefficients[i];

            }
            return result;
        }
        public static Polynomial subtract(Polynomial i1, Polynomial i2)
        {
            
            Polynomial result = new Polynomial(Math.Max(i1.capacity, i2.capacity), true);
            for (int i = 0; i < Math.Min(i1.capacity, i2.capacity); i++)
            {
                result.coefficients[i] = i1.coefficients[i] - i2.coefficients[i];
            }
            for (int i = Math.Min(i1.capacity, i2.capacity); i < Math.Max(i1.capacity, i2.capacity); i++)
            {
                if (i >= i1.capacity) result.coefficients[i] = - i2.coefficients[i];
                else result.coefficients[i] = i1.coefficients[i];

            }
            return result;
        }
        public (Polynomial, Polynomial) split(int index)
        {
            Polynomial result1 = new Polynomial(index, true);
            Polynomial result2 = new Polynomial(capacity-index, true);
            int j = 0;
            int k = 0;
            for (int i = 0; i < capacity; i++)
            {
                if (i < index) { result1.coefficients[j++] = coefficients[i]; }
                else { result2.coefficients[k++] = coefficients[i]; }
            }
            return (result1, result2);
        }

        public void power(int times)
        {
            int[] newCoeff = new int[capacity + times];
            for (int i = 0; i < times; i++)
            {
                newCoeff[i] = 0;
                
            }
            for (int i = times; i < times+capacity; i++)
            {
                newCoeff[i] = coefficients[i-times];
             
            }
            capacity += times;
            coefficients = newCoeff;
        }



        public static Polynomial karatsuba(Polynomial x, Polynomial y)
        {

            if (x.capacity < 3 || y.capacity < 3)
                return Polynomial.seqProduct(x, y);
            int n = Math.Max(x.capacity, y.capacity);
            double m = Math.Ceiling(1.0 * n / 2.0);
            var x_ = x.split((int)m);
            var y_ = y.split((int)m);
            var xL = x_.Item1;
            var xR = x_.Item2;
            var yL = y_.Item1;
            var yR = y_.Item2;

            var a = karatsuba(xR, yR);
            var d = karatsuba(xL, yL);
            var e = Polynomial.subtract(Polynomial.subtract(karatsuba(Polynomial.add(xR, xL), Polynomial.add(yR, yL)), a), d);
            a.power((int)m * 2);
            e.power((int)m);
            return Polynomial.add(Polynomial.add(a, e), d);
        }
        [Serializable]
        internal struct VectorElement
        {
            public int Index;
            public int Value;
        }

        [Serializable]
        internal struct MatrixElement
        {
            public int Row;
            public int Column;
            public int Value;
        }

        private static IEnumerable<int> DivideEvenly(int numerator, int denominator)
        {
            if (numerator <= 0 || denominator <= 0)
                throw new InvalidOperationException("Invalid division!");

            int mod;
            var div = Math.DivRem(numerator, denominator, out mod);

            for (var i = 0; i < denominator; i++)
                yield return i < mod ? div + 1 : div;
        }

        private static void MpiFor(int forStart, int forEnd, int forIncrement, Communicator communicator,
            out int workloadStart,
            out int workloadEnd)
        {
            var forIterations = (forEnd - forStart) / forIncrement + 1;

            var workers = communicator.Size - 1;
            var workload = DivideEvenly(forIterations, workers).ToArray();

            workloadStart = 0;
            workloadEnd = workload[0] - 1;

            for (var i = 1; i < communicator.Rank; ++i)
            {
                workloadStart += workload[i - 1];
                workloadEnd = workloadStart + workload[i] - 1;
            }
        }


        public static void Main(string[] args)
        {
            using (new Environment(ref args))
            {
                var comm = Communicator.world;

                Polynomial x = null, y = null;
                int n;

                if (comm.Rank == 0) 
                {
                    const int degree = 100;
                    x = new Polynomial(degree);
                    y = new Polynomial(degree);

                    n = degree;

                    var di = new int[n];
                    var dpq = new int[n, n];
                    comm.Broadcast(ref x, 0);
                    comm.Broadcast(ref y, 0);

                    for (var i = 1; i <= n; ++i)
                    {
                        var result = comm.Receive<VectorElement>(Communicator.anySource, 1);
                        di[result.Index] = result.Value;
                    }

                    for (var i = 1; i <= n * (n - 1) / 2; ++i)
                    {
                        var result = comm.Receive<MatrixElement>(Communicator.anySource, 2);
                        dpq[result.Row, result.Column] = result.Value;
                    }

                    var z = new int[2 * n - 1];
                    z[0] = di[0];
                    z[2 * n - 2] = di[n - 1];
                    for (var i = 1; i <= 2 * n - 3; ++i)
                    {
                        for (var p = 0; p <= i; ++p)
                        {
                            var q = i - p;
                            if (p < n && q < n && q > p)
                                z[i] += dpq[p, q] - (di[p] + di[q]);
                        }

                        if (i % 2 == 0)
                            z[i] += di[i / 2];
                    }

                    var output = new Polynomial(z.Length);
                    for (int zy = 0; zy < z.Length; zy++) {
                        output.coefficients[zy] = z[zy];
                    }
                    Console.WriteLine(output.printPolynomial());
                    Console.WriteLine(seqProduct(x, y).printPolynomial());
          
                }
                else 
                {
                    // Receive the broadcasted polynomials.
                    comm.Broadcast(ref x, 0);
                    comm.Broadcast(ref y, 0);

                    n = x.capacity;

                    // The "for" lower and upper bounds.
                    int workloadStart, workloadEnd;

                    MpiFor(0, n - 1, 1, comm, out workloadStart, out workloadEnd);
                    for (var i = workloadStart; i <= workloadEnd; ++i)
                        comm.Send(new VectorElement { Index = i, Value = x.coefficients[i] * y.coefficients[i] }, 0, 1);

                    MpiFor(0, 2 * n - 3, 1, comm, out workloadStart, out workloadEnd);
                    for (var i = workloadStart; i <= workloadEnd; ++i)
                        for (var p = 0; p <= i; ++p)
                        {
                            var q = i - p;
                            if (p < n && q < n && q > p)
                                comm.Send(
                                    new MatrixElement
                                    {
                                        Row = p,
                                        Column = q,
                                        Value =
                                            (x.coefficients[p] + x.coefficients[q]) *
                                            (y.coefficients[p] + y.coefficients[q])
                                    }, 0, 2);
                        }
                }
            }
        }
    }

    }

