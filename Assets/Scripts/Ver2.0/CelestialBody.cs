using UnityEngine;

/// <summary>
/// CelestialBody： 天体数据类
/// </summary>
public struct CelestialBody
{
    public int id;
    public Vector2 position;
    public Vector2 velocity;
    public float mass;
    public float radius;
    public float force;

    public Color color;
    public float massInv;
}
