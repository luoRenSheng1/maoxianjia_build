using System.Collections.Generic;
using System.Linq;
using msg;
using UnityEngine;

namespace Engine
{
    using EngineBase;
    
    public class PetDispatched_SCRecv : IReceiver
    {
        public PetDispatched_SC msg;

        public int MsgID()
        {
            return (int)eMsgID.eMsg_PetDispatched_SC;
        }

        public void Process()
        {
            if (msg.Result == eErrCode.eErrCode_Success)
            {
                CityInfo cityInfo = PlayerAttrUtils.GetBuildInfoByServer(msg.Build);
                VillageInfoManager.Instance.SetBuildInfo(cityInfo.BuildType, cityInfo);
                EventDispatcher.GameWorld.DispatchEvent(EventDefine.EVENT_VILLAGE_ONE_BUILD, cityInfo);
                if (msg.PetIdsList.Count > 0)
                {
                    foreach (var item in msg.PetIdsList)
                    {
                        PetItemInfo petItemInfo = PetInfoManager.Instance.GetPet(item);
                        petItemInfo.DispatchBuild = VillageBuildType.None;
                    }
                }
                
                EventDispatcher.GameWorld.DispatchEvent(EventDefine.EVENT_UPDATE_VILLAGE_PET_SC_SUCC);
            }
            else
            {
                UIManager.Instance.ToastByKey(5000+ (int) msg.Result);
            }
        }

        public bool Read(BaseStructRecv mRecv)
        {
            msg = PetDispatched_SC.ParseFrom(mRecv.obj);
            return true;
        }
    }
}
