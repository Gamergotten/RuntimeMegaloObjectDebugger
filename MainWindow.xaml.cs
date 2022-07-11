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
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        // CHANGE THIS IF YOU WANT TO UPDATE THE TOOL
        // YOU WILL REQUIRE A POINTER THAT POINTS TO THE "megalo_objects" STRING
        // "haloreach.dll" IS ALREADY INCLUDED IN THE OFFSET, YOU JUST NEED THE LIST OF OFFSETS TO GET TO THE POINTERS
        // EG. THIS IS "haloreach.dll" + 0x026D9418,0x4F0 IN CHEAT ENGINE
        static long[] megalo_objects_address = new long[2] { 0x026D9418, 0x4F0 };




        public MainWindow()
        {
            InitializeComponent();
        }

        public Mem memory_process = new();

        private void Attempt_hook_button_press(object sender, RoutedEventArgs e)
        {
            debug_text.Text = memory_process.hook_and_open_process("MCC-Win64-Shipping", false)? "successfully hooked" : "failed to hook to the game";
        }

        static byte[] megalo_objects = new byte[16] { 0x6D, 0x65, 0x67, 0x61, 0x6C, 0x6F, 0x5F, 0x6F, 0x62, 0x6A, 0x65, 0x63, 0x74, 0x73, 0x00, 0x00 };

        public Dictionary<int, MegaloObject> loaded_objs = new();
        private void Load_objs_button_press(object sender, RoutedEventArgs e)
        {
            long megalo_objs_address = memory_process.read_pointer("haloreach.dll", megalo_objects_address);
            byte[]? megalo_objs_header = memory_process.read_p_mem(megalo_objs_address, 16);
            if (megalo_objs_header == null)
            {
                debug_text.Text = "failed to read megalo header";
                return;
            }
            if (!megalo_objs_header.SequenceEqual(megalo_objects))
            {
                debug_text.Text = "megalo header does not match";
                return;
            }
            // if we got past those two than we good

            //64 bytes till first count
            int static_objs = memory_process.read_int32(megalo_objs_address + 64);
            //68 bytes till first count
            int count_objs = memory_process.read_int32(megalo_objs_address + 68);
            //72 bytes till first count
            int current_objs = memory_process.read_int32(megalo_objs_address + 72);
            // print debug info
            objs_static.Text = "Objs Static: " + static_objs;
            objs_count.Text = "Objs Count: " + count_objs;
            objs_current.Text = "Objs Current: " + current_objs;
            megalo_address.Text = "0x" + megalo_objs_address.ToString("X");

            //112 bytes offset to the scripted objects
            long base_address = megalo_objs_address + 112;

            for (int i = 0; i < count_objs; i++)
            {
                long current_address = base_address + (i*132);
                if (loaded_objs.ContainsKey(i))
                {
                    MegaloObject meg_object = loaded_objs[i];
                    meg_object.OBJECT_ADDRESS = current_address; // update their address anyway
                    if (meg_object.is_data_open && meg_object.data_element != null)
                    {
                        meg_object.data_element.is_initializing = true;
                        create_megalo_object_data(meg_object.OBJECT_ADDRESS, meg_object.data_element);
                        meg_object.data_element.is_initializing = false;
                    }
                }
                else
                {
                    MegaloObject new_meg_object = new();
                    new_meg_object.main = this;
                    new_meg_object.OBJECT_ADDRESS = current_address;
                    new_meg_object.meg_obj_name.Text = "meg_obj #" + i;
                    loaded_objs.Add(i, new_meg_object);
                    objects_panel.Children.Add(new_meg_object);
                }
            }

           // meg_obj_name;

            var poopoo = "";
        }

        private void loop_box_Checked(object sender, RoutedEventArgs e)
        {
            update_loop();
        }
        // repeat loop var update function 
        private async void update_loop()
        {
            while (loop_box.IsChecked == true)
            {
                Load_objs_button_press(null, null);
                await Task.Delay(5);
            }
        }

        public MegaloObjectData create_megalo_object_data(long object_address, MegaloObjectData? meg_obj_data)
        {
            if (meg_obj_data == null)
                meg_obj_data = new();
            // otherwise overwrite the text on this UI block
            meg_obj_data.scripted_obj_index.Text = memory_process.read_int16(object_address).ToString();

            meg_obj_data.fireteam_spawn_perms.Text = memory_process.read_int16(object_address + 0x2).ToString();

            meg_obj_data.waypoint_token_1_1.Text = memory_process.read_int8(object_address + 0x4).ToString();
            meg_obj_data.waypoint_token_1_2.Text = memory_process.read_int8(object_address + 0x5).ToString();
            meg_obj_data.waypoint_token_1_3.Text = memory_process.read_int8(object_address + 0x6).ToString();
            meg_obj_data.waypoint_token_1_4.Text = memory_process.read_int8(object_address + 0x7).ToString();

            meg_obj_data.waypoint_token_2_1.Text = memory_process.read_int8(object_address + 0x8).ToString();
            meg_obj_data.waypoint_token_2_2.Text = memory_process.read_int8(object_address + 0x9).ToString();
            meg_obj_data.waypoint_token_2_3.Text = memory_process.read_int8(object_address + 0xA).ToString();
            meg_obj_data.waypoint_token_2_4.Text = memory_process.read_int8(object_address + 0xB).ToString();

            meg_obj_data.waypoint_string_index.Text = memory_process.read_int16(object_address + 0xC).ToString();

            meg_obj_data.waypoint_icon.Text = memory_process.read_int16(object_address + 0xE).ToString();

            meg_obj_data.waypoint_vis_type.Text = memory_process.read_int32(object_address + 0x10).ToString();
            meg_obj_data.waypoint_vis_players.Text = memory_process.read_int32(object_address + 0x14).ToString();

            meg_obj_data.waypoint_priority.Text = memory_process.read_int8(object_address + 0x18).ToString();
            meg_obj_data.waypoint_timer.Text = memory_process.read_int8(object_address + 0x19).ToString();
            meg_obj_data.waypoint_min_range.Text = memory_process.read_int8(object_address + 0x1A).ToString();
            meg_obj_data.waypoint_max_range.Text = memory_process.read_int8(object_address + 0x1B).ToString();

            meg_obj_data.pickup_perms_type.Text = memory_process.read_int32(object_address + 0x1C).ToString();
            meg_obj_data.pickup_perms_players.Text = memory_process.read_int32(object_address + 0x20).ToString();

            meg_obj_data.spawn_perms_type.Text = memory_process.read_int32(object_address + 0x24).ToString();
            meg_obj_data.spawn_perms_players.Text = memory_process.read_int32(object_address + 0x28).ToString();

            meg_obj_data.boundary_vis_perms_type.Text = memory_process.read_int32(object_address + 0x2C).ToString();
            meg_obj_data.boundary_vis_perms_players.Text = memory_process.read_int32(object_address + 0x30).ToString();
            meg_obj_data.boundary_player_color.Text = memory_process.read_int32(object_address + 0x34).ToString();

            meg_obj_data.object_identifier.Text = memory_process.read_int32(object_address + 0x38).ToString();

            meg_obj_data.object_object0.Text = memory_process.read_int32(object_address + 0x3C).ToString();
            meg_obj_data.object_object1.Text = memory_process.read_int32(object_address + 0x40).ToString();
            meg_obj_data.object_object2.Text = memory_process.read_int32(object_address + 0x44).ToString();
            meg_obj_data.object_object3.Text = memory_process.read_int32(object_address + 0x48).ToString();

            meg_obj_data.object_team0.Text = memory_process.read_int8(object_address + 0x4C).ToString();
            meg_obj_data.object_team1.Text = memory_process.read_int8(object_address + 0x4D).ToString();

            meg_obj_data.object_number0.Text = memory_process.read_int16(object_address + 0x4E).ToString();
            meg_obj_data.object_number1.Text = memory_process.read_int16(object_address + 0x50).ToString();
            meg_obj_data.object_number2.Text = memory_process.read_int16(object_address + 0x52).ToString();
            meg_obj_data.object_number3.Text = memory_process.read_int16(object_address + 0x54).ToString();
            meg_obj_data.object_number4.Text = memory_process.read_int16(object_address + 0x56).ToString();
            meg_obj_data.object_number5.Text = memory_process.read_int16(object_address + 0x58).ToString();
            meg_obj_data.object_number6.Text = memory_process.read_int16(object_address + 0x5A).ToString();
            meg_obj_data.object_number7.Text = memory_process.read_int16(object_address + 0x5C).ToString();

            meg_obj_data.unknown_2byte.Text = memory_process.read_int16(object_address + 0x5E).ToString();

            meg_obj_data.object_timer0.Text = memory_process.read_int24(object_address + 0x60).ToString();
            meg_obj_data.object_timer0_rate.Text = memory_process.read_int8(object_address + 0x63).ToString();

            meg_obj_data.object_timer1.Text = memory_process.read_int24(object_address + 0x64).ToString();
            meg_obj_data.object_timer1_rate.Text = memory_process.read_int8(object_address + 0x67).ToString();

            meg_obj_data.object_timer2.Text = memory_process.read_int24(object_address + 0x68).ToString();
            meg_obj_data.object_timer2_rate.Text = memory_process.read_int8(object_address + 0x6B).ToString();

            meg_obj_data.object_timer3.Text = memory_process.read_int24(object_address + 0x6C).ToString();
            meg_obj_data.object_timer3_rate.Text = memory_process.read_int8(object_address + 0x6F).ToString();

            meg_obj_data.object_player0.Text = memory_process.read_int8(object_address + 0x70).ToString();
            meg_obj_data.object_player1.Text = memory_process.read_int8(object_address + 0x71).ToString();
            meg_obj_data.object_player2.Text = memory_process.read_int8(object_address + 0x72).ToString();
            meg_obj_data.object_player3.Text = memory_process.read_int8(object_address + 0x73).ToString();

            meg_obj_data.unknown_timer.Text = memory_process.read_int32(object_address + 0x74).ToString();

            meg_obj_data.progress_bar_perms_type.Text = memory_process.read_int32(object_address + 0x78).ToString();
            meg_obj_data.progress_bar_perms_players.Text = memory_process.read_int32(object_address + 0x7C).ToString();

            meg_obj_data.unknown_4byte.Text = memory_process.read_int32(object_address + 0x80).ToString();
            
            return meg_obj_data;
        }

        public int write_count = 0;
        public void notify_successfull_write()
        {
            write_count++;
            debug_text.Text = "Successfully wrote (" + write_count + "times)";
        }
    }
}
