namespace Engine
{
    using System;
    using System.Text;
    using EngineBase;
    
    [Serializable]
    public class BaseStructRecv
    {
        private string key;
        private int iMsgID;

        private byte[] mObj;

        public BaseStructRecv()
        {
        }

        public BaseStructRecv(int msgID)
        {
            this.iMsgID = msgID;
        }

        public void Init(int msgID)
        {
            iMsgID = msgID;
        }

        public string Key
        {
            get
            {
                return key;
            }
            set
            {
                key = value;
            }
        }

        public byte[] obj
        {
            get
            {
                return this.mObj;
            }
            set
            {
                this.mObj = value;
            }
        }

        public int IMsgID
        {
            get { return this.iMsgID; }

            set { this.iMsgID = value; }
        }

        public int Length
        {
            get { return (this.obj != null) ? this.obj.Length : 0; }
        }

        public void Clear()
        {
            iMsgID = -1;
            mObj = null;
        }
    }
}
