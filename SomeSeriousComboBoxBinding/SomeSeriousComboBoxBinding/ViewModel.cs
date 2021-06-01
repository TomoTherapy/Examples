using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SomeSeriousComboBoxBinding
{
    public class ViewModel : INotifyPropertyChanged
    {
        private int _comboX = 4;
        private int _comboY = 10;
        private int _mode;

        public event PropertyChangedEventHandler PropertyChanged;
        public void RaisePropertyChanged(string name)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }

        public ObservableCollection<int> ComboXItems { get; set; }
        public ObservableCollection<int> ComboYItems { get; set; }
        public ObservableCollection<int> ComboModeItems { get; set; }
        public int ComboX { get => _comboX; set { _comboX = value; RaisePropertyChanged(nameof(ComboX)); } }
        public int ComboY { get => _comboY; set { _comboY = value; RaisePropertyChanged(nameof(ComboY)); } }
        public int Mode { get => _mode; set { _mode = value; RaisePropertyChanged(nameof(Mode)); } }

        public ViewModel()
        {
            ComboXItems = new ObservableCollection<int>(new int[] { 1, 2, 3, 4, 5 });
            ComboYItems = new ObservableCollection<int>();
            GenerateComboYItems();
            ComboModeItems = new ObservableCollection<int>(new int[] { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15 });

            ModeChange();
        }

        internal void ComboBoxX_DropDownClosed()
        {
            GenerateComboYItems();
            ComboY = ComboYItems[0];
            ModeChange();
        }

        internal void ComboBoxY_DropDownClosed()
        {
            ModeChange();
        }

        internal void ComboBoxMode_DropDownClosed()
        {
            ComboXYChange();
        }

        private void ModeChange()
        {
            switch (ComboX)
            {
                case 1:
                    switch (ComboY)
                    {
                        case 1: Mode = 1; break;
                        case 2: Mode = 2; break;
                        case 3: Mode = 3; break;
                    }
                    break;
                case 2:
                    switch (ComboY)
                    {
                        case 4: Mode = 4; break;
                        case 5: Mode = 5; break;
                        case 6: Mode = 6; break;
                    }
                    break;
                case 3:
                    switch (ComboY)
                    {
                        case 5: Mode = 7; break;
                        case 6: Mode = 8; break;
                        case 7: Mode = 9; break;
                    }
                    break;
                case 4:
                    switch (ComboY)
                    {
                        case 8: Mode = 10; break;
                        case 9: Mode = 11; break;
                        case 10: Mode = 12; break;
                    }
                    break;
                case 5:
                    switch (ComboY)
                    {
                        case 11: Mode = 13; break;
                        case 12: Mode = 14; break;
                        case 13: Mode = 15; break;
                    }
                    break;
            }
        }

        private void GenerateComboYItems()
        {
            switch (ComboX)
            {
                case 1:
                    ComboYItems.Clear();
                    ComboYItems.Add(1);
                    ComboYItems.Add(2);
                    ComboYItems.Add(3);
                    break;
                case 2:
                    ComboYItems.Clear();
                    ComboYItems.Add(4);
                    ComboYItems.Add(5);
                    ComboYItems.Add(6);
                    break;
                case 3:
                    ComboYItems.Clear();
                    ComboYItems.Add(5);
                    ComboYItems.Add(6);
                    ComboYItems.Add(7);
                    break;
                case 4:
                    ComboYItems.Clear();
                    ComboYItems.Add(8);
                    ComboYItems.Add(9);
                    ComboYItems.Add(10);
                    break;
                case 5:
                    ComboYItems.Clear();
                    ComboYItems.Add(11);
                    ComboYItems.Add(12);
                    ComboYItems.Add(13);
                    break;
            }
        }

        private void ComboXYChange()
        {
            switch (Mode)
            {
                case 1: ComboX = 1; GenerateComboYItems(); ComboY = 1; break;
                case 2: ComboX = 1; GenerateComboYItems(); ComboY = 2; break;
                case 3: ComboX = 1; GenerateComboYItems(); ComboY = 3; break;
                case 4: ComboX = 2; GenerateComboYItems(); ComboY = 4; break;
                case 5: ComboX = 2; GenerateComboYItems(); ComboY = 5; break;
                case 6: ComboX = 2; GenerateComboYItems(); ComboY = 6; break;
                case 7: ComboX = 3; GenerateComboYItems(); ComboY = 5; break;
                case 8: ComboX = 3; GenerateComboYItems(); ComboY = 6; break;
                case 9: ComboX = 3; GenerateComboYItems(); ComboY = 7; break;
                case 10: ComboX = 4; GenerateComboYItems(); ComboY = 8; break;
                case 11: ComboX = 4; GenerateComboYItems(); ComboY = 9; break;
                case 12: ComboX = 4; GenerateComboYItems(); ComboY = 10; break;
                case 13: ComboX = 5; GenerateComboYItems(); ComboY = 11; break;
                case 14: ComboX = 5; GenerateComboYItems(); ComboY = 12; break;
                case 15: ComboX = 5; GenerateComboYItems(); ComboY = 13; break;
            }
        }
    }
}
