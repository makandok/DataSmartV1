using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace ExcelToAndroidXML
{
    static class MyExtensions
    {
        public static string getStringsEntry(this string stringsEntryName, string stringsEntryText)
        {
            return ("<string name='" + stringsEntryName + "'>" + stringsEntryText + "</string>").Replace("'", "\"");
        }

        public static string Clean(this string value)
        {
            //=SUBSTITUTE( SUBSTITUTE( LOWER(I7)," ",""),"/","_")
            return Regex.Replace(value, "[^a-zA-Z0-9]", "").ToLowerInvariant();
        }
    }
}
