using UnityEngine;
using static UnityEditor.ShaderData;

/// <summary>
/// UniverseData：
/// </summary>
public class UniverseData
{
    public DataPool<AstroData> pool;
    public WorldBounds worldBounds;

    // 天体最大半径
    public float MAX_RADIUS { get { return 0.4f * Mathf.Min(worldBounds.height, worldBounds.width); } }

    public void Init()
    {
        pool = new DataPool<AstroData>();
        pool.Reset();
    }

    public void Free()
    {
        if (pool != null)
        {
            pool.Free();
            pool = null;
        }
    }

    public void SetNew()
    {
        worldBounds.SetBounds(GameConfig.universeConfig.width,
                              GameConfig.universeConfig.height,
                              GameConfig.universeConfig.centerPos);
    }

    // 创建天体
    public int CreateAstro(int protoIndex, Vector2 pos, Vector2 vel, long currentTick, float massOverride = -1)
    {
        ref var astro = ref pool.Add(out int id);

        AstroProto proto = ProtoDB.protoSet[protoIndex];
        if (proto != null)
        {
            astro.id = id;
            astro.protoId = proto.id;
            astro.type = proto.type;

            astro.position = pos;
            astro.velocity = vel;
            astro.force = Vector2.zero;
            astro.mass = massOverride > 0 ? massOverride : proto.GetRandomMass(); // 未指定质量时，获取一个原型范围内的随机质量
            astro.radius = proto.GetRadius(astro.mass);
            astro.density = proto.density;
            astro.elasticityCoef = proto.elasticityCeof;
            astro.massInv = astro.mass > 1e-5f ? 1.0f / astro.mass : 0f;
            astro.birthTick = currentTick;
        }
        
        EnsureInWorld(ref astro);
        return id;
    }

    // 销毁天体
    public void DestroyAstro(int id)
    {
        pool.Remove(id);
    }

    private void EnsureInWorld(ref AstroData astro)
    {
        float r = astro.radius;

        float minSafeX = worldBounds.minX + r;
        float maxSafeX = worldBounds.maxX - r;
        float minSafeY = worldBounds.minY + r;
        float maxSafeY = worldBounds.maxY - r;

        astro.position.x = Mathf.Clamp(astro.position.x, minSafeX, maxSafeX);
        astro.position.y = Mathf.Clamp(astro.position.y, minSafeY, maxSafeY);
    }
}

public struct WorldBounds
{
    public float minX;
    public float maxX;
    public float minY;
    public float maxY;

    public float width { get { return maxX - minX; } }
    public float height { get { return maxY - minY; } }
    public Vector2 centerPos { get { return new Vector2((maxX + minX) * 0.5f, (maxY + minY) * 0.5f); } }

    public void SetBounds(float minX, float maxX, float minY, float maxY)
    {
        this.minX = minX;
        this.maxX = maxX;
        this.minY = minY;
        this.maxY = maxY;
    }

    public void SetBounds(float width, float height, Vector2 centerPos)
    {
        minX = centerPos.x - width * 0.5f;
        maxX = centerPos.x + width * 0.5f;
        minY = centerPos.y - height * 0.5f;
        maxY = centerPos.y + height * 0.5f;
    }
}