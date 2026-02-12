using msg;
using UnityEngine;

namespace Engine
{
    using EngineBase;

    public class ProduceEnd_PC_Recv : IReceiver
    {
        public ProduceEnd_PC msg;

        public int MsgID()
        {
            return (int)eMsgID.eMsg_ProduceEnd_PC;
        }

        public void Process()
        {
            CityInfo cityInfo = PlayerAttrUtils.GetBuildInfoByServer(msg.Build);
            VillageInfoManager.Instance.SetBuildInfo(cityInfo.BuildType, cityInfo);
            EventDispatcher.GameWorld.DispatchEvent(EventDefine.EVENT_VILLAGE_ONE_BUILD, cityInfo);
            EventDispatcher.GameWorld.DispatchEvent(EventDefine.EVENT_VILLAGE_GUIDE_PRODUCE_GETREWARD);
        }

        public bool Read(BaseStructRecv mRecv)
        {
            msg = ProduceEnd_PC.ParseFrom(mRecv.obj);
            return true;
        }
    }
}
