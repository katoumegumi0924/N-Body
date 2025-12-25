using UnityEngine;

/// <summary>
/// GameMain：
/// </summary>
public class GameMain : MonoBehaviour
{
    [Header("Config")]
    public GameConfig config;
    public AstroProtoSet protoSet;
    public AstroProto debrisProto;

    [Header("Render")]
    public Mesh mesh;
    public Material material;
    public LineRenderer dragLine;

    private AstroData _data;
    private AstroPhysics _physics;
    private AstroRender _render;
    private PlayerInput _input;
    private UniverseGen _universeGen;

    private Vector2 screenBounds;

    private void OnEnable()
    {
        _data = new AstroData();
        _physics = new AstroPhysics(_data, config, debrisProto);
        _render = new AstroRender(_data, protoSet, mesh, material);
        _universeGen = new UniverseGen(_data, config, protoSet);
        _input = new PlayerInput(_data, protoSet, dragLine, config, _universeGen);

        screenBounds = GetScreenBounds();

        // 尝试获取挂在同一个物体上的 ScenarioLoader
        var scenarioLoader = GetComponent<ScenarioLoader>();
        if (scenarioLoader != null)
        {
            // 【关键一步】手动注入依赖
            scenarioLoader.data = _data;

            // 为了方便，如果 ScenarioLoader 里的 config/protoSet 没拖，
            // 也可以在这里顺便赋值过去
            if (scenarioLoader.config == null) scenarioLoader.config = config;
            if (scenarioLoader.protoSet == null) scenarioLoader.protoSet = protoSet;

            Debug.Log("ScenarioLoader initialized with AstroData.");
        }
    }

    private void OnDisable()
    {
        if (_input != null)
        {
            _input.Free();
            _input = null;
        }

        if (_render != null)
        {
            _render.Free();
            _render = null;
        }

        if (_physics != null)
        {
            _physics.Free();
            _physics = null;
        }

        if (_data != null)
        {
            _data.Free();
            _data = null;
        }
    }

    private void Update()
    {
        // 处理输入
        _input.OnUpdate();

        // 渲染提交
        _render.RenderTick();
    }

    private void FixedUpdate()
    {
        // 物理模拟 
        _physics.LogicTick(Time.fixedDeltaTime, screenBounds);
    }

    private Vector2 GetScreenBounds()
    {
        // 计算屏幕边界
        float h = Camera.main.orthographicSize;
        float w = h * Camera.main.aspect;
        return new Vector2(w, h);
    }
}
