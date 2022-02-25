using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class WaypointManagerWindow : EditorWindow
{
    [MenuItem("Tools/Waypoint Editor")]
    public static void Open() 
    {
        GetWindow<WaypointManagerWindow>();
    }

    public Transform WaypointRoot;

    private void OnGUI()
    {
        SerializedObject obj = new SerializedObject(this);

        EditorGUILayout.PropertyField(obj.FindProperty("WaypointRoot"));

        if (WaypointRoot == null)
        {
            EditorGUILayout.HelpBox("Root transform must be selected. Please assign a root transform.", MessageType.Warning);
        }
        else 
        {
            EditorGUILayout.BeginVertical("box");
            DrawButtons();
            EditorGUILayout.EndVertical();
        }

        obj.ApplyModifiedProperties();
    }

    void DrawButtons() 
    {
        if (GUILayout.Button("Create Waypoint"))
        {
            CreateWaypoint();
        }
        if (Selection.activeGameObject != null && Selection.activeGameObject.GetComponent<Waypoint>())
        {
            if (GUILayout.Button("Add Branch waypoint"))
            {
                CreateBranch();
            }
            if (GUILayout.Button("Create Waypoint Before"))
            {
                CreateWaypointBefore();
            }
            if (GUILayout.Button("Create Waypoint After"))
            {
                CreateWaypointAfter();
            }
            if (GUILayout.Button("Remove Waypoint"))
            {
                RemoveWaypoint();
            }
        }

    }

    void CreateWaypoint()
    {
        GameObject WaypointObject = new GameObject("Waypoint" + WaypointRoot.childCount, typeof(Waypoint));
        WaypointObject.transform.SetParent(WaypointRoot, false);

        Waypoint waypoint = WaypointObject.GetComponent<Waypoint>();
        if (WaypointRoot.childCount > 1)
        {
            waypoint.PreviousWaypoint = WaypointRoot.GetChild(WaypointRoot.childCount - 2).GetComponent<Waypoint>();
            waypoint.PreviousWaypoint.NextWaypoint = waypoint;
            //place the way point at the last position
            waypoint.transform.position = waypoint.PreviousWaypoint.transform.position;
            waypoint.transform.forward = waypoint.PreviousWaypoint.transform.forward;
        }

        Selection.activeGameObject = waypoint.gameObject;
    }

    void CreateWaypointBefore() 
    {
        GameObject waypointObject = new GameObject("Waypoint " + WaypointRoot.childCount, typeof(Waypoint));
        waypointObject.transform.SetParent(WaypointRoot, false);

        Waypoint newWaypoint = waypointObject.GetComponent<Waypoint>();

        Waypoint selectedWaypoint = Selection.activeGameObject.GetComponent<Waypoint>();

        waypointObject.transform.position = selectedWaypoint.transform.position;
        waypointObject.transform.forward = selectedWaypoint.transform.forward;

        if (selectedWaypoint.PreviousWaypoint != null)
        {
            newWaypoint.PreviousWaypoint = selectedWaypoint.PreviousWaypoint;
            selectedWaypoint.PreviousWaypoint.NextWaypoint = newWaypoint;
        }

        newWaypoint.NextWaypoint = selectedWaypoint;

        selectedWaypoint.PreviousWaypoint = newWaypoint;

        newWaypoint.transform.SetSiblingIndex(selectedWaypoint.transform.GetSiblingIndex());

        Selection.activeGameObject = newWaypoint.gameObject;
    }

    void CreateWaypointAfter()
    {
        GameObject waypointObject = new GameObject("Waypoint " + WaypointRoot.childCount, typeof(Waypoint));
        waypointObject.transform.SetParent(WaypointRoot, false);

        Waypoint newWaypoint = waypointObject.GetComponent<Waypoint>();

        Waypoint selectedWaypoint = Selection.activeGameObject.GetComponent<Waypoint>();

        waypointObject.transform.position = selectedWaypoint.transform.position;
        waypointObject.transform.forward = selectedWaypoint.transform.forward;

        newWaypoint.PreviousWaypoint = selectedWaypoint;

        if (selectedWaypoint.NextWaypoint != null)
        {
            selectedWaypoint.NextWaypoint.PreviousWaypoint = newWaypoint;
            newWaypoint.NextWaypoint = selectedWaypoint.NextWaypoint;
        }

        selectedWaypoint.NextWaypoint = newWaypoint;

        newWaypoint.transform.SetSiblingIndex(selectedWaypoint.transform.GetSiblingIndex());

        Selection.activeGameObject = newWaypoint.gameObject;
    }
    void RemoveWaypoint()
    {
        Waypoint selectedWaypoint = Selection.activeGameObject.GetComponent<Waypoint>();

        if (selectedWaypoint.NextWaypoint != null)
        {
            selectedWaypoint.NextWaypoint.PreviousWaypoint = selectedWaypoint.PreviousWaypoint;
        }

        if (selectedWaypoint.PreviousWaypoint != null)
        {
            selectedWaypoint.PreviousWaypoint.NextWaypoint = selectedWaypoint.NextWaypoint;
            Selection.activeGameObject = selectedWaypoint.PreviousWaypoint.gameObject;
        }

        DestroyImmediate(selectedWaypoint.gameObject);
    }

    void CreateBranch()
    {
        GameObject waypointObject = new GameObject("Waypoint " + WaypointRoot.childCount, typeof(Waypoint));
        waypointObject.transform.SetParent(WaypointRoot, false);

        Waypoint waypoint = waypointObject.GetComponent<Waypoint>();

        Waypoint branchedForm = Selection.activeGameObject.GetComponent<Waypoint>();
        branchedForm.branches.Add(waypoint);

        waypoint.transform.position = branchedForm.transform.position;
        waypoint.transform.forward = branchedForm.transform.forward;

        Selection.activeGameObject = waypoint.gameObject;
    }
}
