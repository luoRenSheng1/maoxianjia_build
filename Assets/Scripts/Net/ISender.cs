namespace Engine
{
    using System;

    public interface ISender
    {
        int MsgID();
        bool Build(object data);
        bool Send(BaseStructSend send);
        PacketReliability GetPackageMode();
    }
}

