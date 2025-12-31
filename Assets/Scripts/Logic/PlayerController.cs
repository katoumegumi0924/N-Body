using UnityEngine;
using UnityEngine.EventSystems;

/// <summary>
/// PlayerInput：
/// </summary>
public class PlayerController
{
    private GameData data;
    private UniverseGen universeGen;
    private Camera mainCamera;

    public void Init(GameData _data, UniverseGen _universeGen)
    {
        data = _data;
        universeGen = _universeGen;
        mainCamera = Camera.main;
    }

    public void Free()
    {
        data = null;
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
            data.interactionData.isDragging = true;
            data.interactionData.dragStartPos = mousePos;
        }

        if (data.interactionData.isDragging)
        {
            data.interactionData.dragEndPos = mousePos;
            if (Input.GetMouseButtonUp(0))
            {
                data.interactionData.isDragging = false;
                data.interactionData.dragEndPos = mousePos;
                DVector2 velocity = (data.interactionData.dragStartPos - mousePos) * GameConfig.universeConfig.launchForce;
                SpawnRandomAstro(mousePos, velocity);
            }
        }
    }

    private void HandleKeyboardInput()
    {
        if (universeGen == null) 
            return;
        if (mainCamera == null) 
            mainCamera = Camera.main;

        float h = mainCamera.orthographicSize;

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
            data.universeData.ClearAll();
    }

    private DVector2 GetWorldMousePos()
    {
        Vector3 mouseScreenPos = Input.mousePosition;
        Vector3 worldPos = mainCamera.ScreenToWorldPoint(mouseScreenPos);
        return new DVector2(worldPos.x, worldPos.y);
    }

    private void SpawnRandomAstro(DVector2 pos, DVector2 vel)
    {
        if (ProtoDB.ProtoSet.Count == 0)
            return;
        int protoIndex = Random.Range(0, ProtoDB.ProtoSet.Count);
        data.universeData.CreateAstro(protoIndex, pos, vel, data.clock.totalTicks);
    }
}