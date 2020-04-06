// license:BSD-3-Clause
// copyright-holders:Edward Fast

using System;
using System.Collections.Generic;


namespace mame.plib
{
    public class pstrutil_global
    {
        public static bool endsWith(string str, string value) { return str.EndsWith(value); }
        public static string ucase(string str) { return str.ToUpper(); }
        public static string trim(string str) { return str.Trim(); }
        public static string left(string str, int len) { return str.Substring(0, len); }
        public static string replace_all(string str, string search, string replace) { return str.Replace(search, replace); }
    }
}
