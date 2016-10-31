using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CsMonad
{
    class Program
    {
        static void Main(string[] args)
        {
            MaybeTest.Test1();
            Console.WriteLine();
            MaybeTest.Test2();
            Console.WriteLine();
            MaybeTest.Test3();
            Console.WriteLine();

            EitherTest.Test1();
            Console.WriteLine();
            EitherTest.Test2();
            Console.WriteLine();
        }
    }
}
