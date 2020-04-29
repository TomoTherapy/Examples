using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace AutoResetEventExample
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public AutoResetEvent event1;
        public AutoResetEvent event2;
        private Thread thread;
        public bool isRunning;
        private Class1 class1;

        public MainWindow()
        {
            InitializeComponent();

            event1 = new AutoResetEvent(false);//initialState 를 true로 주면 처음 waitOne에 부딛혔을때 바로 지나가고 값을 false로 바꿈. 
            event2 = new AutoResetEvent(false);

            isRunning = true;
            thread = new Thread(SomeThread) { Name = "JesusThread" };
            thread.Start();

            class1 = new Class1(this);
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            event1.Set();
        }

        private void Button_Copy_Click(object sender, RoutedEventArgs e)
        {
            event2.Set();
        }

        private void SomeThread()
        {
            while (isRunning)
            {
                string name = Thread.CurrentThread.Name;

                Dispatcher.Invoke(new Action(() => 
                {
                    textBlock1.Text += name + " waits on AutoResetEvent #1.\n";
                    scrollViewer.ScrollToBottom();
                }));

                event1.WaitOne();

                Dispatcher.Invoke(new Action(() =>
                {
                    textBlock1.Text += name + " is released from AutoResetEvent #1\n" + name + " waits on AutoResetEvent #2.\n";
                    scrollViewer.ScrollToBottom();
                }));

                event2.WaitOne();

                Dispatcher.Invoke(new Action(() =>
                {
                    textBlock1.Text += name + " is released from AutoResetEvent #2\n" + name + " ends.\n";
                    scrollViewer.ScrollToBottom();
                }));
            }
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            isRunning = false;
            thread.Abort();
            class1.thread.Abort();
        }
    }
}
