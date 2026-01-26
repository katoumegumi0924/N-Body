using UnityEngine;

/// <summary>
/// GameModel：
/// </summary>
public class GameModel
{
    private GameData gameData;
    private GameLogic gameLogic;
    private AstroRenderer astroRenderer;
    private PlayerControllerGizmos playerControllerGizmos;

    public void Init(GameData _gameData, GameLogic _gameLogic)
    {
        gameData = _gameData;
        gameLogic = _gameLogic;

        astroRenderer = new AstroRenderer();
        astroRenderer.Init();

        playerControllerGizmos = new PlayerControllerGizmos();
        playerControllerGizmos.Init();
    }

    public void Free()
    {
        gameData = null;

        if (astroRenderer != null)
        {
            astroRenderer.Free();
            astroRenderer = null;
        }

        if (playerControllerGizmos != null)
        {
            playerControllerGizmos.Free();
            playerControllerGizmos = null;
        }
    }

    public void OnUpdate()
    {
        // 渲染天体
        astroRenderer.RenderTick(gameData);

        // 渲染辅助路径与线框
        playerControllerGizmos.Draw(gameData, gameLogic);
    }
}