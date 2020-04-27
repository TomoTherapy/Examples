using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace LabelToolDemo1
{
    public class PropertiesClass
    {
        public string WorkspacePath { get; set; }
        public ObservableCollection<Sets> Set { get; set; }

        public PropertiesClass()
        {
            WorkspacePath = "";
            Set = new ObservableCollection<Sets>();
        }
    }

    public class Sets
    {
        public string Name { get; set; }
        public string Shortcut { get; set; }
    }
}
