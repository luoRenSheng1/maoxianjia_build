using msg;
using UnityEngine;

namespace Engine
{
    using EngineBase;
    
    public class BuildUnlock_PC_Recv : IReceiver
    {
        public BuildUnlock_PC msg;

        public int MsgID()
        {
            return (int)eMsgID.eMsg_BuildUnlock_PC;
        }

        public void Process()
        {
            CityInfo cityInfo = PlayerAttrUtils.GetBuildInfoByServer(msg.Build);
            VillageInfoManager.Instance.SetBuildInfo(cityInfo.BuildType, cityInfo);
            EventDispatcher.GameWorld.DispatchEvent(EventDefine.EVENT_VILLAGE_ONE_BUILD, cityInfo);
        }

        public bool Read(BaseStructRecv mRecv)
        {
            msg = BuildUnlock_PC.ParseFrom(mRecv.obj);
            return true;
        }
    }
}
