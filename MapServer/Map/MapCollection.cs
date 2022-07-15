using MapServer.Packets;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MapServer.Map
{
    /// <summary>
    /// simulate maps, include map creation/message delivering
    /// </summary>
    public class MapCollection
    {
        public List<Map> ActiveMaps { get; } = new List<Map>();

        public async Task CharaEnterMap(uint mapId, CharaInfo charaInfo)
        {
            var targetMap = ActiveMaps.Find(m => m.id == mapId);
            if (targetMap == null)
            {
                targetMap = new Map(mapId);
                ActiveMaps.Add(targetMap);
                await targetMap.LoadMapData();
            }
            targetMap.AddChara(charaInfo);
        }

        public void CharaLeaveMap(uint mapId, uint charaServerId)
        {
            var targetMap = ActiveMaps.Find(m => m.id == mapId);
            if (targetMap != null)
            {
                targetMap.RemoveChara(charaServerId);
                if(targetMap.IsEmpty)
                {
                    ActiveMaps.Remove(targetMap);
                }
            }
        }
    }
}
