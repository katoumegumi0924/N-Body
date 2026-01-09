using UnityEngine;

/// <summary>
/// DebugSimulationSpeed：
/// </summary>
public class EvolutionSpeed : MonoBehaviour
{
    public GameMain gameMain;

    [Range(0, 8)]
    public int scaleIndex = 3;
    public float currentSpeed;

    private void OnValidate()
    {
        if (gameMain != null && gameMain.gameData != null)
        {
            gameMain.gameData.universeTime._timeScale = SimulationTimeData.scaleSteps[scaleIndex];
            currentSpeed = SimulationTimeData.scaleSteps[scaleIndex];
        }
    }
}
