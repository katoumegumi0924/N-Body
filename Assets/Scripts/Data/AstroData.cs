using UnityEngine;

/// <summary>
/// AstroData：
/// </summary>
public struct AstroData : IPoolElement
{
    // 身份标识
    public int id;
    public bool active; // 天体存活标记
    public int protoId; // 对应原型id
    public AstroType type;

    // 物理状态
    // 位置，速度和力采用 double，避免积分时精度丢失;
    public DVector2 position;
    public DVector2 velocity;
    public DVector2 force;
    public float mass;
    public float radius;
    public float density;
    public float elasticityModulus; // 弹性系数
    public float massInv; // 缓存 1/mass
    public long birthTick; // 天体创建时间，用于计算天体存活时间决定天体颜色

    public int ID { get { return id; } set { id = value; } }

    // 初始化函数
    public void Init(int id, int protoIndex, DVector2 pos, DVector2 vel, long currentTick, float massOverride = -1f)
    {
        this.id = id;
        this.active = true;
        this.protoId = ProtoDB.ProtoSet[protoIndex].id;
        this.type = ProtoDB.ProtoSet[protoIndex].type;

        this.position = pos;
        this.velocity = vel;
        this.force = new DVector2(0, 0);
        this.mass = massOverride > 0 ? massOverride : ProtoDB.ProtoSet[protoIndex].GetRandomMass(); // 未指定质量时，获取一个原型范围内的随机质量
        this.radius = ProtoDB.ProtoSet[protoIndex].GetRadius(this.mass);
        this.density = ProtoDB.ProtoSet[protoIndex].density;
        this.elasticityModulus = ProtoDB.ProtoSet[protoIndex].elasticityModulus;
        this.massInv = this.mass > 0.01f ? 1.0f / mass : 0f;
        this.birthTick = currentTick;
    }

    // 清空函数
    public void Reset()
    {
        // 所有参数设为默认值
        this = default;
    }

    public void InternelUpdate(float deltaTime)
    {
        if (!active)
            return;
        position += velocity * deltaTime;
    }
}

public struct DVector2
{
    public double x;
    public double y;

    public DVector2(double x, double y)
    {
        this.x = x;
        this.y = y;
    }

    // 常用常量
    public static readonly DVector2 zero = new DVector2(0, 0);
    public static readonly DVector2 one = new DVector2(1, 1);
    public static readonly DVector2 up = new DVector2(0, 1);
    public static readonly DVector2 down = new DVector2(0, -1);
    public static readonly DVector2 left = new DVector2(-1, 0);
    public static readonly DVector2 right = new DVector2(1, 0);

    // 运算符重载
    public static DVector2 operator +(DVector2 a, DVector2 b) => new DVector2(a.x + b.x, a.y + b.y);
    public static DVector2 operator -(DVector2 a, DVector2 b) => new DVector2(a.x - b.x, a.y - b.y);
    public static DVector2 operator *(DVector2 a, double d) => new DVector2(a.x * d, a.y * d);
    public static DVector2 operator *(double d, DVector2 a) => new DVector2(a.x * d, a.y * d);
    public static DVector2 operator /(DVector2 a, double d) => new DVector2(a.x / d, a.y / d);
    public static DVector2 operator -(DVector2 a) => new DVector2(-a.x, -a.y);

    // 计算向量长度平方
    public double SqrMagnitude => x * x + y * y;
    // 计算向量长度
    public double Magnitude => System.Math.Sqrt(x * x + y * y);

    // 强制类型转换
    public static explicit operator Vector3(DVector2 v) => new Vector3((float)v.x, (float)v.y, 0);
    public static explicit operator Vector2(DVector2 v) => new Vector3((float)v.x, (float)v.y);

    // 点乘
    public static double Dot(DVector2 a, DVector2 b) => a.x * b.x + a.y * b.y;

    // 返回归一化后的单位向量
    public DVector2 normalized
    {
        get
        {
            double mag = Magnitude;
            if (mag > 0.001d)
                return this / mag;
            return zero;
        }
    }
}