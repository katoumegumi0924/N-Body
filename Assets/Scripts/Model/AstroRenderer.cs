using UnityEngine;

/// <summary>
/// AstroRender：
/// </summary>
public class AstroRenderer
{
    private AstroProtoSet protoSet;
    private Mesh mesh;
    private Material material;

    private const int BATCH_SIZE = 1023;
    private Matrix4x4[] matrices;
    private Vector4[] colors;
    private MaterialPropertyBlock mpb;

    private static readonly int BaseColorId = Shader.PropertyToID("_BaseColor");

    public void Init()
    {
        protoSet = ProtoDB.protoSet;
        mesh =  Mesh.Instantiate(GameConfig.gameResourcesConfig.astroMesh);
        material = Material.Instantiate(GameConfig.gameResourcesConfig.astroMaterial);

        matrices = new Matrix4x4[BATCH_SIZE];
        colors = new Vector4[BATCH_SIZE];
        mpb = new MaterialPropertyBlock();
    }

    public void Free()
    {
        protoSet = null;
        matrices = null;
        colors = null;

        if (mesh != null)
        {
            Mesh.Destroy(mesh);
        }

        if (material != null)
        {
            Material.Destroy(material);
        }

        if (mpb != null)
        {
            mpb.Clear();
            mpb = null;
        }      
    }

    public void RenderTick(GameData data)
    {
        if (mesh == null || material == null)
            return;
        mpb.Clear();

        var pool = data.universeData.pool;
        int cursor = pool.cursor;
        int batchCount = 0;
        long currentTick = data.universeTimeData.tickCounter;

        // 遍历数据进行渲染
        for (int i = 1; i < cursor; ++i)
        {
            if (pool[i].id != i)
                continue;
            ref var astro = ref pool[i];
            AstroProto proto = protoSet.Select(astro.protoId);
            if (proto == null)
                continue;

            // 根据存活时间计算天体颜色
            long ageTick = currentTick - astro.birthTick;
            float age = data.universeTimeData.GetSeconds(ageTick);
            float t = Mathf.Clamp01(age / proto.evolutionTime);

            colors[batchCount] = proto.colorRange.Evaluate(t);

            Vector3 pos = (Vector3)astro.position;
            Vector3 scale = Vector3.one * (astro.radius * 2.0f);
            matrices[batchCount] = Matrix4x4.TRS(pos, Quaternion.identity, scale);

            batchCount++;

            if (batchCount >= BATCH_SIZE)
            {
                FlushBatch(batchCount);
                batchCount = 0;
            }
        }
        // 渲染剩余批次
        if (batchCount > 0)
        {
            FlushBatch(batchCount);
        }
    }

    private void FlushBatch(int count)
    {
        if (count <= 0)
            return;
        mpb.SetVectorArray(BaseColorId, colors);
        Graphics.DrawMeshInstanced(mesh, 0, material, matrices, count, mpb);
    }
}