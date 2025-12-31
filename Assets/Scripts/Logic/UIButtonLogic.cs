using TMPro;
using UnityEngine;

/// <summary>
/// UIButtonLogic：
/// </summary>
public class UIButtonLogic : MonoBehaviour
{
    [SerializeField]
    private GameMain gameMain;
    private GameData gameData;
    public TextMeshProUGUI speedText;
    private float currentSpeed;

    private void Start()
    {
        if (gameMain != null)
        {
            gameData = gameMain.gameData;
        }
    }

    // 使用OnEnable无法加载到gameMain
    //private void OnEnable()
    //{
    //    if (gameMain != null)
    //    {
    //        gameData = gameMain.gameData;
    //    }
    //}

    public void OnClick_SpeedUp()
    {
        if (gameData != null)
        {
            gameData.clock.SpeedUp();
            currentSpeed = gameData.clock.GetStepValue();
            speedText.text = currentSpeed.ToString() + "X";
        }
    }

    public void OnClick_SpeedDown()
    {
        if (gameData != null)
        {
            gameData.clock.SpeedDown();
            currentSpeed = gameData.clock.GetStepValue();
            speedText.text = currentSpeed.ToString() + "X";
        }
    }

    public void OnClick_TogglePause()
    {
        if (gameData != null)
        {
            gameData.clock.TogglePause();
            if (gameData.clock.timeScale > 0)
            {
                currentSpeed = gameData.clock.GetStepValue();
                speedText.text = currentSpeed.ToString() + "X";
            }
            else
            {
                speedText.text = "Pause";
            }
        }
    }
}
