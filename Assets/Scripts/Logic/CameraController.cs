using UnityEngine;
using UnityEngine.Rendering.Universal;

/// <summary>
/// CameraController：
/// </summary>
public class CameraController
{
    private const float ZOOM_SPEED = 0.5f;
    private const float MOVE_SPEED = 2.0f;

    public void Init(GameData gameData)
    {
        Camera mainCam = Camera.main;

        // 根据屏幕初始化边界
        float h = mainCam.orthographicSize * 2f;
        float w = h * mainCam.aspect;
        gameData.universeData.worldBounds = new WorldBounds { width = w, height = h };

        gameData.cameraState.zoom = mainCam.orthographicSize;
        gameData.cameraState.position = Vector2.zero;
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
        ref var state = ref gameData.cameraState;

        // 处理缩放
        float scroll = Input.GetAxis("Mouse ScrollWheel");
        if (System.Math.Abs(scroll) > 0.01f)
        {
            state.zoom -= scroll * state.zoom * ZOOM_SPEED;
            state.zoom = System.Math.Clamp(state.zoom, 0.1f, 1000f);
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
            float speed = state.zoom * MOVE_SPEED;
            state.position += moveDir * (speed * deltaTime);
        }

        // z键重置相机状态
        if (Input.GetKeyDown(KeyCode.Z))
        {
            state.Reset();
        }
    }
}
