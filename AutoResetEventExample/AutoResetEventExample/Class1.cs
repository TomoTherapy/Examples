using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace AutoResetEventExample
{
    class Class1
    {
        MainWindow window;
        public Thread thread;

        public Class1(MainWindow window)
        {
            this.window = window;

            thread = new Thread(JesusChrist);

            thread.Start();
        }

        public void JesusChrist()
        {
            while (window.isRunning)
            {
                window.event1.Set();
                Thread.Sleep(1600);
                window.event2.Set();
                Thread.Sleep(1600);
            }
        }
    }
}
