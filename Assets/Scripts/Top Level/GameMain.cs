using UnityEngine;

/// <summary>
/// GameMain：
/// </summary>
public class GameMain : MonoBehaviour
{
    public GameData gameData;
    public GameLogic gameLogic;
    public GameModel gameModel;

    public void Init()
    {
        ProtoDB.LoadProtoSet();

        gameData = new GameData();
        gameData.Init();
        gameData.SetNew();

        gameLogic = new GameLogic();
        gameLogic.Init(gameData);
        gameLogic.SetNew();

        gameModel = new GameModel();
        gameModel.Init(gameData, gameLogic);
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
        gameLogic.GameTick();
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