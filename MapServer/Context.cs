using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MapServer
{
    public enum RightClickSettingsEnum : uint
    {
        ALLOW_ALL = 0x0000,
        REFUSE_TRADE = 0x0001, //トレード不許可
        REFUSE_PARTY = 0x0002, //パーティ不許可
        REFUSE_HYOUI = 0x00000004, //憑依不許可
        REFUSE_RING = 0x0008, //リング不許可
        REFUSE_RESCUE = 0x0010, //蘇生選択しない
        REFUSE_WORK = 0x0020 //作業請負不許可
    }
}
