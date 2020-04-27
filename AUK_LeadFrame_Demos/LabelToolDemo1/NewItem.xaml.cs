using System.IO;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace LabelToolDemo1
{
    /// <summary>
    /// Interaction logic for NewItem.xaml
    /// </summary>
    public partial class NewItem : Window
    {
        public NewItem()
        {
            InitializeComponent();
        }

        public string NewItemName
        {
            get { return NewItemName_textBox.Text; }
            set { NewItemName_textBox.Text = value; }
        }

        public string Shortcut
        {
            get { return Shortcut_textBox.Text; }
            set { Shortcut_textBox.Text = value.ToString(); }
        }

        private void OK_button_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
        }

        private void Cancel_button_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
        }

        private void Window_MouseDown(object sender, MouseButtonEventArgs e)
        {
            this.DragMove();
        }

        private void Shortcut_textBox_KeyUp(object sender, KeyEventArgs e)
        {
            Regex rgx = new Regex("[^a-zA-Z0-9 -]");
            var box = e.OriginalSource as TextBox;
            box.Text = rgx.Replace(box.Text, "").ToUpper();
        }

        private void NewItemName_textBox_KeyUp(object sender, KeyEventArgs e)
        {
            var box = e.OriginalSource as TextBox;

            string invalid = new string(Path.GetInvalidFileNameChars()) + new string(Path.GetInvalidPathChars()) + "_ ";

            foreach (char c in invalid.ToCharArray())
            {
                box.Text = box.Text.Replace(c.ToString(), "");
            }

            box.CaretIndex = box.Text.Length;
        }
    }
}
