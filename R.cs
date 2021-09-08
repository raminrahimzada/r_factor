using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Numerics;
using System.Text;
using TNumber = System.Numerics.BigInteger;
//using TNumber = System.UInt64;


// ReSharper disable RedundantOverflowCheckingContext
// ReSharper disable BuiltInTypeReferenceStyle
// ReSharper disable ConvertToConstant.Local
// ReSharper disable LoopCanBeConvertedToQuery

namespace RFactor
{
    internal static class R
    {
        private static readonly TNumber Ten = 10;
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
            Digits = Enumerable.Range(0, (int)Ten).Select(x => (TNumber)x).ToArray();
        }

        public static TNumber[] FollowingBy(this TNumber a)
        {
            return new[] {a};
        }

        public static TNumber[] FollowingBy(this TNumber a, TNumber[] arr)
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
            //if (digitArr.Length == 0) throw null;
            if (digitArr.Length == 1) return digitArr[0];
            TNumber result = 0;
            // ReSharper disable once LoopCanBeConvertedToQuery
            foreach (var integer in digitArr)
            {
                checked
                {
                    result = Ten * result + integer;
                }
            }
            return result;
        }

        public static TNumber Number(TNumber firstDigit, TNumber[] digitArr)
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

       
        public static IEnumerable<(TNumber[], TNumber[])> Find1(TNumber n)
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
        public static IEnumerable<(TNumber[], TNumber[])> Find(int step,TNumber n)
        {
            if (step == 1)
            {
                foreach (var tuple in Find1(n))
                {
                    yield return tuple;
                }
                yield break;
            }

            //Console.WriteLine($"started step-{step}");
            Debug.Assert(PowersOfTen.ContainsKey(step));
            var power = PowersOfTen[step];
            var nPower = n % power;
            var stepPrevious = Find(step - 1, n);
            var counter = 0;
            foreach (var (left, right) in stepPrevious)
            {
                counter++;
                //Console.WriteLine(new string('\t',step) +$"step-{step} iteration-{counter}");
                foreach (var leftFirstDigit in Digits)
                {
                    foreach (var rightFirstDigit in Digits)
                    {
                        var ll = Number(leftFirstDigit, left);
                        var rr = Number(rightFirstDigit, right);
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
            Console.WriteLine($"step-{step} yield {counter} items");
        }

        public static bool? Check(ref TNumber ll, ref TNumber rr, ref TNumber power, ref TNumber n, ref TNumber nPower)
        {
            var real = ll;
            checked
            {
                try
                {
                    real *= rr;
                }
                catch (OverflowException)
                {
                    //use BigInteger here if came across to big numbers
                    return CheckBigInteger(ref ll, ref rr, ref power, ref n, ref nPower);
                }
            }
            if (real > n) return null;
            return real % power == nPower;
        }
        public static bool? CheckBigInteger(ref TNumber ll, ref TNumber rr, ref TNumber power, ref TNumber n, ref TNumber nPower)
        {
            //this causes performance penalty since BigInteger is less performant than System.IntXX
            BigInteger real = ll;
            real *= rr;
            if (real > n) return null;
            return real % power == nPower;
        }
    }
}