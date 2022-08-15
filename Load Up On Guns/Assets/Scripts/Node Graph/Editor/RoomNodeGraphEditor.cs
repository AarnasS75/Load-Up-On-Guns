using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditor.Callbacks;
using UnityEditor.MPE;
using System;

public class RoomNodeGraphEditor : EditorWindow
{
    private GUIStyle roomNodeStyle;
    private static RoomNodeGraphSO currentRoomNodeGraph;
    private RoomNodeTypeListSO roomNodeTypeList;


    private const float nodeWidth = 160f;
    private const float nodeHeight = 75f;
    private const int nodePadding = 25;
    private const int nodeBorder = 12;

    [MenuItem("Room Node Graph Editor", menuItem = "Window/Dungeon Editor/Room Node Graph Editor")]

    private void OnEnable()
    {
        roomNodeStyle = new GUIStyle();
        roomNodeStyle.normal.background = EditorGUIUtility.Load("node1") as Texture2D;
        roomNodeStyle.normal.textColor = Color.white;
        roomNodeStyle.padding = new RectOffset(nodePadding, nodePadding, nodePadding, nodePadding);
        roomNodeStyle.border = new RectOffset(nodeBorder, nodeBorder, nodeBorder, nodeBorder);

        roomNodeTypeList = GameResources.Instance.roomNodeTypeList;
    }

    private static void OpenWindow()
    {
        GetWindow<RoomNodeGraphEditor>("Room Node Graph Editor");
    }

    /// <summary>
    ///  Open the room node graph editor window if a room node graph scriptable object asset is double clicked in the inspector
    /// </summary>
    [OnOpenAsset(0)]
    public static bool OnDobleClickAsset(int instanceID, int line)
    {
        RoomNodeGraphSO roomNodeGraph = EditorUtility.InstanceIDToObject(instanceID) as RoomNodeGraphSO;

        if(roomNodeGraph != null)
        {
            OpenWindow();

            currentRoomNodeGraph = roomNodeGraph;

            return true;
        }
        return false;
    }

    /// <summary>
    /// Draw Editor GUI
    /// </summary>
    private void OnGUI()
    {
        // If a scriptable object of type
        if(currentRoomNodeGraph != null)
        {
            ProcessEvent(Event.current);

            // Draw room nodes
            DrawRoomNodes();
        }

        if (GUI.changed)
        {
            Repaint();
        }
    }

    /// <summary>
    /// Draw nodes in the graph window
    /// </summary>
    private void DrawRoomNodes()
    {
        // Loop through all nodes and draw them
        foreach (RoomNodeSO roomNode in currentRoomNodeGraph.roomNodeList)
        {
            roomNode.Draw(roomNodeStyle);
        }

        GUI.changed = true;
    }

    private void ProcessEvent(Event currentEvent)
    {
        ProcessRoomNodeGraphEvets(currentEvent);
    }
    /// <summary>
    /// Process Room Node Graph Events
    /// </summary>
    private void ProcessRoomNodeGraphEvets(Event currentEvent)
    {
        switch (currentEvent.type)
        {
            case EventType.MouseDown:
                ProcessMouseDownEvent(currentEvent);
                break;
            default:
                break;
        }
    }
    /// <summary>
    /// Process mouse down events on the room node graph (not over a node)
    /// </summary>
    /// <param name="currentEvent"></param>
    private void ProcessMouseDownEvent(Event currentEvent)
    {
        // Right click
        if(currentEvent.button == 1)
        {
            ShowContextMenu(currentEvent.mousePosition);
        }
    }

    private void ShowContextMenu(Vector2 mousePosition)
    {
        GenericMenu menu = new GenericMenu();
        menu.AddItem(new GUIContent("Create Room Node"), false, CreateRoomNode, mousePosition);
        
        menu.ShowAsContext();
    }
    /// <summary>
    /// Create node at mouse position
    /// </summary>
    private void CreateRoomNode(object mousePositionObject)
    {
        CreateRoomNode(mousePositionObject, roomNodeTypeList.list.Find(x => x.isNone));   
    }
    /// <summary>
    /// Create a room node at the mouse position - overloaded to also pass in RoomNodeType
    /// </summary>
    private void CreateRoomNode(object mousePositionObject, RoomNodeTypeSO roomNodeType)
    {
        Vector2 mousePosition = (Vector2)mousePositionObject;

        // Create room node scriptable object asset
        RoomNodeSO roomNode = ScriptableObject.CreateInstance<RoomNodeSO>();

        // Add room node to current room node graph room node list
        currentRoomNodeGraph.roomNodeList.Add(roomNode);

        // Set room node values
        roomNode.Initialize(new Rect(mousePosition, new Vector2(nodeWidth, nodeHeight)), currentRoomNodeGraph, roomNodeType);

        // Add room node to room node graph scriptable object asset database
        AssetDatabase.AddObjectToAsset(roomNode, currentRoomNodeGraph);

        AssetDatabase.SaveAssets();
    }
}
