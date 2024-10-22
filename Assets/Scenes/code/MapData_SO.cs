using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "MapData_SO", menuName = "Map/MapData")]
public class MapData_SO : ScriptableObject
{
    public List<MapData> mapDataList;

    public MapData getMapData(string name)
    {
        return mapDataList.Find(m => m.childSceneName == name);
    }
}

[System.Serializable]
public class MapData
{
    [Header("地图信息")]
    public string childSceneName;

    public int gridWidth;
    public int gridHeight;

    [Header("左上角原点")]
    public int originX;

    public int originY;
}