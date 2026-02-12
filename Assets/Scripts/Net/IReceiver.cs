namespace Engine
{
    using System;

    public interface IReceiver
    {
        int MsgID();
        void Process();
        /// <summary>
        /// warning: this function running on child thread
        /// </summary>
        bool Read(BaseStructRecv mRev);
    }
}

