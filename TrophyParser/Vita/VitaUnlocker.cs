using System;
using System.Collections.Generic;
using TrophyParser.Models;

namespace TrophyParser.Vita
{
    public class VitaUnlocker : IUnlocker
    {
        private List<Trophy> _trophies;
        public Trophy this[int id] => _trophies[id];
        public int Count { get; }
        private TROP _trop;
        private TRPTITLE _ttitle;
        private TRPTRANS _tans;
        
        public VitaUnlocker(string path)
        {
            
            _trop = new TROP(path);
            _ttitle = new TRPTITLE(path);
            _tans = new TRPTRANS(path, _trop.Count);
            Count = _trop.Count;
            _trophies = new List<Trophy>();
            for (int i = 0; i < Count; ++i)
            {
                var trophy = _trop[i];
                trophy = _trop[i];
                trophy.IsUnlock = _ttitle[i].IsUnlock | _tans[i].IsUnlock;
                trophy.Time = _ttitle[i].Time;
                _trophies.Add(trophy);
            }
            _trop.PrintState();
        }
        public void UnlockTrophy(int id, DateTime time)
        {
            _tans.Unlock(id, _trophies[id].Type, time);
            _ttitle.PuTrophy(id,time);
        }

        public void LockTrophy(int id)
        {
            _tans.LockTropy(id);
            _ttitle.PopTrophy(id);
        }

        public void ChangeTime(int id, DateTime time)
        {
            _tans.ChangeTime(id,time);
            _ttitle.ChangeTime(id,time);
        }
        public void Save()
        {
            _tans.Save();
            _ttitle.Save();
        }
    }
}