using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MapServer.Packets
{
    public class CharaSkillData
    {
        public ushort[] SkillID { get; set; }

        public byte[] SkillLv { get; set; }

        public byte[] SkillReserved { get; set; }

        public byte[] SkillAcquireLv { get; set; }

        public byte Job { get; set; }

        public byte SkillNum { get; set; }
    }
}
