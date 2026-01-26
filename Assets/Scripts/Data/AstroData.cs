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

    // 初始化函数
    public void Init(int id, int protoIndex, Vector2 pos, Vector2 vel, long currentTick, float massOverride = -1f)
    {
        AstroProto proto = ProtoDB.protoSet[protoIndex];

        this.id = id;
        this.protoId = proto.id;
        this.type = proto.type;

        this.position = pos;
        this.velocity = vel;
        this.force = new Vector2(0, 0);
        this.mass = massOverride > 0 ? massOverride : proto.GetRandomMass(); // 未指定质量时，获取一个原型范围内的随机质量
        this.radius = proto.GetRadius(this.mass);
        this.density = proto.density;
        this.elasticityCoef = proto.elasticityCeof;
        this.massInv = this.mass > 1e-5f ? 1.0f / mass : 0f;
        this.birthTick = currentTick;
    }

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