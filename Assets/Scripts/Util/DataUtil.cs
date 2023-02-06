using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public static class DataUtil
{
    public static Transform RecursiveFind(Transform parent, string childName) {
        foreach (Transform child in parent)
        {
            if(child.name == childName)
            {
                return child;
            }
            else
            {
                Transform found = RecursiveFind(child, childName);
                if (found != null)
                {
                    return found;
                }
            }
        }
        return null;
    }
}