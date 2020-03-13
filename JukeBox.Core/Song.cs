using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace JukeBox.Core
{
    public class Song
    {
        public string Title { get; set; }
        public string Author { get; set; }
        public string Artist { get; set; }
        public Stream Track { get; set; }
    }
}
