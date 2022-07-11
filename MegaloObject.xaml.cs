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

namespace RuntimeMegaloObjectDebugger
{
    /// <summary>
    /// Interaction logic for MegaloObject.xaml
    /// </summary>
    public partial class MegaloObject : UserControl
    {
        public MegaloObject()
        {
            InitializeComponent();
        }

        public MainWindow main;
        public MegaloObjectData? data_element;

        public long OBJECT_ADDRESS;

        public bool is_data_open = false;
        private void toggle_data_button_press(object sender, RoutedEventArgs e)
        {
            is_data_open = !is_data_open;
            if (is_data_open)
            {
                expand_button.Content = "-";
                data_element = main.create_megalo_object_data(OBJECT_ADDRESS, null);

                data_element.parent = this;
                data_panel.Children.Add(data_element);

                data_element.is_initializing = false;
            }
            else // data is not being opened
            {
                expand_button.Content = "+";
                data_element = null;
                data_panel.Children.Clear();
            }
        }

        public bool write_change(string change, string info)
        {
            if (string.IsNullOrEmpty(info))
                return false;

            //if (string.IsNullOrEmpty(change))
            //    change = "0";

            string[] type_and_offset = info.Split(":");

            long offset = Convert.ToInt64(type_and_offset[1]); // potentially error prone but whatever
            offset += OBJECT_ADDRESS;
            switch (type_and_offset[0])
            {
                case "int8":
                    try
                    {   // have to use try because convert doesn't out a bool
                        return main.memory_process.write_int8(offset, Convert.ToByte(change));
                    }
                    catch
                    {
                        return false;
                    }
                case "int16":
                    try
                    {   // have to use try because convert doesn't out a bool
                        return main.memory_process.write_int16(offset, Convert.ToInt16(change));
                    }
                    catch
                    {
                        return false;
                    }
                case "int24":
                    try
                    {   // have to use try because convert doesn't out a bool
                        return main.memory_process.write_int24(offset, Convert.ToInt32(change));
                    }
                    catch
                    {
                        return false;
                    }
                case "int32":
                    try
                    {   // have to use try because convert doesn't out a bool
                        return main.memory_process.write_int32(offset, Convert.ToInt32(change));
                    }
                    catch
                    {
                        return false;
                    }
            }
            return false;
        }
    }
}
