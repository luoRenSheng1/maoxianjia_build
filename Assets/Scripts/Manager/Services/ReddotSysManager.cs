
using System.Collections.Generic;
using Engine;
using EngineBase;
using msg;

public class RedPointInfo
{
    public eRedPointType Type;
    public int Param;
    public bool IsRead;
}
public class ReddotSysManager : Singleton<ReddotSysManager>
{
    private Dictionary<eRedPointType, RedPointInfo> _redpointDict = new Dictionary<eRedPointType, RedPointInfo>();
    public void OnInit()
    {

    }

    public void OnDispose()
    {
   
    }

    public void SendDailyResetCS()
    {
        var builder = DailyReset_CS.CreateBuilder();
        DailyReset_CS dailyReset = builder.Build();
        GameManager.Instance.Connection?.SendMessage((int) eMsgID.eMsg_DailyReset_CS, dailyReset);
    }

    // public void SendReadRedPointsCS()
    // {
    //     var builder = ReadRedPoints_CS.CreateBuilder();
    //     builder.RedPoints = ;
    //     GameManager.Instance.Connection?.SendMessage((int) eMsgID.eMsg_ReadRedPoints_CS, builder.Build());
    // }

    public void SendRedPointCS()
    {
        var builder = GetRedPoints_CS.CreateBuilder();
        GetRedPoints_CS redPointsCs = builder.Build();
        GameManager.Instance.Connection?.SendMessage((int) eMsgID.eMsg_GetRedPoints_CS, redPointsCs);
    }

    public void SyncRedPointInfo(eRedPointType type, RedPointInfo redPointInfo)
    {
        if (_redpointDict.ContainsKey(type))
        {
            _redpointDict[type] = redPointInfo;
        }
        else
        {
            _redpointDict.Add(type, redPointInfo);
        }
    }

    public void ReadRedPoint(eRedPointType type)
    {
        if (_redpointDict.ContainsKey(type))
        {
            _redpointDict[type].IsRead = true;
        }
        
        EventDispatcher.GameWorld.DispatchEvent(EventDefine.EVENT_REDPOINT_UPDATE);
    }

    public RedPointInfo GetRedPointByType(eRedPointType type)
    {
        if (_redpointDict.TryGetValue(type, out RedPointInfo redPointInfo))
        {
            return redPointInfo;
        }

        return null;
    }
}
