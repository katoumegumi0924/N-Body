using UnityEngine;
using UnityEngine.EventSystems;

/// <summary>
/// PlayerInput：
/// </summary>
public class PlayerController
{
    private GameData gameData;
    private UniverseGen universeGen;

    public void Init(GameData _data, UniverseGen _universeGen)
    {
        gameData = _data;
        universeGen = _universeGen;
    }

    public void Free()
    {
        gameData = null;
    }

    public void OnUpdate()
    {
        HandleMouseInput();
        HandleKeyboardInput();
    }

    public void HandleMouseInput()
    {
        DVector2 mousePos = GetWorldMousePos();
        if (Input.GetMouseButtonDown(0) && EventSystem.current != null)
        {
            if (EventSystem.current.IsPointerOverGameObject())
            {
                return; // 鼠标在UI上，不执行任何拖拽逻辑
            }
        }

        if (Input.GetMouseButtonDown(0))
        {
            gameData.interactionData.isDragging = true;
            gameData.interactionData.dragStartPos = mousePos;
        }

        if (gameData.interactionData.isDragging)
        {
            gameData.interactionData.dragEndPos = mousePos;
            if (Input.GetMouseButtonUp(0))
            {
                gameData.interactionData.isDragging = false;
                gameData.interactionData.dragEndPos = mousePos;
                DVector2 velocity = (gameData.interactionData.dragStartPos - mousePos) * GameConfig.universeConfig.launchForce;
                SpawnRandomAstro(mousePos, velocity);
            }
        }
    }

    private void HandleKeyboardInput()
    {
        if (universeGen == null) 
            return;

        float h = (float)gameData.universeData.worldBounds.height;

        // 生成稳定的模拟天体系统
        if (Input.GetKeyDown(KeyCode.Alpha1)) 
            universeGen.LoadBinaryStars(h);
        if (Input.GetKeyDown(KeyCode.Alpha2))
            universeGen.LoadStarSystem(h);
        if (Input.GetKeyDown(KeyCode.Alpha3))
            universeGen.LoadThreeBodyFigure8(h);
        if (Input.GetKeyDown(KeyCode.Alpha4))
            universeGen.LoadSunEarthMoon(h);

        // 重置/清空
        if (Input.GetKeyDown(KeyCode.R))
            gameData.universeData.ClearAll();
    }

    private DVector2 GetWorldMousePos()
    {
        Vector3 mouseScreenPos = Input.mousePosition;
        Vector3 worldPos = Camera.main.ScreenToWorldPoint(mouseScreenPos);
        return new DVector2(worldPos.x, worldPos.y);
    }

    private void SpawnRandomAstro(DVector2 pos, DVector2 vel)
    {
        if (ProtoDB.ProtoSet.Count == 0)
            return;
        int protoIndex = Random.Range(0, ProtoDB.ProtoSet.Count);
        gameData.universeData.CreateAstro(protoIndex, pos, vel, gameData.clock.totalTicks);
    }
}