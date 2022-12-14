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
    /// <summary>
    /// Get Room Node By roomNodeType
    /// </summary>
    public RoomNodeSO GetRoomNode(RoomNodeTypeSO roomNodeType)
    {
        foreach (RoomNodeSO node in roomNodeList)
        {
            if (node.roomNodeType == roomNodeType)
            {
                return node;
            }
        }
        return null;
    }
    /// <summary>
    /// Get room node by room nodeID
    /// </summary>
    public RoomNodeSO GetRoomNode(string nodeID)
    {
        if (roomNodeDictionary.TryGetValue(nodeID, out RoomNodeSO roomNode))
        {
            return roomNode;
        }
        return null;
    }
    /// <summary>
    /// Get child room nodes for supplied parent room node
    /// </summary>
    public IEnumerable<RoomNodeSO> GetChildRoomNodes(RoomNodeSO parentRoomNode)
    {
        foreach (string childNodeID in parentRoomNode.childRoomroomNodeIDList)
        {
            yield return GetRoomNode(childNodeID);
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
