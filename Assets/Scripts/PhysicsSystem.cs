using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

/// <summary>
/// PhysicsSystem：
/// </summary>
public class PhysicsSystem : MonoBehaviour
{
    public GameObject bodyPrefab;        //天体对象预制体
    public float G = 1f;                //引力常数

    private List<BodyRenderer> renderers = new List<BodyRenderer>();        //天体渲染列表

    private void Start()
    {
        //中心的大质量天体
        CreatCelestialBody(new Vector2(0, 0), Vector2.zero, 20);

        //小质量天体 拥有向右的初速度
        CreatCelestialBody(new Vector2(0, 5), new Vector2(4, 0), 2);
    }

    void CreatCelestialBody( Vector2 pos, Vector2 vel, float mass )
    {
        //创建天体数据
        CelestialBody newCelestialBodyData = new CelestialBody(pos, vel, mass);

        //创建天体对象
        GameObject celestialBodyObj = Instantiate(bodyPrefab);
        BodyRenderer renderer = celestialBodyObj.AddComponent<BodyRenderer>();
        renderer.Initialize(newCelestialBodyData);

        renderers.Add(renderer);
    }

    private void FixedUpdate()
    {
        //清空天体的上一帧的受力情况
        foreach( var r in renderers)
        {
            r.data.Force = Vector2.zero;
        }

        //两层循环，计算两两天体之间的引力
        for ( int i = 0; i<renderers.Count; i++ )
        {
            for ( int j = i + 1; j<renderers.Count; j++ )
            {
                CelestialBody a = renderers[i].data;
                CelestialBody b = renderers[j].data;

                Vector2 direction = b.Position - a.Position;
                float distance = direction.magnitude;

                // 防除零保护
                if (distance < 0.1f) continue;

                //引力计算 G * ( m1 * m2 ) / r * r
                float forceMagnitude = G * (a.Mass * b.Mass) / ( distance * distance );
                Vector2 forceDir = direction;

                a.Force += forceDir * forceMagnitude;
                b.Force -= forceDir * forceMagnitude;
            }
        }

        //更新所有天体的速度和位置
        foreach( var r in renderers)
        {
            CelestialBody c = r.data;

            //计算加速度 F = Ma，a = F / M
            Vector2 acceleration = c.Force / c.Mass;

            //计算位置和速度
            c.Velocity += acceleration * Time.fixedDeltaTime;
            c.Position += c.Velocity * Time.fixedDeltaTime;

            r.UpdateVisuals();
        }
    }
}
