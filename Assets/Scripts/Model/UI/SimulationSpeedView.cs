using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// SimulationSpeedView：
/// </summary>
public class SimulationSpeedView : MonoBehaviour
{
    [Header("Buttons")]
    [SerializeField]
    private Button speedUpButton;
    [SerializeField]
    private Button speedDownButton;
    [SerializeField]
    private Button pauseButton;
    [SerializeField]
    private TextMeshProUGUI speedText;

    public void Init(GameData gameData)
    {
        speedUpButton.onClick.AddListener(() => gameData.interactionData.reqSpeedUp = true);
        speedDownButton.onClick.AddListener(() => gameData.interactionData.reqSpeedDown = true);
        pauseButton.onClick.AddListener(() => gameData.interactionData.reqTogglePause = true);
    }

    public void Free()
    {
        speedUpButton.onClick.RemoveAllListeners();
        speedDownButton.onClick.RemoveAllListeners();
        pauseButton.onClick.RemoveAllListeners();
    }

    public void RefreshSpeedText(float currentScale)
    {
        if (currentScale <= 0)
        {
            speedText.text = "Pause";
        }
        else
        {
            speedText.text = $"{currentScale}x";
        }
    }
}