using CommonLib;
using CommonLib.Packets;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MapServer.Packets
{
    public class ItemInfo
    {
        public byte Size { get; set; } //データサイズ 0xCE + name_length（ペット以外は1）

        public uint Index { get; set; }// インベントリ インデックス

        public uint ItemID { get; set; } // アイテムID

        public uint IconID { get; set; } // 見た目,フィギュア,スケッチ情報（ItemID or MobIDが入る

        public byte Part { get; set; } // アイテムの場所(かばんや装備箇所) 0x02 in bag
        public uint Unk { get; set; } // 20141122-
        public uint Status { get; set; }  // 鑑定済み:0x01 カードロック？:0x20 パートナー？:0x800 性能開放済み0x1000
        public ushort Durability { get; set; }  // 耐久度 or 親密度
        public ushort MaxDurability { get; set; } // 最大耐久度 or 最大親密度
        public ushort UpgradeCount { get; set; }          // 強化回数 (09/05/28追加より 残り強化回数から強化回数に？
        public ushort CardSlot { get; set; }       // カードスロット数？09/05/28追加
        public uint[] CardIDArray { get; set; } = new uint[10];     // カードID？09/05/28追加
        public byte Dye { get; set; }            // 染色
        public ushort Count { get; set; }          // 個数
        public ulong SalesPrice { get; set; }    // ゴーレム販売価格 15/11/26よりuint->ulong
        public ushort SalesCount { get; set; }    // ゴーレム販売個数
        public ushort Weight { get; set; }         // 最大重量
        public ushort Capacity { get; set; }           // 最大容量

        public ushort Str { get; set; }            // 腕力
        public ushort Mag { get; set; }            // 魔力
        public ushort Vit { get; set; }            // 体力
        public ushort Dex { get; set; }            // 器用
        public ushort Agi { get; set; }            // 敏捷
        public ushort Int { get; set; }            // 知識
        public ushort Luk { get; set; }            // 運 （ペットの場合現在HP（14/07/25からは不明）
        public ushort Cha { get; set; }            // 魅力 （ペットの場合転生回数（14/07/25からは不明）
        public ushort Hp { get; set; }             // HP（使用出来るアイテムは回復
        public ushort Sp { get; set; }             // SP（同上

        public ushort Mp { get; set; }            // MP（同上
        public ushort Spd { get; set; }          // 移動速度
        public ushort AtkN { get; set; }           // 物理攻撃力(叩)
        public ushort AtkS { get; set; }       // 物理攻撃力(斬)
        public ushort AtkT { get; set; }        // 物理攻撃力(突)
        public ushort MAtk { get; set; }        // 魔法攻撃力
        public ushort Def { get; set; }            // 物理防御
        public ushort MDef { get; set; }          // 魔法防御
        public ushort MleHit { get; set; }       // 近命中力
        public ushort RngHit { get; set; }        // 遠命中力
        public ushort MagHit { get; set; }       // 魔命中力
        public ushort MleAvd { get; set; }     // 近回避
        public ushort RngAvd { get; set; }      // 遠回避
        public ushort MagAvd { get; set; }    // 魔回避
        public ushort Cri { get; set; }   // クリティカル
        public ushort CriAvd { get; set; } // クリティカル回避
        public ushort HpRec { get; set; }        // 回復力     （14/07/25より表示）//倉では参照されない  eax+06
        public ushort MpRec { get; set; }        // 魔法回復力 （14/07/25より表示）//倉では参照されない
        public ushort SpRec { get; set; }        // スタミナ回復力　（14/07/25より表示）//倉では参照されない
        public ushort NonEle { get; set; }      // 無属性　（14/07/25より表示）//表示されない
        public ushort Fire { get; set; }           // 火属性
        public ushort Water { get; set; }          // 水属性
        public ushort Wind { get; set; }           // 風属性
        public ushort Earth { get; set; }          // 地属性
        public ushort Light { get; set; }          // 光属性
        public ushort Dark { get; set; }           // 闇属性
        public ushort Poison { get; set; }         // 毒（＋なら毒回復。－なら毒状態に
        public ushort Consolidation { get; set; }          // 石化
        public ushort Paralyze { get; set; }       // 麻痺
        public ushort Sleep { get; set; }          // 睡眠
        public ushort Silence { get; set; }        // 沈黙
        public ushort Slow { get; set; }           // 鈍足
        public ushort Confuse { get; set; }        // 混乱
        public ushort Freeze { get; set; }         // 凍結
        public ushort Stun { get; set; }           // 気絶
        public ushort AtkSpd { get; set; }      // 攻撃速度（14/07/25から通常表示）（より前はペットステ（攻撃速度
        public ushort MagSpd { get; set; }      // 詠唱速度（14/07/25から通常表示）（より前はペットステ（詠唱速度
                                                // ushort  unknown?{ get; set; }       // （14/07/25に削除）ペットステ？（スタミナ回復力？倉では参照されない
        public ulong BuyPrice { get; set; }      // ゴーレム露店の買取価格 15/11/26よりuint->{ get; set; }ulong
        public ushort BuyCount { get; set; }      // ゴーレム露店の買取個数
        public ulong ShopSellPrice { get; set; }          // 商人露店の販売価格 15/11/26よりuint->{ get; set; }ulong
        public ushort ShopSellCount { get; set; }          // 商人露店の販売個数

        public ushort NameLength { get; set; }    // Length of Name
        public string Name { get; set; }           // 固有ネーム（ペットの名前とか
                                                   //（"{ get; set; }ab"{ get; set; }という名前ならname_length = 0003, name = 03 'a' 'b' '\0'
        public byte unknown10 { get; set; } = 0;     // 0固定？
        public uint RentTime { get; set; } = 0xffffffff;     // 貸出品のとき残り貸出期間(秒)、それ以外-1　
        public byte IsRental { get; set; }      // 貸出アイテムフラグ
        public byte PartnerLv { get; set; }     // partnerのbase lv （14/07/25に追加）
        public byte PartnerRebirth { get; set; }  // partnerの転生フラグ（14/07/25に追加）
    }

    public class ItemInfoExt : ItemInfo, ISendPacket
    {
        public byte PacketType { get; set; } // 01の場合アイテムデータが入っている。 02の場合はインベントリデータ終了のシグナ

        public ItemInfoExt(byte type)
        {
            PacketType = type;
        }

        public BasePacket ToPacket()
        {
            if(PacketType == 2)
            {
                return new BasePacket(0x0203, new byte[] { 0x02 ,0x00});
            }

            if (string.IsNullOrEmpty(Name))
            {
                int len = Name.Length;
                if (len > 24)
                {
                    Logger.Error($"NameLength Too Long, Error Item:{ItemID}, Error Name:{Name}");
                    len = 24;
                }
                NameLength = (ushort)len;
            }

            //todo

            return new BasePacket(0x0203);
        }
    }
}
