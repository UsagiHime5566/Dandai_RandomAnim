using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OverrideAvatarController : AvatarController
{
    protected override void MapBones()
	{
        foreach (var item in LogBonesNameMap.instance.MapBones)
        {
            Transform obj = gameObject.transform.FindDeepChildCustom(item.boneName);

            Debug.Log($"try find {item.boneName} , result:{obj}");

            if(obj != null){
                bones[item.index] = obj;
            }
        }
    }

    [EasyButtons.Button]
    void SopAnimation(){
        var anim = GetComponent<Animator>();
    }
}
