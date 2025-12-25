using UnityEngine;

/// <summary>
/// UniverseData：数据容器类，负责存储，创建和销毁所有天体数据
/// </summary>
public class UniverseData
{
    // 天体数组
    public CelestialBody[] celestialBodyPool;

    public int capacity;
    public int cursor;

    // 回收站，存储被销毁id重复利用
    private int[] recycleIds;
    private int recycleCount;

    public UniverseData(int maxCount)
    {
        capacity = maxCount;
        celestialBodyPool = new CelestialBody[capacity];
        recycleIds= new int[capacity];
        cursor = 1; // cursor = 0 作为无效指针
        recycleCount = 0;
    }

    // 创建天体
    public int CreateCelestialBody(Vector2 pos, Vector2 vel, float mass, Color color)
    {
        int id;

        // 优先使用回收站的id
        if(recycleCount > 0)
        {
            id = recycleIds[--recycleCount];
        }
        else
        {
            // 天体指针后移，创建新的天体id
            id = cursor++;
            if(id >= capacity)
            {
                // 超出最大天体数量时不再创建
                Debug.LogWarning("当前天体数量已满，无法继续创建");
                return 0;
            }
        }

        // 取出新id对应的天体数据，进行初始化
        ref var body = ref celestialBodyPool[id];
        body.id = id;
        body.position = pos;
        body.velocity = vel;
        body.mass = mass;
        body.color = color;
        body.massInv = 1.0f / mass;
        // 半径计算公式 r = sqrt( mass / PI )
        body.radius = Mathf.Sqrt(mass / Mathf.PI);

        return id;
    }

    // 销毁天体
    public void FreeCelestialBody(int id)
    {
        if (id <= 0 || id >= capacity) return;
        if (celestialBodyPool[id].id == 0) return;

        // 天体id标记为0，视为删除
        celestialBodyPool[id].id = 0;
        celestialBodyPool[id].mass = 0;

        // 删除的id压入回收站
        recycleIds[recycleCount++] = id;
    }

    // 清空所有的天体
    public void Clear()
    {
        System.Array.Clear(celestialBodyPool, 0, celestialBodyPool.Length);
        cursor = 1;
        recycleCount = 0;
    }
 }
