namespace Engine
{
    using System;
    using EngineBase;
    
    public class BaseStructSend
    {
        public int iSubCommand;

        public byte[] buffer;
        private int mPosition;

        private byte[] mObj;

        public BaseStructSend()
        {
            this.mPosition = 0;
            this.iSubCommand = 0;
        }

        public int Length
        {
            get
            {
                return this.mPosition;
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

        public void Reset(int subCommand)
        {
            this.mPosition = 0;
            this.iSubCommand = subCommand;
        }

        public void lobbyNormal(ProtocolMessage protocol, object context)
        {
            buffer = PackageProtocol.encode(iSubCommand, obj);
        }
    }
}
