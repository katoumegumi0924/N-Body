using UnityEngine;

/// <summary>
/// GameData：
/// </summary>
public class GameData
{
    public UniverseData universeData;
    public InteractionData interactionData;
    public SimulationClock clock;

    public void Init()
    {
        universeData = new UniverseData();
        universeData.Init();

        interactionData = new InteractionData();
        interactionData.Reset();

        clock = new SimulationClock();
        clock.Init();
    }

    public void Free()
    {
        if (universeData != null)
        {
            universeData.Free();
            universeData = null;
        }

        if (clock != null)
        {
            clock.Free();
            clock = null;
        }
    }
}
