using Unity.Mathematics;
using UnityEngine;

/// <summary>
/// UniverseSystem：
/// </summary>
public class UniverseSystem : MonoBehaviour
{
    [Header("Setting")]
    public int maxStarCount = 4096;
    public float G = 10.0f;              // 引力常数

    public bool enableCollision = true;
    public float timeScale = 1.0f;

    [Header("Rendering")]
    public Mesh bodyMesh;
    public Material bodyMaterial;

    // 天体数据
    public UniverseData universeData;

    // 渲染缓冲
    private Matrix4x4[] _drawMatrices;
    private Vector4[] _drawColors;
    private MaterialPropertyBlock _mpb; // 用于临时改变天体材质球颜色

    // 交互逻辑变量
    private bool _isDragging;
    private Vector2 _dragStartPos;
    private Vector2 _dragEndPos;

    private void Awake()
    {
        universeData = new UniverseData(maxStarCount);

        _drawMatrices = new Matrix4x4[1023];
        _drawColors = new Vector4[1023];
        _mpb = new MaterialPropertyBlock();

        bodyMaterial.enableInstancing = true;

        // 测试数据
        universeData.CreateCelestialBody(Vector2.zero, new Vector2(100, 0), 5f, Color.red);
        universeData.CreateCelestialBody(new Vector2(10,0), new Vector2(-100,0), 5f, Color.blue);
    }

    private void Update()
    {
        // 输入处理
        HandleInput();

        // 物理逻辑处理
        float dt = Time.deltaTime * timeScale;
        if (dt > 0)
        {
            PhysicsTick(dt);
        }

        // 处理渲染
        RenderTick();
    }

    private void HandleInput()
    {
        if (Input.GetMouseButtonDown(0))
        {
            _isDragging = true;
            _dragStartPos = GetMouseWorldPos();
        }

        if (Input.GetMouseButtonUp(0) && _isDragging)
        {
            _isDragging = false;
            _dragEndPos = GetMouseWorldPos();

            Vector2 dragVector = _dragStartPos - _dragEndPos;
            Vector2 velocity = dragVector * 1f;         // 拉力系数

            // 随机质量和颜色
            float mass = UnityEngine.Random.Range(1f, 50f);
            Color c = Color.HSVToRGB(UnityEngine.Random.value, 0.7f, 1.0f);

            // 生成天体
            universeData.CreateCelestialBody(_dragEndPos, velocity, mass, c);
        }
    }

    private Vector2 GetMouseWorldPos()
    {
        Vector3 mousepoint = Input.mousePosition;
        mousepoint.z = 10;
        return Camera.main.ScreenToWorldPoint(mousepoint);
    }

    private void PhysicsTick(float dt)
    {
        var pool = universeData.celestialBodyPool;
        int cursor = universeData.cursor;

        // 计算引力
        for (int i = 1; i < cursor; i++)
        {
            if (pool[i].id == 0) 
                continue;

            ref var bodyA = ref pool[i];
            Vector2 totalForce = Vector2.zero;

            for (int j = 1; j < cursor; j++)
            {
                if (i == j || pool[j].id == 0) 
                    continue;

                ref var bodyB = ref pool[j];

                Vector2 dir = bodyB.position - bodyA.position;
                float distSq = dir.magnitude;

                if (distSq > 0.1f)
                {
                    // 万有引力公式 F = (G * M1 * M2) / R^2
                    float forceMag = (G * bodyA.mass * bodyB.mass) / distSq;
                    totalForce += forceMag * dir.normalized;
                } 
            }
            Vector2 acceleration = totalForce / bodyA.mass;
            bodyA.velocity += acceleration * dt; 
        }
        
        // 处理碰撞
        if (enableCollision)
        {
            ResolveCollision();
        }

        // 处理移动和边界
        float boundary = 100f; // 世界大小
        for (int i = 1; i < cursor; i++)
        {
            if (pool[i].id == 0) 
                continue;
            ref var body = ref pool[i];

            body.position += body.velocity * dt;
            if (body.position.sqrMagnitude > boundary * boundary)
            {
                // 向中心移动
                body.velocity -= body.position.normalized * 1000f * dt;
            }
        }
    }

    private void RenderTick()
    {
        if (bodyMesh == null || bodyMaterial == null) return;
        _mpb.Clear();

        var pool = universeData.celestialBodyPool;
        int cursor = universeData.cursor;
        int batchIndex = 0;

        for (int i = 0; i < cursor; i++)
        {
            if (pool[i].id == 0)
                continue;
            ref var body = ref pool[i];

            Vector3 pos = new Vector3( body.position.x, body.position.y, 0);
            Quaternion rot = Quaternion.identity;
            Vector3 scale = Vector3.one * body.radius * 2f;

            _drawMatrices[batchIndex] = Matrix4x4.TRS( pos, rot, scale );
            _drawColors[batchIndex] = body.color;
            batchIndex++;

            if (batchIndex >= 1023)
            {
                _mpb.SetVectorArray("_BaseColor", _drawColors);
                Graphics.DrawMeshInstanced(bodyMesh, 0, bodyMaterial, _drawMatrices,batchIndex, _mpb);
                batchIndex = 0;             
            }
        }
        if (batchIndex > 0)
        {
            _mpb.SetVectorArray("_BaseColor", _drawColors);
            Graphics.DrawMeshInstanced(bodyMesh, 0, bodyMaterial, _drawMatrices, batchIndex, _mpb);
            batchIndex = 0;
        }
    }

    private void ResolveCollision()
    {
        var pool = universeData.celestialBodyPool;
        int cursor = universeData.cursor;

        for (int i = 1; i < cursor; i++)
        {
            if (pool[i].id == 0) 
                continue;

            for (int j = i + 1; j < cursor; j++)
            {
                ref var bodyA = ref pool[i];
                ref var bodyB = ref pool[j];

                // 检测是否碰撞
                Vector2 dir = bodyB.position - bodyA.position;
                float distSq = dir.sqrMagnitude;
                float radiusSum = bodyA.radius + bodyB.radius;

                // 距离小于半径之和，发生碰撞
                if (distSq < radiusSum * radiusSum)
                {
                    // 保证大质量天体在前，方便处理
                    if (bodyA.mass > bodyB.mass)
                    {
                        ProcessCollision( ref bodyA, ref bodyB );
                    }
                    else
                    {
                        ProcessCollision(ref bodyB, ref bodyA);
                    }
                }
            }
        }
    }

    // 处理b1(较大质量天体)和b2(较小质量天体)的碰撞
    private void ProcessCollision(ref CelestialBody b1, ref CelestialBody b2)
    {
        // 质量比，超过4被定义为质量悬殊（吞噬）
        float radio = b1.mass / b2.mass;
        // 质量超过25定义为大质量天体
        bool isHugeb1 = b1.mass > 25f;
        bool isHugeb2 = b2.mass > 25f;

        // 质量悬殊，吞噬
        if (radio > 4f)
        {
            // 动量守恒
            float newMass = b1.mass + b2.mass;
            Vector2 newVel = (b1.velocity * b1.mass + b2.velocity * b2.mass) / newMass;

            // 更新b1
            b1.mass = newMass;
            b1.velocity = newVel;
            b1.radius = Mathf.Sqrt(b1.mass / Mathf.PI);

            // 销毁b2
            universeData.FreeCelestialBody(b2.id);
        }
        // 两个大质量天体合并，炸出部分小天体
        else if (isHugeb1 && isHugeb2)
        {
            // 碎片质量设定为总质量的40%
            float debrisMess = (b1.mass + b2.mass) * 0.4f;
            float newMass = (b1.mass + b2.mass) * 0.6f;

            // 动量守恒
            Vector2 newVel = ( b1.velocity * b1.mass + b2.velocity * b2.mass ) / ( b1.mass + b2.mass );

            // 合并中心
            Vector2 hitCenter = (b1.position + b2.position) * 0.5f;

            // 更新b1
            b1.mass = newMass;
            b1.velocity = newVel;
            b1.radius = Mathf.Sqrt(b1.mass / Mathf.PI);

            // 销毁b2
            universeData.FreeCelestialBody(b2.id);

            // 炸出随机数量小天体
            int debrisCount = UnityEngine.Random.Range(1, 5);
            float massPerDebris = debrisMess / debrisCount;
            for (int i = 0; i < debrisCount; i++)
            {
                Vector2 randomDir = UnityEngine.Random.insideUnitCircle.normalized;
                Vector2 spawnPos = b1.position + randomDir * (b1.radius + 2f);
                Vector2 spawnVel = b1.position + randomDir * UnityEngine.Random.Range(3, 5);

                universeData.CreateCelestialBody(spawnPos, spawnVel, massPerDebris, Color.gray);
            }
        }
        // 两个小天体碰撞
        else
        {
            //// 简单的分离位置，防止重叠粘连
            //Vector2 dir = (b1.position - b2.position).normalized;
            //b1.position += dir * 0.5f;
            //b2.position -= dir * 0.5f;

            //// 交换速度分量
            //Vector2 vRel = b1.velocity - b2.velocity;
            //float restitution = 0.9f; // 弹性系数 < 1

            //// 互换速度并衰减
            //Vector2 tempV = b1.velocity;
            //b1.velocity = Vector2.Lerp(b1.velocity, b2.velocity, 0.5f) * restitution;
            //b2.velocity = Vector2.Lerp(b2.velocity, tempV, 0.5f) * restitution;

            Vector2 collisionDir = b2.position - b1.position;
            float dist = collisionDir.magnitude;

            if (dist < 0.01f) return;

            // 法线向量
            Vector2 normal = collisionDir / dist;

            // 位置修正
            float radiusSum = b1.radius + b2.radius;
            float penetration = radiusSum - dist;

            // 天体重叠
            if (penetration > 0)
            {
                float totalMass = b1.mass + b2.mass;

                if(totalMass > 0)
                {
                    float m1Ratio = b2.mass / totalMass;
                    float m2Ratio = b1.mass / totalMass;

                    // 按质量比例推开
                    b1.position -= m1Ratio * penetration * normal;
                    b2.position += m2Ratio * penetration * normal;
                }
            }

            Vector2 relativeVel = b2.velocity - b1.velocity;
            float velAlongNormal = Vector2.Dot(relativeVel, normal);
            if (velAlongNormal > 0) return;

            // 计算冲量
            float restitution = 0.9f; // 弹性系数
            float j = -(1 + restitution) * velAlongNormal;
            float invMassSum = b1.massInv + b2.massInv;

            // 避免无限质量导致的除以零
            if (invMassSum == 0) return;

            j /= invMassSum;

            // 应用冲量改变速度
            Vector2 impulse = j * normal;

            b1.velocity -= impulse * b1.massInv;
            b2.velocity += impulse * b2.massInv;
        }
    }

    // Gizmo 辅助线
    void OnDrawGizmos()
    {
        if (_isDragging)
        {
            Vector2 currentMouse = GetMouseWorldPos();
            Gizmos.color = Color.white;
            Gizmos.DrawLine(_dragStartPos, currentMouse);
            Gizmos.DrawWireSphere(_dragStartPos, 1.0f);
        }
    }
}
