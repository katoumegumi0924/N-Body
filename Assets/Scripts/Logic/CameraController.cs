using UnityEngine;
using UnityEngine.Rendering.Universal;

/// <summary>
/// CameraController：
/// </summary>
public class CameraController
{
    private Camera camera;

    private float lastSize;
    private float lastAspect;

    public Vector2 screenBounds { get; private set; }

    public void Init(Camera _camera)
    {
        camera = _camera;
        CheckAndRefreshBounds();
    }

    public void Free()
    {
        camera = null;
    }

    public void OnUpdate()
    {
        CheckAndRefreshBounds();
    }

    private void CalculateBounds()
    {
        float h = lastSize;
        float w = h * lastAspect;

        screenBounds = new Vector2(w, h);
    }

    private void CheckAndRefreshBounds()
    {
        float currentSize = camera.orthographicSize;
        float currentAspect = camera.aspect;

        bool sizeChanged = Mathf.Abs(currentSize - lastSize) > 0.001f;
        bool aspectChanged = Mathf.Abs(currentAspect - lastAspect) > 0.001f;

        if (sizeChanged || aspectChanged)
        {
            lastSize = currentSize;
            lastAspect = currentAspect;

            CalculateBounds();
        }
    }
}
