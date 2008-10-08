using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Serialization;

namespace MPlayerFront
{
    [XmlRoot("Options")]
    public class MFOptions
    {
        public string Host = null;
        public string User = null;
        public string Password = null;
        public string PathToPLink = null;
        public bool UseSSH = true;
        public string KeyFile = null;
        public int Port = 22;

        public Commands Commands = new Commands();

        public Controls Controls = new Controls();

        public EscapeChars EscapeChars = new EscapeChars();
    }

    public class Commands
    {
        public string ListFiles = "ls -lL --time-style=+%F\\ %T";
        public string CDDirectory = "cd";
        public string CurrentDirectory = "pwd";
        public string StatFiletype = "stat -c%F";
    }

    public class Controls
    {
        public string PlayFile = "DISPLAY=:0.0 mplayer -quiet -fs -sid 0";

        [XmlIgnore]
        public string RealPlayFile
        {
            get
            {
                string cmd = PlayFile;

                if (Program.Options.Controls.Channel2 && !Program.Options.Controls.Channel1)
                {
                    cmd += (" -af channels=2:2:1:0:1:1");
                }
                else if (!Program.Options.Controls.Channel2 && Program.Options.Controls.Channel1)
                {
                    cmd += (" -af channels=1");
                }

                return cmd;
            }
        }

        public string PlayList = "DISPLAY=:0.0 mplayer -quiet -fs -playlist";
        public string Pause = "p";
        public string Stop = "q";
        public string VolumeUp = "*";
        public string VolumeDown = "/";
        public string AudioDelayInc = "+";
        public string AudioDelayDec = "-";
        public string SubtitleDelayInc = "x";
        public string SubtitleDelayDec = "z";
        public string PlayListForward = ">";
        public string PlayListBackwords = "<";
        public Seeking Seeking = new Seeking();

        [XmlIgnore]
        public bool Channel1 = true;
        [XmlIgnore]
        public bool Channel2 = true;
    }

    public class Seeking
    {
        //public string F10Secs = "\xE0\x4D";
        //public string B10Secs = "\xE0\x4B";
        //public string F1Mins = "\xE0\x48";
        //public string B1Mins = "\xE0\x50";
        //public string F10Mins = "\xE0\x49";
        //public string B10Mins = "\xE0\x51";
        public string F10Secs = "a";
        public string B10Secs = "c";
        public string F1Mins = "m";
        public string B1Mins = "g";
        public string F10Mins = "j";
        public string B10Mins = "k";
    }

    public class EscapeChars
    {
        public string[] Chars = 
        {
            "\\", "\\\\",
            "'", "\\'",
            "`", "\\`",
            "|", "\\|",
            "(", "\\(",
            ")", "\\)",
            "*", "\\*",
            "#", "\\#",
            "&", "\\&",
            "~", "\\~"
        };
    }
}
