using UnityEngine;

/// <summary>
/// PlayerControllerGizmos：
/// </summary>
public class PlayerControllerGizmos
{
    public DragLineRenderer dragLineRenderer;
    public BoxRenderer worldBoundsRenderer;

    public void Init()
    {
        dragLineRenderer = new DragLineRenderer();
        dragLineRenderer.Init();

        worldBoundsRenderer = new BoxRenderer();
        worldBoundsRenderer.Init();
    }

    public void Free()
    {
        if (dragLineRenderer != null)
        {
            dragLineRenderer.Free();
            dragLineRenderer = null;

            worldBoundsRenderer.Free();
            worldBoundsRenderer = null;
        }
    }

    public void Draw(GameData gameData, GameLogic gameLogic)
    {
        // 渲染拖拽路径
        dragLineRenderer.Draw(gameLogic.playerController.isDragging,
                              gameLogic.playerController.dragStartPos,
                              gameLogic.playerController.dragEndPos);

        // 渲染世界边界
        worldBoundsRenderer.Draw(gameData.universeData.worldBounds);
    }
}
