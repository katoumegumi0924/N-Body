using UnityEngine;

/// <summary>
/// BoxRender：
/// </summary>
public class BoxRenderer
{
    private LineRenderer line;
    private GameObject lineObj;

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

    public void Draw(WorldBounds bounds)
    {
        if (line == null) 
            return;

        float x = (float)bounds.width * 0.5f;
        float y = (float)bounds.height * 0.5f;

        line.SetPosition(0, new Vector3(-x, y, 0));
        line.SetPosition(1, new Vector3(x, y, 0));
        line.SetPosition(2, new Vector3(x, -y, 0));
        line.SetPosition(3, new Vector3(-x, -y, 0));
    }
}
