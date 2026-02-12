using System.Collections.Generic;
using System.Linq;
using msg;

namespace Engine
{
    using EngineBase;
    
    public class NewItems_PC_Recv : IReceiver
    {
        public NewItems_PC msg;

        public int MsgID()
        {
            return (int)eMsgID.eMsg_NewItems_PC;
        }

        public void Process()
        {
            List<ItemData> ret = new List<ItemData>();
            PlayerAttrUtils.GetItemData(msg.ItemList.ToList(), ref ret);

            EventDispatcher.GameWorld.DispatchEvent(EventDefine.EVENT_ITEM_UPDATE);
            
        }

        public bool Read(BaseStructRecv mRecv)
        {
            msg = NewItems_PC.ParseFrom(mRecv.obj);
            return true;
        }
    }
}
