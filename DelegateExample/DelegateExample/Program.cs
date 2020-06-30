using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace DelegateExample
{
    delegate int MyDelegate(int a, int b);

    class Calculator
    {
        public int Plus(int a, int b)
        {
            return a + b;
        }

        public static int Minus(int a, int b)
        {
            return a - b;
        }
    }

    class Program
    {
        static void Main(string[] args)
        {
            Calculator Calc = new Calculator();
            MyDelegate CallBack;

            CallBack = new MyDelegate(Calc.Plus);
            Console.WriteLine(CallBack(3, 4));

            CallBack = new MyDelegate(Calculator.Minus);
            Console.WriteLine(CallBack(7, 5));


            Thread t = new Thread(() => { });

            Action<int, int> sum = (a, b) => {
                Console.WriteLine(a + b + " Jesus Christ!!");
            };

            Action<string> print = (txt) =>
            {
                Console.WriteLine(txt);
            };

            sum(3, 21);
            print("Jesus fucks me everyday");


            Profile[] profiles =
            {
                new Profile(){Name = "주님", Height = 189},
                new Profile(){Name = "암컷", Height = 156},
                new Profile(){Name = "호빗", Height = 134},
                new Profile(){Name = "하이고", Height = 177},
                new Profile(){Name = "트롤", Height = 168},
                new Profile(){Name = "드워프", Height = 161},
                new Profile(){Name = "한국인", Height = 173},
            };

            var profs = from profile in profiles
                        where profile.Height < 170
                        orderby profile.Height ascending
                        select profile;

            foreach(var n in profs)
            {
                Console.WriteLine(n.Name + " : " + n.Height);
            }

            Profile aaa = profiles.Single((a) => a.Name == "주님");

            Console.WriteLine(aaa.Name + "の箕は : " + aaa.Height);

            Func<double, double> fun = new Func<double, double>((x) => { return Math.Pow(x, x); });

            Console.WriteLine(fun(9));

            Thread tt = new Thread(new ThreadStart(()=> { }));



            Console.Read();
        }
    }

    class Profile
    {
        public string Name { get; set; }
        public int Height { get; set; }
    }
}
