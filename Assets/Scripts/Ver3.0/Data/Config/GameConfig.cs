using UnityEngine;

/// <summary>
/// GameConfig：
/// </summary>

[CreateAssetMenu(fileName = "GameConfig", menuName = "NBody/GameConfig")]
public class GameConfig : ScriptableObject
{
    [Tooltip("引力常数")]
    public float G = 10.0f;

    [Tooltip("世界范围")]
    public float universeRadius = 100f;

    [Tooltip("大质量天体阈值")]
    public float hugeMass = 100;

    [Tooltip("吞噬所需倍率")]
    public float swallowThreshold = 5f;

    [Tooltip("大质量天体合并损耗率")]
    public float lossRatio = 0.4f;

    [Tooltip("拖拽生成天体时的发射力度")]
    public float launchForce = 1.0f;

    [Tooltip("新生天体碰撞保护时长")]
    public float spawnImmunityTime = 0.5f;

    [Tooltip("撞击产生碎片的速度范围")]
    public float minDebrisSpeed = 50f;
    public float maxDebrisSpeed = 100f;
}
