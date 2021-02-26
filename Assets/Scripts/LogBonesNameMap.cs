using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LogBonesNameMap : HimeLib.SingletonMono<LogBonesNameMap>
{
    public AvatarController source;

    public List<MapBone> MapBones;
    
    [EasyButtons.Button]
    void GetMapStrings(){
        MapBones = new List<MapBone>();

        for (int i = 0; i < source.bones.Length; i++)
        {
            if(source.bones[i] == null)
                continue;

            MapBone temp = new MapBone(){
                index = i,
                boneName = source.bones[i].name
            };
            MapBones.Add(temp);
        }
    }

    [System.Serializable]
    public class MapBone
    {
        public int index;
        public string boneName;
    }
}
