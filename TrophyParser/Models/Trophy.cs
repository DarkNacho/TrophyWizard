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
        public TrophyInfo TrophyInfo { get; set; }
        public bool IsUnlock{ get => TrophyInfo == null || TrophyInfo.IsUnlock; }
        public Trophy(int id, string hidden, string ttype, int pid, string name, string detail, int gid) 
        {
            Id = id;
            Hidden = hidden;
            Type = ttype;
            Pid = pid;
            Name = name;
            Detail = detail;
            Gid = gid;
            TrophyInfo = null;
        }
        public override string ToString() 
        {
            StringBuilder sb = new StringBuilder();
            sb.Append(Id).Append(" ").AppendLine(Name);
            sb.AppendLine(Detail);
            sb.Append("Hidden: ").AppendLine(Hidden);
            sb.Append("Type: ").AppendLine(Type);
            sb.Append(TrophyInfo != null ? TrophyInfo.ToString() : "NULL\n");
            return sb.ToString();
        }
    }
}