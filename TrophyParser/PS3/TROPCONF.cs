using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml;
using TrophyParser.Models;

namespace TrophyParser.PS3
{
    /// <summary>
    /// From https://github.com/darkautism/TROPHYParser
    /// Refactor by DarkNacho
    /// </summary>
    public class TROPCONF : SFMParse
    {

        public override SFMHearder Header { get; }
        private List<Trophy> _trophies;
        public override Trophy this[int index] => _trophies[index];
        public override bool HasPlatinum { get; set; }
        public override int Count { get; }
    
        public TROPCONF(string path)
        {
            FileStream reader = null;
            if (path == null) throw new Exception("Path cannot be null!");
            if (!File.Exists(path + "TROPCONF.SFM"))
                throw new Exception("Cannot find TROPCONF.SFM.");
            try
            {
                Header = new SFMHearder();
                reader = new FileStream(path + "TROPCONF.SFM", FileMode.Open);
                reader.Position = 0x40;
                _trophies = Parse(reader).ToList();
                Count = _trophies.Count;
            }
            catch (IOException)
            {
                throw new Exception("Cannot Open TROPCONF.SFM.");
            }
        }
    }
}