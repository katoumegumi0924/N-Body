using UnityEngine;

/// <summary>
/// GameResources：
/// </summary>
[CreateAssetMenu(fileName = "GameResourcesConfig", menuName = "NBody/GameConfig/GameResourcesConfig")]
public class GameResourcesConfig : ScriptableObject
{
    [Header("Rendering Assets")]
    public Mesh astroMesh;
    public Material astroMaterial;

    [Header("Prefabs")]
    public GameObject dragLinePrefab;
}
