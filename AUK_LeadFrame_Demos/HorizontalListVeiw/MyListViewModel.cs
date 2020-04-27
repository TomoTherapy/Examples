using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.ObjectModel;
using System.Windows.Input;
using System.ComponentModel;

namespace HorizontalListVeiw
{
    class MyListViewModel// : INotifyPropertyChanged
    {
        public ObservableCollection<MyItem> m_MyList;
        public ObservableCollection<MyItem> MyList { get; set; }
        /*public ObservableCollection<MyItem> MyList
        {
            get { return m_MyList; }
            set
            {
                m_MyList = value;
                RaisePropertyChanged("MyList");
            } 
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void RaisePropertyChanged(string propertyName)
        {
            if (this.PropertyChanged != null)
                this.PropertyChanged(this, new PropertyChangedEventArgs(propertyName));

            //PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }*/


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
            if(item != null)
                MyList.Remove(item);
        }

        public bool CanDelete(object obj)
        {
            return true;    //항상 true 리턴
        }

        //
    }

    public class MyItem
    {
        public string TextName { get; set; }
        public string TextButton { get; set; }

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
