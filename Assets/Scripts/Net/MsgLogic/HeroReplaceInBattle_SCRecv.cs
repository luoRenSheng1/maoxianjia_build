using msg;
using UnityEngine;

namespace Engine
{
    using EngineBase;
    
    public class HeroReplaceInBattle_SCRecv : IReceiver
    {
        public HeroReplaceInBattle_SC msg;

        public int MsgID()
        {
            return (int)eMsgID.eMsg_HeroReplaceInBattle_SC;
        }

        public void Process()
        {
            if (msg.Result == eErrCode.eErrCode_Success)
            {
                HeroInfoManager.Instance.UpdateMyHeroInfo((int) msg.HeroId);
            }
            else
            {
                UIManager.Instance.ToastByKey(5000+ (int) msg.Result);
            }

        }

        public bool Read(BaseStructRecv mRecv)
        {
            msg = HeroReplaceInBattle_SC.ParseFrom(mRecv.obj);
            return true;
        }
    }
}
