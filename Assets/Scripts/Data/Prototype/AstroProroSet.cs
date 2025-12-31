using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

/// <summary>
/// AstroProroSet：
/// </summary>
[CreateAssetMenu(fileName = "AstroProtoSet", menuName = "NBody/AstroProtoSet")]
public class AstroProtoSet : ScriptableObject
{
    // 所有的天体配置原型
    [SerializeField]
    private AstroProto[] astroProtos;

    private Dictionary<int, AstroProto> astroProtoIdMap;

    private void OnEnable()
    {
        InitMap();
    }

    private void OnValidate()
    {
        InitMap();
    }

    public int Count => astroProtos.Length;

    private void InitMap()
    {
        astroProtoIdMap = new Dictionary<int, AstroProto>();

        if (astroProtos == null) 
            return;

        foreach (var proto in astroProtos)
        {
            if (proto != null && !astroProtoIdMap.ContainsKey(proto.id))
            {
                astroProtoIdMap.Add(proto.id, proto);
            }
        }
    }

    // 根据id查询对应天体原型
    public AstroProto Select(int id)
    {
        if (astroProtoIdMap.TryGetValue(id, out var proto))
            return proto;

        return null;
    }

    // 索引器，通过AstroProtoSet[i]来访问对应原型
    public AstroProto this[int index]
    {
        get
        {
            return astroProtos[index];
        }
    }
}
