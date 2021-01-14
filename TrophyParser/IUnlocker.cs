using System;
using System.Collections.Generic;
using TrophyParser.Models;

namespace TrophyParser
{
    public interface IUnlocker : IEnumerable<Trophy>
    {
        Trophy this[int id] { get;}
        int Count { get; }
        void UnlockTrophy(int id, DateTime time);
        void UnlockTrophy(Trophy trophy, DateTime time);
        void LockTrophy(int id);
        void LockTrophy(Trophy trophy);
        void ChangeTime(int id, DateTime time);
        void ChangeTime(Trophy trophy, DateTime time);
        void Save();
    }
}