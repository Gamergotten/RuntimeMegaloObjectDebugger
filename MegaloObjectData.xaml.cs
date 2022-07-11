using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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
using System.Text.RegularExpressions;

namespace RuntimeMegaloObjectDebugger
{
    /// <summary>
    /// Interaction logic for MegaloObjectData.xaml
    /// </summary>
    public partial class MegaloObjectData : UserControl
    {
        public MegaloObjectData()
        {
            InitializeComponent();
        }
        public MegaloObject parent;
        public bool is_initializing = true;

        private void scripted_obj_index_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (is_initializing) return;
            TextBox? target_box = sender as TextBox;
            if (target_box == null)
                return;

            // this is a little gross but whatever
            string fixed_text = Regex.Replace(target_box.Text, "[^0-9\\-]", "");
            if (string.IsNullOrEmpty(fixed_text)) fixed_text = "0";
            is_initializing = true;
            target_box.Text = fixed_text;

            if (parent.write_change(fixed_text, (string)target_box.Tag))
            {
                target_box.BorderBrush = Brushes.White;
                target_box.SelectionTextBrush = Brushes.White;
                target_box.Foreground = Brushes.White;
                parent.main.notify_successfull_write();
            }
            else
            {
                target_box.BorderBrush = Brushes.Red;
                target_box.SelectionTextBrush = Brushes.Red;
                target_box.Foreground = Brushes.Red;
            }
            is_initializing = false;
        }
    }
}
