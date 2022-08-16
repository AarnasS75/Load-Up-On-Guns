using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "RoomNodeGraph", menuName = "Scriptable Objects/RoomNodeGraph")]
public class RoomNodeGraphSO : ScriptableObject
{
    [HideInInspector] public RoomNodeTypeListSO roomNodeTypeListSO;
    [HideInInspector] public List<RoomNodeSO> roomNodeList = new List<RoomNodeSO>();
    [HideInInspector] public Dictionary<string, RoomNodeSO> roomNodeDictionary = new Dictionary<string, RoomNodeSO>();

    private void Awake()
    {
        LoadRoomNodeDictionary();
    }

    private void LoadRoomNodeDictionary()
    {
        roomNodeDictionary.Clear();

        foreach (RoomNodeSO roomNode in roomNodeList)
        {
            roomNodeDictionary[roomNode.id] = roomNode;
        }
    }

    #region Editor code
#if UNITY_EDITOR
    [HideInInspector] public RoomNodeSO roomNodeToDrawFrom = null;
    [HideInInspector] public Vector2 linePosition;

    // Repopulate dictionary everytime a change in the editor has been made
    public void OnValidate()
    {
        LoadRoomNodeDictionary();
    }

    public void SetNodeToDrawConnectionLineFrom(RoomNodeSO node, Vector2 position)
    {
        roomNodeToDrawFrom = node;
        linePosition = position;
    }

#endif
#endregion
}
