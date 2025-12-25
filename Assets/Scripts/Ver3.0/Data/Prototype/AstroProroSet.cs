using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// AstroProroSet：
/// </summary>
[CreateAssetMenu(fileName = "AstroProtoSet", menuName = "NBody/AstroProtoSet")]
public class AstroProtoSet : ScriptableObject, ISerializationCallbackReceiver
{
    // 所有的天体配置原型
    public List<AstroProto> astroProtoList;

    private Dictionary<int, AstroProto> astroProtoIdMap;

    public void OnBeforeSerialize() { }

    public void OnAfterDeserialize()
    {
        astroProtoIdMap = new Dictionary<int, AstroProto>();
        for (int i = 0; i < astroProtoList.Count; ++i)
        {
            if (!astroProtoIdMap.ContainsKey(astroProtoList[i].id))
                astroProtoIdMap[astroProtoList[i].id] = astroProtoList[i];
        }
    }

    // 根据id查询对应天体原型
    public AstroProto Select(int id)
    {
        if (astroProtoIdMap == null)
            return null;

        if (astroProtoIdMap.TryGetValue(id, out var proto))
        {
            return proto;
        }

        return null;
    }
}
