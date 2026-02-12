using msg;

namespace Engine
{
    /// <summary>
    /// 离开深埋宝藏小地图（接收回包）
    /// </summary>
    public class ExitBoxMap_SCRecv : IReceiver
    {
        public ExitBoxMap_SC msg;
        public int MsgID()
        {
            return (int)eMsgID.eMsg_ExitBoxMap_SC;
        }

        public void Process()
        {
            if (msg == null || msg.Result != eErrCode.eErrCode_Success) { return; }

        }

        public bool Read(BaseStructRecv mRecv)
        {
            LogUtils.LogWarning("ExitBoxMap_SC_Recv Read " + mRecv.Length);
            msg = ExitBoxMap_SC.ParseFrom(mRecv.obj);
            return true;
        }
    }
}