using System;
using System.Collections.Generic;
using System.Text;

namespace MPlayerFront
{
    public class MFHelper
    {
        public static string NormalizeString(string val)
        {
            string newVal = val.Replace("\n", "");

            newVal = val.Replace("\r", "");

            return newVal.Trim();
        }

        public static string NormalizeFileName(string playFile)
        {
            string file = playFile;

            for (int i = 0; i < Program.Options.EscapeChars.Chars.Length; i += 2)
            {
                file = file.Replace(Program.Options.EscapeChars.Chars[i],
                    Program.Options.EscapeChars.Chars[i + 1]);
            }

            string result = file.Replace(" ", "\\ ");

            return result;
        }
    }
}
