using msg;

namespace Engine
{
    using EngineBase;
    
    public class NewPlayerHolyItem_PC_Recv : IReceiver
    {
        public NewPlayerHolyItem_PC msg;

        public int MsgID()
        {
            return (int)eMsgID.eMsg_NewPlayerHolyItem_PC;
        }

        public void Process()
        {
            foreach (var item in msg.HolyItemList)
            {
                HolyItemInfo holyItemInfo = new HolyItemInfo();
                holyItemInfo.HolyId = (int)item.HolyId;
                holyItemInfo.Level = (int)item.Level;
                holyItemInfo.TableKeyId = (int)item.TableKeyId;
                
                HolyManager.Instance.UpdateHolyInfo(holyItemInfo);
            }
            EventDispatcher.GameWorld.DispatchEvent(EventDefine.EVENT_UPDATE_HolyInfo);
        }

        public bool Read(BaseStructRecv mRecv)
        {
            msg = NewPlayerHolyItem_PC.ParseFrom(mRecv.obj);
            return true;
        }
    }
}