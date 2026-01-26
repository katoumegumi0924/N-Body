using UnityEngine;

/// <summary>
/// UniverseGen：
/// </summary>
public class UniverseGen
{
    private GameData data;
    private readonly float G = GameConfig.universeConfig.G;

    public void Init(GameData _data)
    {
        data = _data;
    }

    public void Free()
    {
        data = null;
    }

    public void SetNew()
    {
        float orbitRadius = Mathf.Min(GameConfig.universeConfig.height, GameConfig.universeConfig.width);
        LoadSunEarthMoon(orbitRadius);
    }

    // 双星系统
    public void LoadBinaryStars(float _orbitRadius)
    {
        ResetScene();

        float orbitRadius = _orbitRadius * 0.3f; // 轨道半径
        float starMass = 200f;

        float astroRadius = orbitRadius * 0.1f;

        float speed = Mathf.Sqrt((G * starMass) / (4.0f * orbitRadius));

        CreateAstro(new Vector2(-orbitRadius, 0), new Vector2(0, speed), starMass, astroRadius, 0);
        CreateAstro(new Vector2(orbitRadius, 0), new Vector2(0, -speed), starMass, astroRadius, 0);

        Debug.Log("[UniverseGen] 双星系统生成完毕");
    }

    // 恒星系统
    public void LoadStarSystem(float _orbitRadius)
    {
        ResetScene();

        float sunMass = 5000f;

        float sunRadius = _orbitRadius * 0.05f;

        // 创建恒星
        CreateAstro(Vector2.zero, Vector2.zero, sunMass, sunRadius, 2);

        // 创建行星
        int planetCount = 50;
        for (int i = 0; i < planetCount; i++)
        {
            float dist = Random.Range(_orbitRadius * 0.2f, _orbitRadius * 1.5f);
            float angle = Random.Range(0f, Mathf.PI * 2);
            Vector2 pos = new Vector2(Mathf.Cos(angle), Mathf.Sin(angle)) * dist;

            float planetSpeed = Mathf.Sqrt((G * sunMass) / dist);
            Vector2 velDir = new Vector2(-pos.y, pos.x).normalized;
            Vector2 vel = velDir * planetSpeed * Random.Range(0.8f, 1.2f);

            CreateAstro(pos, vel, Random.Range(1f, 10f), Random.Range(sunRadius * 0.1f, sunRadius * 0.2f), 1);
        }
        Debug.Log("[UniverseGen] 恒星系统生成完毕");
    }

    // 三体8字形轨道
    public void LoadThreeBodyFigure8(float _orbitRadius)
    {
        ResetScene();

        float orbitRadius = _orbitRadius * 0.4f;
        float astorSpeed = 50.0f;

        Vector2 p1_base = new Vector2(0.97000436f, -0.24308753f);
        Vector2 v1_base = new Vector2(0.46620368f, 0.43236573f);
        Vector2 v2_base = new Vector2(0.93240737f, 0.86473146f);

        Vector2[] posBase = new Vector2[] { p1_base, -p1_base, Vector2.zero };
        Vector2[] velBase = new Vector2[] { v1_base, v1_base, -2f * v1_base };

        float requiredMass = (astorSpeed * astorSpeed * orbitRadius) / G;

        float astroRadius = orbitRadius / 20.0f;

        for (int i = 0; i < 3; i++)
        {
            Vector2 finalPos = posBase[i] * orbitRadius;
            Vector2 finalVel = velBase[i] * astorSpeed;

            CreateAstro(finalPos, finalVel, requiredMass, astroRadius, 4);
        }

        Debug.Log("[UniverseGen] 三体8字形轨道生成完毕");
    }

    // 日地月系统
    public void LoadSunEarthMoon(float _orbitRadius)
    {
        ResetScene();

        float AU = _orbitRadius * 0.3f;
        float EM_Dist = AU * 0.08f;

        float sunRadius = 12f;
        float earthRadius = 4f;
        float moonRadius = 2f;

        float sunMass = 480f;
        float earthMass = 32f;
        float moonMass = 0.2f;

        float earthSpeed = Mathf.Sqrt((G * sunMass) / AU);
        float moonSpeed = Mathf.Sqrt((G * earthMass) / EM_Dist);
        float sunSpeed = -(earthMass * earthSpeed + moonMass * moonSpeed) / sunMass;

        // 太阳
        Vector2 sunVelocity = new Vector2(0, sunSpeed);
        CreateAstro(Vector2.zero, sunVelocity, sunMass, sunRadius, 2);

        // 地球
        Vector2 earthPosition = new Vector2(AU, 0);
        Vector2 earthVelocity = new Vector2(0, earthSpeed);
        CreateAstro(earthPosition, earthVelocity, earthMass, earthRadius, 1);

        // 月亮
        Vector2 moonPosition = earthPosition + new Vector2(EM_Dist, 0);
        Vector2 moonVelocity = earthVelocity + new Vector2(0, moonSpeed);
        CreateAstro(moonPosition, moonVelocity, moonMass, moonRadius, 0);

        Debug.Log("[UniverseGen] 日地月系统生成完毕");
    }

    // 层级三体系统
    public void LoadHierarchicalTripleSystem(float _orbitRadius)
    {
        ResetScene();

        // 轨道半径
        float orbitInner = _orbitRadius * 0.1f;
        float orbitOuter = _orbitRadius * 0.8f;

        float astroRadius = orbitInner * 0.2f;

        // 质量
        float massA = 1000f;
        float massB = 1000f;
        float massC = 1000f;
        float massInner = massA + massB;
        float massTotal = massInner + massC;

        // 速度
        float innerRelativeSpeed = Mathf.Sqrt(G * massInner / orbitInner);
        float speedA = innerRelativeSpeed * (massB / massInner);
        float speedB = -innerRelativeSpeed * (massA / massInner);

        float outerRelativeSpeed = Mathf.Sqrt(G * massTotal / orbitOuter);
        float speedC = outerRelativeSpeed * (massInner / massTotal);
        float speedInner = -outerRelativeSpeed * (massC / massTotal);

        // 天体A
        Vector2 posA = new Vector2(orbitInner * (massB / massInner), 0);
        Vector2 velA = new Vector2(speedInner, speedA);
        CreateAstro(posA, velA, massA, astroRadius, 4);

        // 天体B
        Vector2 posB = new Vector2(-orbitInner * (massA / massInner), 0);
        Vector2 velB = new Vector2(speedInner, speedB);
        CreateAstro(posB, velB, massB, astroRadius, 4);

        // 天体C
        Vector2 posC = new Vector2(0, orbitOuter);
        Vector2 velC = new Vector2(speedC, 0);
        CreateAstro(posC, velC, massC, astroRadius, 4);

        Debug.Log("[UniverseGen] 层级三体系统生成完毕");
    }

    // 拉格朗日L4,L5点
    public void LoadLagrangePoints(float _orbitRadius)
    {
        ResetScene();

        float orbitRadius = _orbitRadius * 0.4f;

        float radiusA = orbitRadius * 0.1f;
        float radiusB = orbitRadius * 0.05f;


        float massA = 1000f;
        float massB = 30f;
        float massTotal = massA + massB;

        // 角速度
        float omega = Mathf.Sqrt((G * massTotal) / (orbitRadius * orbitRadius * orbitRadius));

        float r1 = orbitRadius * (massB / massTotal);
        float r2 = orbitRadius * (massA / massTotal);

        Vector2 posA = new Vector2(-r1, 0);
        Vector2 velA = new Vector2(0, -r1 * omega);
        CreateAstro(posA, velA, massA, radiusA, 3);

        Vector2 posB = new Vector2(r2, 0);
        Vector2 velB = new Vector2(0, r2 * omega);
        CreateAstro(posB, velB, massB, radiusB, 1);

        // L4位置
        float l4X = (r2 - r1) * 0.5f;
        float l4Y = orbitRadius * Mathf.Sqrt(3.0f) * 0.5f;
        Vector2 l4Center = new Vector2(l4X, l4Y);
        int asteroidCount = 30;
        for (int i = 0; i < asteroidCount; ++i)
        {
            Vector2 offset = new Vector2(Random.Range(-0.01f, 0.01f) * orbitRadius, Random.Range(-0.01f, 0.01f) * orbitRadius);
            Vector2 pos = l4Center + offset;
            Vector2 vel = new Vector2(-omega * pos.y, omega * pos.x);
            CreateAstro(pos, vel, 0.1f, 2f, 0);
        }

        // L5位置
        float l5X = (r2 - r1) * 0.5f;
        float l5Y = -orbitRadius * Mathf.Sqrt(3.0f) * 0.5f;
        Vector2 l5Center = new Vector2(l5X, l5Y);
        asteroidCount = 30;
        for (int i = 0; i < asteroidCount; ++i)
        {
            Vector2 offset = new Vector2(Random.Range(-0.01f, 0.01f) * orbitRadius, Random.Range(-0.01f, 0.01f) * orbitRadius);
            Vector2 pos = l5Center + offset;
            Vector2 vel = new Vector2(-omega * pos.y, omega * pos.x);
            CreateAstro(pos, vel, 0.1f, 2f, 0);
        }

        Debug.Log("[UniverseGen] 拉格朗日L4,L5点生成完毕");
    }

    // 克伦佩勒环
    public void LoadKlempererRosette(float _orbitRadius)
    {
        ResetScene();

        float orbitRadius = _orbitRadius * 0.3f;
        int astroCount = 6;
        float astroMass = 500f;
        float astroRadius = 0.05f * orbitRadius;

        // 几何力因子，所有其他天体对该天体的引力在径向上的和的系数
        float sigma = 0;
        for (int i = 1; i < astroCount - 1; ++i)
        {
            float angle = (i * Mathf.PI) / astroCount;
            sigma += 1.0f / Mathf.Sin(angle);  
        }

        float orbitSpeed = Mathf.Sqrt((G * astroMass * sigma) / (4.0f * orbitRadius));

        for (int i = 0; i < astroCount; ++i)
        {
            float theta = (i * 2.0f * Mathf.PI) / astroCount;
            Vector2 pos = new Vector2(orbitRadius * Mathf.Cos(theta), orbitRadius * Mathf.Sin(theta));
            Vector2 velDir = new Vector2(-Mathf.Sin(theta), Mathf.Cos(theta));
            Vector2 vel = velDir * orbitSpeed;
            CreateAstro(pos, vel, astroMass, astroRadius, 2);
        }

        Debug.Log("[UniverseGen] 克伦佩勒环生成完毕");
    }

    // 克伦佩勒环 包含中心天体
    public void LoadKlempererRosetteCore(float _orbitRadius)
    {
        ResetScene();

        float orbitRadius = _orbitRadius * 0.3f;
        int astroCount = 6;
        float astroMass = 200f;
        float coreMass = 1000f;
        float astroRadius = 0.05f * orbitRadius;
        float coreRadius = 0.1f * orbitRadius;

        CreateAstro(Vector2.zero, Vector2.zero, coreMass, coreRadius, 4);

        // 几何力因子，所有其他环上天体对该天体的引力在径向上的和的系数
        float sigma = 0;
        for (int i = 1; i < astroCount - 1; ++i)
        {
            float angle = (i * Mathf.PI) / astroCount;
            sigma += 1.0f / Mathf.Sin(angle);
        }

        // 计算切线方向速度
        float term1 = coreMass / orbitRadius;
        float term2 = (astroMass * sigma) / (4.0f * orbitRadius);
        float orbitSpeed = Mathf.Sqrt(G * (term1 + term2));

        for (int i = 0; i < astroCount; ++i)
        {
            float theta = (i * 2.0f * Mathf.PI) / astroCount;
            Vector2 pos = new Vector2(orbitRadius * Mathf.Cos(theta), orbitRadius * Mathf.Sin(theta));
            Vector2 velDir = new Vector2(-Mathf.Sin(theta), Mathf.Cos(theta));
            Vector2 vel = velDir * orbitSpeed;
            CreateAstro(pos, vel, astroMass, astroRadius, 2);
        }

        Debug.Log("[UniverseGen] 克伦佩勒环（包含中心天体）生成完毕");
    }

    // 稳定等边三角形
    public void LoadStableLagrangeTriangle(float _orbitRadius)
    {
        ResetScene();

        float orbitRadius = _orbitRadius * 0.4f;

        float m1 = 1000f;
        float m2 = 10f;
        float m3 = 10f;
        float mTotal = m1 + m2 + m3;

        float omega = Mathf.Sqrt(G * mTotal / (orbitRadius * orbitRadius * orbitRadius));

        // 等边三角形
        Vector2 p1 = new Vector2(-orbitRadius * Mathf.Sqrt(3) / 3.0f, 0);
        Vector2 p2 = new Vector2(orbitRadius * Mathf.Sqrt(3) / 6.0f, orbitRadius / 2.0f);
        Vector2 p3 = new Vector2(orbitRadius * Mathf.Sqrt(3) / 6.0f, -orbitRadius / 2.0f);

        Vector2 com = (p1 * m1 + p2 * m2 + p3 * m3) / mTotal;
        p1 -= com;
        p2 -= com;
        p3 -= com;

        Vector2 vel1 = new Vector2(-omega * p1.y, omega * p1.x);
        Vector2 vel2 = new Vector2(-omega * p2.y, omega * p2.x);
        Vector2 vel3 = new Vector2(-omega * p3.y, omega * p3.x);

        CreateAstro(p1, vel1, m1, 10f, 4);
        CreateAstro(p2, vel2, m2, 8f, 4);
        CreateAstro(p3, vel3, m3, 8f, 4);

        Debug.Log("[UniverseGen] 三体稳定等边三角形生成完毕");
    }

    public void ResetScene()
    {
        data.universeData.pool.Reset();
    }

    private void CreateAstro(Vector2 pos, Vector2 vel, float mass, float visualRadius, int protoIndex)
    {
        if (ProtoDB.protoSet.Count == 0)
            return;

        if (protoIndex < 0 || protoIndex >= ProtoDB.protoSet.Count)
            return;

        int id = data.universeData.CreateAstro(protoIndex, pos, vel, data.universeTimeData.tickCounter, mass);

        // 这里强行更改一下半径和密度，演示中可能需要大密度的天体
        if (id > 0)
        {
            data.universeData.pool[id].density = mass / (visualRadius * visualRadius);
            data.universeData.pool[id].radius = visualRadius;
        }
    }
}