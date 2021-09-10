using System;
using System.Linq;
using System.Numerics;

namespace RFactor
{
    class Program
    {         
        static void Main()
        {            
            ulong N = 1;
            //N = N << 40;
            //N++;
            
            N = int.MaxValue;
            //N = 173 * 857;
            //N = 999999999L;
            //N = 99999999999L;
            Console.WriteLine($"started factoring {N} on {DateTime.Now.ToLongTimeString()}");
            var counter = 0;
            //var stepCount = N.ToString().Length;
            var dt = DateTime.Now;

            var steps = R.Find(N).ToArray();
            var total = (DateTime.Now - dt);
            foreach (var (bb, dd) in steps)
            {
                Console.WriteLine(
                    $"[{++counter}]\t {R.ToString(bb)} * {R.ToString(dd)} = {R.Number(bb) * R.Number(dd)}");
            }

            Console.WriteLine("spent : "+total);
            Console.ReadLine();
        }
    }
}

/*
 * int.max->00:00:00.2047912
 * 2^20->00:00:00.0164829
 * 2^25->00:00:00.0331482
 * 2^30->00:00:00.3606766
 * 2^35->00:00:00.7528593
 * 2^40->00:00:08.8903395
 * 2^45->00:02:13.2954906
 *

 */
