using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class ResRefLoadTest : MonoBehaviour
{
    public ResourceReference reference;
	// Use this for initialization
	void Start ()
	{
	    string path = AssetDatabase.GetAssetPath(reference.Object);
	    int j = 1;
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
