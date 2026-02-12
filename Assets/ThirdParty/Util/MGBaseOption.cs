namespace ThirdParty.Util
{
    public class MGBaseOption<T>
    {
        public System.Action<T> success;
        public System.Action<T> fail;
    }
}