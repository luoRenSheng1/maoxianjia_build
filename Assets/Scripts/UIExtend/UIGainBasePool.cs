using System.Collections.Generic;
using Engine;

public class ObjectPool<T> where T : class, new()
{
    private Stack<T> m_objectStack = new Stack<T>();

    public T New()
    {
        return (m_objectStack.Count == 0) ? new T() : m_objectStack.Pop();
    }

    public void Store(T t)
    {
        m_objectStack.Push(t);
    }
}

public class UIGainBasePool
{
    private static ObjectPool<UIItemsGain> _poolUIGain = new ObjectPool<UIItemsGain>();

    public static UIItemsGain CreateUIGainBase()
    {
        // if(!VillageInfoManager.Instance.IsInVillageHome)
        //     // GameManager.Instance.SoundManager.PlayEffect(12);
        return _poolUIGain.New();
    }
    
    public static void ReycleUIGainBase(UIItemsGain gainBase)
    {
        _poolUIGain.Store(gainBase);
    }
}
