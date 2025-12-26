using UnityEngine;

/// <summary>
/// GameData：
/// </summary>
public class GameData
{
    public AstroData astroData;

    public void Init()
    {
        astroData = new AstroData();
        astroData.Init();
    }

    public void Free()
    {
        if (astroData != null)
        {
            astroData.Free();
            astroData = null;
        }
    }

    public void AddAstro(AstroProto proto, Vector2 pos, Vector2 vel, float currentTime, float massOverride = -1)
    {
        astroData.CreateAstro(proto, pos, vel, currentTime, massOverride);
    }

    public void RemoveAstro(int id)
    {
        astroData.FreeAstro(id);
    }
}
