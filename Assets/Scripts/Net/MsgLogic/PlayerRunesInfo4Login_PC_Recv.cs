using System.Collections.Generic;
using msg;
using UnityEngine;

namespace Engine
{
    using EngineBase;
    
    public class PlayerRunesInfo4Login_PC_Recv : IReceiver
    {
        public PlayerRunesInfo4Login_PC msg;

        public int MsgID()
        {
            return (int)eMsgID.eMsg_PlayerRunesInfo4Login_PC;
        }

        public void Process()
        {
            foreach (var item in msg.RunesList)
            {
                RuneInfo runeInfo = new RuneInfo();
                runeInfo.Guid = item.Guid;
                runeInfo.Level = (int) item.Level;
                runeInfo.Quality = (int) item.Quality;
                runeInfo.SkillId = (int) item.SkillId;
                runeInfo.ItemId = (int) item.ItemId;
                runeInfo.MagicTimes = (int) item.AtkCounter;
                RuneInfoManager.Instance.UpdateRuneInfo(runeInfo);
            }
            
            if(msg.SplitInfo.IsEnd)
                EventDispatcher.GameWorld.DispatchEvent(EventDefine.EVENT_UPDATE_RuneInfo);
        }

        public bool Read(BaseStructRecv mRecv)
        {
            msg = PlayerRunesInfo4Login_PC.ParseFrom(mRecv.obj);
            return true;
        }
    }
}
