using UnityEngine;

/// <summary>
/// GameData：
/// </summary>
public class GameData
{
    public UniverseData universeData;
    public SimulationClock clock;
    public InteractionData interactionData;
    public CameraState cameraState;

    public void Init()
    {
        universeData = new UniverseData();
        universeData.Init();

        clock = new SimulationClock();
        clock.Init();

        interactionData = new InteractionData();
        interactionData.Reset();

        cameraState = new CameraState();
        cameraState.Reset();
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

        interactionData.Reset();
        cameraState.Reset();
    }
}