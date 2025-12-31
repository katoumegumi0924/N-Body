using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// UniverseLogic：
/// </summary>
public class UniverseLogic
{
    // 暂存爆炸请求
    private List<ExplosionRequest> pendingExplosion = new List<ExplosionRequest>();
    // 暂存销毁请求
    private List<int> pendingFree = new List<int>();

    private readonly float G = GameConfig.universeConfig.G;

    public void Init()
    {
        if (pendingExplosion == null)
        {
            pendingExplosion = new List<ExplosionRequest>();
        }
        else
        {
            pendingExplosion.Clear();
        }

        if (pendingFree == null)
        {
            pendingFree = new List<int>();
        }
        else
        {
            pendingFree.Clear();
        }
    }

    public void Free()
    {
        if (pendingExplosion != null)
        {
            pendingExplosion.Clear();
            pendingExplosion = null;
        }

        if (pendingFree != null)
        {
            pendingFree.Clear();
            pendingFree = null;
        }
    }

    public void LogicTick(GameData data, double deltaTime, Vector2 screenBounds)
    {
        CalculatePosition(data, deltaTime, screenBounds);
        HandleCollision(data);
        PostTickProcess(data);
    }

    // 计算引力和速度
    private void CalculatePosition(GameData data, double deltaTime, Vector2 screenBounds)
    {
        var pool = data.universeData.pool;
        int cursor = data.universeData.pool.cursor;

        // 重置天体所受引力
        for (int i = 1; i < cursor; ++i)
        {
            if (pool[i].active)
            {
                pool[i].force = new DVector2(0, 0);
       
            }
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

                DVector2 dir = astroB.position - astroA.position;
                double distSqr = dir.SqrMagnitude;
                double dist = System.Math.Sqrt(distSqr);

                // 防止除零
                distSqr = distSqr < 0.01f ? 0.1f : distSqr;
                // 引力计算公式 F = G * (m1 * m2) / r^3 * dir;
                DVector2 forceVec = (G * (double)astroA.mass * (double)astroB.mass / (distSqr * dist)) * dir;

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
            DVector2 acceleration = astro.force * (double)astro.massInv;

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

    private void HandleCollision(GameData data)
    {
        var pool = data.universeData.pool;
        int cursor = data.universeData.pool.cursor;

        long immunityTicks = (long)(GameConfig.universeConfig.spawnImmunityTime * 10_000_000);

        for (int i = 1; i < cursor; ++i)
        {
            if (!pool[i].active || data.clock.totalTicks < pool[i].birthTick + immunityTicks)
                continue;
            for (int j = i + 1; j < cursor; ++j)
            {
                // 再次检查pool[i]，可能在上一次循环中被吞噬或销毁
                if (!pool[i].active)
                    break;

                if (!pool[j].active || data.clock.totalTicks < pool[j].birthTick + immunityTicks)
                    continue;

                ref var astroA = ref pool[i];
                ref var astroB = ref pool[j];

                DVector2 dir = astroB.position - astroA.position;
                double distSqr = dir.SqrMagnitude;
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

    private void ProcessCollision(ref AstroData major, ref AstroData minor)
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

    private void MergeAstro(ref AstroData major, ref AstroData minor)
    {
        double m1 = (double)major.mass;
        double m2 = (double)minor.mass;
        double totalMass = m1 + m2;

        // 动量守恒计算新速度
        DVector2 newVel = (m1 * major.velocity + m2 * minor.velocity) / totalMass;

        // 更新major
        major.velocity = newVel;
        major.mass = (float)totalMass;
        major.massInv = (float)(1.0f / totalMass);
        major.radius = Mathf.Sqrt(major.mass / major.density);

        // 销毁minor 不应该在循环中销毁
        pendingFree.Add(minor.ID);
    }

    private void MergeAndExplode(ref AstroData major, ref AstroData minor)
    {
        double m1 = (double)major.mass;
        double m2 = (double)minor.mass;

        double totalMass = m1 + m2;
        // 根据合并损耗率计算碎片质量与合并后质量
        float debrisTotalMass = (float)totalMass * GameConfig.universeConfig.lossRatio;
        float newMass = (float)totalMass - debrisTotalMass;

        DVector2 newVel = (m1 * major.velocity + m2 * minor.velocity) / totalMass;
        DVector2 centerPos = (major.position + minor.position) / 2;

        // 更新major
        major.position = centerPos;
        major.velocity = newVel;
        major.mass = newMass;
        major.radius = Mathf.Sqrt(major.mass / major.density);

        // 销毁minor 不应该在循环中销毁
        pendingFree.Add(minor.ID);

        // 记录爆炸生成新天体的请求
        pendingExplosion.Add(new ExplosionRequest()
        {
            center = centerPos,
            velocity = major.velocity,
            totalMass = debrisTotalMass,
            count = Random.Range(5, 10),
            offset = major.radius
        });

    }

    private void NonFullyElasticCollide(ref AstroData major, ref AstroData minor)
    {
        DVector2 dir = minor.position - major.position;
        double distSqr = dir.SqrMagnitude;
        double dist = System.Math.Sqrt(distSqr);

        if (dist < 0.01f)
            return;

        DVector2 normal = dir / dist;

        // 位置修正
        double penetration = (double)major.radius + (double)minor.radius - dist;
        if (penetration > 0)
        {
            double m1 = (double)major.mass;
            double m2 = (double)minor.mass;
            double totalMass = m1 + m2;

            // 按质量反比分配移动量
            double moveMajor = penetration * (m2 / totalMass);
            double moveMinor = penetration * (m1 / totalMass);

            major.position -= normal * moveMajor;
            minor.position += normal * moveMinor;
        }

        // 计算minor的相对速度
        DVector2 relativeVel = minor.velocity - major.velocity;
        double velAlongNormal = DVector2.Dot(relativeVel, normal);
        // 正在分离，不处理
        if (velAlongNormal > 0)
            return;

        // 非完全弹性碰撞
        // 使用平均弹性系数
        double e = (double)(major.elasticityModulus + minor.elasticityModulus) * 0.5;
        double j = -(1.0 + e) * velAlongNormal;
        double invMassSum = (double)major.massInv + (double)minor.massInv;
        if (invMassSum > 0)
        {
            j /= (double)major.massInv + (double)minor.massInv;
            DVector2 impulse = j * normal;

            major.velocity -= impulse * (double)major.massInv;
            minor.velocity += impulse * (double)minor.massInv;
        }
    }

    private void PostTickProcess(GameData data)
    {
        // 集中销毁
        if (pendingFree.Count > 0)
        {
            for (int i = 0; i < pendingFree.Count; ++i)
            {
                data.universeData.FreeAstro(pendingFree[i]);
            }
            pendingFree.Clear();
        }

        // 集中创建
        if (pendingExplosion.Count > 0)
        {
            for (int i = 0; i < pendingExplosion.Count; ++i)
            {
                var request = pendingExplosion[i];
                float massPerDebris = request.totalMass / request.count;
                for (int j = 0; j < request.count; ++j)
                {
                    Vector2 randomDir = Random.insideUnitCircle.normalized;
                    DVector2 dir = new DVector2(randomDir.x, randomDir.y);
                    DVector2 spawnPos = request.center + dir * request.offset;
                    DVector2 spawnVel = request.velocity + dir * Random.Range(GameConfig.universeConfig.minDebrisSpeed, GameConfig.universeConfig.maxDebrisSpeed);

                    // 需要优化 生成不同原型的天体，现在写死为了小行星
                    data.universeData.CreateAstro(0, spawnPos, spawnVel, data.clock.totalTicks, massPerDebris);
                }
            }
            pendingExplosion.Clear();
        }
    }
}

// 暂存爆炸请求，防止在遍历过程中改变pool
public struct ExplosionRequest
{
    public DVector2 center;            // 爆炸中心位置
    public DVector2 velocity;          // 爆炸赋予的初速度
    public float totalMass;            // 碎片总质量
    // public int protoId;             // 碎片原型id
    public int count;                  // 碎片个数
    public float offset;               // 离爆炸中心的偏移距离
}