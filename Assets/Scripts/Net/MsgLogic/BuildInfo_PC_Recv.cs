using msg;
using UnityEngine;

namespace Engine
{
    using EngineBase;
    
    public class BuildInfo_PC_Recv : IReceiver
    {
        public BuildInfo_PC msg;

        public int MsgID()
        {
            return (int)eMsgID.eMsg_BuildInfo_PC;
        }

        public void Process()
        {
            CityInfo cityInfo = PlayerAttrUtils.GetBuildInfoByServer(msg.Build);
            VillageInfoManager.Instance.SetBuildInfo(cityInfo.BuildType, cityInfo);
            EventDispatcher.GameWorld.DispatchEvent(EventDefine.EVENT_VILLAGE_ALL_BUILD);
        }

        public bool Read(BaseStructRecv mRecv)
        {
            msg = BuildInfo_PC.ParseFrom(mRecv.obj);
            return true;
        }
    }
}
