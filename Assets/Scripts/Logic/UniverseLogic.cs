using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// UniverseLogic：
/// </summary>
public class UniverseLogic
{
    // 暂存爆炸请求
    private List<ExplosionRequest> _delay_explosion_list = new List<ExplosionRequest>();
    // 暂存销毁请求
    private List<int> _delay_remove_list = new List<int>();

    private readonly float G = GameConfig.universeConfig.G;

    public void Init()
    {
        if (_delay_explosion_list == null)
        {
            _delay_explosion_list = new List<ExplosionRequest>();
        }
        else
        {
            _delay_explosion_list.Clear();
        }

        if (_delay_remove_list == null)
        {
            _delay_remove_list = new List<int>();
        }
        else
        {
            _delay_remove_list.Clear();
        }
    }

    public void Free()
    {
        if (_delay_explosion_list != null)
        {
            _delay_explosion_list.Clear();
            _delay_explosion_list = null;
        }

        if (_delay_remove_list != null)
        {
            _delay_remove_list.Clear();
            _delay_remove_list = null;
        }
    }

    public void LogicTick(GameData gameData)
    {
        float dt = gameData.universeTimeData.tickDelta * TimeData.tickDeltaTime;
        CalculatePosition(gameData, dt);
        HandleCollision(gameData);
        PostTickProcess(gameData);  
    }

    // 计算引力和速度
    private void CalculatePosition(GameData gameData, float dt)
    {
        var pool = gameData.universeData.pool;
        int cursor = pool.cursor;

        // 重置天体所受引力
        for (int i = 1; i < cursor; ++i)
        {
            if (pool[i].id == i)
            {
                pool[i].force = Vector2.zero;
            }
        }

        // 两层循环计算所有天体引力
        for (int i = 1; i < cursor; ++i)
        {
            if (pool[i].id != i)
                continue;     
            for (int j = i + 1; j < cursor; ++j)
            {
                if (pool[j].id != j)
                    continue;

                ref var astroA = ref pool[i];
                ref var astroB = ref pool[j];

                Vector2 dir = astroB.position - astroA.position;
                float distSqr = dir.sqrMagnitude;
                // 防止除零
                distSqr = distSqr < 1e-5f ? 1e-5f : distSqr;
                float dist = Mathf.Sqrt(distSqr);

                float force = G * astroA.mass * astroB.mass / (distSqr + GameConfig.universeConfig.gravitySoft);
                Vector2 forceVec = force * (dir / dist);

                astroA.force += forceVec;
                astroB.force -= forceVec;
            }
        }

        float boundsMaxX = gameData.universeData.worldBounds.maxX;
        float boundsMinX = gameData.universeData.worldBounds.minX;
        float boundsMaxY = gameData.universeData.worldBounds.maxY;
        float boundsMinY = gameData.universeData.worldBounds.minY;
        // 计算加速度，处理移动
        for (int i = 1; i < cursor; ++i)
        {
            if (pool[i].id != i)
                continue;

            ref var astro = ref pool[i];

            Vector2 acceleration = astro.force * astro.massInv;
            astro.velocity += acceleration * dt;
            astro.InternelUpdate(dt);

            // 边界处理
            float maxX = boundsMaxX - astro.radius;
            float minX = boundsMinX + astro.radius;
            if (astro.position.x > maxX)
            {
                astro.position.x = maxX;
                if (astro.velocity.x > 0)
                    astro.velocity.x = -astro.velocity.x;
            }
            else if (astro.position.x < minX)
            {
                astro.position.x = minX;
                if (astro.velocity.x < 0)
                    astro.velocity.x = -astro.velocity.x;
            }

            float maxY = boundsMaxY - astro.radius;
            float minY = boundsMinY + astro.radius;
            if (astro.position.y > maxY)
            {
                astro.position.y = maxY;
                if (astro.velocity.y > 0)
                    astro.velocity.y = -astro.velocity.y;
            }
            else if (astro.position.y < minY)
            {
                astro.position.y = minY;
                if (astro.velocity.y < 0)
                    astro.velocity.y = -astro.velocity.y;
            }
        }
    }

    private void HandleCollision(GameData data)
    {
        var pool = data.universeData.pool;
        int cursor = pool.cursor;

        for (int i = 1; i < cursor; ++i)
        {
            if (pool[i].id != i)
                continue;
            for (int j = i + 1; j < cursor; ++j)
            {
                if (pool[j].id != j)
                    continue;

                ref var astroA = ref pool[i];
                ref var astroB = ref pool[j];

                Vector2 dir = astroB.position - astroA.position;
                float distSqr = dir.sqrMagnitude;
                float radiusSum = astroA.radius + astroB.radius;
                if (distSqr < radiusSum * radiusSum)
                {
                    // 处理碰撞时保证大质量天体在前
                    if (astroA.mass > astroB.mass)
                        ProcessCollision(data, ref astroA, ref astroB);
                    else
                        ProcessCollision(data, ref astroB, ref astroA);
                }    
            }
        }
    }

    private void ProcessCollision(GameData data, ref AstroData major, ref AstroData minor)
    {
        float massRatio = major.mass * minor.massInv;
        // 质量悬殊，吞噬
        if (massRatio > GameConfig.universeConfig.swallowThreshold)
        {
            MergeAstro(data, ref major, ref minor);
        }
        // 两个大质量天体，融合 分裂
        else if (major.mass > GameConfig.universeConfig.hugeMass && minor.mass > GameConfig.universeConfig.hugeMass)
        {
            MergeAndExplode(data, ref major, ref minor);
        }
        // 两个小质量天体，非完全弹性碰撞
        else
        {
            NonFullyElasticCollide(ref major, ref minor);
        }   
    }

    private void MergeAstro(GameData data, ref AstroData major, ref AstroData minor)
    {
        float m1 = major.mass;
        float m2 = minor.mass;
        float totalMass = m1 + m2;
        float maxRadius = data.universeData.MAX_RADIUS;

        // 动量守恒计算新速度
        Vector2 newVel = (m1 * major.velocity + m2 * minor.velocity) / totalMass;

        // 更新major
        major.velocity = newVel;
        major.mass = totalMass;
        major.massInv = 1.0f / totalMass;
        major.radius = Mathf.Sqrt(major.mass / major.density) > maxRadius ? maxRadius : Mathf.Sqrt(major.mass / major.density);

        // 销毁minor 不应该在循环中销毁
        _delay_remove_list.Add(minor.ID);
    }

    private void MergeAndExplode(GameData data, ref AstroData major, ref AstroData minor)
    {
        float m1 = major.mass;
        float m2 = minor.mass;
        float totalMass = m1 + m2;
        float maxRadius = data.universeData.MAX_RADIUS;

        // 根据合并损耗率计算碎片质量与合并后质量
        float debrisTotalMass = totalMass * GameConfig.universeConfig.lossRatio;
        float newMass = totalMass - debrisTotalMass;

        Vector2 newVel = (m1 * major.velocity + m2 * minor.velocity) / totalMass;
        Vector2 centerPos = (major.position + minor.position) * 0.5f;

        // 更新major
        major.position = centerPos;
        major.velocity = newVel;
        major.mass = newMass;
        major.radius = Mathf.Sqrt(major.mass / major.density) > maxRadius ? maxRadius : Mathf.Sqrt(major.mass / major.density);

        // 销毁minor 不应该在循环中销毁
        _delay_remove_list.Add(minor.ID);

        // 记录爆炸生成新天体的请求
        _delay_explosion_list.Add(new ExplosionRequest()
        {
            center = centerPos,
            velocity = major.velocity,
            totalMass = debrisTotalMass,
            count = Random.Range(5, 10),
            protoId = debrisTotalMass > 200 ? 1 : 0, // 碎片总质量超过200，炸出行星
            offset = major.radius * 1.35f
        });
    }

    private void NonFullyElasticCollide(ref AstroData major, ref AstroData minor)
    {
        Vector2 dir = minor.position - major.position;
        if (dir == Vector2.zero)
            dir = Vector2.right * 0.01f;

        float distSqr = dir.sqrMagnitude;
        float dist = Mathf.Sqrt(distSqr);

        Vector2 normal = dir / dist;

        // 位置修正
        float penetration = major.radius + minor.radius - dist;
        if (penetration > 0)
        {
            float m1 = major.mass;
            float m2 = minor.mass;
            float totalMass = m1 + m2;

            // 按质量反比分配移动量
            float moveMajor = penetration * (m2 / totalMass);
            float moveMinor = penetration * (m1 / totalMass);

            major.position -= normal * moveMajor;
            minor.position += normal * moveMinor;
        }

        // 计算minor的相对速度
        Vector2 relativeVel = minor.velocity - major.velocity;
        float velAlongNormal = Vector2.Dot(relativeVel, normal);
        // 正在分离，不处理
        if (velAlongNormal > 0)
            return;

        // 非完全弹性碰撞
        // 使用平均弹性系数
        float e = (major.elasticityCoef + minor.elasticityCoef) * 0.5f;
        float j = -(1.0f + e) * velAlongNormal;
        float invMassSum = major.massInv + minor.massInv;
        if (invMassSum > 0)
        {
            j /= invMassSum;
            Vector2 impulse = j * normal;
            major.velocity -= impulse * major.massInv;
            minor.velocity += impulse * minor.massInv;
        }
    }

    private void PostTickProcess(GameData data)
    {
        // 集中销毁
        if (_delay_remove_list.Count > 0)
        {
            for (int i = 0; i < _delay_remove_list.Count; ++i)
            {
                data.universeData.FreeAstro(_delay_remove_list[i]);
            }
            _delay_remove_list.Clear();
        }

        // 集中创建
        if (_delay_explosion_list.Count > 0)
        {
            for (int i = 0; i < _delay_explosion_list.Count; ++i)
            {
                var request = _delay_explosion_list[i];
                float massPerDebris = request.totalMass / request.count;
                for (int j = 0; j < request.count; ++j)
                {
                    Vector2 randomDir = Random.insideUnitCircle.normalized;
                    Vector2 dir = new Vector2(randomDir.x, randomDir.y);
                    Vector2 spawnPos = request.center + dir * request.offset;
                    Vector2 spawnVel = request.velocity + dir * Random.Range(GameConfig.universeConfig.minDebrisSpeed, GameConfig.universeConfig.maxDebrisSpeed);

                    data.universeData.CreateAstro(request.protoId, spawnPos, spawnVel, data.universeTimeData.tickCounter, massPerDebris);
                }
            }
            _delay_explosion_list.Clear();
        }
    }
}

// 暂存爆炸请求，防止在遍历过程中改变pool
public struct ExplosionRequest
{
    public Vector2 center;             // 爆炸中心位置
    public Vector2 velocity;           // 爆炸赋予的初速度
    public float totalMass;            // 碎片总质量
    public int protoId;                // 碎片原型id
    public int count;                  // 碎片个数
    public float offset;               // 离爆炸中心的偏移距离
}