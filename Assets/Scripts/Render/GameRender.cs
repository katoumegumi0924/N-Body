using UnityEngine;

/// <summary>
/// GameRender：
/// </summary>
public class GameRender
{
    private AstroRender astroRender;
    private AstroData astroData;
    public void Init(AstroData _astroData, AstroProtoSet _protoSet, Mesh _mesh, Material _material)
    {
        astroData = _astroData;
        astroRender = new AstroRender();
        astroRender.Init(astroData, _protoSet, _mesh, _material);
    }

    public void Free()
    {
        if (astroData != null)
        {
            for (int i = 0; i < astroData.cursor; ++i)
            {
                astroData.pool[i].Clear();
            }
        }

        if (astroRender != null)
        {
            astroRender.Free();
            astroRender = null;
        }
    }

    public void OnUpdate()
    {
        astroRender.RenderTick();
    }
}
