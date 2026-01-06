using UnityEngine;

/// <summary>
/// CameraState：
/// </summary>
public struct CameraState
{
    public Vector2 position; // 相机位置
    public float zoom; // 缩放等级

    public void Reset()
    {
        position = Vector2.zero;
        zoom = 100f;
    }
}