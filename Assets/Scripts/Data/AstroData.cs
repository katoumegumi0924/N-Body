using UnityEngine;

/// <summary>
/// AstroData：
/// </summary>
public struct AstroData : IPoolElement
{
    // 身份标识
    public int id;
    public int protoId; // 对应原型id
    public AstroType type;

    // 物理状态
    public Vector2 position;
    public Vector2 velocity;
    public Vector2 force;
    public float mass;
    public float radius;
    public float density;
    public float elasticityCoef; // 弹性系数
    public float massInv; // 缓存 1/mass
    public long birthTick; // 天体创建时间，用于计算天体存活时间决定天体颜色

    public int ID { get { return id; } set { id = value; } }

    // 清空函数
    public void Reset()
    {
        // 所有参数设为默认值
        this = default;
    }

    public void InternelUpdate(float dt)
    {
        position += velocity * dt;
    }
}