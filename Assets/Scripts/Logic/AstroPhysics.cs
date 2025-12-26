using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// AstroPhysics：
/// </summary>
public class AstroPhysics
{
    private AstroData _data;
    private AstroProto _debrisProto;

    // 暂存爆炸请求
    private List<ExplosionRequest> explosionRequestList = new List<ExplosionRequest>();

    public void Init(AstroData data, AstroProto debrisProto)
    {
        _data = data;
        _debrisProto = debrisProto;
    }

    public void Free()
    {
        _data = null;
        _debrisProto = null;

        if (explosionRequestList != null)
        {
            explosionRequestList.Clear();
            explosionRequestList = null;
        }
    }

    public void LogicTick(float deltaTime, Vector2 screenBounds)
    {
        CalculatePosition(deltaTime, screenBounds);
        HandleCollision();
        ProcessExplosion(_debrisProto);
    }

    // 计算引力和速度
    private void CalculatePosition(float deltaTime, Vector2 screenBounds)
    {
        var pool = _data.pool;
        int cursor = _data.cursor;
        float G = GameConfig.universeConfig.G;

        // 重置天体所受引力
        for (int i = 1; i < cursor; ++i)
        {
            if (pool[i].active)
                pool[i].force = Vector2.zero;
        }

        // 两层循环计算所有天体引力
        for (int i = 1; i < cursor; ++i)
        {
            if (!pool[i].active)
                continue;     
            for (int j = i + 1; j < cursor; ++j)
            {
                if (!pool[j].active)
                    continue;
                ref var astroA = ref pool[i];
                ref var astroB = ref pool[j];

                Vector2 dir = astroB.position - astroA.position;
                float distSqr = dir.sqrMagnitude;
                float dist = Mathf.Sqrt(distSqr);

                // 防止除零
                distSqr = distSqr < 0.01f ? 0.1f : distSqr;
                dist = dist < 0.01f ? 0.1f : dist;
                // 引力计算公式 F = G * (m1 * m2) / r^3 * dir
                Vector2 forceVec = G * (astroA.mass * astroB.mass) / (distSqr * dist) * dir;

                astroA.force += forceVec;
                astroB.force -= forceVec;
            }
        }

        // 计算加速度，处理移动
        for (int i = 1; i < cursor; ++i)
        {
            if (!pool[i].active)
                continue;

            ref var astro = ref pool[i];
            Vector2 acceleration = astro.force * astro.massInv;
            astro.velocity += acceleration * deltaTime;
            astro.position += astro.velocity * deltaTime;

            // 边界处理
            float xLimit = screenBounds.x - astro.radius;
            float yLimit = screenBounds.y - astro.radius;
            if (astro.position.x > xLimit)
            {
                astro.position.x = xLimit;
                if (astro.velocity.x > 0)
                    astro.velocity.x = -astro.velocity.x;
            }
            else if (astro.position.x < -xLimit)
            {
                astro.position.x = -xLimit;
                if (astro.velocity.x < 0)
                    astro.velocity.x = -astro.velocity.x;
            }
            
            if (astro.position.y > yLimit)
            {
                astro.position.y = yLimit;
                if (astro.velocity.y > 0)
                    astro.velocity.y = -astro.velocity.y;
            }
            else if (astro.position.y < -yLimit)
            {
                astro.position.y = -yLimit;
                if (astro.velocity.y < 0)
                    astro.velocity.y = -astro.velocity.y;
            }
        }
    }

    private void HandleCollision()
    {
        var pool = _data.pool;
        int cursor = _data.cursor;

        float time = Time.time;
        float immunityDuration = GameConfig.universeConfig.spawnImmunityTime;

        for (int i = 1; i < cursor; ++i)
        {
            if (!pool[i].active || time < pool[i].birthTime + immunityDuration)
                continue;
            for (int j = i + 1; j < cursor; ++j)
            {
                // 再次检查pool[i]，可能在上一次循环中被吞噬或销毁
                if (!pool[i].active)
                    break;

                if (!pool[j].active || time < pool[j].birthTime + immunityDuration)
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
                        ProcessCollision(ref astroA, ref astroB);
                    else
                        ProcessCollision(ref astroB, ref astroA);
                }    
            }
        }
    }

    private void ProcessCollision(ref Astro major, ref Astro minor)
    {
        float massRatio = major.mass * minor.massInv;
        // 质量悬殊，吞噬
        if (massRatio > GameConfig.universeConfig.swallowThreshold)
        {
            MergeAstro(ref major, ref minor);
        }
        // 两个大质量天体，融合 分裂
        else if (major.mass > GameConfig.universeConfig.hugeMass && minor.mass > GameConfig.universeConfig.hugeMass)
        {
            MergeAndExplode(ref major, ref minor);
        }
        // 两个小质量天体，非完全弹性碰撞
        else
        {
            NonFullyElasticCollide(ref major, ref minor);
        }
    }

    private void MergeAstro(ref Astro major, ref Astro minor)
    {
        // 动量守恒计算新速度
        float totalMass = major.mass + minor.mass;
        Vector2 newVel = (major.mass * major.velocity + minor.mass * minor.velocity) / totalMass;

        // 更新major
        major.velocity = newVel;
        major.mass = totalMass;
        major.massInv = 1.0f / totalMass;
        major.radius = Mathf.Sqrt(major.mass / major.density);

        // 销毁minor
        _data.FreeAstro(minor.id);
    }

    private void MergeAndExplode(ref Astro major, ref Astro minor)
    {
        float totalMass = major.mass + minor.mass;
        // 根据合并损耗率计算碎片质量与合并后质量
        float debrisTotalMass = totalMass * GameConfig.universeConfig.lossRatio;
        float newMass = totalMass - debrisTotalMass;

        Vector2 newVel = (major.mass * major.velocity + minor.mass * minor.velocity) / totalMass;
        Vector2 centerPos = (major.position + minor.position) / 2;

        // 更新major
        major.mass = newMass;
        major.position = centerPos;
        major.velocity = newVel;
        major.radius = Mathf.Sqrt(major.mass / major.density);

        // 销毁minor
        _data.FreeAstro(minor.id);

        // 记录爆炸生成新天体的请求
        explosionRequestList.Add(new ExplosionRequest()
        {
            center = centerPos,
            velocity = major.velocity,
            totalMass = debrisTotalMass,
            count = Random.Range(5, 10),
            offset = major.radius
        });

    }

    private void NonFullyElasticCollide(ref Astro major, ref Astro minor)
    {
        Vector2 dir = minor.position - major.position;
        float dist = dir.magnitude;
        if (dist < 0.01f)
            return;
        Vector2 normal = dir / dist;

        // 位置修正
        float penetration = (major.radius + minor.radius) - dist;
        if (penetration > 0)
        {
            float totalMass = major.mass + minor.mass;
            // 按质量反比分配移动量
            major.position -= normal * (penetration * (minor.mass / totalMass));
            minor.position += normal * (penetration * (major.mass / totalMass));
        }

        Vector2 relativeVel = minor.velocity - major.velocity;
        float velAlongNormal = Vector2.Dot(relativeVel, normal);
        // 正在分离，不处理
        if (velAlongNormal > 0) 
            return; 

        // 非完全弹性碰撞
        // 取平均弹性系数
        float e = (major.elasticityModulus + minor.elasticityModulus) * 0.5f;
        float j = -(1 + e) * velAlongNormal;
        j /= (major.massInv + minor.massInv);

        Vector2 impulse = j * normal;
        major.velocity -= impulse * major.massInv;
        minor.velocity += impulse * minor.massInv;
    }

    public void ProcessExplosion(AstroProto debrisProto)
    {
        for (int i = 0; i < explosionRequestList.Count; ++i)
        {
            float massPerDebris = explosionRequestList[i].totalMass / explosionRequestList[i].count;
            for (int j = 0; j < explosionRequestList[i].count; ++j)
            {
                Vector2 dir = Random.insideUnitCircle.normalized;
                Vector2 spawnPos = explosionRequestList[i].center + dir * explosionRequestList[i].offset;
                Vector2 spawnVel = explosionRequestList[i].velocity + dir * Random.Range(GameConfig.universeConfig.minDebrisSpeed, GameConfig.universeConfig.maxDebrisSpeed);

                _data.CreateAstro(debrisProto, spawnPos, spawnVel, Time.time, massPerDebris);
            }
        }
        explosionRequestList.Clear();
    }
}

// 暂存爆炸请求，防止在遍历过程中改变pool
public struct ExplosionRequest
{
    public Vector2 center;      // 爆炸中心位置
    public Vector2 velocity;    // 爆炸赋予的初速度
    public float totalMass;     // 碎片总质量
    // public int protoId;         // 碎片原型id
    public int count;           // 碎片个数
    public float offset;        // 离爆炸中心的偏移距离
}