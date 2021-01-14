using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using TrophyParser.Models;

namespace TrophyParser.Vita
{
    
    public class TRPTITLE
    {
        private BinaryReader _reader;
        private BinaryWriter _writer;
        private long pointer = -1;
        public List<TrophyInfo> _trophies = new List<TrophyInfo>();
        private string _path;

        public TrophyInfo this[int index] => _trophies[index]; 
        private byte[] Block
        {
            get
            {
                if (pointer == -1)
                {
                    _reader.BaseStream.Position = 0;
                    var pos = _reader.ReadBytes((int) _reader.BaseStream.Length).Search
                    (
                        "50000000000000000000000000000000".ToBytes(), 2
                    );
                    if(pos == -1) throw new Exception("Can't find TrophyBloack");
                    pointer = pos + 16;
                    _reader.BaseStream.Position = pointer;
                    return _reader.ReadBytes(25);
                    
                }
                _reader.BaseStream.Position += 71;
                return _reader.BaseStream.Position == _reader.BaseStream.Length ? null : _reader.ReadBytes(25);
            }
            set
            {
                _writer.Write(value);
                _writer.BaseStream.Position += 71;
            }
            
        }
        
        public TRPTITLE(string path)
        {
            _path = path;
            _reader = new BinaryReader(new FileStream(path + "TRPTITLE.DAT", FileMode.Open));
            var block = Block;
            do
            {
                var time = block.Skip(9).Take(8).ToArray();
                Array.Reverse(time);
                ulong t = BitConverter.ToUInt64(time);
                _trophies.Add( new TrophyInfo
                {
                    Time = new DateTime().AddMilliseconds(t/1000),
                    Unknown = block[3]
                    
                });
                block = Block;
            } while (block.Any());
            _reader.Close();
        }

        public void PuTrophy(int id, DateTime time)
        {
            _trophies[id].Time = time;
            _trophies[id].Unknown = 0x50;
            //TODO: Change Progress
        }

        public void PopTrophy(int id)
        {
            if (_trophies[id].IsSync) throw new Exception("Can't delete sync trophies");
            _trophies[id].Time = null;
            _trophies[id].Type = 0;
            //TODO: Change Progress
        }
        public void ChangeTime(int id, DateTime time) => _trophies[id].Time = time;
        
        public void Save()
        {
            _writer = new BinaryWriter(new FileStream(_path + "TRPTITLE.DAT", FileMode.Open));
            _writer.BaseStream.Position = pointer;
            foreach (var trophy in _trophies)
            {
                var  data = new List<byte>();
                data.Add((byte) (trophy.IsUnlock ? 0x01 : 0x00));
                data.AddRange(new byte[]{0,0,trophy.Unknown, 0, 0, 0, 0, 0 });
                var time = trophy.Time.HasValue ? BitConverter.GetBytes(trophy.Time.Value.Ticks / 10) : BitConverter.GetBytes((long)0);
                Array.Reverse(time);
                data.AddRange(time);
                data.AddRange(new byte[]{0,0,0,0,0,0,0,0});
                Block = data.ToArray();
            }
            _writer.Flush();
            _writer.Close();
        }

    }
}