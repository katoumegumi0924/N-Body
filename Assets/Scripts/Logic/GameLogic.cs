using UnityEngine;

/// <summary>
/// GameLogic：
/// </summary>
public class GameLogic
{
    private AstroData astroData;
    private AstroPhysics astroPhysics;
    private PlayerController playerController;
    private CameraController cameraController;
    private UniverseGen universeGen;

    private AstroProtoSet protoSet;
    private LineRenderer line;


    public void Init(AstroData _astroData, AstroProtoSet _protoSet)
    {
        if(GameConfig.gameResourcesConfig.dragLinePrefab != null)
        {
            var drawObj = Object.Instantiate(GameConfig.gameResourcesConfig.dragLinePrefab);
            this.line = drawObj.GetComponent<LineRenderer>();
        }

        this.protoSet = _protoSet;

        astroData = _astroData;

        astroPhysics = new AstroPhysics();
        astroPhysics.Init(astroData, protoSet.astroProtoList[0]);

        universeGen = new UniverseGen();
        universeGen.Init(astroData, protoSet);

        playerController = new PlayerController();
        playerController.Init(astroData, protoSet, line, universeGen);

        cameraController = new CameraController();
        cameraController.Init(Camera.main);
    }

    public void Free()
    {
        astroData = null;
        astroPhysics = null;
        playerController = null;
    }

    public void GameTick(float deltaTime)
    {
        astroPhysics.LogicTick(deltaTime, cameraController.screenBounds);
    }

    public void OnUpdate()
    {
        cameraController.OnUpdate();
        playerController.OnUpdate();
    }

}
