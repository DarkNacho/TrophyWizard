using System;
using System.Text;

namespace TrophyParser.Models
{
    public class Trophy
    {
        public int Id { get;}
        public string Hidden { get;}
        public string Type { get;}
        public int Pid { get;}
        public string Name { get;}
        public string Detail { get;}
        public int Gid { get;}
        public DateTime Time { get; set; }
        public bool IsUnlock { get; set; }
        public bool IsSync { get; set; }
        public Trophy(int id, string hidden, string ttype, int pid, string name, string detail, int gid) 
        {
            Id = id;
            Hidden = hidden;
            Type = ttype;
            Pid = pid;
            Name = name;
            Detail = detail;
            Gid = gid;
            IsUnlock = false;
            IsSync = false;
        }
        public override string ToString() 
        {
            StringBuilder sb = new StringBuilder();
            sb.Append(Id).Append(" ").AppendLine(Name);
            sb.AppendLine(Detail);
            sb.Append("Hidden: ").AppendLine(Hidden);
            sb.Append("Type: ").AppendLine(Type);
            sb.Append("Unlock: ").AppendLine(IsUnlock ? "YES" : "NO");
            sb.Append("Sync: ").AppendLine(IsSync ? "YES" : "NO");
            sb.AppendLine(Time.ToString());
            return sb.ToString();
        }
    }
}