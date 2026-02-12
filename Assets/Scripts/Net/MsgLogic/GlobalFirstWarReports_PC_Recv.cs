using System.Collections.Generic;
using System.Linq;
using msg;
using UnityEngine;

namespace Engine
{
    using EngineBase;
    
    public class GlobalFirstWarReports_PC_Recv : IReceiver
    {
        public GlobalFirstWarReports_PC msg;

        public int MsgID()
        {
            return (int)eMsgID.eMsg_GlobalFirstWarReports_PC;
        }

        public void Process()
        {
            List<BattleReportVo> battleReportVos = new List<BattleReportVo>();
            foreach (var item in msg.WarReportsList)
            {
                BattleReportVo battleReportVo = new BattleReportVo()
                {
                    ReportType = item.WarReport,
                    Param = item.ParamsList.ToList(),
                    TimeStamp =item.TimeStamp
                };
                battleReportVos.Add(battleReportVo);
            }
            PvpRankDataManager.Instance.SetBattleReport(battleReportVos);
            
            EventDispatcher.GameWorld.DispatchEvent(EventDefine.EVENT_PVP_REPORT_UPDATE);
        }

        public bool Read(BaseStructRecv mRecv)
        {
            msg = GlobalFirstWarReports_PC.ParseFrom(mRecv.obj);
            return true;
        }
    }
}
