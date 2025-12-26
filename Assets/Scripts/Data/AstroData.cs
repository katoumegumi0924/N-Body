using System;
using UnityEngine;

/// <summary>
/// AstroData：
/// </summary>
public class AstroData
{
    public Astro[] pool;

    public int cursor;      // 游标位置
    public int capacity;    // 当前容量，可扩容
    public int count;       // 当前存活的天体数量

    // 回收站
    private int[] recycleIds;
    private int recycleCursor;

    // 初始容量
    private const int INITIAL_CAP = 64;

    public void Init()
    {
        if (pool != null)
        {
            for (int i = 0; i < cursor; ++i)
            {
                pool[i].Clear();
            }
        }

        capacity = INITIAL_CAP;
        pool = new Astro[INITIAL_CAP];
        recycleIds = new int[INITIAL_CAP];
        cursor = 1;
        count = 0;
        recycleCursor = 0;
    }

    public void Free()
    {
        pool = null;
        recycleIds = null;
        cursor = 1;
        capacity = 0;
        recycleCursor = 0;
        count = 0;
    }

    // 创建天体
    public int CreateAstro(AstroProto proto, Vector2 pos, Vector2 vel, float currentTime,float massOverride = -1)
    {
        int id;

        // 优先使用回收站的Id
        if (recycleCursor > 0)
        {
            id = recycleIds[--recycleCursor];
        }
        else
        {
            if (cursor >= capacity)
            {
                // 动态扩容
                Expand();
            }
            id = cursor++;
        }

        // 数据初始化
        ref var astro = ref pool[id];
        astro.Init(id, proto, pos, vel, currentTime, massOverride);

        count++;
        return id;
    }

    // 销毁天体
    public void FreeAstro(int id)
    {
        if (id <= 0 || id >= capacity)
            return;

        ref var astro = ref pool[id];
        if (!astro.active)
            return;

        // 清空内存
        astro.Clear();

        // 回收id
        recycleIds[recycleCursor++] = id;
        count--;
    }

    // 重置状态 销毁所有天体
    public void ClearAll()
    {
        for (int i = 1; i < cursor; ++i)
        {
            if (pool[i].active)
                pool[i].Clear();
        }

        cursor = 1;
        count = 0;
        recycleCursor = 0;
    }

    private void Expand()
    {
        int newCap = capacity * 2;
        Astro[] newPool = new Astro[newCap];
        Array.Copy(pool, newPool, capacity);
        pool = newPool;

        int[] newRecycle = new int[newCap];
        Array.Copy(recycleIds, newRecycle, recycleCursor);
        recycleIds = newRecycle;

        capacity = newCap;
    }

}

public struct Astro
{
    // 身份标识
    public int id;
    public bool active; // 天体存活标记
    public int protoId; // 对应原型id
    public AstroType type;

    // 物理状态
    public Vector2 position;
    public Vector2 velocity;
    public Vector2 force; // 每帧受到的力
    public float mass;
    public float radius;
    public float density;
    public float elasticityModulus; // 弹性系数
    public float massInv; // 缓存 1/mass
    public float birthTime; // 天体创建时间，用于计算天体存活时间决定天体颜色

    // 初始化函数
    public void Init(int id, AstroProto proto, Vector2 pos, Vector2 vel, float currentTime, float massOverride = -1f)
    {
        this.id = id;
        this.active = true;
        this.protoId = proto.id;
        this.type = proto.type;

        this.position = pos;
        this.velocity = vel;
        this.mass = massOverride > 0 ? massOverride : proto.GetRandomMass(); // 未指定质量时，获取一个原型范围内的随机质量
        this.radius = proto.GetRadius(this.mass);
        this.density = proto.density;
        this.elasticityModulus = proto.elasticityModulus;
        this.massInv = this.mass > 0.01f ? 1.0f / mass : 0f;
        this.birthTime = currentTime;
    }
    
    // 清空函数
    public void Clear()
    {
        // 所有参数设为默认值
        this = default;
    }
}