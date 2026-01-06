using UnityEngine;

/// <summary>
/// InteractionData：
/// </summary>
public struct InteractionData
{
    // dragLine参数
    public bool isDragging;
    public DVector2 dragStartPos;
    public DVector2 dragEndPos;

    // 加速按钮点击状态
    public bool reqSpeedUp;
    public bool reqSpeedDown;
    public bool reqTogglePause;

    public void Reset()
    {
        this = default;
    }
}