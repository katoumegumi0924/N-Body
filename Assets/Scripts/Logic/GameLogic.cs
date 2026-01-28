using UnityEngine;

/// <summary>
/// GameLogic：
/// </summary>
public class GameLogic
{
    private GameData gameData;
    private UniverseLogic universeLogic;
    private UniverseGen universeGen;

    public TimeLogic universeTimeLogic;
    public PlayerController playerController;
    public CameraController cameraController;

    public void Init(GameData _gameData)
    {
        gameData = _gameData;

        universeLogic = new UniverseLogic();
        universeLogic.Init();

        universeGen = new UniverseGen();
        universeGen.Init(gameData);

        universeTimeLogic = new TimeLogic();
        universeTimeLogic.Init(gameData);

        playerController = new PlayerController();
        playerController.Init(gameData, universeGen);

        cameraController = new CameraController();
        cameraController.Init();
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

        if (universeTimeLogic != null)
        {
            universeTimeLogic.Free();
            universeTimeLogic = null;
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
        universeTimeLogic.SetNew();
        playerController.SetNew();
        cameraController.SetNew();
    }

    public void GameTick()
    {
        universeTimeLogic.EarlyTick();

        universeLogic.LogicTick(gameData);

        universeTimeLogic.LateTick();
    }

    public void OnUpdate()
    {
        playerController.OnUpdate();
        cameraController.OnUpdate();
    }
}