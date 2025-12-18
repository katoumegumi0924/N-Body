using UnityEngine;

/// <summary>
/// BodyRenderer：根据CelestialBody的数据，渲染天体对象的位置和大小
/// </summary>
public class BodyRenderer : MonoBehaviour
{
    public CelestialBody data;

    public void Initialize( CelestialBody bodyData )
    {
        data = bodyData;
        UpdateVisuals();
    }

    public void UpdateVisuals()
    {
        transform.position = data.Position;
        transform.localScale = Vector2.one * data.Radius * 2;
    }
}
