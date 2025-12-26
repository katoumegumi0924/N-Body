using UnityEngine;

/// <summary>
/// GameMain：
/// </summary>
public class GameMain : MonoBehaviour
{
    private GameData gameData;
    private GameLogic gameLogic;
    private GameRender gameRender;

    public void Init()
    {
        gameData = new GameData();
        gameData.Init();

        gameLogic = new GameLogic();
        gameLogic.Init(gameData.astroData, GameConfig.gameResourcesConfig.protoSet);

        gameRender = new GameRender();
        gameRender.Init(gameData.astroData, GameConfig.gameResourcesConfig.protoSet, 
                         GameConfig.gameResourcesConfig.astroMesh, GameConfig.gameResourcesConfig.astroMaterial);
    }

    public void Free()
    {
        if (gameRender != null)
        {
            gameRender.Free();
            gameRender = null;
        }

        if (gameLogic != null)
        {
            gameLogic.Free();
            gameLogic = null;
        }

        if (gameData != null)
        {
            gameData.Free();
            gameData = null;
        }
    }

    private void Update()
    {
        gameLogic.OnUpdate();
        gameRender.OnUpdate();
    }

    private void FixedUpdate()
    {
        gameLogic.GameTick(Time.fixedDeltaTime);
    }

    private void OnEnable()
    {
        Init();
    }

    private void OnDisable()
    {
        Free();
    }
}
