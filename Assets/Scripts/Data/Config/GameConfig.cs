using UnityEngine;

/// <summary>
/// GameConfig：
/// </summary>
public static class GameConfig
{
    private static UniverseConfig _universeConfig;
    private static GameResourcesConfig _gameResourcesConfig;

    public static UniverseConfig universeConfig
    {
        get
        {
            if (_universeConfig == null)
            {
                _universeConfig = Resources.Load<UniverseConfig>("Config/UniverseConfig");

                if (_universeConfig == null)
                {
                    Debug.LogError("错误：在 Resources 文件夹下找不到名为 'UniverseConfig' 的配置文件！");
                }
            }
            return _universeConfig;
        }
    }

    public static GameResourcesConfig gameResourcesConfig
    {
        get
        {
            if (_gameResourcesConfig == null)
            {
                _gameResourcesConfig = Resources.Load<GameResourcesConfig>("Config/GameResourcesConfig");

                if (_gameResourcesConfig == null)
                {
                    Debug.LogError("错误：在 Resources 文件夹下找不到名为 'GameResourcesConfig' 的配置文件！");
                }
            }
            return _gameResourcesConfig;
        }
    } 
}
