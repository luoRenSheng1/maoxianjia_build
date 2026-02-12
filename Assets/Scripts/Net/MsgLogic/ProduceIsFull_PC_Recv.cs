using msg;
using UnityEngine;

namespace Engine
{
    using EngineBase;

    public class ProduceIsFull_PC_Recv : IReceiver
    {
        public ProduceIsFull_PC msg;

        public int MsgID()
        {
            return (int)eMsgID.eMsg_ProduceIsFull_PC;
        }

        public void Process()
        {
            CityInfo cityInfo = PlayerAttrUtils.GetBuildInfoByServer(msg.Build);
            VillageInfoManager.Instance.SetBuildInfo(cityInfo.BuildType, cityInfo);
            EventDispatcher.GameWorld.DispatchEvent(EventDefine.EVENT_VILLAGE_ONE_BUILD, cityInfo);
            EventDispatcher.GameWorld.DispatchEvent(EventDefine.EVENT_VILLAGE_PRODUCT_FULL);
            EventDispatcher.GameWorld.DispatchEvent(EventDefine.EVENT_VILLAGE_GUIDE_PRODUCE_GETREWARD);
        }

        public bool Read(BaseStructRecv mRecv)
        {
            msg = ProduceIsFull_PC.ParseFrom(mRecv.obj);
            return true;
        }
    }
}
