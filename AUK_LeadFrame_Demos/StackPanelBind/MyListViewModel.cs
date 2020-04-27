using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.ObjectModel;
using System.Windows.Input;
using System.ComponentModel;

namespace StackPanelBind
{
    class MyListViewModel
    {
        public ObservableCollection<MyItem> MyList;

        public MyListViewModel()
        {
            //List 생성
            MyList = new ObservableCollection<MyItem>();
            //Delete Command 생성
            DeleteCommand = new RelayCommand(Delete, CanDelete);
        }

        //Delete Command 설정
        private ICommand _deleteCommand;
        public ICommand DeleteCommand
        {
            get
            {
                if (_deleteCommand == null)
                {
                    _deleteCommand = new RelayCommand(Delete, CanDelete);
                }
                return _deleteCommand;
            }

            set
            {
                _deleteCommand = value;
            }
        }

        public void Delete(object obj)
        {
            MyItem item = obj as MyItem;
            if (item != null)
                MyList.Remove(item);
        }

        public bool CanDelete(object obj)
        {
            return true;    //항상 true 리턴
        }

        //
    }

    public class MyItem : INotifyPropertyChanged
    {
        public string m_TextName;
        public string TextName
        {
            get { return this.m_TextName; }
            set
            {
                if (this.m_TextName != value)
                {
                    this.m_TextName = value;
                    this.NotifyPropertyChanged("TextName");
                }
            }
        }

        public string m_TextButton;
        public string TextButton
        {
            get { return this.m_TextButton; }
            set
            {
                if (this.m_TextButton != value)
                {
                    this.m_TextButton = value;
                    this.NotifyPropertyChanged("TextButton");
                }
            }
        }


        public event PropertyChangedEventHandler PropertyChanged;
        public void NotifyPropertyChanged(string propName)
        {
            if (this.PropertyChanged != null)
                this.PropertyChanged(this, new PropertyChangedEventArgs(propName));
        }

        public MyItem()
        {
            TextName = string.Empty;
            TextButton = string.Empty;
        }

        public MyItem(string Name)
        {
            TextName = Name;
            TextButton = string.Empty;
        }

        public MyItem(string Name, string ButtonName)
        {
            TextName = Name;
            TextButton = ButtonName;
        }
    }

}
