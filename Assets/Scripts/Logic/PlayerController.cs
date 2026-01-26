using UnityEngine;

/// <summary>
/// PlayerController：
/// </summary>
public class PlayerController
{
    private GameData gameData;
    private UniverseGen universeGen;

    private bool isInputEnable = true;
    public bool isDragging = false;
    public Vector2 dragStartPos;
    public Vector2 dragEndPos;

    public void Init(GameData _data, UniverseGen _universeGen)
    {
        gameData = _data;
        universeGen = _universeGen;
    }

    public void Free()
    {
        gameData = null;
        
        if (universeGen != null)
        {
            universeGen.Free();
            universeGen = null;
        }
    }

    public void OnUpdate()
    {
        HandleMouseInput();
        HandleKeyboardInput();
    }

    public void HandleMouseInput()
    {
        // 演示稳定天体系统时禁用鼠标生成新的天体
        if (!isInputEnable)
            return;

        Vector2 mousePos = GetWorldMousePos();
        if (Input.GetMouseButtonDown(0))
        {
            isDragging = true;
            dragStartPos = mousePos;
        }

        if (isDragging)
        {
            dragEndPos = mousePos;
            if (Input.GetMouseButtonUp(0))
            {
                isDragging = false;
                dragEndPos = mousePos;
                Vector2 velocity = (dragStartPos - dragEndPos) * GameConfig.universeConfig.launchForce;
                SpawnRandomAstro(mousePos, velocity);
            }
        }
    }

    private void HandleKeyboardInput()
    {
        if (universeGen == null) 
            return;

        float orbitRadius = Mathf.Min(gameData.universeData.worldBounds.height, gameData.universeData.worldBounds.width);

        // 生成稳定的模拟天体系统
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            universeGen.LoadBinaryStars(orbitRadius);
            isInputEnable = false;
        } 
            
        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            universeGen.LoadStarSystem(orbitRadius);
            isInputEnable = false;
        }
            
        if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            universeGen.LoadThreeBodyFigure8(orbitRadius);
            isInputEnable = false;
        }
            
        if (Input.GetKeyDown(KeyCode.Alpha4))
        {
            universeGen.LoadSunEarthMoon(orbitRadius);
            isInputEnable = false;
        }

        if (Input.GetKeyDown(KeyCode.Alpha5))
        {
            universeGen.LoadHierarchicalTripleSystem(orbitRadius);
            isInputEnable = false;
        }

        if (Input.GetKeyDown(KeyCode.Alpha6))
        {
            universeGen.LoadLagrangePoints(orbitRadius);
            isInputEnable = false;
        }

        if (Input.GetKeyDown(KeyCode.Alpha7))
        {
            universeGen.LoadKlempererRosette(orbitRadius);
            isInputEnable = false;
        }

        if (Input.GetKeyDown(KeyCode.Alpha8))
        {
            universeGen.LoadKlempererRosetteCore(orbitRadius);
            isInputEnable = false;
        }

        if (Input.GetKeyDown(KeyCode.Alpha9))
        {
            universeGen.LoadStableLagrangeTriangle(orbitRadius);
            isInputEnable = false;
        }

        // 重置/清空
        if (Input.GetKeyDown(KeyCode.R))
        {
            universeGen.ResetScene();
            isInputEnable = true;
        }    
    }

    private Vector2 GetWorldMousePos()
    {
        Vector3 mouseScreenPos = Input.mousePosition;
        Vector3 worldPos = Camera.main.ScreenToWorldPoint(mouseScreenPos);
        return new Vector2(worldPos.x, worldPos.y);
    }

    private void SpawnRandomAstro(Vector2 pos, Vector2 vel)
    {
        if (ProtoDB.protoSet.Count == 0)
            return;
        int protoIndex = Random.Range(0, ProtoDB.protoSet.Count);
        gameData.universeData.CreateAstro(protoIndex, pos, vel, gameData.universeTimeData.tickCounter);
    }
}