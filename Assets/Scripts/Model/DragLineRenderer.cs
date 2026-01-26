using UnityEngine;

/// <summary>
/// DragLineRender：
/// </summary>
public class DragLineRenderer
{
    private GameObject lineObj;
    private LineRenderer line;

    private const float LINE_WIDTH_COEF = 0.005f;

    public void Init()
    {
        if (GameConfig.gameResourcesConfig.dragLinePrefab == null)
            return;

        lineObj = GameObject.Instantiate(GameConfig.gameResourcesConfig.dragLinePrefab);
        line = lineObj.GetComponent<LineRenderer>();

        if (line != null)
        {
            line.enabled = false;
            line.positionCount = 2;
        }
    }

    public void Free()
    {
        // 需要销毁lineObj
        if (lineObj != null)
        {
            GameObject.Destroy(lineObj);
            lineObj = null;
            line = null;
        }
    }

    public void Draw(bool active, Vector2 startPos, Vector2 endPos, Camera camera)
    {
        // line渲染
        if (line == null)
            return;

        if (line.enabled != active)
            line.enabled = active;

        if (active)
        {
            line.startWidth = LINE_WIDTH_COEF * camera.orthographicSize;
            line.endWidth = LINE_WIDTH_COEF * camera.orthographicSize;

            line.SetPosition(0, startPos);
            line.SetPosition(1, endPos);
        }
    }
}
