using msg;
using UnityEngine;

namespace Engine
{
    using EngineBase;
    
    public class PetSwitchBetweenBuilds_SCRecv : IReceiver
    {
        public PetSwitchBetweenBuilds_SC msg;

        public int MsgID()
        {
            return (int)eMsgID.eMsg_PetSwitchBetweenBuilds_SC;
        }

        public void Process()
        {
            if (msg.Result == eErrCode.eErrCode_Success)
            {
                CityInfo srccityInfo = PlayerAttrUtils.GetBuildInfoByServer(msg.SrcBuild);
                VillageInfoManager.Instance.SetBuildInfo(srccityInfo.BuildType, srccityInfo);
                CityInfo dstcityInfo = PlayerAttrUtils.GetBuildInfoByServer(msg.DstBuild);
                VillageInfoManager.Instance.SetBuildInfo(dstcityInfo.BuildType, dstcityInfo);
                if (msg.PetId > 0)
                {
                    PetItemInfo petItemInfo = PetInfoManager.Instance.GetPet(msg.PetId);
                    petItemInfo.DispatchBuild = VillageBuildType.None;
                    petItemInfo.BuildInnerIndex = -1;
                }
                EventDispatcher.GameWorld.DispatchEvent(EventDefine.EVENT_VILLAGE_ONE_BUILD, srccityInfo);                
                EventDispatcher.GameWorld.DispatchEvent(EventDefine.EVENT_VILLAGE_ONE_BUILD, dstcityInfo);
                EventDispatcher.GameWorld.DispatchEvent(EventDefine.EVENT_UPDATE_VILLAGE_PET_SC_SUCC);
            }
            else
            {
                UIManager.Instance.ToastByKey(5000+ (int) msg.Result);
            }

        }

        public bool Read(BaseStructRecv mRecv)
        {
            msg = PetSwitchBetweenBuilds_SC.ParseFrom(mRecv.obj);
            return true;
        }
    }
}
