using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace ClassLibrary1.ViewModels
{
    public class UserControl1_ViewModel : INotifyPropertyChanged
    {
        private string message;

        public string Message { get => message; set { message = value; RaisePropertyChanged(nameof(Message)); } }

        public event PropertyChangedEventHandler PropertyChanged;
        public void RaisePropertyChanged(string name)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }

        internal void Button_Click()
        {
            MessageBox.Show(Message);
        }
    }
}
