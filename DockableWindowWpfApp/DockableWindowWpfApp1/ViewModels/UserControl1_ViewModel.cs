using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DockableWindowWpfApp1.ViewModels
{
    public class UserControl1_ViewModel : ViewModelBase
    {
        private bool flag;
        private string _text1;
        private string _text2;

        public string Text1 { get => _text1; set { _text1 = value; RaisePropertyChanged(nameof(Text1)); } }
        public string Text2 { get => _text2; set { _text2 = value; RaisePropertyChanged(nameof(Text2)); } }


        internal void Button_Click()
        {
            flag = !flag;

            if (flag)
            {
                Text1 = "Click!";
                Text2 = "Meh";
            }
            else
            {
                Text1 = "Meh";
                Text2 = "Click!";
            }
        }
    }
}
