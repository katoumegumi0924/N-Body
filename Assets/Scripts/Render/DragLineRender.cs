using Unity.VisualScripting;
using UnityEngine;

/// <summary>
/// DragLineRender：
/// </summary>
public class DragLineRender
{
    private GameObject lineObj;
    private LineRenderer line;

    public void Init()
    {
        if (GameConfig.gameResourcesConfig.dragLinePrefab == null)
            return;

        lineObj = Object.Instantiate(GameConfig.gameResourcesConfig.dragLinePrefab);
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
            Object.Destroy(lineObj);
            lineObj = null;
            line = null;
        }
        
    }

    public void Draw(bool active, DVector2 startPos, DVector2 endPos)
    {
        // line渲染
        if (line == null)
            return;

        if (line.enabled != active)
            line.enabled = active;

        if (active)
        {
            line.SetPosition(0, (Vector2)startPos);
            line.SetPosition(1, (Vector2)endPos);
        }
    }
}
