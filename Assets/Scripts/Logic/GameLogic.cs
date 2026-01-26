using UnityEngine;

/// <summary>
/// GameLogic：
/// </summary>
public class GameLogic
{
    private GameData gameData;
    private UniverseLogic universeLogic;
    private UniverseGen universeGen;

    public PlayerController playerController;
    public CameraController cameraController;

    public void Init(GameData _gameData)
    {
        gameData = _gameData;

        universeLogic = new UniverseLogic();
        universeLogic.Init();

        universeGen = new UniverseGen();
        universeGen.Init(gameData);

        playerController = new PlayerController();
        playerController.Init(gameData, universeGen);

        cameraController = new CameraController();
        cameraController.Init(gameData);
    }

    public void Free()
    {
        gameData = null;

        if (universeLogic != null)
        {
            universeLogic.Free();
            universeLogic = null;
        }
        
        if (universeGen != null)
        {
            universeGen.Free();
            universeGen = null;
        }

        if (playerController != null)
        {
            playerController.Free();
            playerController = null;
        }
        
        if (cameraController != null)
        {
            cameraController.Free();
            cameraController = null;
        }
    }

    public void SetNew()
    {
        universeGen.SetNew();
    }

    public void GameTick()
    {
        gameData.universeTimeData.EarlyTick();

        universeLogic.LogicTick(gameData);

        gameData.universeTimeData.LateTick();
    }

    public void OnUpdate()
    {
        playerController.OnUpdate();
        cameraController.OnUpdate(gameData);
    }
}