using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace CsMonad
{
    public static class TaskUtils
    {
        public static Task<A> ToTask<A>(this A a)
        {
            return Task.FromResult(a);
        }

        public static Task<B> SelectMany<A, B>(this Task<A> x, Func<A, Task<B>> f)
        {
            return x.ContinueWith(u => f(u.Result).Result);
        }

        public static Task<C> SelectMany<A, B, C>(this Task<A> x, Func<A, Task<B>> f, Func<A, B, C> g)
        {
            return x.SelectMany(a => f(a).SelectMany(b => g(a, b).ToTask()));
        }

        public static Task<B> Select<A, B>(this Task<A> x, Func<A, B> f)
        {
            return x.SelectMany(a => f(a).ToTask());
        }
    }

    public static class TaskUtilsTest
    {
        private static Task<int> heavyWork1()
        {
            var tcs = new TaskCompletionSource<int>();
            Task.Run(() =>
            {
                Console.WriteLine("heavyWork1 started");
                Thread.Sleep(1000);
                tcs.SetResult(3);
            });
            return tcs.Task;
        }

        private static Task<string> heavyWork2()
        {
            var tcs = new TaskCompletionSource<string>();
            Task.Run(() =>
            {
                Console.WriteLine("heavyWork2 started");
                Thread.Sleep(1000);
                tcs.SetResult("piyo");
            });
            return tcs.Task;
        }

        public static void Test1()
        {
            var result = heavyWork1().ContinueWith(t1 =>
                         heavyWork2().ContinueWith(t2 =>
                         Task.FromResult(Enumerable.Repeat(t2.Result, t1.Result).Aggregate(string.Empty, (acc, x) => $"{acc}{x}! ")).Result).Result);

            Console.WriteLine($"result is {result.Result}");
        }

        public static void Test2()
        {
            var result = heavyWork1().SelectMany(x1 =>
                         heavyWork2().SelectMany(x2 =>
                         Task.FromResult(Enumerable.Repeat(x2, x1).Aggregate(string.Empty, (acc, x) => $"{acc}{x}! "))));

            Console.WriteLine($"result is {result.Result}");
        }

        public static void Test3()
        {
            var result = (from x1 in heavyWork1()
                          from x2 in heavyWork2()
                          select Enumerable.Repeat(x2, x1).Aggregate(string.Empty, (acc, x) => $"{acc}{x}! "));

            Console.WriteLine($"result is {result.Result}");
        }
    }
}
