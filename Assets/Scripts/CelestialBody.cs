using UnityEngine;

/// <summary>
/// CelestialBody：
/// 天体类，纯数据类
/// </summary>
[System.Serializable]
public class CelestialBody
{
    public Vector2 Position;
    public Vector2 Velocity;
    public float Mass;
    public float Radius;

    public Vector2 Force;       //每帧的受力情况

    public CelestialBody(Vector2 pos, Vector2 initialVel ,float mass)
    {
        Position = pos;
        Velocity = initialVel;
        Mass = mass;
        // 根据 密度=质量/面积 假定密度为1，算出半径 r = Sqrt(m / PI)
        Radius = Mathf.Sqrt(mass / Mathf.PI );
    }
}
