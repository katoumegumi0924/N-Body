using UnityEngine;

/// <summary>
/// GameResources：
/// </summary>
[CreateAssetMenu(fileName = "GameResourcesConfig", menuName = "NBody/GameConfig/GameResourcesConfig")]
public class GameResourcesConfig : ScriptableObject
{
    [Header("AstroProtoSet")]
    public AstroProtoSet protoSet;

    [Header("Rendering Assets")]
    public Mesh astroMesh;
    public Material astroMaterial;

    [Header("Prefabs")]
    public GameObject dragLinePrefab;
}
