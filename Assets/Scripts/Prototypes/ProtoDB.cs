using UnityEngine;

/// <summary>
/// ProtoDB：
/// </summary>
public static class ProtoDB
{
    private static AstroProtoSet _astroProtoSet;

    public static AstroProtoSet protoSet { get { return _astroProtoSet; } }

    public static void LoadProtoSet()
    {
        if (_astroProtoSet == null)
        {
            _astroProtoSet = Resources.Load<AstroProtoSet>("Prototypes/AstroProtoSet");

            if (_astroProtoSet == null)
            {
                Debug.LogError("错误：在 Resources 文件夹下找不到名为 'AstroProtoSet' 的配置文件！");
                return;
            }
        }
    }   
}
