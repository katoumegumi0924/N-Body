using UnityEngine;

/// <summary>
/// GameMain：
/// </summary>
public class GameMain : MonoBehaviour
{
    private GameData gameData;
    private GameLogic gameLogic;
    private GameModel gameModel;

    public void Init()
    {
        // 允许程序在后台运行，避免失去焦点时停止
        Application.runInBackground = true;

        gameData = new GameData();
        gameData.Init();

        gameLogic = new GameLogic();
        gameLogic.Init(gameData);

        gameModel = new GameModel();
        gameModel.Init(gameData);
    }

    public void Free()
    {
        if (gameModel != null)
        {
            gameModel.Free();
            gameModel = null;
        }

        if (gameLogic != null)
        {
            gameLogic.Free();
            gameLogic = null;
        }

        if (gameData != null)
        {
            gameData.Free();
            gameData = null;
        }
    }

    private void Update()
    {
        gameLogic.OnUpdate();
        gameModel.OnUpdate();
    }

    private void FixedUpdate()
    {
        gameLogic.GameTick(Time.fixedDeltaTime);
    }

    private void OnEnable()
    {
        Init();
    }

    private void OnDisable()
    {
        Free();
    }
}