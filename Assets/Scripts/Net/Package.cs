using System;

namespace Engine
{
    public class Package
    {
        public int type;
        public byte[] body;

        public Package(int type, byte[] body)
        {
            this.type = type;
            this.body = body;
        }
    }
}