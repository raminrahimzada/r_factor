// Open this to use BigInteger for all numeric types
// May decrease performance
//#define BIGINT


// Open this to use BigInteger when ulong is not enough
// otherwise with throw overflow exception when n > 2^30 approx
#define BIGINT_FALLBACK


using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

#if BIGINT
using TNumber = System.Numerics.BigInteger;
#else
using TNumber = System.UInt64;
#endif



// ReSharper disable RedundantOverflowCheckingContext
// ReSharper disable BuiltInTypeReferenceStyle
// ReSharper disable ConvertToConstant.Local
// ReSharper disable LoopCanBeConvertedToQuery

namespace RFactor
{
    public static class R
    {
#if BIGINT
        private static readonly TNumber Ten = 10;
#else
        private const TNumber Ten = 10;
#endif

        public const int MaxDigitLengthOfNumber = 20;
        private static readonly TNumber[] Digits;
        private static readonly Dictionary<int, TNumber> PowersOfTen = new();

        static R()
        {
            PowersOfTen.Add(0, 1);
            for (var i = 1; i < MaxDigitLengthOfNumber; i++)
            {
                checked
                {
                    PowersOfTen.Add(i, Ten * PowersOfTen[i - 1]);
                }
            }

            var ten = (int) Ten;
            Digits = Enumerable.Range(0, ten).Select(x => (TNumber)x).ToArray();
        }

        private static TNumber[] FollowingBy(this TNumber a)
        {
            return new[] {a};
        }

        private static TNumber[] FollowingBy(this TNumber a, TNumber[] arr)
        {
            var result = new TNumber[arr.Length + 1];
            result[0] = a;
            Array.Copy(arr, 0, result, 1, arr.Length);
            return result;
        }

        public static string ToString(TNumber[] arr)
        {
            var sb = new StringBuilder();
            foreach (var d in arr)
            {
                sb.Append(d);
            }
            return sb.ToString();
        }

        public static TNumber Number(TNumber[] digitArr)
        {
            if (digitArr.Length == 1) return digitArr[0];
            TNumber result = 0;
            foreach (var integer in digitArr)
            {
                checked
                {
                    result = Ten * result + integer;
                }
            }
            return result;
        }

        public static TNumber Number(TNumber firstDigit,TNumber[] digitArr)
        {
            if (digitArr.Length == 0) return firstDigit;

            var result = firstDigit;
            foreach (var digit in digitArr)
            {
                checked
                {
                    result = Ten * result + digit;
                }
            }
            return result;
        }

       
        private static IEnumerable<(TNumber[], TNumber[])> Find1(TNumber n)
        {
            var nTen = n % Ten;
            foreach (var i in Digits)
            {
                foreach (var j in Digits)
                {
                    TNumber real;
                    checked
                    {
                        real = i * j;
                    }
                    if (real > n) continue;
                    if ((real - nTen) % Ten == 0)
                    {
                        var p = (i.FollowingBy(), j.FollowingBy());
                        yield return p;
                    }
                }
            }
        }

        private static int GetStep(TNumber n)
        {
            int counter = 0;
            while (n>0)
            {
                ++counter;
                n /= 10;
            }

            return counter;
        }
        public static IEnumerable<(TNumber[], TNumber[])> Find(TNumber n)
        {
            return Find(GetStep(n), n);
        }
        private static IEnumerable<(TNumber[], TNumber[])> Find(int step,TNumber n)
        {
#if DEBUG
            var  counter= 0;
#endif
            if (step == 1)
            {
                foreach (var tuple in Find1(n))
                {
#if DEBUG
                    ++counter; 
#endif
                    yield return tuple;
                }
#if DEBUG
                    Console.WriteLine($"step-{step} yield {counter} items");
#endif
                yield break;
            }

            //Console.WriteLine($"started step-{step}");
            Debug.Assert(PowersOfTen.ContainsKey(step));
            var power = PowersOfTen[step];
            var nPower = n % power;
            var stepPrevious = Find(step - 1, n);

            foreach (var (left, right) in stepPrevious)
            {
                //Console.WriteLine(Number(left) + "*" + Number(right) + " = " + (Number(left) * Number(right)));
#if DEBUG
                ++counter;
#endif
                //Console.WriteLine(new string('\t',step) +$"step-{step} iteration-{counter}");
                foreach (var leftFirstDigit in Digits)
                {
                    foreach (var rightFirstDigit in Digits)
                    {
                        var ll = Number(leftFirstDigit, left);
                        var rr = Number(rightFirstDigit,right);
                        ll %= power;
                        rr %= power;
                        //a[left] * b[right]
                        var ok = Check(ref ll, ref rr, ref power, ref n, ref nPower);
                        if (ok == null) continue;
                        if (ok.Value)
                        {
                            var p = (leftFirstDigit.FollowingBy(left), rightFirstDigit.FollowingBy(right));
                            yield return p;
                        }
                    }
                }
            }
#if DEBUG
            Console.WriteLine($"step-{step} yield {counter} items");
#endif
        }

        private static bool? Check(ref TNumber ll, ref TNumber rr, ref TNumber power, ref TNumber n, ref TNumber nPower)
        {
            var real = ll;
            checked
            {
#if !BIGINT_FALLBACK
                real *= rr;
#else
                try
                {
                    real *= rr;
                }
                catch (OverflowException)
                {
                    //use BigInteger here if came across to big numbers
                    return CheckBigInteger(ref ll, ref rr, ref power, ref n, ref nPower);
                }
#endif
            }
            if (real > n) return null;
            return real % power == nPower;
        }
#if BIGINT_FALLBACK
        private static bool? CheckBigInteger(ref TNumber ll, ref TNumber rr, ref TNumber power, ref TNumber n, ref TNumber nPower)
        {
            //this causes performance penalty since BigInteger is less performant than System.IntXX
            System.Numerics.BigInteger real = ll;
            real *= rr;
            if (real > n) return null;
            return real % power == nPower;
        }
#endif
    }
}
