using UnityEngine;

/// <summary>
/// UniverseConfig：
/// </summary>
[CreateAssetMenu(fileName = "UniverseConfig", menuName = "NBody/GameConfig/UniverseConfig")]
public class UniverseConfig : ScriptableObject
{
    [Tooltip("引力常数")]
    public float G = 10.0f;

    [Tooltip("大质量天体阈值")]
    public float hugeMass = 100f;

    [Tooltip("吞噬所需倍率")]
    public float swallowThreshold = 5f;

    [Tooltip("大质量天体合并损耗率")]
    public float lossRatio = 0.4f;

    [Tooltip("拖拽生成天体时的发射力度")]
    public float launchForce = 1.0f;

    [Tooltip("撞击产生碎片的速度范围")]
    public float minDebrisSpeed = 50f;
    public float maxDebrisSpeed = 100f;

    [Tooltip("物理帧执行频率")]
    public float physicsStep = 0.02f;
    [Tooltip("单帧最大执行次数")]
    public int maxStepsPerFrame = 1000;

    [Tooltip("宇宙初始边界属性")]
    public float width = 400;
    public float height = 200;
    public Vector2 centerPos = Vector2.zero;
}
