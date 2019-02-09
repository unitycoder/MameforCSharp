// license:BSD-3-Clause
// copyright-holders:Edward Fast

using System;
using System.Collections.Generic;


namespace mame
{
    public static class dualhsxs_global
    {
        static readonly byte [] layout_dualhsxs_data =
        {
            120, 156, 189, 147, 177, 110, 194,  48,  16, 134, 247,  62, 197, 233,  38,  24,  76,  18, 194, 208,  33,  14,   3,  72,  29,  59,  32,  30, 192, 144,  83,  99,
            225, 216, 200, 118,  72, 242, 246,  56,  80,   9, 169, 173, 146, 150,  74, 153, 108, 223,  47, 255, 250, 238,  63,  93, 182, 110,  43,   5,  23, 178,  78,  26,
            205,  49,  89, 196, 184, 206, 179,  74,  84, 164,  68, 103, 106, 255, 144, 150, 152, 103,  23,  73,  13, 232,  32, 114, 220, 214,  66, 193,  78,  22, 196,  14,
             29, 235, 207,  32, 187, 163,  37, 210,  32, 117,  65,  45, 199,  56,  84,  14, 166, 214, 133, 131, 174, 127, 193, 173,   6,  37, 201, 143, 210, 115,  76,  17,
             26,  89, 248, 146, 227,  10, 163,  60, 139, 238, 159, 191, 154,  36,  63, 152, 172,  22, 113,  58, 238,  19, 245, 176, 195, 200,  48, 123,  19, 103,  69, 206,
            205,  39, 132, 127, 154, 252,  61, 140, 130, 237, 131, 179, 157, 130, 150, 165, 183, 148, 127, 103,  52,  74,  60, 109, 210, 236,  31, 224,  27, 115,  60, 121,
             33, 213,  55, 206, 191, 199,  99, 172,  36, 237, 133,  15, 235,   3, 214, 132,  75, 176,  79,  94, 227, 129,  54, 158, 204, 226, 179, 139, 232, 177, 182, 249,
            203,  21, 192, 215,  52, 230
        };


        static readonly internal_layout layout_dualhsxs = new internal_layout
        (
            985, layout_dualhsxs_data.Length, 1, layout_dualhsxs_data
        );
    }
}
