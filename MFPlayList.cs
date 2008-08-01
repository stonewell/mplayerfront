using System;
using System.Collections.Generic;
using System.Text;

namespace MPlayerFront
{
    [System.Xml.Serialization.XmlRoot("PlayList")]
    public class MFPlayList
    {
        public List<string> Files = new List<string>();
        public string CurrentFile;
    }
}
