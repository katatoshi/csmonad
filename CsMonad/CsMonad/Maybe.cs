using System;

namespace CsMonad
{
    public class Maybe<A>
    {
        public static readonly Maybe<A> Nothing = new Maybe<A>();

        public static Maybe<A> Just(A just)
        {
            return new Maybe<A>(just);
        }

        private readonly A just;

        private readonly bool isNothing;

        private Maybe()
        {
            isNothing = true;
        }

        private Maybe(A just)
        {
            this.just = just;
            isNothing = false;
        }

        public void CaseOf(Action matchNothing, Action<A> matchJust)
        {
            if (isNothing)
            {
                matchNothing();
            }
            else
            {
                matchJust(just);
            }
        }

        public U CaseOf<U>(Func<U> matchNothing, Func<A, U> matchJust)
        {
            return isNothing ? matchNothing() : matchJust(just);
        }

        public override string ToString()
        {
            return isNothing ? "Nothing" : $"Just {just.ToString()}";
        }
    }

    public static class Maybe
    {
        public static Maybe<A> ToMaybe<A>(this A a)
        {
            return Maybe<A>.Just(a);
        }

        public static Maybe<B> SelectMany<A, B>(this Maybe<A> x, Func<A, Maybe<B>> f)
        {
            return x.CaseOf(matchNothing: ()   => Maybe<B>.Nothing,
                            matchJust   : just => f(just));
        }

        public static Maybe<C> SelectMany<A, B, C>(this Maybe<A> x, Func<A, Maybe<B>> f, Func<A, B, C> g)
        {
            return x.SelectMany(a => f(a).SelectMany(b => g(a, b).ToMaybe()));
        }

        public static Maybe<B> Select<A, B>(this Maybe<A> x, Func<A, B> f)
        {
            return x.SelectMany(a => f(a).ToMaybe());
        }

        public static Maybe<A> Where<A>(this Maybe<A> x, Func<A, bool> p)
        {
            return (from a1 in x
                    from a2 in p(a1) ? a1.ToMaybe() : Maybe<A>.Nothing
                    select a2);
        }
    }

    public static class MaybeTest
    {
        private static Maybe<double> maybeQuotient(double dividend, double divisor)
        {
            return divisor == 0
                ? Maybe<double>.Nothing
                : Maybe<double>.Just(dividend / divisor);
        }

        private static Maybe<double> maybeSqrt(double x)
        {
            return x < 0
                ? Maybe<double>.Nothing
                : Maybe<double>.Just(Math.Sqrt(x));
        }

        public static void Test1()
        {
            var m1 = maybeQuotient(3.0, 2.0);
            var m2 = maybeSqrt(9.0);

            var result1 = m1.SelectMany(x1 =>
                          m2.SelectMany(x2 =>
                          (x1 * x2).ToMaybe()));
            
            result1.CaseOf(matchNothing: ()   => Console.WriteLine("result is nothing"),
                           matchJust   : just => Console.WriteLine($"result is {just}"));

            var m3 = maybeQuotient(3.0, 0);
            var m4 = maybeSqrt(9.0);

            var result2 = m3.SelectMany(x1 =>
                          m4.SelectMany(x2 =>
                          (x1 * x2).ToMaybe()));
            
            result2.CaseOf(matchNothing: ()   => Console.WriteLine("result is nothing"),
                           matchJust   : just => Console.WriteLine($"result is {just}"));

            var m5 = maybeQuotient(3.0, 2.0);
            var m6 = maybeSqrt(-1.0);

            var result3 = m5.SelectMany(x1 =>
                          m6.SelectMany(x2 =>
                          (x1 * x2).ToMaybe()));
            
            result3.CaseOf(matchNothing: ()   => Console.WriteLine("result is nothing"),
                           matchJust   : just => Console.WriteLine($"result is {just}"));
        }

        public static void Test2()
        {
            var m1 = maybeQuotient(3.0, 2.0);
            var m2 = maybeSqrt(9.0);

            var result1 = (from x1 in m1
                           from x2 in m2
                           select x1 + x2);
            
            result1.CaseOf(matchNothing: ()   => Console.WriteLine("result is nothing"),
                           matchJust   : just => Console.WriteLine($"result is {just}"));

            var m3 = maybeQuotient(3.0, 0);
            var m4 = maybeSqrt(9.0);

            var result2 = (from x1 in m3
                           from x2 in m4
                           select x1 + x2);
            
            result2.CaseOf(matchNothing: ()   => Console.WriteLine("result is nothing"),
                           matchJust   : just => Console.WriteLine($"result is {just}"));

            var m5 = maybeQuotient(3.0, 2.0);
            var m6 = maybeSqrt(-1.0);

            var result3 = (from x1 in m5
                           from x2 in m6
                           select x1 + x2);
            
            result3.CaseOf(matchNothing: ()   => Console.WriteLine("result is nothing"),
                           matchJust   : just => Console.WriteLine($"result is {just}"));
        }

        public static void Test3()
        {
            var m1 = maybeQuotient(3.0, 2.0);
            var m2 = maybeSqrt(9.0);

            var result1 = (from x1 in m1
                           from x2 in m2
                           let y = x1 * x2
                           where 5 < y
                           select y);
            
            result1.CaseOf(matchNothing: ()   => Console.WriteLine("result is nothing"),
                           matchJust   : just => Console.WriteLine($"result is {just}"));

            var m3 = maybeQuotient(3.0, 2.0);
            var m4 = maybeSqrt(16.0);

            var result2 = (from x1 in m3
                           from x2 in m4
                           let y = x1 * x2
                           where 5 < y
                           select y);
            
            result2.CaseOf(matchNothing: ()   => Console.WriteLine("result is nothing"),
                           matchJust   : just => Console.WriteLine($"result is {just}"));
        }
    }
}
