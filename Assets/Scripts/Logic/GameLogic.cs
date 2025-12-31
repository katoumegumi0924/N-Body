using UnityEngine;

/// <summary>
/// GameLogic：
/// </summary>
public class GameLogic
{
    private GameData gameData;
    private UniverseLogic universeLogic;
    private PlayerController playerController;
    private CameraController cameraController;
    private UniverseGen universeGen;

    // 时间累加器 指定时间执行一次物理Logic，避免倍速情况下dt过大
    private double accumulator;

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
        cameraController.Init(Camera.main);
    }

    public void Free()
    {
        gameData = null;
        universeLogic = null;
        playerController = null;
    }

    public void GameTick(float deltaTime)
    {
        // 获取固定步长和最大步数
        double pStep = GameConfig.universeConfig.physicsStep;
        int maxSteps = GameConfig.universeConfig.maxStepsPerFrame;

        double dt = deltaTime * gameData.clock.timeScale;
        accumulator += dt;
        int stepCount = 0;
        while (accumulator >= pStep)
        { 
            // 推进Ticks
            long ticksInStep = gameData.clock.ToTicks(pStep);
            gameData.clock.totalTicks += ticksInStep;

            // 执行核心物理逻辑
            universeLogic.LogicTick(gameData, pStep, cameraController.screenBounds);

            // 消耗累加器
            accumulator -= pStep;

            // 限制单次执行的最大步数
            stepCount++;
            if (stepCount > maxSteps)
            {
                accumulator = 0;
                break;
            }
        }  
    }

    public void OnUpdate()
    {
        cameraController.OnUpdate();
        playerController.OnUpdate();
    }

}
