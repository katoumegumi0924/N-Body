using UnityEngine;

/// <summary>
/// BoxRender：
/// </summary>
public class BoxRenderer
{
    private LineRenderer line;
    private GameObject lineObj;

    private const float BOX_WIDTH_COEF = 0.004f;

    public void Init()
    {
        GameObject prefab = GameConfig.gameResourcesConfig.dragLinePrefab;
        if (prefab != null)
        {
            lineObj = GameObject.Instantiate(prefab);
            line = lineObj.GetComponent<LineRenderer>();
        }

        if (line != null)
        {
            line.useWorldSpace = true;
            line.loop = true;
            line.positionCount = 4;
        }
    }

    public void Free()
    {
        if (lineObj != null)
        {
            GameObject.Destroy(lineObj);
            lineObj = null;
            line = null;
        }
    }

    public void Draw(WorldBounds bounds, Camera camera)
    {
        if (line == null) 
            return;

        line.loop = true;
        line.startWidth = BOX_WIDTH_COEF * camera.orthographicSize;
        line.endWidth = BOX_WIDTH_COEF * camera.orthographicSize;

        line.SetPosition(0, new Vector3(bounds.minX, bounds.minY, 0));
        line.SetPosition(1, new Vector3(bounds.maxX, bounds.minY, 0));
        line.SetPosition(2, new Vector3(bounds.maxX, bounds.maxY, 0));
        line.SetPosition(3, new Vector3(bounds.minX, bounds.maxY, 0));
    }
}
