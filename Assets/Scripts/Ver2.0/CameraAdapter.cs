using UnityEngine;

/// <summary>
/// CameraAdpater：
/// </summary>
public class CameraAdpater : MonoBehaviour
{
    public float targetRadius = 100f;
    public float bufferScale = 1.5f;

    float neededSize = 100f;

    private void Start()
    {
        AdjustCamera();
    }

    private void Update()
    {
        AdjustCamera();
    }

    void AdjustCamera()
    {
        Camera cam = Camera.main;
        if (cam == null) return;

        float screenAspect = (float)Screen.width / (float)Screen.height; 

        if( screenAspect > 1.0f)
        {
            neededSize = targetRadius;
        }
        else
        {
            neededSize = targetRadius / screenAspect;
        }

        cam.orthographicSize = neededSize * bufferScale;
    }

    void OnDrawGizmos()
    {
        // 绿色圆圈：逻辑边界
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(Vector3.zero, neededSize); 

        if (Camera.main != null)
        {
            Gizmos.color = Color.red;
            float h = Camera.main.orthographicSize;
            float w = h * Camera.main.aspect;
            // 画出相机视野矩形
            Gizmos.DrawWireCube(Camera.main.transform.position, new Vector3(w * 2, h * 2, 1));
        }
    }
}

