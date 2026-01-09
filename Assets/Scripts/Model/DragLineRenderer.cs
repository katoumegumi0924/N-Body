using Unity.VisualScripting;
using UnityEngine;

/// <summary>
/// DragLineRender：
/// </summary>
public class DragLineRenderer
{
    private GameObject lineObj;
    private LineRenderer line;

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

    public void Draw(bool active, Vector2 startPos, Vector2 endPos)
    {
        // line渲染
        if (line == null)
            return;

        if (line.enabled != active)
            line.enabled = active;

        if (active)
        {
            line.SetPosition(0, startPos);
            line.SetPosition(1, endPos);
        }
    }
}
