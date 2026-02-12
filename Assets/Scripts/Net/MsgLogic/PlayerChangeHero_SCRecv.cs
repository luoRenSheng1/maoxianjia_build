using msg;
using UnityEngine;

namespace Engine
{
    using EngineBase;
    
    public class PlayerChangeHero_SCRecv : IReceiver
    {
        public PlayerChangeHero_SC msg;

        public int MsgID()
        {
            return (int)eMsgID.eMsg_PlayerChangeHero_SC;
        }

        public void Process()
        {
            if (msg.Result == eErrCode.eErrCode_Success)
            {
                int heroId = msg.HeroId;
                DataManager.Instance.mRoleData.heroId = heroId;
                PlayerAttrUtils.UpdateFinance(msg.Finance);
                var myHero = new HeroInfo();
                myHero.HeroUnit = ConfigUtils.GetHeroById(heroId);
                HeroInfoManager.Instance.UpdateMyHeroInfo(myHero.HeroUnit.Id);
                EventDispatcher.GameWorld.DispatchEvent(EventDefine.EVENT_ROLE_TRANSFER_SUCCESS);
                EventDispatcher.GameWorld.DispatchEvent(EventDefine.EVENT_POWER_CHANGE);
            }
            else
            {
                UIManager.Instance.ToastByKey(5000+ (int) msg.Result);
            }
        }

        public bool Read(BaseStructRecv mRecv)
        {
            msg = PlayerChangeHero_SC.ParseFrom(mRecv.obj);
            return true;
        }
    }
}
