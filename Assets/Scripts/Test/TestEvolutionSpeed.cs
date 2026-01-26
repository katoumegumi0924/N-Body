using UnityEngine;

/// <summary>
/// TestEvolutionSpeed：
/// </summary>
public class TestEvolutionSpeed : MonoBehaviour
{
    public GameMain gameMain;

    [Range(0, 5)]
    public int scaleIndex = 3;
    public float currentTickDelta;

    public bool pauseToggle = false;

    private void OnEnable()
    {
        if (gameMain != null && gameMain.gameData != null)
        {
            if (pauseToggle)
            {
                gameMain.gameData.universeTimeData.TogglePause();
                pauseToggle = false;
            }


            gameMain.gameData.universeTimeData.SetSpeed(scaleIndex);
            currentTickDelta = gameMain.gameData.universeTimeData.tickDelta;
        }
    }

    private void OnValidate()
    {
        if (gameMain != null && gameMain.gameData != null)
        {
            if (pauseToggle)
            {
                gameMain.gameData.universeTimeData.TogglePause();
                pauseToggle = false;
            }
            

            gameMain.gameData.universeTimeData.SetSpeed(scaleIndex);
            currentTickDelta = gameMain.gameData.universeTimeData.tickDelta;
        }
    }
}
