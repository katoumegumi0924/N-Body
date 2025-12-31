using Unity.VisualScripting.FullSerializer;
using UnityEngine;

/// <summary>
/// UniverseGen：
/// </summary>
public class UniverseGen
{
    private GameData data;

    public void Init(GameData _data)
    {
        data = _data;
    }

    public void Free()
    {
        data = null;
    }

    // 双星系统
    public void LoadBinaryStars(float screenHeight)
    {
        ResetScene();

        float radius = screenHeight * 0.5f; // 轨道半径
        float starMass = 2000f;

        float speed = Mathf.Sqrt((GameConfig.universeConfig.G * starMass) / (4.0f * radius));

        CreateAstro(new DVector2(-radius, 0), new DVector2(0, speed), starMass, 8f, 0);
        CreateAstro(new DVector2(radius, 0), new DVector2(0, -speed), starMass, 8f, 0);

        Debug.Log("[UniverseGen] 双星系统生成完毕");
    }

    // 恒星系统
    public void LoadStarSystem(float screenHeight)
    {
        ResetScene();

        float sunMass = 5000f;

        // 创建恒星
        CreateAstro(DVector2.zero, DVector2.zero, sunMass, 10f, 2);

        // 创建行星
        int planetCount = 50;
        for (int i = 0; i < planetCount; i++)
        {
            float dist = Random.Range(screenHeight * 0.2f, screenHeight * 1.5f);
            float angle = Random.Range(0f, Mathf.PI * 2);
            DVector2 pos = new DVector2(Mathf.Cos(angle), Mathf.Sin(angle)) * dist;

            float orbitSpeed = Mathf.Sqrt((GameConfig.universeConfig.G * sunMass) / dist);
            DVector2 velDir = new DVector2(-pos.y, pos.x).normalized;
            DVector2 vel = velDir * orbitSpeed * Random.Range(0.8f, 1.2f);

            CreateAstro(pos, vel, Random.Range(1f, 10f), Random.Range(1f, 2f), 1);
        }
        Debug.Log("[UniverseGen] 恒星系统生成完毕");
    }

    // 三体8字形轨道
    public void LoadThreeBodyFigure8(float h)
    {
        ResetScene();

        float scaleSize = h * 0.6f;
        float scaleSpeed = 20.0f;

        DVector2 p1_base = new DVector2(0.97000436f, -0.24308753f);
        DVector2 v1_base = new DVector2(0.46620368f, 0.43236573f);
        DVector2 v2_base = new DVector2(0.93240737f, 0.86473146f);

        DVector2[] posBase = new DVector2[] { p1_base, -p1_base, DVector2.zero };
        DVector2[] velBase = new DVector2[] { v1_base, v1_base, -2f * v1_base };

        float gameG = GameConfig.universeConfig.G;

        float requiredMass = (scaleSpeed * scaleSpeed * scaleSize) / gameG;

        var proto = ProtoDB.ProtoSet[4];

        float visualRadius = scaleSize / 15.0f;

        for (int i = 0; i < 3; i++)
        {
            DVector2 finalPos = posBase[i] * scaleSize;
            DVector2 finalVel = velBase[i] * scaleSpeed;

            CreateAstro(finalPos, finalVel, requiredMass, visualRadius, 4);
        }

        Debug.Log("[UniverseGen] 三体8字形轨道生成完毕");
    }

    // 日地月系统
    public void LoadSunEarthMoon(float h)
    {
        ResetScene();

        float AU = h * 0.7f;
        float EM_Dist = AU * 0.2f;

        float sunRadius = 12f;
        float earthRadius = 4f;
        float moonRadius = 1.5f;

        float sunMass = 12000f;
        float earthMass = 800f;
        float moonMass = 5f;

        // 太阳
        CreateAstro(DVector2.zero, DVector2.zero, sunMass, sunRadius, 2);

        // 地球
        float vEarth = Mathf.Sqrt((GameConfig.universeConfig.G * sunMass) / AU);
        DVector2 earthPosition = new DVector2(AU, 0);
        DVector2 earthVelocity = new DVector2(0, vEarth);
        CreateAstro(earthPosition, earthVelocity, earthMass, earthRadius, 1);

        // 月亮
        float vMoon = Mathf.Sqrt((GameConfig.universeConfig.G * earthMass) / EM_Dist);
        DVector2 moonPosition = earthPosition + new DVector2(EM_Dist, 0);
        DVector2 moonVelocity = earthVelocity + new DVector2(0, vMoon);
        CreateAstro(moonPosition, moonVelocity, moonMass, moonRadius, 0);
    }

    public void ResetScene()
    {
        data.universeData.ClearAll();
    }

    private void CreateAstro(DVector2 pos, DVector2 vel, float mass, float visualRadius, int protoIndex)
    {
        if ( ProtoDB.ProtoSet.Count == 0)
            return;

        if (protoIndex < 0 || protoIndex >= ProtoDB.ProtoSet.Count)
            return;

        int id = data.universeData.CreateAstro(protoIndex, pos, vel, data.clock.totalTicks, mass);

        // 这里强行更改一下半径和密度，演示中可能需要大密度的天体
        if (id > 0)
        {
            data.universeData.pool[id].density = mass / (visualRadius * visualRadius);
            data.universeData.pool[id].radius = visualRadius;
        }
    }
}
