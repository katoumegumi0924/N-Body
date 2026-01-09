using UnityEngine;

/// <summary>
/// GameData：
/// </summary>
public class GameData
{
    public UniverseData universeData;
    public SimulationTimeData universeTime;

    public void Init()
    {
        universeData = new UniverseData();
        universeData.Init();

        universeTime = new SimulationTimeData();
        universeTime.Init();
    }

    public void SetNew()
    {
        universeData.worldBounds.SetBounds(GameConfig.universeConfig.width, 
                                           GameConfig.universeConfig.height, 
                                           GameConfig.universeConfig.centerPos);
    }

    public void Free()
    {
        if (universeData != null)
        {
            universeData.Free();
            universeData = null;
        }

        if (universeTime != null)
        {
            universeTime.Free();
            universeTime = null;
        }
    }
}