using msg;

namespace Engine
{
    using EngineBase;
    
    public class PlayerHolyItemsInfo4Login_PC_Recv : IReceiver
    {
        public PlayerHolyItemsInfo4Login_PC msg;

        public int MsgID()
        {
            return (int)eMsgID.eMsg_PlayerHolyItemsInfo4Login_PC;
        }

        public void Process()
        {
            foreach (var item in msg.HolyItemsList)
            {
                HolyItemInfo holyItemInfo = new HolyItemInfo();
                holyItemInfo.HolyId = (int)item.HolyId;
                holyItemInfo.Level = (int)item.Level;
                holyItemInfo.TableKeyId = (int)item.TableKeyId;
                
                HolyManager.Instance.UpdateHolyInfo(holyItemInfo);
            }

            if (msg.SplitInfo.IsEnd)
                EventDispatcher.GameWorld.DispatchEvent(EventDefine.EVENT_UPDATE_HolyInfo);
            
        }

        public bool Read(BaseStructRecv mRecv)
        {
            msg = PlayerHolyItemsInfo4Login_PC.ParseFrom(mRecv.obj);
            return true;
        }
    }
}