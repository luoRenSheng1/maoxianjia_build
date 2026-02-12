using FairyGUI;
using msg;
using UnityEngine;

namespace Engine
{
    using EngineBase;

    public class KickPlayer_SCRecv : IReceiver
    {
        public KickPlayer_SC msg;

        public int MsgID()
        {
            return (int)eMsgID.eMsg_KickPlayer_SC;
        }

        public void Process()
        {
            GTween.Clean();
            ItemInfoManager.Instance.Clear();
            EquipManager.Instance.Clear();
            GameManager.Instance.Connection.ActiveClose();
            UIManager.Instance.CloseAllUIPanel();
            UIManager.Instance.ShowUIPanel("Login");
            GameManager.Instance.Pause = true;
            MessageBoxView.MessageParam param = new MessageBoxView.MessageParam
            {
                OkCallBack = null
            };
            UIManager.Instance.ShowUIPanel("MessageBox", ConfigUtils.GetStringByKey(205), param);
        }

        public bool Read(BaseStructRecv mRecv)
        {
            msg = KickPlayer_SC.ParseFrom(mRecv.obj);
            return true;
        }
    }
}
