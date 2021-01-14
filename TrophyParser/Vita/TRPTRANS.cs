using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using TrophyParser.Models;

namespace TrophyParser.Vita
{
    public class TRPTRANS
    {
        private BinaryReader _reader;
        private BinaryWriter _writer;
        private const int _pointer = 0x377;
        public List<TrophyInfo> _trophies = new List<TrophyInfo>();
        private string _path;
        public TrophyInfo this[int index] => _trophies[index]; 
        private byte[] Block
        {
            get
            {
                var block = _reader.BaseStream.Position == _reader.BaseStream.Length ? null : _reader.ReadBytes(57);
                _reader.BaseStream.Position += 119;
                return block;
            }
        }

        public TRPTRANS(string path, int count)
        {
            _path = path;
            _reader = new BinaryReader(new FileStream(path + "TRPTRANS.DAT", FileMode.Open));
            _reader.BaseStream.Position = _pointer;
            for (int i = 0; i < count; ++i)
            {
                var block = Block;
                var time = block.Skip(41).Take(8).ToArray();
                Array.Reverse(time);
                ulong t = BitConverter.ToUInt64(time);
                _trophies.Add(new TrophyInfo
                {
                    IsUnlock = block[0] == 0x02,
                    Time = new DateTime().AddMilliseconds(t / 1000),
                    Unknown = block[35]
                });
            }
            _reader.Close();
        }

        private byte GetTrophyType(string type)
        {
            return type[0] switch
            {
                'P' => 0x01,
                'G' => 0x02,
                'S' => 0x03,
                'B' => 0x04,
                _ => 0x0
            };
        }
        public void Unlock(int id, string type, DateTime time)
        {
            _trophies[id].Type = GetTrophyType(type);
            _trophies[id].Time = time;
            _trophies[id].Unknown = 0x50;
            _trophies[id].IsUnlock = true;
            //TODO: Change Progress
        }
        public void ChangeTime(int id, DateTime time) => _trophies[id].Time = time;

        public void LockTropy(int id)
        {
            _trophies[id].IsUnlock = false;
            _trophies[id].Type = 0x00;
            //TODO: set time to 0
            //TODO: Change Progress
        }

        public void Save()
        {
            _writer = new BinaryWriter(new FileStream(_path + "TRPTRANS.DAT", FileMode.Open));
            _writer.BaseStream.Position = _pointer;
            foreach (var trophy in _trophies)
            {
                
                _writer.Write((byte) (trophy.IsUnlock ? 0x02 : 0x00));
                var time = BitConverter.GetBytes(trophy.Time.Ticks/10);
                Array.Reverse(time);
                _writer.BaseStream.Position += 31;
                _writer.Write(trophy.Type);
                _writer.BaseStream.Position += 2;
                _writer.Write(trophy.Unknown);
                _writer.BaseStream.Position += 5;
                _writer.Write(time);
                _writer.Write(new byte[]{0,0,0,0,0,0,0,0});
                _writer.BaseStream.Position += 119;
            }
            _writer.Flush();
            _writer.Close();
        }
    }
}