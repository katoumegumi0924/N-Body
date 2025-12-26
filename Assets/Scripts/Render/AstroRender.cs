using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UI;

/// <summary>
/// AstroRender：
/// </summary>
public class AstroRender
{
    private AstroData _data;
    private AstroProtoSet _protoSet;

    private Mesh _mesh;
    private Material _material;

    private const int BATCH_SIZE = 1023;
    private Matrix4x4[] _matrices;
    private Vector4[] _colors;
    private MaterialPropertyBlock _mpb;

    private static readonly int BaseColorId = Shader.PropertyToID("_BaseColor");

    public void Init(AstroData data, AstroProtoSet protoSet, Mesh mesh, Material material)
    {
        _data = data;
        _protoSet = protoSet;
        _mesh = mesh;
        _material = material;

        _matrices = new Matrix4x4[BATCH_SIZE];
        _colors = new Vector4[BATCH_SIZE];
        _mpb = new MaterialPropertyBlock();

        _mesh.bounds = new Bounds(Vector3.zero, Vector3.one * 20000f);
    }

    public void Free()
    {
        _data = null;
        _protoSet = null;
        _mesh = null;
        _material = null;
        _matrices = null;
        _colors = null;
        _mpb = null;
    }

    public void RenderTick()
    {
        if (_mesh == null || _material == null)
            return;
        _mpb.Clear();

        var pool = _data.pool;
        int cursor = _data.cursor;
        int batchCount = 0;
        float time = Time.time;

        // 遍历数据进行渲染
        for (int i = 1; i < cursor; ++i)
        {
            if (!pool[i].active)
                continue;
            ref var astro = ref pool[i];
            AstroProto proto = _protoSet.Select(astro.protoId);
            if (proto == null)
                continue;

            // 根据存活时间计算天体颜色
            float age = time - astro.birthTime;
            float t = Mathf.Clamp01(age / proto.evolutionTime);

            // 需要优化 
            _colors[batchCount] = proto.colorRange.Evaluate(t);

            Vector3 pos = new Vector3(astro.position.x, astro.position.y, 0);
            Vector3 scale = Vector3.one * (astro.radius * 2.0f);
            _matrices[batchCount] = Matrix4x4.TRS(pos, Quaternion.identity, scale);

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
        _mpb.SetVectorArray(BaseColorId, _colors);
        Graphics.DrawMeshInstanced(_mesh, 0, _material, _matrices, count, _mpb);
    }
}
