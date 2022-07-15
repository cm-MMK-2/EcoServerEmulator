using CommonLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MapServer.Packets
{
    public class SelectCharaRequestInfo
    {

        public uint Fix { get; private set; } //0固定？
        public byte SelectedSlot { get;private set; } //選択したキャラの番号
        public byte MapMove { get; private set; } // ログイン時は0、マップ切り替え時は1

        public SelectCharaRequestInfo(byte[] data)
        {
            Fix = data.ToUInt32();
            SelectedSlot = data[4];
            MapMove = data[5];
        }
    }
}
