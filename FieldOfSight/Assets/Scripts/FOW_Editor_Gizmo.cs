using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;


[CustomEditor(typeof(FOW_Enemy))]
public class FOW_Editor : Editor
{
    private void OnSceneGUI()
    {
        ShowVisionCircle();
    }
    void ShowVisionCircle()
    {
        FOW_Enemy fow = (FOW_Enemy)target;
        Handles.color = Color.white;
        Handles.DrawWireArc(fow.transform.position, Vector3.up, Vector3.forward, 360, fow.ViewRadius);//show the how far the target can observe

        Vector3 firstDir = fow.AngleDir(fow.ViewAngle / 2, false);
        Vector3 secondDir = fow.AngleDir(-fow.ViewAngle / 2, false);

        Handles.DrawLine(fow.transform.position, fow.transform.position + firstDir * fow.ViewRadius);
        Handles.DrawLine(fow.transform.position, fow.transform.position + secondDir * fow.ViewRadius);//show the range of sight by using Angle dir

        Handles.color = Color.red;//show all detected targets
        foreach (Transform transform in fow.VisibleObjects)
        {
            Handles.DrawLine(fow.transform.position, transform.position);//Show the detected Target 
        }

    }
}
