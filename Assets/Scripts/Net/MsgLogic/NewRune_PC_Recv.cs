using System.Collections.Generic;
using msg;
using UnityEngine;

namespace Engine
{
    using EngineBase;
    
    public class NewRune_PC_Recv : IReceiver
    {
        public NewRune_PC msg;

        public int MsgID()
        {
            return (int)eMsgID.eMsg_NewRune_PC;
        }

        public void Process()
        {
            RuneInfo runeInfo = new RuneInfo();
            runeInfo.SkillId = (int) msg.Runes.SkillId;
            runeInfo.Guid = msg.Runes.Guid;
            runeInfo.Level = (int) msg.Runes.Level;
            runeInfo.Quality = (int) msg.Runes.Quality;
            runeInfo.ItemId = (int) msg.Runes.ItemId;
            runeInfo.MagicTimes = (int) msg.Runes.AtkCounter;
   
            RuneInfoManager.Instance.UpdateRuneInfo(runeInfo);
            
            EventDispatcher.GameWorld.DispatchEvent(EventDefine.EVENT_UPDATE_RuneInfo);
        }

        public bool Read(BaseStructRecv mRecv)
        {
            msg = NewRune_PC.ParseFrom(mRecv.obj);
            return true;
        }
    }
}
