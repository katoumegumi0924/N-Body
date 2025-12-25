using UnityEngine;

public class ScenarioLoader : MonoBehaviour
{
    // 强依赖注入
    public AstroData data;
    public GameConfig config;
    public AstroProtoSet protoSet; // 需要确保里面有 Planet, Star 等类型的原型

    // 用于画轨迹验证（Gizmos）
    private bool _showDebugTrails = true;

    private void Update()
    {
        // 快捷键加载场景
        if (Input.GetKeyDown(KeyCode.Alpha1)) LoadTwoBodyCircular();
        if (Input.GetKeyDown(KeyCode.Alpha2)) LoadThreeBodyFigure8();
        if (Input.GetKeyDown(KeyCode.Alpha3)) LoadGalaxyDisk();
        if (Input.GetKeyDown(KeyCode.Alpha4)) LoadBinaryStars();
        if (Input.GetKeyDown(KeyCode.Alpha5)) LoadSolarSystem();
        if (Input.GetKeyDown(KeyCode.Alpha6)) LoadSunEarthMoon();
    }

    /// <summary>
    /// 验证 1：完美的圆形轨道
    /// </summary>
    public void LoadTwoBodyCircular()
    {
        ResetScene();

        // 假设 G = 1 (或者从 config 读取)
        float G = config.G;

        // 1. 创建恒星 (中心)
        // 假设 ProtoId 1 是恒星, 2 是行星
        var starProto = protoSet.astroProtoList[4];
        float starMass = 100f;
        Vector2 starPos = Vector2.zero;
        Vector2 starVel = Vector2.zero;

        data.CreateAstro(starProto, starPos, starVel, Time.time, starMass);

        // 2. 创建行星 (绕转)
        var planetProto = protoSet.astroProtoList[1];
        float planetMass = 10f; // 质量要足够小，不影响恒星 (或者计算双星质心)
        float radius = 20f;    // 轨道半径

        // 核心公式：v = Sqrt(GM / r)
        float speed = Mathf.Sqrt(G * starMass / radius);

        Vector2 planetPos = new Vector2(radius, 0);
        Vector2 planetVel = new Vector2(0, speed); // 垂直于位置向量

        data.CreateAstro(planetProto, planetPos, planetVel, Time.time, planetMass);

        Debug.Log("[Scenario] Two-Body Circular Loaded. Expected Orbit: Perfect Circle.");
    }

    public void LoadThreeBodyFigure8()
    {
        ResetScene();

        // --- 1. 自动适配屏幕大小 ---
        float screenHalfHeight = Camera.main.orthographicSize;

        // 让整个 8 字轨道占据屏幕高度的 60% 左右，留出 40% 的安全边距
        // 这样绝对不会撞到边界
        float scaleSize = screenHalfHeight * 0.6f;

        // 保持之前的视觉速度
        float scaleSpeed = 15f;

        // --- 2. 自动计算需要的质量 (物理核心) ---
        // 公式：M = (V^2 * R) / G
        // 只要公式在，无论 scaleSize 变得多小，质量都会自动变小以适应轨道
        float G_game = config.G;
        if (G_game < 0.0001f) { Debug.LogError("Config.G is too small!"); return; }

        float requiredMass = (scaleSpeed * scaleSpeed * scaleSize) / G_game;

        Debug.Log($"[Figure-8] 适配屏幕: ScaleSize={scaleSize}, Mass={requiredMass}");

        // --- 3. 创建天体 ---
        var proto = protoSet.astroProtoList.Count > 0 ? protoSet.astroProtoList[0] : null;

        // 8 字轨道的数学常数 (Chenciner & Montgomery)
        Vector2 p1_base = new Vector2(0.97000436f, -0.24308753f);
        Vector2 v1_base = new Vector2(0.46620368f, 0.43236573f);
        Vector2 v2_base = new Vector2(0.93240737f, 0.86473146f);

        Vector2[] posBase = new Vector2[] { p1_base, -p1_base, Vector2.zero };
        Vector2[] velBase = new Vector2[] { v1_base, v1_base, -2f * v1_base };

        for (int i = 0; i < 3; i++)
        {
            Vector2 finalPos = posBase[i] * scaleSize;
            Vector2 finalVel = velBase[i] * scaleSpeed;

            // 创建天体
            int id = data.CreateAstro(proto, finalPos, finalVel, Time.time, requiredMass);

            // --- 4. 强制修正半径 ---
            // 既然轨道变小了，天体本身也要变小，否则看起来很挤
            // 建议设置为轨道尺度的 1/15
            data.pool[id].radius = scaleSize / 15.0f;
        }
    }

    /// <summary>
    /// 验证 3：星系盘 (压力测试)
    /// </summary>
    public void LoadGalaxyDisk()
    {
        ResetScene();

        var starProto = protoSet.astroProtoList[0];
        var planetProto = protoSet.astroProtoList[1];

        float G = config.G;
        float centerMass = 5000f;

        // 1. 中心黑洞/恒星
        data.CreateAstro(starProto, Vector2.zero, Vector2.zero, Time.time, centerMass);

        // 2. 生成 500 个旋臂天体
        int count = 500;
        for (int i = 0; i < count; i++)
        {
            float angle = Random.Range(0f, Mathf.PI * 2);
            float dist = Random.Range(100f, 600f);

            // 开普勒速度 v = Sqrt(GM/r)
            float speed = Mathf.Sqrt(G * centerMass / dist);

            Vector2 pos = new Vector2(Mathf.Cos(angle), Mathf.Sin(angle)) * dist;

            // 速度方向：位置向量旋转 90 度
            Vector2 velDir = new Vector2(-pos.y, pos.x).normalized;
            Vector2 vel = velDir * speed;

            // 加上一点随机扰动，让星系更自然
            vel *= Random.Range(0.95f, 1.05f);

            data.CreateAstro(planetProto, pos, vel, Time.time, Random.Range(0.5f, 2f));
        }

        Debug.Log("[Scenario] Galaxy Disk Loaded.");
    }

    private void ResetScene()
    {
        // 暴力重置：因为我们不能直接调用 _data.Free() (那是彻底销毁内存)
        // 我们应该调用一个 "ClearAll" 方法。
        // 在 AstroData 中添加一个 ClearAll() { cursor = 1; count = 0; recycleCursor = 0; }
        // 暂时假设你有这个方法，或者手动回收所有

        // 这种写法比较低效但安全：
        for (int i = 1; i < data.cursor; i++)
        {
            if (data.pool[i].active)
                data.FreeAstro(i);
        }
        // 或者去 AstroData 加一个 FastClear
    }

    // 简易 Gizmos 辅助验证
    private void OnDrawGizmos()
    {
        if (!Application.isPlaying || data == null || !_showDebugTrails) return;

        Gizmos.color = Color.green;
        // 只画前几个天体的速度方向
        for (int i = 1; i < Mathf.Min(data.cursor, 10); i++)
        {
            if (data.pool[i].active)
            {
                Gizmos.DrawRay(data.pool[i].position, data.pool[i].velocity);
            }
        }
    }

    /// <summary>
    /// 系统 4：双星系统 (最稳的验证)
    /// </summary>
    public void LoadBinaryStars()
    {
        ResetScene();

        // 1. 设置基础参数
        float G = config.G;
        float screenH = Camera.main.orthographicSize;

        // 两个恒星距离屏幕中心的距离
        float radius = screenH * 0.5f;
        // 恒星质量 (大一点，引力才明显)
        float starMass = 2000f;

        // 2. 计算维持圆形轨道所需的速度
        // 物理公式推导：
        // 引力 F = G * M * M / (2r)^2  (距离是 2r)
        // 向心力 F = M * v^2 / r
        // 联立得：v = Sqrt(G * M / (4 * r))
        float speed = Mathf.Sqrt((G * starMass) / (4.0f * radius));

        // 3. 生成恒星 A (左边)
        // 这里的 10f 是给一个看起来不错的半径
        // 为了方便，可以直接指定半径，无视密度公式
        CreateVisualAstro(new Vector2(-radius, 0), new Vector2(0, speed), starMass, 8f, 0);

        // 4. 生成恒星 B (右边，速度相反)
        CreateVisualAstro(new Vector2(radius, 0), new Vector2(0, -speed), starMass, 8f, 0);

        Debug.Log("[Scenario] 双星系统加载完毕。理论上它们会永远转下去。");
    }

    /// <summary>
    /// 系统 5：随机太阳系 (非常解压)
    /// </summary>
    public void LoadSolarSystem()
    {
        ResetScene();

        float G = config.G;
        float screenH = Camera.main.orthographicSize;

        // 1. 创建中心恒星 (太阳)
        float sunMass = 5000f; // 质量一定要大
        // 太阳静止在中间
        CreateVisualAstro(Vector2.zero, Vector2.zero, sunMass, 10f, 3);

        // 2. 生成行星
        int planetCount = 50;
        var planetProto = protoSet.astroProtoList.Count > 1 ? protoSet.astroProtoList[1] : protoSet.astroProtoList[0];

        for (int i = 0; i < planetCount; i++)
        {
            // 随机轨道半径 (从很近到屏幕边缘)
            float dist = Random.Range(screenH * 0.2f, screenH * 1.5f);

            // 随机角度
            float angle = Random.Range(0f, Mathf.PI * 2);
            Vector2 pos = new Vector2(Mathf.Cos(angle), Mathf.Sin(angle)) * dist;

            // 核心公式：v = Sqrt(GM / r)
            // 这是卫星绕地球、地球绕太阳的标准公式
            float orbitSpeed = Mathf.Sqrt((G * sunMass) / dist);

            // 速度方向：位置向量旋转 90 度
            Vector2 velDir = new Vector2(-pos.y, pos.x).normalized;

            // 稍微加一点点随机性，让轨道变成椭圆，而不是死板的正圆
            float speedVariation = Random.Range(0.8f, 1.2f);
            Vector2 vel = velDir * orbitSpeed * speedVariation;

            // 行星质量要小，否则会干扰中心恒星
            float planetMass = Random.Range(1f, 10f);
            float planetRadius = Random.Range(1f, 2f);

            // 假设 protoId 1 是行星
            int pid = 1;
            if (protoSet.astroProtoList.Count <= 1) pid = 0;

            CreateVisualAstro(pos, vel, planetMass, planetRadius, pid);
        }

        Debug.Log("[Scenario] 随机太阳系加载完毕。观察行星的公转速度差异。");
    }

    // 辅助方法：创建并强行修改半径
    private void CreateVisualAstro(Vector2 pos, Vector2 vel, float mass, float radius, int protoIndex)
    {
        if (protoSet.astroProtoList.Count == 0) return;
        var proto = protoSet.astroProtoList[Mathf.Clamp(protoIndex, 0, protoSet.astroProtoList.Count - 1)];

        // 调用 Create
        int id = data.CreateAstro(proto, pos, vel, Time.time, mass);

        // 强行覆盖半径，保证视觉效果
        // 注意：如果你用了上一轮的 ref Astro 优化，这里需要获取引用
        data.pool[id].radius = radius;
        data.pool[id].density = mass / (radius * radius);
    }

    /// <summary>
    /// 系统 6：日-地-月 三体系统
    /// </summary>
    public void LoadSunEarthMoon()
    {
        ResetScene();

        float G = config.G;
        float screenH = Camera.main.orthographicSize;

        // --- 1. 参数设定 (游戏化比例) ---

        // 距离设定
        float distSunEarth = screenH * 0.5f;  // 地日距离：占据屏幕大部分
        float distEarthMoon = distSunEarth * 0.15f; // 地月距离：大约是地日距离的 1/6 (为了看清楚，比真实比例大得多)

        // 质量设定 (为了稳定，中心天体质量要呈现指数级差异)
        float sunMass = 11000f;   // 太阳：绝对统治力
        float earthMass = 800f;   // 地球：能拉住月球，但会被太阳拉住
        float moonMass = 1f;     // 月球：小不点

        // --- 2. 创建太阳 (Sun) ---
        // 太阳位于屏幕中心，静止
        CreateVisualAstro(Vector2.zero, Vector2.zero, sunMass, 15f, 4); // 半径15，Proto 0(恒星)

        // --- 3. 创建地球 (Earth) ---
        // 初始位置：X轴正方向
        Vector2 earthPos = new Vector2(distSunEarth, 0);

        // 地球公转速度 (绕太阳)
        // 公式：v = Sqrt(G * M_sun / R)
        float earthSpeed = Mathf.Sqrt((G * sunMass) / distSunEarth);
        Vector2 earthVel = new Vector2(0, earthSpeed); // 垂直于位置，向Y轴运动

        CreateVisualAstro(earthPos, earthVel, earthMass, 2f, 1); // 半径5，Proto 1(行星)

        // --- 4. 创建月球 (Moon) ---
        // 难点：月球的速度是 "地球速度 + 绕地速度" 的矢量和

        // 月球位置：在地球外侧 (X轴更远处)
        Vector2 moonPos = new Vector2(distSunEarth + distEarthMoon, 0);

        // 月球绕地速度
        // 公式：v = Sqrt(G * M_earth / r)
        // 注意：这里 M 是地球质量，r 是地月距离
        float moonOrbitalSpeed = Mathf.Sqrt((G * earthMass) / distEarthMoon);

        // 月球的绝对速度 = 地球公转速度 + 月球绕地速度
        // 因为都在 X 轴，速度都在 Y 轴，直接相加即可
        Vector2 moonVel = new Vector2(0, earthSpeed + moonOrbitalSpeed);

        CreateVisualAstro(moonPos, moonVel, moonMass, 1f, 0); // 半径2，Proto 1(行星/卫星)

        Debug.Log($"[Scenario] 日地月系统加载。E-Speed:{earthSpeed:F2}, M-RelSpeed:{moonOrbitalSpeed:F2}");
    }
}