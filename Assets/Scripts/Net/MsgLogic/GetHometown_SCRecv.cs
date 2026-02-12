using msg;
using UnityEngine;

namespace Engine
{
    using EngineBase;
    
    public class GetHometown_SCRecv : IReceiver
    {
        public GetHometown_SC msg;

        public int MsgID()
        {
            return (int)eMsgID.eMsg_Hometown_SC;
        }

        public void Process()
        {
            if (msg.Result == eErrCode.eErrCode_Success)
            {
                foreach (var buildInfo in msg.BuildsList)
                {
                    CityInfo cityInfo = PlayerAttrUtils.GetBuildInfoByServer(buildInfo);
                    VillageInfoManager.Instance.SetBuildInfo(cityInfo.BuildType, cityInfo);
                }
                EventDispatcher.GameWorld.DispatchEvent(EventDefine.EVENT_REDPOINT_UPDATE);
                EventDispatcher.GameWorld.DispatchEvent(EventDefine.EVENT_VILLAGE_ALL_BUILD);
                EventDispatcher.GameWorld.DispatchEvent(EventDefine.EVENT_VILLAGE_GUIDE_PRODUCE_GETREWARD);
            }
            else
            {
                UIManager.Instance.ToastByKey(5000+ (int) msg.Result);
            }
        }

        public bool Read(BaseStructRecv mRecv)
        {
            msg = GetHometown_SC.ParseFrom(mRecv.obj);
            return true;
        }
    }
}
