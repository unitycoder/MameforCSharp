// license:BSD-3-Clause
// copyright-holders:Edward Fast

using System;
using System.Collections.Generic;


namespace mame
{
    public static class cmddata_global
    {
        static readonly rgb_t BUTTON_COLOR_RED        = new rgb_t(255,64,64);
        static readonly rgb_t BUTTON_COLOR_YELLOW     = new rgb_t(255,238,0);
        static readonly rgb_t BUTTON_COLOR_GREEN      = new rgb_t(0,255,64);
        static readonly rgb_t BUTTON_COLOR_BLUE       = new rgb_t(0,170,255);
        static readonly rgb_t BUTTON_COLOR_PURPLE     = new rgb_t(170,0,255);
        static readonly rgb_t BUTTON_COLOR_PINK       = new rgb_t(255,0,170);
        static readonly rgb_t BUTTON_COLOR_AQUA       = new rgb_t(0,255,204);
        static readonly rgb_t BUTTON_COLOR_SILVER     = new rgb_t(255,0,255);
        static readonly rgb_t BUTTON_COLOR_NAVY       = new rgb_t(255,160,0);
        static readonly rgb_t BUTTON_COLOR_LIME       = new rgb_t(190,190,190);


        // command.dat symbols assigned to Unicode PUA U+E000
        public const UInt32 COMMAND_UNICODE = 0xe000;
        public const UInt32 MAX_GLYPH_FONT = 150;


        // Define Game Command Font Converting Conditions
        public const char COMMAND_DEFAULT_TEXT    = '_';

        // Define Expanded Game Command ShortCut
        public const char COMMAND_EXPAND_TEXT     = '^';

        // Define Simple Game Command ShortCut
        public const char COMMAND_CONVERT_TEXT    = '@';

        // Defined Game Command Font Color Array
        public static readonly rgb_t [] color_table =
        {
            null,                // dummy
            BUTTON_COLOR_RED,    // BTN_A
            BUTTON_COLOR_YELLOW, // BTN_B
            BUTTON_COLOR_GREEN,  // BTN_C
            BUTTON_COLOR_BLUE,   // BTN_D
            BUTTON_COLOR_PINK,   // BTN_E
            BUTTON_COLOR_PURPLE, // BTN_F
            BUTTON_COLOR_AQUA,   // BTN_G
            BUTTON_COLOR_SILVER, // BTN_H
            BUTTON_COLOR_NAVY,   // BTN_I
            BUTTON_COLOR_LIME,   // BTN_J
            BUTTON_COLOR_RED,    // BTN_K
            BUTTON_COLOR_YELLOW, // BTN_L
            BUTTON_COLOR_GREEN,  // BTN_M
            BUTTON_COLOR_BLUE,   // BTN_N
            BUTTON_COLOR_PINK,   // BTN_O
            BUTTON_COLOR_PURPLE, // BTN_P
            BUTTON_COLOR_AQUA,   // BTN_Q
            BUTTON_COLOR_SILVER, // BTN_R
            BUTTON_COLOR_NAVY,   // BTN_S
            BUTTON_COLOR_LIME,   // BTN_T
            BUTTON_COLOR_RED,    // BTN_U
            BUTTON_COLOR_YELLOW, // BTN_V
            BUTTON_COLOR_GREEN,  // BTN_W
            BUTTON_COLOR_BLUE,   // BTN_X
            BUTTON_COLOR_PINK,   // BTN_Y
            BUTTON_COLOR_PURPLE, // BTN_Z
            BUTTON_COLOR_RED,    // BTN_1
            BUTTON_COLOR_YELLOW, // BTN_2
            BUTTON_COLOR_GREEN,  // BTN_3
            BUTTON_COLOR_BLUE,   // BTN_4
            BUTTON_COLOR_PINK,   // BTN_5
            BUTTON_COLOR_PURPLE, // BTN_6
            BUTTON_COLOR_AQUA,   // BTN_7
            BUTTON_COLOR_SILVER, // BTN_8
            BUTTON_COLOR_NAVY,   // BTN_9
            BUTTON_COLOR_LIME,   // BTN_10
            BUTTON_COLOR_BLUE,   // BTN_DEC
            BUTTON_COLOR_RED,    // BTN_INC
            null,                // BTN_+
            null,                // DIR_...
            null,                // DIR_1
            null,                // DIR_2
            null,                // DIR_3
            null,                // DIR_4
            BUTTON_COLOR_RED,    // Joystick Ball
            null,                // DIR_6
            null,                // DIR_7
            null,                // DIR_8
            null,                // DIR_9
            null,                // DIR_N
            BUTTON_COLOR_RED,    // BTN_START
            BUTTON_COLOR_YELLOW, // BTN_SELECT
            BUTTON_COLOR_PINK,   // BTN_PUNCH
            BUTTON_COLOR_PURPLE, // BTN_KICK
            BUTTON_COLOR_BLUE,   // BTN_GUARD
            null,
            BUTTON_COLOR_YELLOW, // Light Punch
            BUTTON_COLOR_NAVY,   // Middle Punch
            BUTTON_COLOR_RED,    // Strong Punch
            BUTTON_COLOR_LIME,   // Light Kick
            BUTTON_COLOR_AQUA,   // Middle Kick
            BUTTON_COLOR_BLUE,   // Strong Kick
            BUTTON_COLOR_PURPLE, // 3 Kick
            BUTTON_COLOR_PINK,   // 3 Punch
            BUTTON_COLOR_PURPLE, // 2 Kick
            BUTTON_COLOR_PINK,   // 2 Punch
            BUTTON_COLOR_RED,    // CUSTOM_1
            BUTTON_COLOR_YELLOW, // CUSTOM_2
            BUTTON_COLOR_GREEN,  // CUSTOM_3
            BUTTON_COLOR_BLUE,   // CUSTOM_4
            BUTTON_COLOR_PINK,   // CUSTOM_5
            BUTTON_COLOR_PURPLE, // CUSTOM_6
            BUTTON_COLOR_AQUA,   // CUSTOM_7
            BUTTON_COLOR_SILVER, // CUSTOM_8
            BUTTON_COLOR_RED,    // (Cursor Up)
            BUTTON_COLOR_YELLOW, // (Cursor Down)
            BUTTON_COLOR_GREEN,  // (Cursor Left)
            BUTTON_COLOR_BLUE,   // (Cursor Right)
            null,                // Non Player Lever
            BUTTON_COLOR_LIME,   // Gray Color Lever
            BUTTON_COLOR_RED,    // 1 Player Lever
            BUTTON_COLOR_YELLOW, // 2 Player Lever
            BUTTON_COLOR_GREEN,  // 3 Player Lever
            BUTTON_COLOR_BLUE,   // 4 Player Lever
            BUTTON_COLOR_PINK,   // 5 Player Lever
            BUTTON_COLOR_PURPLE, // 6 Player Lever
            BUTTON_COLOR_AQUA,   // 7 Player Lever
            BUTTON_COLOR_SILVER  // 8 Player Lever
        };

        public static readonly int COLOR_BUTTONS = color_table.Length;


        public static readonly fix_command_t [] default_text =
        {
            // Alphabetic Buttons (NeoGeo): A~D,H,Z
            new fix_command_t( 'A', 1 ),     // BTN_A
            new fix_command_t( 'B', 2 ),     // BTN_B
            new fix_command_t( 'C', 3 ),     // BTN_C
            new fix_command_t( 'D', 4 ),     // BTN_D
            new fix_command_t( 'H', 8 ),     // BTN_H
            new fix_command_t( 'Z', 26 ),    // BTN_Z
            // Numerical Buttons (Capcom): 1~10
            new fix_command_t( 'a', 27 ),    // BTN_1
            new fix_command_t( 'b', 28 ),    // BTN_2
            new fix_command_t( 'c', 29 ),    // BTN_3
            new fix_command_t( 'd', 30 ),    // BTN_4
            new fix_command_t( 'e', 31 ),    // BTN_5
            new fix_command_t( 'f', 32 ),    // BTN_6
            new fix_command_t( 'g', 33 ),    // BTN_7
            new fix_command_t( 'h', 34 ),    // BTN_8
            new fix_command_t( 'i', 35 ),    // BTN_9
            new fix_command_t( 'j', 36 ),    // BTN_10
            // Directions of Arrow, Joystick Ball
            new fix_command_t( '+', 39 ),    // BTN_+
            new fix_command_t( '.', 40 ),    // DIR_...
            new fix_command_t( '1', 41 ),    // DIR_1
            new fix_command_t( '2', 42 ),    // DIR_2
            new fix_command_t( '3', 43 ),    // DIR_3
            new fix_command_t( '4', 44 ),    // DIR_4
            new fix_command_t( '5', 45 ),    // Joystick Ball
            new fix_command_t( '6', 46 ),    // DIR_6
            new fix_command_t( '7', 47 ),    // DIR_7
            new fix_command_t( '8', 48 ),    // DIR_8
            new fix_command_t( '9', 49 ),    // DIR_9
            new fix_command_t( 'N', 50 ),    // DIR_N
            // Special Buttons
            new fix_command_t( 'S', 51 ),    // BTN_START
            new fix_command_t( 'P', 53 ),    // BTN_PUNCH
            new fix_command_t( 'K', 54 ),    // BTN_KICK
            new fix_command_t( 'G', 55 ),    // BTN_GUARD
            // Composition of Arrow Directions
            new fix_command_t( '!',  90 ),   // Arrow
            new fix_command_t( 'k', 100 ),   // Half Circle Back
            new fix_command_t( 'l', 101 ),   // Half Circle Front Up
            new fix_command_t( 'm', 102 ),   // Half Circle Front
            new fix_command_t( 'n', 103 ),   // Half Circle Back Up
            new fix_command_t( 'o', 104 ),   // 1/4 Cir For 2 Down
            new fix_command_t( 'p', 105 ),   // 1/4 Cir Down 2 Back
            new fix_command_t( 'q', 106 ),   // 1/4 Cir Back 2 Up
            new fix_command_t( 'r', 107 ),   // 1/4 Cir Up 2 For
            new fix_command_t( 's', 108 ),   // 1/4 Cir Back 2 Down
            new fix_command_t( 't', 109 ),   // 1/4 Cir Down 2 For
            new fix_command_t( 'u', 110 ),   // 1/4 Cir For 2 Up
            new fix_command_t( 'v', 111 ),   // 1/4 Cir Up 2 Back
            new fix_command_t( 'w', 112 ),   // Full Clock Forward
            new fix_command_t( 'x', 113 ),   // Full Clock Back
            new fix_command_t( 'y', 114 ),   // Full Count Forward
            new fix_command_t( 'z', 115 ),   // Full Count Back
            new fix_command_t( 'L', 116 ),   // 2x Forward
            new fix_command_t( 'M', 117 ),   // 2x Back
            new fix_command_t( 'Q', 118 ),   // Dragon Screw Forward
            new fix_command_t( 'R', 119 ),   // Dragon Screw Back
            // Big letter Text
            new fix_command_t( '^', 121 ),   // AIR
            new fix_command_t( '?', 122 ),   // DIR
            new fix_command_t( 'X', 124 ),   // TAP
            // Condition of Positions
            new fix_command_t( '|', 125 ),   // Jump
            new fix_command_t( 'O', 126 ),   // Hold
            new fix_command_t( '-', 127 ),   // Air
            new fix_command_t( '=', 128 ),   // Squatting
            new fix_command_t( '~', 131 ),   // Charge
            // Special Character Text
            new fix_command_t( '`', 135 ),   // Small Dot
            new fix_command_t( '@', 136 ),   // Double Ball
            new fix_command_t( ')', 137 ),   // Single Ball
            new fix_command_t( '(', 138 ),   // Solid Ball
            new fix_command_t( '*', 139 ),   // Star
            new fix_command_t( '&', 140 ),   // Solid star
            new fix_command_t( '%', 141 ),   // Triangle
            new fix_command_t( '$', 142 ),   // Solid Triangle
            new fix_command_t( '#', 143 ),   // Double Square
            new fix_command_t( ']', 144 ),   // Single Square
            new fix_command_t( '[', 145 ),   // Solid Square
            new fix_command_t( '{', 146 ),   // Down Triangle
            new fix_command_t( '}', 147 ),   // Solid Down Triangle
            new fix_command_t( '<', 148 ),   // Diamond
            new fix_command_t( '>', 149 ),   // Solid Diamond
            new fix_command_t( '\0', 0 )    // end of array
        };


        public static readonly fix_command_t [] expand_text =
        {
            // Alphabetic Buttons (NeoGeo): S (Slash Button)
            new fix_command_t( 's', 19 ),    // BTN_S
            // Special Buttons
            new fix_command_t( 'S', 52 ),    // BTN_SELECT
            // Multiple Punches & Kicks
            new fix_command_t( 'E', 57 ),    // Light  Punch
            new fix_command_t( 'F', 58 ),    // Middle Punch
            new fix_command_t( 'G', 59 ),    // Strong Punch
            new fix_command_t( 'H', 60 ),    // Light  Kick
            new fix_command_t( 'I', 61 ),    // Middle Kick
            new fix_command_t( 'J', 62 ),    // Strong Kick
            new fix_command_t( 'T', 63 ),    // 3 Kick
            new fix_command_t( 'U', 64 ),    // 3 Punch
            new fix_command_t( 'V', 65 ),    // 2 Kick
            new fix_command_t( 'W', 66 ),    // 2 Pick
            // Composition of Arrow Directions
            new fix_command_t( '!', 91 ),    // Continue Arrow
            // Charge of Arrow Directions
            new fix_command_t( '1', 92 ),    // Charge DIR_1
            new fix_command_t( '2', 93 ),    // Charge DIR_2
            new fix_command_t( '3', 94 ),    // Charge DIR_3
            new fix_command_t( '4', 95 ),    // Charge DIR_4
            new fix_command_t( '6', 96 ),    // Charge DIR_6
            new fix_command_t( '7', 97 ),    // Charge DIR_7
            new fix_command_t( '8', 98 ),    // Charge DIR_8
            new fix_command_t( '9', 99 ),    // Charge DIR_9
            // Big letter Text
            new fix_command_t( 'M', 123 ),   // MAX
            // Condition of Positions
            new fix_command_t( '-', 129 ),   // Close
            new fix_command_t( '=', 130 ),   // Away
            new fix_command_t( '*', 132 ),   // Serious Tap
            new fix_command_t( '?', 133 ),   // Any Button
            new fix_command_t( '\0', 0 )    // end of array
        };


        public static readonly fix_strings_t [] convert_text =
        {
            // Alphabetic Buttons: A~Z
            new fix_strings_t( "A-button",  1, 0 ), // BTN_A
            new fix_strings_t( "B-button",  2, 0 ), // BTN_B
            new fix_strings_t( "C-button",  3, 0 ), // BTN_C
            new fix_strings_t( "D-button",  4, 0 ), // BTN_D
            new fix_strings_t( "E-button",  5, 0 ), // BTN_E
            new fix_strings_t( "F-button",  6, 0 ), // BTN_F
            new fix_strings_t( "G-button",  7, 0 ), // BTN_G
            new fix_strings_t( "H-button",  8, 0 ), // BTN_H
            new fix_strings_t( "I-button",  9, 0 ), // BTN_I
            new fix_strings_t( "J-button", 10, 0 ), // BTN_J
            new fix_strings_t( "K-button", 11, 0 ), // BTN_K
            new fix_strings_t( "L-button", 12, 0 ), // BTN_L
            new fix_strings_t( "M-button", 13, 0 ), // BTN_M
            new fix_strings_t( "N-button", 14, 0 ), // BTN_N
            new fix_strings_t( "O-button", 15, 0 ), // BTN_O
            new fix_strings_t( "P-button", 16, 0 ), // BTN_P
            new fix_strings_t( "Q-button", 17, 0 ), // BTN_Q
            new fix_strings_t( "R-button", 18, 0 ), // BTN_R
            new fix_strings_t( "S-button", 19, 0 ), // BTN_S
            new fix_strings_t( "T-button", 20, 0 ), // BTN_T
            new fix_strings_t( "U-button", 21, 0 ), // BTN_U
            new fix_strings_t( "V-button", 22, 0 ), // BTN_V
            new fix_strings_t( "W-button", 23, 0 ), // BTN_W
            new fix_strings_t( "X-button", 24, 0 ), // BTN_X
            new fix_strings_t( "Y-button", 25, 0 ), // BTN_Y
            new fix_strings_t( "Z-button", 26, 0 ), // BTN_Z
            // Special Moves and Buttons
            new fix_strings_t( "decrease", 37, 0 ), // BTN_DEC
            new fix_strings_t( "increase", 38, 0 ), // BTN_INC
            new fix_strings_t( "BALL",     45, 0 ),  // Joystick Ball
            new fix_strings_t( "start",    51, 0 ),   // BTN_START
            new fix_strings_t( "select",   52, 0 ), // BTN_SELECT
            new fix_strings_t( "punch",    53, 0 ),   // BTN_PUNCH
            new fix_strings_t( "kick",     54, 0 ),  // BTN_KICK
            new fix_strings_t( "guard",    55, 0 ),   // BTN_GUARD
            new fix_strings_t( "L-punch",  57, 0 ), // Light Punch
            new fix_strings_t( "M-punch",  58, 0 ), // Middle Punch
            new fix_strings_t( "S-punch",  59, 0 ), // Strong Punch
            new fix_strings_t( "L-kick",   60, 0 ), // Light Kick
            new fix_strings_t( "M-kick",   61, 0 ), // Middle Kick
            new fix_strings_t( "S-kick",   62, 0 ), // Strong Kick
            new fix_strings_t( "3-kick",   63, 0 ), // 3 Kick
            new fix_strings_t( "3-punch",  64, 0 ), // 3 Punch
            new fix_strings_t( "2-kick",   65, 0 ), // 2 Kick
            new fix_strings_t( "2-punch",  66, 0 ), // 2 Pick
            // Custom Buttons and Cursor Buttons
            new fix_strings_t( "custom1",  67, 0 ), // CUSTOM_1
            new fix_strings_t( "custom2",  68, 0 ), // CUSTOM_2
            new fix_strings_t( "custom3",  69, 0 ), // CUSTOM_3
            new fix_strings_t( "custom4",  70, 0 ), // CUSTOM_4
            new fix_strings_t( "custom5",  71, 0 ), // CUSTOM_5
            new fix_strings_t( "custom6",  72, 0 ), // CUSTOM_6
            new fix_strings_t( "custom7",  73, 0 ), // CUSTOM_7
            new fix_strings_t( "custom8",  74, 0 ), // CUSTOM_8
            new fix_strings_t( "up",       75, 0 ),    // (Cursor Up)
            new fix_strings_t( "down",     76, 0 ),  // (Cursor Down)
            new fix_strings_t( "left",     77, 0 ),  // (Cursor Left)
            new fix_strings_t( "right",    78, 0 ),   // (Cursor Right)
            // Player Lever
            new fix_strings_t( "lever",    79, 0 ),   // Non Player Lever
            new fix_strings_t( "nplayer",  80, 0 ), // Gray Color Lever
            new fix_strings_t( "1player",  81, 0 ), // 1 Player Lever
            new fix_strings_t( "2player",  82, 0 ), // 2 Player Lever
            new fix_strings_t( "3player",  83, 0 ), // 3 Player Lever
            new fix_strings_t( "4player",  84, 0 ), // 4 Player Lever
            new fix_strings_t( "5player",  85, 0 ), // 5 Player Lever
            new fix_strings_t( "6player",  86, 0 ), // 6 Player Lever
            new fix_strings_t( "7player",  87, 0 ), // 7 Player Lever
            new fix_strings_t( "8player",  88, 0 ), // 8 Player Lever
            // Composition of Arrow Directions
            new fix_strings_t( "-->",      90, 0 ), // Arrow
            new fix_strings_t( "==>",      91, 0 ), // Continue Arrow
            new fix_strings_t( "hcb",     100, 0 ), // Half Circle Back
            new fix_strings_t( "huf",     101, 0 ), // Half Circle Front Up
            new fix_strings_t( "hcf",     102, 0 ), // Half Circle Front
            new fix_strings_t( "hub",     103, 0 ), // Half Circle Back Up
            new fix_strings_t( "qfd",     104, 0 ), // 1/4 Cir For 2 Down
            new fix_strings_t( "qdb",     105, 0 ), // 1/4 Cir Down 2 Back
            new fix_strings_t( "qbu",     106, 0 ), // 1/4 Cir Back 2 Up
            new fix_strings_t( "quf",     107, 0 ), // 1/4 Cir Up 2 For
            new fix_strings_t( "qbd",     108, 0 ), // 1/4 Cir Back 2 Down
            new fix_strings_t( "qdf",     109, 0 ), // 1/4 Cir Down 2 For
            new fix_strings_t( "qfu",     110, 0 ), // 1/4 Cir For 2 Up
            new fix_strings_t( "qub",     111, 0 ), // 1/4 Cir Up 2 Back
            new fix_strings_t( "fdf",     112, 0 ), // Full Clock Forward
            new fix_strings_t( "fub",     113, 0 ), // Full Clock Back
            new fix_strings_t( "fuf",     114, 0 ), // Full Count Forward
            new fix_strings_t( "fdb",     115, 0 ), // Full Count Back
            new fix_strings_t( "xff",     116, 0 ), // 2x Forward
            new fix_strings_t( "xbb",     117, 0 ), // 2x Back
            new fix_strings_t( "dsf",     118, 0 ), // Dragon Screw Forward
            new fix_strings_t( "dsb",     119, 0 ), // Dragon Screw Back
            // Big letter Text
            new fix_strings_t( "AIR",     121, 0 ), // AIR
            new fix_strings_t( "DIR",     122, 0 ), // DIR
            new fix_strings_t( "MAX",     123, 0 ), // MAX
            new fix_strings_t( "TAP",     124, 0 ), // TAP
            // Condition of Positions
            new fix_strings_t( "jump",    125, 0 ),  // Jump
            new fix_strings_t( "hold",    126, 0 ),  // Hold
            new fix_strings_t( "air",     127, 0 ), // Air
            new fix_strings_t( "sit",     128, 0 ), // Squatting
            new fix_strings_t( "close",   129, 0 ), // Close
            new fix_strings_t( "away",    130, 0 ),  // Away
            new fix_strings_t( "charge",  131, 0 ), // Charge
            new fix_strings_t( "tap",     132, 0 ), // Serious Tap
            new fix_strings_t( "button",  133, 0 ), // Any Button
            new fix_strings_t( null,        0, 0 )    // end of array
        };
    }


    public class fix_command_t
    {
        public char glyph_char;
        public UInt32 glyph_code;

        public fix_command_t(char glyph_char, UInt32 glyph_code) { this.glyph_char = glyph_char; this.glyph_code = glyph_code; }
    }


    public class fix_strings_t
    {
        public string glyph_str;
        public int glyph_code;
        public UInt32 glyph_str_len;

        public fix_strings_t(string glyph_str, int glyph_code, UInt32 glyph_str_len) { this.glyph_str = glyph_str; this.glyph_code = glyph_code; this.glyph_str_len = glyph_str_len; }
    }
}
