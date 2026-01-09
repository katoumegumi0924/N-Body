using System;
using UnityEngine;

/// <summary>
/// UniverseData：
/// </summary>
public class UniverseData
{
    public DataPool<AstroData> pool;
    public WorldBounds worldBounds;
    
    // 天体最大半径
    public float MAX_RADIUS { get { return 0.4f * Math.Min(worldBounds.height, worldBounds.width); } }

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

    // 创建天体
    public int CreateAstro(int protoIndex, Vector2 pos, Vector2 vel, long currentTick, float massOverride = -1)
    {
        ref var astro = ref pool.Add(out int id);
        astro.Init(id, protoIndex, pos, vel, currentTick, worldBounds, massOverride);
        return id;
    }

    // 销毁天体
    public void FreeAstro(int id)
    {
        pool.Remove(id);
    }

    // 重置状态 销毁所有天体
    public void ClearAll()
    {
        pool.ClearAll();
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
    public Vector2 centerPos { get { return new Vector2((maxX + minX) * 0.5f, (maxX + minX) * 0.5f); } }

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