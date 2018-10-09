using System.Collections;
using System.Collections.Generic;
using UnityEngine;
// Touchable component

using UnityEngine;
using UnityEngine.UI;
using UnityEditor;


public class Play : Text
{
    protected override void Awake()
    {
        base.Awake();
    }
}
 
 // Touchable_Editor component, to prevent treating the component as a Text object.
 
 
 
 [CustomEditor(typeof(Play))]
public class Play_Editor : Editor
{
    public override void OnInspectorGUI()
    {
        Debug.Log("xd");
    }
}
