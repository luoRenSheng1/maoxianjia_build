using msg;

namespace Engine
{
    /// <summary>
    /// 离开奇遇山洞小地图（接收回包）
    /// </summary>
    public class ExitCave_SCRecv : IReceiver
    {
        public ExitCave_SC msg;
        public int MsgID()
        {
            return (int)eMsgID.eMsg_ExitCave_SC;
        }

        public void Process()
        {
            if (msg == null || msg.Result != eErrCode.eErrCode_Success) { return; }

        }

        public bool Read(BaseStructRecv mRecv)
        {
            LogUtils.LogWarning("ExitCave_SC_Recv Read " + mRecv.Length);
            msg = ExitCave_SC.ParseFrom(mRecv.obj);
            return true;
        }
    }
}