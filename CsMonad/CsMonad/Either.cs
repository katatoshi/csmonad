using System;

namespace CsMonad
{
    public class Either<L, R>
    {
        public static Either<L, R> Left(L left)
        {
            return new Either<L, R>(left);
        }

        public static Either<L, R> Right(R right)
        {
            return new Either<L, R>(right);
        }

        private readonly L left;

        private readonly R right;

        private readonly bool isLeft;

        private Either(L left)
        {
            this.left = left;
            isLeft = true;
        }

        private Either(R right)
        {
            this.right = right;
            isLeft = false;
        }

        public U CaseOf<U>(Func<L, U> matchLeft, Func<R, U> matchRight)
        {
            return isLeft ? matchLeft(left) : matchRight(right);
        }

        public void CaseOf(Action<L> matchLeft, Action<R> matchRight)
        {
            if (isLeft)
            {
                matchLeft(left);
            }
            else
            {
                matchRight(right);
            }
        }

        public override string ToString()
        {
            return isLeft ? $"Left {left.ToString()}" : $"Right {right.ToString()}";
        }
    }

    public static class Either
    {
        public static Either<L, A> ToEither<L, A>(this A a)
        {
            return Either<L, A>.Right(a);
        }

        public static Either<L, B> SelectMany<L, A, B>(this Either<L, A> x, Func<A, Either<L, B>> f)
        {
            return x.CaseOf(matchLeft : left  => Either<L, B>.Left(left),
                            matchRight: right => f(right));
        }

        public static Either<L, C> SelectMany<L, A, B, C>(this Either<L, A> x, Func<A, Either<L, B>> f, Func<A, B, C> g)
        {
            return x.SelectMany(a => f(a).SelectMany(b => g(a, b).ToEither<L, C>()));
        }

        public static Either<L, B> Select<L, A, B>(this Either<L, A> x, Func<A, B> f)
        {
            return x.SelectMany(a => f(a).ToEither<L, B>());
        }

        // ※Either には Maybe の Nothing に相当するものがないので Where は定義できない（Haskell の MonadPlus の mfilter を参照）。
    }

    public static class EitherTest
    {
        private static Either<string, double> eitherQuotient(double dividend, double divisor)
        {
            return divisor == 0
                ? Either<string, double>.Left("0 除算は定義されていません。")
                : Either<string, double>.Right(dividend / divisor);
        }

        private static Either<string, double> eitherSqrt(double x)
        {
            return x < 0
                ? Either<string, double>.Left("負の実数の平方根は実数の範囲では定義されていません。")
                : Either<string, double>.Right(Math.Sqrt(x));
        }

        public static void Test1()
        {
            var m1 = eitherQuotient(3.0, 2.0);
            var m2 = eitherSqrt(9.0);

            var result1 = m1.SelectMany(x1 =>
                          m2.SelectMany(x2 =>
                          (x1 * x2).ToEither<string, double>()));
            
            result1.CaseOf(matchLeft : left  => Console.WriteLine($"result is {left}"),
                           matchRight: right => Console.WriteLine($"result is {right}"));

            var m3 = eitherQuotient(3.0, 0);
            var m4 = eitherSqrt(9.0);

            var result2 = m3.SelectMany(x1 =>
                          m4.SelectMany(x2 =>
                          (x1 * x2).ToEither<string, double>()));
            
            result2.CaseOf(matchLeft : left  => Console.WriteLine($"result is {left}"),
                           matchRight: right => Console.WriteLine($"result is {right}"));
            
            var m5 = eitherQuotient(3.0, 2.0);
            var m6 = eitherSqrt(-1.0);

            var result3 = m5.SelectMany(x1 =>
                          m6.SelectMany(x2 =>
                          (x1 * x2).ToEither<string, double>()));
            
            result3.CaseOf(matchLeft : left  => Console.WriteLine($"result is {left}"),
                           matchRight: right => Console.WriteLine($"result is {right}"));
        }

        public static void Test2()
        {
            var m1 = eitherQuotient(3.0, 2.0);
            var m2 = eitherSqrt(9.0);

            var result1 = (from x1 in m1
                           from x2 in m2
                           select x1 * x2);
            
            result1.CaseOf(matchLeft : left  => Console.WriteLine($"result is {left}"),
                           matchRight: right => Console.WriteLine($"result is {right}"));

            var m3 = eitherQuotient(3.0, 0);
            var m4 = eitherSqrt(9.0);

            var result2 = (from x1 in m3
                           from x2 in m4
                           select x1 * x2);
            
            result2.CaseOf(matchLeft : left  => Console.WriteLine($"result is {left}"),
                           matchRight: right => Console.WriteLine($"result is {right}"));
            
            var m5 = eitherQuotient(3.0, 2.0);
            var m6 = eitherSqrt(-1.0);

            var result3 = (from x1 in m5
                           from x2 in m6
                           select x1 * x2);
            
            result3.CaseOf(matchLeft : left  => Console.WriteLine($"result is {left}"),
                           matchRight: right => Console.WriteLine($"result is {right}"));
        }
    }
}
