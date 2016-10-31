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
            #region MaybeTest
            MaybeTest.Test1();
            Console.WriteLine();

            MaybeTest.Test2();
            Console.WriteLine();

            MaybeTest.Test3();
            Console.WriteLine();
            #endregion

            #region EitherTest
            EitherTest.Test1();
            Console.WriteLine();

            EitherTest.Test2();
            Console.WriteLine();
            #endregion

            #region TaskUtilsTest
            TaskUtilsTest.Test1();
            Console.WriteLine();

            TaskUtilsTest.Test2();
            Console.WriteLine();

            TaskUtilsTest.Test3();
            Console.WriteLine();
            #endregion
        }
    }
}
