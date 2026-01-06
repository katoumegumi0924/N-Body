using UnityEngine;

/// <summary>
/// GameModel：
/// </summary>
public class GameModel
{
    private GameData gameData;
    private AstroRender astroRender;
    private DragLineRender dragLineRender;
    private BoundsRender boundsRender;
    private SimulationSpeedView uiView;

    private Camera camera;

    public void Init(GameData _gameData)
    {
        gameData = _gameData;

        astroRender = new AstroRender();
        astroRender.Init();

        dragLineRender = new DragLineRender();
        dragLineRender.Init();

        boundsRender = new BoundsRender();
        boundsRender.Init();

        uiView = Object.FindAnyObjectByType<SimulationSpeedView>();
        if (uiView != null)
            uiView.Init(gameData);
    }

    public void Free()
    {
        if (gameData != null)
        {
            for (int i = 0; i < gameData.universeData.pool.cursor; ++i)
            {
                gameData.universeData.pool[i].Reset();
            }
        }

        if (astroRender != null)
        {
            astroRender.Free();
            astroRender = null;
        }

        if (dragLineRender != null)
        {
            dragLineRender.Free();
            dragLineRender = null;
        }

        if (boundsRender != null)
        {
            boundsRender.Free();
            boundsRender = null;
        }

        if (uiView != null)
        {
            uiView.Free();
            uiView = null;
        }
    }

    public void OnUpdate()
    {
        // 渲染天体
        astroRender.RenderTick(gameData);

        // 渲染拖拽路径
        dragLineRender.Draw(gameData.interactionData.isDragging,
                            gameData.interactionData.dragStartPos,
                            gameData.interactionData.dragEndPos);

        // 渲染倍速文字
        uiView.RefreshSpeedText(gameData.clock.timeScale);

        // 渲染世界边界
        boundsRender.Draw(gameData.universeData.worldBounds);

        // 同步相机数据
        SyncCamera(gameData);
    }

    // 同步相机数据
    private void SyncCamera(GameData gameData)
    {
        if (camera == null)
        {
            camera = Camera.main;
        }

        var state = gameData.cameraState;
        camera.transform.position = new Vector3(state.position.x, state.position.y, -10f);
        camera.orthographicSize = state.zoom;
    }
}