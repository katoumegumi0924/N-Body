using UnityEngine;

/// <summary>
/// GameData：
/// </summary>
public class GameData
{
    public UniverseData universeData;
    public TimeData universeTimeData;

    public void Init()
    {
        universeData = new UniverseData();
        universeData.Init();

        universeTimeData = new TimeData();
        universeTimeData.Init();
    }

    public void SetNew()
    {
        universeData.SetNew();
        universeTimeData.SetNew();
    }

    public void Free()
    {
        if (universeData != null)
        {
            universeData.Free();
            universeData = null;
        }

        if (universeTimeData != null)
        {
            universeTimeData.Free();
            universeTimeData = null;
        }
    }
}