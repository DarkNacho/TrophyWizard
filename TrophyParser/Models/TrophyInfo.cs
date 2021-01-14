using System;
using System.Text;

namespace TrophyParser.Models
{
    public class TrophyInfo
    {
        public bool IsUnlock { get => Time.HasValue; }
        public byte Unknown;
        public DateTime? Time = null;
        public byte Type;
        public bool IsSync;

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("Unlock: ").AppendLine(IsUnlock ? "YES" : "NO");
            sb.Append("Sync: ").AppendLine(IsSync ? "YES" : "NO");
            sb.AppendLine(Time.ToString());
            return sb.ToString();
        }
    }
}