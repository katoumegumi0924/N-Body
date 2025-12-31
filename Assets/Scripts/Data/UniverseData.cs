using System;
using UnityEngine;

/// <summary>
/// UniverseData：
/// </summary>
public class UniverseData
{
    public DataPool<AstroData> pool;

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
    public int CreateAstro(int protoIndex, DVector2 pos, DVector2 vel, long currentTick,float massOverride = -1)
    {
        ref var astro = ref pool.Add(out int id);
        astro.Init(id, protoIndex, pos, vel,currentTick, massOverride);
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