using msg;
using UnityEngine;

namespace Engine
{
    using EngineBase;
    
    public class TestMsg_Proto2_SCRecv : IReceiver
    {
        public TestMsg_Proto2_SC msg;

        public int MsgID()
        {
            return (int)eMsgID.eTestMsg_Proto2_SC;
        }

        public void Process()
        {
            LogUtils.LogWarningFormat("TestMsg_Proto2_SCRecv Process {0} {1}", msg.TestInt32, msg.TestString);
            for (int i = 0; i < msg.TestRepeatedMemberList.Count; i++)
            {
                MemberTestAttr memberTestAttr = msg.TestRepeatedMemberList[i];
                Debug.Log(memberTestAttr.AttrId);
                Debug.Log(memberTestAttr.AttrValue);
                Debug.Log(memberTestAttr.AttrName);
                for (int j = 0; j < memberTestAttr.AttrRepeatedIdList.Count; j++)
                {
                    Debug.Log(memberTestAttr.AttrRepeatedIdList[j]);
                }
                
                for (int j = 0; j < memberTestAttr.AttrRepeatedValueList.Count; j++)
                {
                    Debug.Log(memberTestAttr.AttrRepeatedValueList[j]);
                }
            }

            
        }

        public bool Read(BaseStructRecv mRecv)
        {
            msg = TestMsg_Proto2_SC.ParseFrom(mRecv.obj);
            return true;
        }
    }
}
