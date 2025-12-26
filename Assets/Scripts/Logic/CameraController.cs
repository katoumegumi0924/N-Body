using UnityEngine;
using UnityEngine.Rendering.Universal;

/// <summary>
/// CameraController：
/// </summary>
public class CameraController
{
    private Camera _camera;

    private float _lastSize;
    private float _lastAspect;

    public Vector2 screenBounds { get; private set; }

    public void Init(Camera camera)
    {
        _camera = camera;
    }

    public void Free()
    {
        _camera = null;
    }

    public void OnUpdate()
    {
        CheckAndRefreshBounds();
    }

    private void CalculateBounds()
    {
        float h = _lastSize;
        float w = h * _lastAspect;

        screenBounds = new Vector2(w, h);
    }

    private void CheckAndRefreshBounds()
    {
        float currentSize = _camera.orthographicSize;
        float currentAspect = _camera.aspect;

        bool sizeChanged = Mathf.Abs(currentSize - _lastSize) > 0.001f;
        bool aspectChanged = Mathf.Abs(currentAspect - _lastAspect) > 0.001f;

        if (sizeChanged || aspectChanged)
        {
            _lastSize = currentSize;
            _lastAspect = currentAspect;

            CalculateBounds();
        }
    }
}
