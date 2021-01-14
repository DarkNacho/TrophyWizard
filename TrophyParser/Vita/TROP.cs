using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using TrophyParser.Models;

namespace TrophyParser.Vita
{
  
    public class TROP : SFMParse
    {
        public override SFMHearder Header { get; }
        private List<Trophy> _trophies;
        public override Trophy this[int index] => _trophies[index];
        public override bool HasPlatinum { get; set; }
        public override int Count { get; }
        public TROP(string path)
        {
            FileStream reader = null;
            if (path == null) throw new Exception("Path cannot be null!");
            if (!File.Exists(path + "TROP.SFM"))
                throw new Exception("Cannot find TROP.SFM.");
            try
            {
                Header = new SFMHearder();
                reader = new FileStream(path + "TROP.SFM", FileMode.Open);
                reader.Position = 0x0;
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