using System.Collections.Generic;
using msg;
using UnityEngine;

namespace Engine
{
    using EngineBase;
    
    public class RuneAutoSell_PC_Recv : IReceiver
    {
        public RuneAutoSell_PC msg;

        public int MsgID()
        {
            return (int)eMsgID.eMsg_RuneAutoSell_PC;
        }

        public void Process()
        {
            List<RuneInfo> sellRunes = new List<RuneInfo>();
            foreach (var item in msg.SoldRunesList)
            {
                RuneInfo runeInfo = new RuneInfo();
                runeInfo.Guid = item.Guid;
                runeInfo.Level = (int) item.Level;
                runeInfo.Quality = (int) item.Quality;
                runeInfo.ItemId = (int) item.ItemId;
                runeInfo.MagicTimes = (int) item.AtkCounter;
                runeInfo.SkillId = (int) item.SkillId;
                sellRunes.Add(runeInfo);
            }

            double gold = msg.Gold;
            PlayerAttrUtils.UpdateFinance(msg.Finance);
        }

        public bool Read(BaseStructRecv mRecv)
        {
            msg = RuneAutoSell_PC.ParseFrom(mRecv.obj);
            return true;
        }
    }
}
