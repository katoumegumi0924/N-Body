using UnityEngine;

/// <summary>
/// AstroProto：
/// </summary>
[CreateAssetMenu(fileName = "NewAstroProto", menuName = "NBody/NewAstroProto")]
public class AstroProto : ScriptableObject
{
    [Header("类型参数")]
    public int id;
    public string astroName;
    public AstroType type;

    [Header("物理参数")]
    public float minMass = 1.0f;
    public float maxMass = 10.0f;
    public float density = 1.0f; // 密度
    [Tooltip("弹性系数")]
    [Range(0f, 1f)]
    public float elasticityModulus = 1.0f;

    [Header("表现参数")]
    // 同类型天体颜色相近
    public Gradient colorRange;
    // 天体颜色随时间变化
    public float evolutionTime = 60f;

    // 辅助方法，生成随机属性
    public float GetRandomMass()
    {
        return Random.Range(minMass, maxMass);
    }

    public float GetRadius(float mass)
    {
        if (density < 0.1f)
            density = 0.1f;
        return Mathf.Sqrt(mass / density);
    }
}

public enum AstroType
{
    None = 0,
    Asteroid = 1,       // 小行星  
    Planet = 2,         // 行星    
    Star = 3,           // 恒星
    NeutronStar = 4,    // 中子星
    BlackHole = 5       // 黑洞
}