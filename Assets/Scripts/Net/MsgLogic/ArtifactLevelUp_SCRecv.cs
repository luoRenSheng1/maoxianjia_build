using msg;

namespace Engine
{
    using EngineBase;
    
    public class ArtifactLevelUp_SCRecv : IReceiver
    {
        public ArtifactLevelUp_SC msg;

        public int MsgID()
        {
            return (int)eMsgID.eMsg_ArtifactLevelUp_SC;
        }

        public void Process()
        {
            if (msg.Result == eErrCode.eErrCode_Success)
            {
                EquipManager.Instance.UpdateArtifact((int)msg.AttrId, (int)msg.Levels);
                
                PlayerAttrUtils.UpdateFinance(msg.Finance);

                foreach (var item in msg.ItemsList)
                {
                    ItemInfoManager.Instance.ReduceItem((int)item.Id,(int)item.Num);
                }
                
                EventDispatcher.GameWorld.DispatchEvent(EventDefine.EVENT_ARTIFACT_UPDATE);
                EventDispatcher.GameWorld.DispatchEvent(EventDefine.EVENT_ROLE_UPDATE);
                EventDispatcher.GameWorld.DispatchEvent(EventDefine.EVENT_POWER_CHANGE);
            }
            else
            {
                UIManager.Instance.ToastByKey(5000+ (int) msg.Result);
            }
            
        }
        
        public bool Read(BaseStructRecv mRecv)
        {
            msg = ArtifactLevelUp_SC.ParseFrom(mRecv.obj);
            return true;
        }
    }
}