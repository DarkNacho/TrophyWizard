using System;
using System.IO;
using TrophyParser.Models;
using TrophyParser.PS3;

namespace TrophyParser
{
    public interface IUnlocker
    {
        Trophy this[int id] { get;}
        int Count { get; }
        void UnlockTrophy(int id, DateTime time);
        void LockTrophy(int id);
        void ChangeTime(int id, DateTime time);
        void Save();
    }
}