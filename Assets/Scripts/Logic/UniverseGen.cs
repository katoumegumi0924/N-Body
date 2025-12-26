using UnityEngine;

/// <summary>
/// UniverseGen：
/// </summary>
public class UniverseGen
{
    private AstroData _data;
    private AstroProtoSet _protoSet;

    public void Init(AstroData data, AstroProtoSet protoSet)
    {
        _data = data;
        _protoSet = protoSet;
    }

    public void Free()
    {
        _data = null;
        _protoSet = null;
    }

    // 双星系统
    public void LoadBinaryStars(float screenHeight)
    {
        ResetScene();

        float radius = screenHeight * 0.5f; // 轨道半径
        float starMass = 2000f;

        float speed = Mathf.Sqrt((GameConfig.universeConfig.G * starMass) / (4.0f * radius));

        CreateAstro(new Vector2(-radius, 0), new Vector2(0, speed), starMass, 8f, 0);
        CreateAstro(new Vector2(radius, 0), new Vector2(0, -speed), starMass, 8f, 0);

        Debug.Log("[UniverseGen] 双星系统生成完毕");
    }

    // 恒星系统
    public void LoadStarSystem(float screenHeight)
    {
        ResetScene();

        float sunMass = 5000f;

        // 创建恒星
        CreateAstro(Vector2.zero, Vector2.zero, sunMass, 10f, 2);

        // 创建行星
        int planetCount = 50;
        for (int i = 0; i < planetCount; i++)
        {
            float dist = Random.Range(screenHeight * 0.2f, screenHeight * 1.5f);
            float angle = Random.Range(0f, Mathf.PI * 2);
            Vector2 pos = new Vector2(Mathf.Cos(angle), Mathf.Sin(angle)) * dist;

            float orbitSpeed = Mathf.Sqrt((GameConfig.universeConfig.G * sunMass) / dist);
            Vector2 velDir = new Vector2(-pos.y, pos.x).normalized;

            // 稍微随机化轨道偏心率
            Vector2 vel = velDir * orbitSpeed * Random.Range(0.8f, 1.2f);

            CreateAstro(pos, vel, Random.Range(1f, 10f), Random.Range(1f, 2f), 1);
        }
        Debug.Log("[UniverseGen] 恒星系统生成完毕");
    }

    public void ResetScene()
    {
        _data.ClearAll();
    }

    private void CreateAstro(Vector2 pos, Vector2 vel, float mass, float visualRadius, int protoIndex)
    {
        if (_protoSet.astroProtoList.Count == 0)
            return;

        if (protoIndex < 0 || protoIndex > _protoSet.astroProtoList.Count)
            return;

        var proto = _protoSet.astroProtoList[protoIndex];

        int id = _data.CreateAstro(proto, pos, vel, Time.time, mass);

        // 这里强行更改一下半径和密度，演示中可能需要大密度的天体
        if (id > 0)
        {
            _data.pool[id].density = mass / visualRadius;
            _data.pool[id].radius = visualRadius;
        }
    }
}
