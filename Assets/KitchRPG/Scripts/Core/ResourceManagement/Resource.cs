using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Object = UnityEngine.Object;

[Serializable]
public class ResourceReference
{
    public Object Object;
    public string Path;

}

[System.Serializable]
public class ResourceReferenceList : ReorderableArray<ResourceReference>
{

}

[CreateAssetMenu(fileName = "New Resource Bundle", menuName = "Resource Management/Resource Bundle")]
public class ResourceBundle : ScriptableObject
{
    [Reorderable]
    public ResourceReferenceList list;
}