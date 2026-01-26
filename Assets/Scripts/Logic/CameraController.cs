using UnityEngine;

/// <summary>
/// CameraController：
/// </summary>
public class CameraController
{
    private Vector2 _camera_position;
    private float _camera_zoom;

    private const float ZOOM_SPEED = 0.5f;
    private const float MOVE_SPEED = 2.0f;
    private const float MAX_ZOOM = 1000f;
    private const float MIN_ZOOM = 1.0f;

    public Camera mainCamera;

    public void Init(GameData gameData)
    {
        mainCamera = Camera.main;

        _camera_position = Vector2.zero;
        _camera_zoom = mainCamera.orthographicSize; 
    }

    public void Free()
    {
        mainCamera = null;
    }

    public void OnUpdate(GameData gameData)
    {
        UpdateCamera(gameData);
    }

    public void UpdateCamera(GameData gameData)
    {
        // 处理缩放
        float scroll = Input.GetAxis("Mouse ScrollWheel");
        if (System.Math.Abs(scroll) > 0.01f)
        {
            _camera_zoom -= scroll * _camera_zoom * ZOOM_SPEED;
            _camera_zoom = Mathf.Clamp(_camera_zoom, MIN_ZOOM, MAX_ZOOM);

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
            _camera_position += moveDir * (speed * Time.deltaTime);

            mainCamera.transform.position = new Vector3(_camera_position.x, _camera_position.y, -10f);
        }

        // z键重置相机状态
        if (Input.GetKeyDown(KeyCode.Z))
        {
            _camera_position = GameConfig.universeConfig.centerPos;
            _camera_zoom = GameConfig.universeConfig.height * 0.5f;
            mainCamera.transform.position = new Vector3(_camera_position.x, _camera_position.y, -10f);
            mainCamera.orthographicSize = _camera_zoom;
        }
    }
}
