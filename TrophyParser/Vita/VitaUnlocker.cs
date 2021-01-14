using System;
using System.Collections;
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
            foreach (var trophy in _trophies)
            {
                trophy.TrophyInfo = _ttitle[trophy.Id];
                _trophies.Add(trophy);
            }
            /*
            for (int i = 0; i < Count; ++i)
            {
                var trophy = _trop[i];
                trophy = _trop[i];
                trophy.TrophyInfo = _ttitle[i];
                _trophies.Add(trophy);
            }*/
        }
        public void UnlockTrophy(int id, DateTime time)
        {
            _tans.Unlock(id, _trophies[id].Type, time);
            _ttitle.PuTrophy(id,time);
        }
        public void UnlockTrophy(Trophy trophy, DateTime time) => UnlockTrophy(trophy.Id, time);
        public void LockTrophy(int id)
        {
            _tans.LockTropy(id);
            _ttitle.PopTrophy(id);
        }
        public void LockTrophy(Trophy trophy) => LockTrophy(trophy.Id);
        public void ChangeTime(int id, DateTime time)
        {
            _tans.ChangeTime(id,time);
            _ttitle.ChangeTime(id,time);
        }
        public void ChangeTime(Trophy trophy, DateTime time) => ChangeTime(trophy.Id, time);
        public void Save()
        {
            _tans.Save();
            _ttitle.Save();
        }
        public IEnumerator<Trophy> GetEnumerator() => _trophies.GetEnumerator();
        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }
}