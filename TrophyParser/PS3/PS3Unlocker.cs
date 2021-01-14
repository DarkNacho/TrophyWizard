using System;
using System.Collections.Generic;
using System.Text;
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
            for (int i = 0; i < Count; ++i)
            {
                var trophy = _tconf[i];
                var trophyInfo = _tpsn[i];
                if (trophyInfo.HasValue)
                    trophy.TrophyInfo = new TrophyInfo{ Time = trophyInfo.Value.Time, IsSync = trophyInfo.Value.IsSync, IsUnlock = true};
                _trophies.Add(trophy);
            }
          
        }

        public void ChangeTime(int id, DateTime time)
        {
            _tpsn.ChangeTime(id, time);
            TROPUSR.TrophyTimeInfo tti = _tusr.trophyTimeInfoTable[id];
            tti.Time = time;
            _tusr.trophyTimeInfoTable[id] = tti;
            _trophies[id].TrophyInfo.Time = time;
        }
        public void UnlockTrophy(int id, DateTime time)
        {
            _tusr.UnlockTrophy(id, time);
            _tpsn.PutTrophy(id, _tusr.trophyTypeTable[id].Type, time);
            _trophies[id].TrophyInfo = new TrophyInfo { Time = time, IsSync = false, IsUnlock = true };

        }
        
        public void LockTrophy(int id)
        {
            _tusr.LockTrophy(id);
            _tpsn.DeleteTrophyByID(id);
            _trophies[id].TrophyInfo = null;
            //throw new NotImplementedException();
        }

        public void Save()
        {
            _tpsn.Save();
            _tusr.Save();
            //TODO: Encrypt
        }

        public override string ToString()
        {
            var sb = new StringBuilder();
            sb.AppendLine(_tconf.ToString());
            _trophies.ForEach(t => sb.AppendLine(t.ToString()));
            return sb.ToString();
        }
    }
}