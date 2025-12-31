using UnityEngine;

/// <summary>
/// InteractionData：
/// </summary>
public struct InteractionData
{
    public bool isDragging;
    public DVector2 dragStartPos;
    public DVector2 dragEndPos;

    public void Reset()
    {
        this = default;
    }

}
