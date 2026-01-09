using UnityEngine;
using UnityEngine.Rendering.Universal;

/// <summary>
/// CameraController：
/// </summary>
public class CameraController
{
    private Vector2 _camera_position;
    private float _camera_zoom;

    private const float ZOOM_SPEED = 0.5f;
    private const float MOVE_SPEED = 2.0f;

    private Camera mainCamera;

    public void Init(GameData gameData)
    {
        mainCamera = Camera.main;

        _camera_position = Vector2.zero;
        _camera_zoom = mainCamera.orthographicSize; 
    }

    public void Free()
    {

    }

    public void OnUpdate(GameData gameData, float deltaTime)
    {
        UpdateCamera(gameData, deltaTime);
    }

    public void UpdateCamera(GameData gameData, float deltaTime)
    {
        // 处理缩放
        float scroll = Input.GetAxis("Mouse ScrollWheel");
        if (System.Math.Abs(scroll) > 0.01f)
        {
            _camera_zoom -= scroll * _camera_zoom * ZOOM_SPEED;
            _camera_zoom = System.Math.Clamp(_camera_zoom, 0.1f, 450f);

            mainCamera.orthographicSize = _camera_zoom;
        }

        // 处理平移
        Vector2 moveDir = Vector2.zero;
        if (Input.GetKey(KeyCode.W))
        {
            moveDir.y += 1;
        }
        if (Input.GetKey(KeyCode.S))
        {
            moveDir.y -= 1;
        }
        if (Input.GetKey(KeyCode.A))
        {
            moveDir.x -= 1;
        }
        if (Input.GetKey(KeyCode.D))
        {
            moveDir.x += 1;
        }

        if (moveDir != Vector2.zero)
        {
            float speed = _camera_zoom * MOVE_SPEED;
            _camera_position += moveDir * (speed * deltaTime);

            mainCamera.transform.position = new Vector3(_camera_position.x, _camera_position.y, -10f);
        }

        // z键重置相机状态
        if (Input.GetKeyDown(KeyCode.Z))
        {
            mainCamera.transform.position = new Vector3(0f, 0f, -10f);
            mainCamera.orthographicSize = 100f;
        }
    }
}
