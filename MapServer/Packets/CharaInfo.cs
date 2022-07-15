﻿using CommonLib.Packets;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MapServer.Packets
{
    public class CharaInfo :ISendPacket
    {
        public uint ServerCharaID { get; set; } // サーバキャラID
        public uint CharaID { get; set; } // キャラ固有ID
        public uint Unk1 { get; set; } = 1; //1? 2015/11/26確認
        public byte Unk { get; set; } = 0; //0or1？ ushortではない   キャラクタ基本情報
        public string Name { get; set; } // キャラクタ名 
        public byte Race { get; set; }
        public byte Form { get; set; } // 09/12/20確認 DEM FORM状態
        public byte Sex { get; set; }
        public ushort HairStyle { get; set; }
        public byte HairColor { get; set; }
        public ushort Wig { get; set; } // つけ髪とか
        public byte Extra { get; set; } = 0xff; // 常に0xff?
        public ushort Face { get; set; } // 11/10/27にbyte->{get; set;}ushort                                         //転生関係 10/08/19に追加
        public byte RebirthLv { get; set; }   // 転生前のレベル。転生する前は0
        public byte Ex { get; set; }        // 転生特典 しっぽとかわっかとか
        public byte Wing { get; set; }      // 転生特典 翼
        public byte WingColor { get; set; } // 転生特典 翼色             
        public uint Map { get; set; }// いる場所
        public byte X { get; set; }
        public byte Y { get; set;}
        public byte Dir { get; set;}
        // ステータス情報
        public uint Hp { get; set; }
        public uint MaxHp { get; set;}
        public uint Mp { get; set; }
        public uint MaxMp{get; set;}
        public uint Sp { get; set; }
        public uint MaxSp { get; set; }
        public uint Ep { get; set; }
        public uint MaxEp { get; set;} // 07/12/21より
        public ushort Unknown1 { get; set; } = 9;// 09/12/20確認 値は09確認
        public ushort[] Status { get; set; } = new ushort[8]; //ステータス数（常に0x08)str, dex, int, vit, agi, mag, luk, cha
        public ushort[] Unknown2 { get; set; } = new ushort[20]; // len=20? 値は全て0?
        public ushort[] Unknown3 { get; set; } = new ushort[0]; // len =0?
        public ushort[] Unknown4 { get; set; } = new ushort[0]; //len = 0?
        public uint HyouiTarget { get; set; } = 0xFFFFFFFF;//憑依対象サーバーキャラID
        public byte HyouiPart { get; set; } = 0xFF;//憑依場所 ( r177b等も参照
                                                // 所持金
        public ulong Gold { get; set; } //2015/11/26 uint→ulong
        public ulong Unknown6 { get; set; } //0? 2015/11/26 確認
        public byte Unknown7 { get; set; }

        /*
        Head
        HeadAcce
        Face
        FaceAcce
        ChestAcce
        Top
        Bottom
        Backpack
        Right
        Left
        Shoes
        Socks
        Pet
        Effect
        */
        public uint[] Equipments { get; set; } = new uint[14];

        public ushort[] LeftHandMotion { get; set; } = new ushort[3];// 左手モーションタイプ size=3 { 片手, 両手, 攻撃}

        public ushort[] RightHandMotion { get; set; } = new ushort[3];// 右手モーションタイプ size=3 chr_act_tbl.csvを参照する

        public ushort[] RidingMotion { get; set; } = new ushort[3];// 乗り物モーションタイプ size=3
        public uint RidePetID { get; set; } //(itemid
        public byte RidePetColor { get; set; }//ロボ用
        public uint Range { get; set; }     //武器の射程
        public uint Unknown8 { get; set; }   //0?
        public uint Mode1 { get; set; }   //2 r0fa7参照
        public uint Mode2 { get; set; }   //0 r0fa7参照
        public byte Unknown9 { get; set; } //0?
        public byte Guest { get; set; } //ゲストIDかどうか (07/11/22より)
        public byte Unk2{get; set;} 
        public uint Unknown10 { get; set; }//師弟関係？
        public uint Unknown11 { get; set; } = 1;//1？
        //public byte Unknown12 { get; set; }// 2015/11/26確認
        public uint Unknown13 { get; set; } = 0x0d;//1？
        public ushort ShowTinyIcon { get; set; }//0or1？
        public ulong DailyDungeonCleared { get; set; } // 16/04/20追加 0x01でデイリークエストの！を非表示

        public uint[] Unknown_ { get; set; } = new uint[4];

        public BasePacket ToPacket()
        {
            return BasePacket.MakeUniversalPacket(this, 0x01ff);
        }
    }
}
