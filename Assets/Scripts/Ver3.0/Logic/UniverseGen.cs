using UnityEngine;

/// <summary>
/// UniverseGen：
/// </summary>
public class UniverseGen
{
    private readonly AstroData _data;
    private readonly GameConfig _config;
    private readonly AstroProtoSet _protoSet;

    public UniverseGen(AstroData data, GameConfig config, AstroProtoSet protoSet)
    {
        _data = data;
        _config = config;
        _protoSet = protoSet;
    }

    // 双星系统
    public void LoadBinaryStars(float screenHeight)
    {
        ResetScene();

        float G = _config.G;
        float radius = screenHeight * 0.5f; // 轨道半径
        float starMass = 2000f;

        float speed = Mathf.Sqrt((G * starMass) / (4.0f * radius));

        CreateAstro(new Vector2(-radius, 0), new Vector2(0, speed), starMass, 8f, 0);
        CreateAstro(new Vector2(radius, 0), new Vector2(0, -speed), starMass, 8f, 0);

        Debug.Log("[UniverseGen] 双星系统生成完毕");
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
            _data.pool[id].radius = visualRadius;
            _data.pool[id].density = mass / visualRadius;
        }
    }
}
