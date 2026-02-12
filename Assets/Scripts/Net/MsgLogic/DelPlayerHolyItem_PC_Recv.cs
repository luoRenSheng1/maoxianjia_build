using msg;

namespace Engine
{
    using EngineBase;
    
    public class DelPlayerHolyItem_PC_Recv : IReceiver
    {
        public DelPlayerHolyItem_PC msg;

        public int MsgID()
        {
            return (int)eMsgID.eMsg_DelPlayerHolyItem_PC;
        }

        public void Process()
        {
            foreach (var item in msg.HolyItemIdsList)
            {
                HolyManager.Instance.DelHolyItemInfo((int)item);
            }
            EventDispatcher.GameWorld.DispatchEvent(EventDefine.EVENT_UPDATE_HolyInfo);
        }

        public bool Read(BaseStructRecv mRecv)
        {
            msg = DelPlayerHolyItem_PC.ParseFrom(mRecv.obj);
            return true;
        }
    }
}