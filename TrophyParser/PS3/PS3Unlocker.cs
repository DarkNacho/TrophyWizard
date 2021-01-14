using System;
using System.Collections;
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
                if (trophyInfo.HasValue) trophy.TrophyInfo = new TrophyInfo{ Time = trophyInfo.Value.Time, IsSync = trophyInfo.Value.IsSync};
                else trophy.TrophyInfo = new TrophyInfo { Time = null, IsSync = false};
                _trophies.Add(trophy);
            }
          
        }

        
        public void UnlockTrophy(int id, DateTime time)
        {
            _tusr.UnlockTrophy(id, time);
            _tpsn.PutTrophy(id, _tusr.trophyTypeTable[id].Type, time);
            _trophies[id].TrophyInfo = new TrophyInfo { Time = time, IsSync = false};

        }

        public void UnlockTrophy(Trophy trophy, DateTime time) => UnlockTrophy(trophy.Id, time);

        public void LockTrophy(int id)
        {
            _tusr.LockTrophy(id);
            _tpsn.DeleteTrophyByID(id);
            _trophies[id].TrophyInfo = null;
        }

        public void LockTrophy(Trophy trophy) => LockTrophy(trophy.Id);
        public void ChangeTime(int id, DateTime time)
        {
            _tpsn.ChangeTime(id, time);
            TROPUSR.TrophyTimeInfo tti = _tusr.trophyTimeInfoTable[id];
            tti.Time = time;
            _tusr.trophyTimeInfoTable[id] = tti;
            _trophies[id].TrophyInfo.Time = time;
        }
        public void ChangeTime(Trophy trophy, DateTime time) => ChangeTime(trophy.Id, time);
        public void Save()
        {
            _tpsn.Save();
            _tusr.Save();
            //TODO: Encrypt
        }

        public IEnumerator<Trophy> GetEnumerator() => _trophies.GetEnumerator();
        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
        public override string ToString()
        {
            var sb = new StringBuilder();
            sb.AppendLine(_tconf.ToString());
            _trophies.ForEach(t => sb.AppendLine(t.ToString()));
            return sb.ToString();
        }
    }
}