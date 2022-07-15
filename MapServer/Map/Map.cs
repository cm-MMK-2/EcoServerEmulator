using CommonLib;
using MapServer.Packets;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace MapServer.Map
{
    /// <summary>
    /// single map
    /// </summary>
    public class Map
    {
        /// <summary>
        /// incremental id for character in this map
        /// </summary>
        public int charaServerIDCounter = -1;

        /// <summary>
        /// in map characters
        /// </summary>
        private Dictionary<uint, CharaInfo> charas = new Dictionary<uint, CharaInfo>();

        /// <summary>
        /// No character in map
        /// </summary>
        public bool IsEmpty => charas.Count == 0;

        public uint id { get; private set; }

        private MapData data;

        private Portal[] portals;

        public Map(uint mapId)
        {
            id = mapId;
        }

        public async Task LoadMapData()
        {
            data = await DatabaseManager.SelectAsync<MapData>($"SELECT SizeX, SizeY FROM Map WHERE id={id}");
        }

        public void AddChara(CharaInfo pi)
        {
            pi.ServerCharaID = (uint)Interlocked.Increment(ref charaServerIDCounter);
            charas.Add(pi.ServerCharaID, pi);
        }

        public void RemoveChara(uint serverCharaId)
        {
            charas.Remove(serverCharaId);
        }

        public void AfterCharaMove(uint moveCharaServerId)
        {
            //byte[] data = new byte[20];

            foreach (uint charaServerId in charas.Keys.ToList())
            {
               if(charaServerId != moveCharaServerId)
               {
                    //0x11f9
               }
            }
        }
    }
}
