using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using IND880.Advance.APIClient;

namespace 称重量提取
{
    public class global
    {
        public static string[] strdata = new string[4];
        public static string IP = "";
        public static int IP_pro = 0;
        public static APIClient mm;
        public static string[,] IOdata = new string[2,5];
        public static string IOyes;
        public static string UI;
    }
}
