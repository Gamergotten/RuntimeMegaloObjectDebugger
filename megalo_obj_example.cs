using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RuntimeMegaloObjectDebugger
{
    internal class megalo_obj_example
    {
        struct string_token // 4 bytes
        {
            byte param3;
            byte param2;
            byte param1;
            byte var_type; // is two incremented from what it would be in megalo code
        }
        struct player_set // 8 bytes
        {
            int player_set_type;
            int set_players;
        }
        struct obj_ref // 4 bytes
        {
            int obj_ID;
        }
        struct team_ref // 1 byte
        {
            byte team_ID;
        }
        struct timer_ref // 4 bytes
        {
            // commented so i dont get errors in my code, int24 isn't real
            // int24 time; // is multiplied by 1200 of the timers actual number
            byte rate; // enum type
        }
        struct player_ref // 1 byte
        {
            byte player_ID;
        }

        class scripted_obj // 132 bytes
        {
            short index; // 0x00

            short fireteam_spawn_perms;   // 0x02

            string_token waypoint_token1; // 0x04
            string_token waypoint_token2; // 0x08
            short waypoint_string_index;  // 0x0C

            short waypoint_icon;     // 0x0E
            player_set waypoint_vis; // 0x10
            byte waypoint_priority;  // 0x18
            byte waypoint_timer;     // 0x19
            byte waypoint_min_range; // 0x1A
            byte waypoint_max_range; // 0x1B

            player_set pickup_perms; // 0x1C

            player_set spawn_perms;  // 0x24

            player_set boundary_vis_perms; // 0x2C
            int boundary_player_color;   // 0x34

            obj_ref object_identifier;     // 0x38
                                           // 4 nested objects
            obj_ref object_object0; // 0x3C
            obj_ref object_object1; // 0x40
            obj_ref object_object2; // 0x44
            obj_ref object_object3; // 0x48
                                    // 2 nested teams
            team_ref object_team0;  // 0x4C
            team_ref object_team1;  // 0x4D
                                    // 8 nested numbers
            short object_number0;   // 0x4E
            short object_number1;   // 0x50
            short object_number2;   // 0x52
            short object_number3;   // 0x54
            short object_number4;   // 0x56
            short object_number5;   // 0x58
            short object_number6;   // 0x5A
            short object_number7;   // 0x5C
            short unknown2byte;     // 0x5E
                                    // 4 nested timers
            timer_ref object_timer0;   // 0x60
            timer_ref object_timer1;   // 0x64
            timer_ref object_timer2;   // 0x68
            timer_ref object_timer3;   // 0x6C
                                       // 4 nested players
            player_ref object_player0; // 0x70
            player_ref object_player1; // 0x71
            player_ref object_player2; // 0x72
            player_ref object_player3; // 0x73

            int tick_thing;          // 0x74

            player_set progress_bar_perms; // 0x78

            int unknown_or_padding;      // 0x80
        }


    }
}
