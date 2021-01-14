using System;
using System.Collections.Generic;
using TrophyParser.Models;

namespace TrophyParser.PS3
{
    /// <summary>
    /// Adapter for https://github.com/darkautism/PS3TrophyIsGood
    /// </summary>
    public class PS3Unlocker : IUnlocker
    {
        private List<Trophy> _trophies;
        private SFMParse _tconf;
        private TROPTRNS _tpsn;
        private TROPUSR _tusr;
        
        public Trophy this[int id] => _trophies[id];
        public int Count { get; }

        public PS3Unlocker(string path)
        {
            //TODO: Decrypt
            _tconf = new TROPCONF(path);
            _tpsn = new TROPTRNS(path);
            _tusr = new TROPUSR(path);
            Count = _tconf.Count;
            _trophies = new List<Trophy>();
            for (int i = 0; i < _trophies.Count; ++i)
            {
                var trophy = _tconf[i];
                if (_tpsn[i].HasValue)
                {
                    trophy.Time = _tpsn[i].Value.Time;
                    trophy.IsSync = _tpsn[i].Value.IsSync;
                    trophy.IsUnlock = true;
                }
                _trophies.Add(trophy);
            }
        }

        public void ChangeTime(int id, DateTime time)
        {
            _tpsn.ChangeTime(id, time);
            TROPUSR.TrophyTimeInfo tti = _tusr.trophyTimeInfoTable[id];
            tti.Time = time;
            _tusr.trophyTimeInfoTable[id] = tti;
        }
        public void UnlockTrophy(int id, DateTime time)
        {
            _tusr.UnlockTrophy(id, time);
            _tpsn.PutTrophy(id, _tusr.trophyTypeTable[id].Type, time);
        }
        
        public void LockTrophy(int id)
        {
            throw new NotImplementedException();
        }

        public void Save()
        {
            _tpsn.Save();
            _tusr.Save();
            //TODO: Encrypt
        }
    }
}