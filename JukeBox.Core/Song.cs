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
        public int TrackNumber { get; set; }
        public IEnumerable<Kentico.Kontent.Delivery.Asset> Track { get; set; }
    }
}
