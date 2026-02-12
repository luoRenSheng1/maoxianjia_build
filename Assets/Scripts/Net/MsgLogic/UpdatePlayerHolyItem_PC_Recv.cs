using msg;

namespace Engine
{
    using EngineBase;
    
    public class UpdatePlayerHolyItem_PC_Recv : IReceiver
    {
        public UpdatePlayerHolyItem_PC msg;

        public int MsgID()
        {
            return (int)eMsgID.eMsg_UpdatePlayerHolyItem_PC;
        }

        public void Process()
        {
            HolyItemInfo holyItemInfo = new HolyItemInfo();
            holyItemInfo.HolyId = (int)msg.HolyItem.HolyId;
            holyItemInfo.Level = (int)msg.HolyItem.Level;
            holyItemInfo.TableKeyId = (int)msg.HolyItem.TableKeyId;
            
            HolyManager.Instance.UpdateHolyInfo(holyItemInfo);

            EventDispatcher.GameWorld.DispatchEvent(EventDefine.EVENT_UPDATE_HolyInfo);
        }

        public bool Read(BaseStructRecv mRecv)
        {
            msg = UpdatePlayerHolyItem_PC.ParseFrom(mRecv.obj);
            return true;
        }
    }
}