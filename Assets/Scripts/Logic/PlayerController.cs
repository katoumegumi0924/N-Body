using UnityEngine;

/// <summary>
/// PlayerInput：
/// </summary>
public class PlayerController
{
    private AstroData _data;
    private AstroProtoSet _protoSet;
    private LineRenderer _dragLine;
    private UniverseGen _universeGen;

    private bool _isDragging;
    private Vector2 _dragStart;
    private Camera _mainCamera;

    public void Init(AstroData data, AstroProtoSet protoSet, LineRenderer line, UniverseGen universeGen)
    {
        _data = data;
        _protoSet = protoSet;
        _dragLine = line;
        _universeGen = universeGen;

        _mainCamera = Camera.main;

        if (_dragLine != null)
            _dragLine.enabled = false;
    }

    public void Free()
    {
        _data = null;
        _protoSet = null;
        _isDragging = false;

        if (_dragLine != null)
        {
            _dragLine.enabled = false;
            _dragLine.positionCount = 0;
        }
    }

    public void OnUpdate()
    {
        HandleMouseInput();
        HandleKeyboardInput();
    }

    public void HandleMouseInput()
    {
        Vector2 mousePos = GetWorldMousePos();

        if (Input.GetMouseButtonDown(0))
        {
            _isDragging = true;
            _dragStart = mousePos;
            DrawLine(true, _dragStart, _dragStart);
        }

        if (_isDragging)
        {
            if (_dragLine != null)
                _dragLine.SetPosition(1, mousePos);

            if (Input.GetMouseButtonUp(0))
            {
                _isDragging = false;
                DrawLine(false, Vector2.zero, Vector2.zero);

                Vector2 velocity = (_dragStart - mousePos) * GameConfig.universeConfig.launchForce;
                SpawnRandomAstro(mousePos, velocity);
            }
        }
    }

    private void HandleKeyboardInput()
    {
        if (_universeGen == null) return;
        if (_mainCamera == null) _mainCamera = Camera.main;
        float h = _mainCamera.orthographicSize;

        // 生成稳定的模拟天体系统
        if (Input.GetKeyDown(KeyCode.Alpha1)) 
            _universeGen.LoadBinaryStars(h);
        if (Input.GetKeyDown(KeyCode.Alpha2))
            _universeGen.LoadStarSystem(h);

        // 重置/清空
        if (Input.GetKeyDown(KeyCode.R))
            _data.ClearAll();
    }

    private Vector2 GetWorldMousePos()
    {
        Vector3 mouseScreenPos = Input.mousePosition;
        Vector3 worldPos = _mainCamera.ScreenToWorldPoint(mouseScreenPos);
        return new Vector2(worldPos.x, worldPos.y);
    }

    private void DrawLine(bool active, Vector2 startPos, Vector2 endPos)
    {
        if (_dragLine == null)
            return;
        if (_dragLine.enabled != active)
            _dragLine.enabled = active;
        if (active)
        {
            _dragLine.SetPosition(0, startPos);
            _dragLine.SetPosition(1, endPos);
        }
    }

    private void SpawnRandomAstro(Vector2 pos, Vector2 vel)
    {
        if (_protoSet.astroProtoList.Count == 0)
            return;
        var proto = _protoSet.astroProtoList[Random.Range(0, _protoSet.astroProtoList.Count)];
        _data.CreateAstro(proto, pos, vel, Time.time);
    }
}