using UnityEngine;

/// <summary>
/// GameModel：
/// </summary>
public class GameModel
{
    private GameData gameData;
    private AstroRender astroRender;
    private DragLineRender dragLineRender;

    public void Init(GameData _gameData)
    {
        gameData = _gameData;

        astroRender = new AstroRender();
        astroRender.Init();

        dragLineRender = new DragLineRender();
        dragLineRender.Init();
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
    }

    public void OnUpdate()
    {
        astroRender.RenderTick(gameData);
        dragLineRender.Draw(gameData.interactionData.isDragging,
                            gameData.interactionData.dragStartPos,
                            gameData.interactionData.dragEndPos);
    }
}
