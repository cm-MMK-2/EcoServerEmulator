using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

/***********************************************
  *
  *　　　　　　iヽ　　　　　　/ヽ 
  *　　　 　 　|　ﾞ、　　　　/ 　ﾞi 
  *　 　 　 　 |　　  ﾞ''─'''"　  l 
  *　　　　　,/　　　 　 　 　 　　ヽ 
  *　　　　 ,iﾞ 　　　  　 　 　 　  \
  *　　　 　i!　　　●　 　 　　●　  |i　　　　　　　 
  *　　　　 ﾞi,,　　* （__人__）　　,/　　　　　　　　　　　 
  *　　　　　 ヾ､,,　　　/￣￣￣￣￣￣￣￣￣/　　　　　 
  *　　 　　 /ﾞ "　　　 /　　　　          /
  *　　　   "⌒ﾞヽ　　 /　　　 　 　　　  /
  *　　　　 |　　 　 i/　　　　　　      /
  *  ====== ヽ,＿,,ノ/＿＿＿＿＿＿＿＿＿/ ========
  *
  *  <author>cm</author>
  *
  ***********************************************/

namespace CommonLib.Packets
{
    public class BasePacket
    {
        public BasePacket() { }

        /// <summary>
        /// make new packet for sending to client
        /// </summary>
        /// <param name="protocolID"></param>
        /// <param name="data"></param>
        public BasePacket(UInt16 protocolID, byte[] data = null)
        {
            DataLength = (data != null ? (UInt16)(data.Length + 2) : (UInt16)2);
            ProtocolID = protocolID;
            if (data != null)
            {
                Data = new byte[data.Length];
                Buffer.BlockCopy(data, 0, Data, 0, data.Length);
            }
        }

        /// <summary>
        /// use to parse raw data
        /// </summary>
        /// <param name="dataIn"></param>
        public BasePacket(byte[] dataIn)
        {
            try
            {
                if (dataIn.Length < 4)
                {
                    Logger.Error("Packet length must be equal or larger than 4");
                }

                DataLength = (UInt16)((dataIn[0] << 8) + dataIn[1]);
                ProtocolID = (UInt16)((dataIn[2] << 8) + dataIn[3]);
                if (DataLength > 2)
                {
                    Data = new byte[DataLength - 2];
                    Buffer.BlockCopy(dataIn, 4, Data, 0, DataLength - 2); //error
                }
            }
            catch(Exception e)
            {
                Logger.Error(e);
                Logger.Error($"Error Message:{dataIn.ToHexString()}");
            }
        }
        public byte[] Data { get; set; } = null;
        public UInt16 DataLength { get; set; }
        public UInt16 ProtocolID { get; set; }
        public string ToHexString()
        {
            return BitConverter.ToString(Data).Replace('-', ' ');
        }

        public byte[] ToBytes()
        {
            byte[] bytes;
            if (Data != null)
            {
                bytes = new byte[Data.Length + 4];
                bytes.Fill(DataLength.ToBytes(), 0);
                bytes.Fill(ProtocolID.ToBytes(), 2);
                bytes.Fill(Data, 4);
                
            }
            else
            {
                bytes = new byte[4];
                bytes.Fill(DataLength.ToBytes(), 0);
                bytes.Fill(ProtocolID.ToBytes(), 2);
            }
            return bytes;
        }


        /// <summary>
        /// value type: byte ushort uint ulong string byte[] ushort[] uint[] ulong[] string[]
        /// </summary>
        /// <param name="packet"></param>
        /// <param name="protocolID"></param>
        /// <returns></returns>
        public static BasePacket MakeUniversalPacket(ISendPacket packet, ushort protocolID)
        {
            var properties = packet.GetType().GetProperties();
            List<byte> dataList = new List<byte>();
            for (int i = 0; i < properties.Length; i++)
            {
                Type type = properties[i].PropertyType;

                if (type == typeof(byte))
                {
                    dataList.Add((byte)properties[i].GetValue(packet));
                }
                else if (type == typeof(ushort))
                {
                    ushort data = (ushort)properties[i].GetValue(packet);
                    dataList.AddRange(data.ToBytes());
                }
                else if(type == typeof(uint))
                {
                    uint data = (uint)properties[i].GetValue(packet);
                    dataList.AddRange(data.ToBytes());
                }
                else if (type == typeof(ulong))
                {
                    ulong data = (ulong)properties[i].GetValue(packet);
                    dataList.AddRange(data.ToBytes());
                }
                else if (type == typeof(string))
                {
                    string data = (string)properties[i].GetValue(packet);
                    dataList.AddRange(data.ToTBytes());
                }
                else if (type == typeof(byte[]))
                {
                    byte[] data = (byte[])properties[i].GetValue(packet);
                    int length = data.Length;
                    if (length > 253)
                    {
                        dataList.Add(253);
                        dataList.AddRange(length.ToBytes());
                        dataList.AddRange(data);
                    }
                    else
                    {
                        dataList.Add((byte)length);
                        for (int k = 0; k < length; k++)
                        {
                            dataList.AddRange(data);
                        }
                    }
                }
                else if (type == typeof(ushort[]))
                {
                    ushort[] data = (ushort[])properties[i].GetValue(packet);
                    int length = data.Length;
                    if (length > 253)
                    {
                        dataList.Add(253);
                        dataList.AddRange(length.ToBytes());
                        for (int k = 0; k < length; k++)
                        {
                            dataList.AddRange(data[k].ToBytes());
                        }
                    }
                    else
                    {
                        dataList.Add((byte)length);
                        for (int k = 0; k < length; k++)
                        {
                            dataList.AddRange(data[k].ToBytes());
                        }
                    }
                }
                else if (type == typeof(uint[]))
                {
                    uint[] data = (uint[])properties[i].GetValue(packet);
                    int length = data.Length;
                    if (length > 253)
                    {
                        dataList.Add(253);
                        dataList.AddRange(length.ToBytes());
                        for (int k = 0; k < length; k++)
                        {
                            dataList.AddRange(data[k].ToBytes());
                        }
                    }
                    else
                    {
                        dataList.Add((byte)length);
                        for (int k = 0; k < length; k++)
                        {
                            dataList.AddRange(data[k].ToBytes());
                        }
                    }
                }
                else if (type == typeof(ulong[]))
                {
                    ulong[] data = (ulong[])properties[i].GetValue(packet);
                    int length = data.Length;
                    if (length > 253)
                    {
                        dataList.Add(253);
                        dataList.AddRange(length.ToBytes());
                        for (int k = 0; k < length; k++)
                        {
                            dataList.AddRange(data[k].ToBytes());
                        }
                    }
                    else
                    {
                        dataList.Add((byte)length);
                        for (int k = 0; k < length; k++)
                        {
                            dataList.AddRange(data[k].ToBytes());
                        }
                    }
                }
                else if (type == typeof(string[]))
                {
                    string[] data = (string[])properties[i].GetValue(packet);
                    int length = data.Length;
                    if (length > 253)
                    {
                        dataList.Add(253);
                        dataList.AddRange(length.ToBytes());
                        for (int k = 0; k < length; k++)
                        {
                            dataList.AddRange(data[k].ToXBytes());
                        }
                    }
                    else
                    {
                        dataList.Add((byte)(length));
                        for (int k = 0; k < length; k++)
                        {
                            dataList.AddRange(data[k].ToXBytes());
                        }
                    }

                }
                else
                {
                    Logger.Error($"Unknown type for packet:{protocolID.ToString("x")}, type:{type.Name}");
                }
            }

            return new BasePacket(protocolID, dataList.ToArray());
        }
    }
}
