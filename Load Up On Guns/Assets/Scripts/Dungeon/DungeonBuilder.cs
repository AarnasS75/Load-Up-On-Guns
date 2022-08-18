using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System;

[DisallowMultipleComponent]
public class DungeonBuilder : SingletonMonobehaviour<DungeonBuilder>
{
    public Dictionary<string, Room> dungeonBuilderRoomDictionary = new Dictionary<string, Room>();
    private Dictionary<string, RoomTemplateSO> roomTemplateDictionary = new Dictionary<string, RoomTemplateSO>();
    private List<RoomTemplateSO> roomTemplateList = null;
    private RoomNodeTypeListSO roomNodeTypeList;
    private bool dungeonBuildSuccessful;

    protected override void Awake()
    {
        base.Awake();

        LoadRoomNodeTypeList();

        GameResources.Instance.dimmedMaterial.SetFloat("Alpha_Slider", 1f);
    }
    /// <summary>
    /// Load the room node type list
    /// </summary>
    private void LoadRoomNodeTypeList()
    {
        roomNodeTypeList = GameResources.Instance.roomNodeTypeList;
    }

    public bool GenerateDungeon(DungeonLevelSO currentDungeonLevel)
    {
        roomTemplateList = currentDungeonLevel.roomTemplateList;

        LoadRoomTemplatesToDictionary();

        dungeonBuildSuccessful = false;
        int dungeonBuildAttempts = 0;

        while (!dungeonBuildSuccessful && dungeonBuildAttempts < Settings.maxDungeonBuildAttempts)
        {
            dungeonBuildAttempts++;

            RoomNodeGraphSO roomNodeGraph = SelectRandomRoomNodeFromGraph(currentDungeonLevel.roomNodeGraphList);

            int dungeonRebuildAttemptsForNodeGraph = 0;
            dungeonBuildSuccessful = false;

            // Loop until dungeon successfully built or more than max attempts for node graph
            while (!dungeonBuildSuccessful && dungeonRebuildAttemptsForNodeGraph <= Settings.maxDungeonRebuildAttemptsForRoomGraph)
            {
                ClearDungeon();

                dungeonRebuildAttemptsForNodeGraph++;

                dungeonBuildSuccessful = AttemptToBuildRandomDungeon(roomNodeGraph);
            }

            if (dungeonBuildSuccessful)
            {
                InstantiateRoomGameobjects();
            }
        }
        return dungeonBuildSuccessful;
    }

    private void LoadRoomTemplatesToDictionary()
    {
        roomTemplateDictionary.Clear();

        foreach (RoomTemplateSO roomTemplate in roomTemplateList)
        {
            if (!roomTemplateDictionary.ContainsKey(roomTemplate.guid))
            {
                roomTemplateDictionary.Add(roomTemplate.guid, roomTemplate);
            }
            else
            {
                print("Duplicate room template key in " + roomNodeTypeList);
            }
        }
    }

    /// <summary>
    /// Select a random room node graph from the list of room node graphs
    /// </summary>
    private RoomNodeGraphSO SelectRandomRoomNodeFromGraph(List<RoomNodeGraphSO> roomNodeGraphList)
    {
        if (roomNodeGraphList.Count > 0)
        {
            return roomNodeGraphList[UnityEngine.Random.Range(0, roomNodeGraphList.Count)];
        }
        else
        {
            print("No room node graphs in list");
            return null;
        }
    }

    /// <summary>
    /// Get a room template by room template ID
    /// </summary>
    public RoomTemplateSO GetRoomTemplate(string roomTemplateID)
    {
        if(roomTemplateDictionary.TryGetValue(roomTemplateID, out RoomTemplateSO roomTemplate))
        {
            return roomTemplate;
        }
        else
        {
            return null;
        }
    }

    /// <summary>
    /// Get a room by room ID
    /// </summary>
    public Room GetRoomByRoomID(string roomID)
    {
        if (dungeonBuilderRoomDictionary.TryGetValue(roomID, out Room room))
        {
            return room;
        }
        else
        {
            return null;
        }
    }

    /// <summary>
    /// Clear dungeon room gameobjects and dungeon room dictionary
    /// </summary>
    private void ClearDungeon()
    {
        if(dungeonBuilderRoomDictionary.Count > 0)
        {
            foreach (KeyValuePair<string, Room> keyValuePair in dungeonBuilderRoomDictionary)
            {
                Room room = keyValuePair.Value;

                if(room.instantiatedRoom != null)
                {
                    Destroy(room.instantiatedRoom.gameObject);
                }
            }
            dungeonBuilderRoomDictionary.Clear();
        }
    }

    private bool AttemptToBuildRandomDungeon(RoomNodeGraphSO roomNodeGraph)
    {
        // Create open room node queue
        Queue<RoomNodeSO> openRoomNodeQueue = new Queue<RoomNodeSO>();

        // Add entrance node to room node Queue From node graph
        RoomNodeSO entranceNode = roomNodeGraph.GetRoomNode(roomNodeTypeList.list.Find(x => x.isEntrance));

        if(entranceNode != null)
        {
            openRoomNodeQueue.Enqueue(entranceNode);
        }
        else
        {
            print("No entrance node");
            return false;
        }

        // Start with no room overlaps
        bool noRoomOverlaps = true;

        // Process open room nodes queue
        noRoomOverlaps = ProcessRoomsInOpenRoomNodeQueue(roomNodeGraph, openRoomNodeQueue, noRoomOverlaps);

        if(openRoomNodeQueue.Count == 0 && noRoomOverlaps)
        {
            return true;
        }
        else
        {
            return false;
        }
    }


    private void InstantiateRoomGameobjects()
    {
        
    }
    /// <summary>
    /// Process rooms in the open room node queue
    /// </summary>
    private bool ProcessRoomsInOpenRoomNodeQueue(RoomNodeGraphSO roomNodeGraph, Queue<RoomNodeSO> openRoomNodeQueue, bool noRoomOverlaps)
    {
        while(openRoomNodeQueue.Count > 0 && noRoomOverlaps == true)
        {
            // Get next node from queue
            RoomNodeSO roomNode = openRoomNodeQueue.Dequeue();

            // Add child nodes to queue from room node graph (with links to this parent Room)
            foreach (RoomNodeSO childRoomNode in roomNodeGraph.GetChildRoomNodes(roomNode))
            {
                openRoomNodeQueue.Enqueue(childRoomNode);
            }

            // If the room is the entrance, mark as positioned and add to room dictionary
            if (roomNode.roomNodeType.isEntrance)
            {
                RoomTemplateSO roomTemplate = GetRandomRoomTemplate(roomNode.roomNodeType);

                Room room = CreateRoomFromRoomTemplate(roomTemplate, roomNode);

                room.isPositioned = true;

                dungeonBuilderRoomDictionary.Add(room.id, room);
            }
            // If room type is not an entrance
            else
            {
                Room parentRoom = dungeonBuilderRoomDictionary[roomNode.parentRoomNodeIDList[0]];

                noRoomOverlaps = CanPlaceRoomWithNoOverlaps(roomNode, parentRoom);
            }
        }

        return noRoomOverlaps;
    }
    /// <summary>
    /// Attempt to place the room node in the dungeon
    /// </summary>
    private bool CanPlaceRoomWithNoOverlaps(RoomNodeSO roomNode, Room parentRoom)
    {
        // Assuming there are overlaps initially
        bool roomOverlaps = true;

        while (roomOverlaps)
        {
            // Select random unconnected available doorway for parent
            List<Doorway> unconnectedAvailableParentDoorways = GetConnectedAvailableDoorways(parentRoom.doorwayList).ToList();

            if(unconnectedAvailableParentDoorways.Count == 0)
            {
                // If no more doorways to try, then overlap failure
                return false;
            }

            Doorway doorwayParent = unconnectedAvailableParentDoorways[UnityEngine.Random.Range(0, unconnectedAvailableParentDoorways.Count)];

            // Get a random room template for room node, that is consistent with the parent door orientation
            RoomTemplateSO roomTemplate = GetRandomTemplateForRoomConsistentWithParent(roomNode, doorwayParent);

            Room room = CreateRoomFromRoomTemplate(roomTemplate, roomNode);

            // Place the room - returns true, if the room doesn't overlap
            if(PlaceTheRoom(parentRoom, doorwayParent, room))
            {
                roomOverlaps = false;

                room.isPositioned = true;

                dungeonBuilderRoomDictionary.Add(room.id, room);
            }
            else
            {
                roomOverlaps = true;
            }

        }
        return true;
    }

    /// <summary>
    /// Get random room template for room node taking into account the parent doorway orientation
    /// </summary>
    private RoomTemplateSO GetRandomTemplateForRoomConsistentWithParent(RoomNodeSO roomNode, Doorway doorwayParent)
    {
        RoomTemplateSO roomTemplate = null;

        // If room node is a corridor, then select random correct Corridor room template based on
        // parent doorway orientation
        if (roomNode.roomNodeType.isCorridor)
        {
            switch (doorwayParent.orientation)
            {
                case Orientation.north:
                case Orientation.south:
                    roomTemplate = GetRandomRoomTemplate(roomNodeTypeList.list.Find(x => x == x.isCorridorNS));
                    break;
                case Orientation.east:
                case Orientation.west:
                    roomTemplate = GetRandomRoomTemplate(roomNodeTypeList.list.Find(x => x == x.isCorridorEW));
                    break;

                case Orientation.none:
                    break;

                default:
                    break;
            }
        }
        // Select random room emplate
        else
        {
            roomTemplate = GetRandomRoomTemplate(roomNode.roomNodeType);
        }

        return roomTemplate;
    }

    /// <summary>
    /// Get a random room template from roomTemplateList that matches the roomType and return it
    /// </summary>
    private RoomTemplateSO GetRandomRoomTemplate(RoomNodeTypeSO roomNodeType)
    {
        List<RoomTemplateSO> matchingRoomTemplateList = new List<RoomTemplateSO>();

        // Loop throuh room template list
        foreach (RoomTemplateSO roomTemplate in roomTemplateList)
        {
            // Add matching room templates
            if (roomTemplate.roomNodeType == roomNodeType)
            {
                matchingRoomTemplateList.Add(roomTemplate);
            }
        }

        if (matchingRoomTemplateList.Count == 0)
        {
            return null;
        }

        return matchingRoomTemplateList[UnityEngine.Random.Range(0, matchingRoomTemplateList.Count)];
    }
    /// <summary>
    /// Returns true, if the room doesn't overlap
    /// </summary>
    private bool PlaceTheRoom(Room parentRoom, Doorway doorwayParent, Room room)
    {
        // Get current room doorway position
        Doorway doorway = GetOppositeDoorway(doorwayParent, room.doorwayList);

        if(doorway == null)
        {
            doorwayParent.isUnavailable = true;
            return false;
        }

        // Calculate 'world' grid parent doorway position
        Vector2Int parentDoorwayPosition = parentRoom.lowerBounds + doorwayParent.position - parentRoom.templateLowerBounds;

        Vector2Int adjustemt = Vector2Int.zero;

        switch (doorway.orientation)
        {
            case Orientation.north:
                adjustemt = new Vector2Int(0, -1);
                break;
            case Orientation.south:
                adjustemt = new Vector2Int(0, 1);
                break;
            case Orientation.east:
                adjustemt = new Vector2Int(-1, 0);
                break;
            case Orientation.west:
                adjustemt = new Vector2Int(1, 0);
                break;
           case Orientation.none:
                break;
            default:
                break;
        }

        // Calculate room lower and upper bounds, based on positioning to align with parent doorway
        room.lowerBounds = parentDoorwayPosition + adjustemt + room.templateLowerBounds - doorway.position;
        room.upperBounds = room.lowerBounds + room.templateUpperBounds - room.templateLowerBounds;

        Room overlappingRoom = CheckForRoomOverlap(room);

        if(overlappingRoom == null)
        {
            // Mark the doorways as connected and unavailable
            doorwayParent.isConnected = true;
            doorwayParent.isUnavailable = true;

            return true;

        }
        else
        {
            doorwayParent.isUnavailable = true;
            return false;
        }
    }


    /// <summary>
    /// Get the doorway from the doorway list, that has the opposite orientation to doorway
    /// </summary>
    private Doorway GetOppositeDoorway(Doorway doorwayParent, List<Doorway> doorwayList)
    {
        foreach (Doorway doorway in doorwayList)
        {
            if (doorwayParent.orientation == Orientation.east && doorway.orientation == Orientation.west)
            {
                return doorway;
            }
            else if (doorwayParent.orientation == Orientation.south && doorway.orientation == Orientation.north)
            {
                return doorway;
            } 
            else if (doorwayParent.orientation == Orientation.west && doorway.orientation == Orientation.east)
            {
                return doorway;
            } 
            else if (doorwayParent.orientation == Orientation.north && doorway.orientation == Orientation.south)
            {
                return doorway;
            }
        }

        return null;
    }
    /// <summary>
    /// Check for room that overlap the upper and lwer bounds parameters
    /// </summary>
    private Room CheckForRoomOverlap(Room roomToTest)
    {
        foreach (KeyValuePair<string, Room> keyValuePair in dungeonBuilderRoomDictionary)
        {
            Room room = keyValuePair.Value;

            // Skip room if the testing room is the same as room that hasn't been positioned yet
            if (room.id == roomToTest.id || !room.isPositioned)
            {
                continue;
            }

            if(IsOverlappingRoom(roomToTest, room))
            {
                return room;
            }
        }

        return null;
    }
    /// <summary>
    /// Check if two rooms overlap each other
    /// </summary>
    private bool IsOverlappingRoom(Room room1, Room room2)
    {
        bool isOverlappingX = IsOverlappingInterval(room1.lowerBounds.x, room1.upperBounds.x, room2.lowerBounds.x, room2.upperBounds.x);
        bool isOverlappingY = IsOverlappingInterval(room1.lowerBounds.y, room1.upperBounds.y, room2.lowerBounds.y, room2.upperBounds.y);

        if (isOverlappingX && isOverlappingY)
        {
            return true;
        }
        else
        {
            return false;
        }

    }
    /// <summary>
    /// Check if interval 1 overlaps interval 2 (if one room X axis value overlaps other room x value, and same for Y)
    /// </summary>
    private bool IsOverlappingInterval(int imin1, int imax1, int imin2, int imax2)
    {
        if (Mathf.Max(imin1, imin2) <= MathF.Min(imax1, imax2))
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    /// <summary>
    /// Get unconnected doorways
    /// </summary>
    private IEnumerable<Doorway> GetConnectedAvailableDoorways(List<Doorway> doorwayList)
    {
        foreach (Doorway doorway in doorwayList)
        {
            if(!doorway.isConnected || !doorway.isUnavailable)
            {
                yield return doorway;
            }
        }
    }

    /// <summary>
    /// Create a room, based on roomTemplate and layoutNode, and return the created room
    /// </summary>
    private Room CreateRoomFromRoomTemplate(RoomTemplateSO roomTemplate, RoomNodeSO roomNode)
    {
        Room room = new Room();

        room.id = roomNode.id;
        room.roomTemplateID = roomTemplate.guid;
        room.prefab = roomTemplate.prefab;
        room.roomNodeType = roomTemplate.roomNodeType;
        room.lowerBounds = roomTemplate.lowerBounds;
        room.upperBounds = roomTemplate.upperBounds;
        room.spawnPositionArray = roomTemplate.spawnPositionArray;
        room.templateLowerBounds = roomTemplate.lowerBounds;
        room.templateUpperBounds = roomTemplate.upperBounds;

        room.childRoomIDList = CopyStringList(roomNode.childRoomroomNodeIDList);
        room.doorwayList = CopyDoorwayList(roomTemplate.doorwayList);

        if (roomNode.parentRoomNodeIDList.Count == 0) // Entrance
        {
            room.parentRoomID = "";
            room.isPreviouslyVisited = true;
        }
        else
        {
            room.parentRoomID = roomNode.parentRoomNodeIDList[0];
        }

        return room;
    }

    private List<Doorway> CopyDoorwayList(List<Doorway> oldDoorwayList)
    {
        List<Doorway> newDoorwayList = new List<Doorway>();

        foreach (Doorway doorway in oldDoorwayList)
        {
            Doorway newDoorway = new Doorway();

            newDoorway.position = doorway.position;
            newDoorway.orientation = doorway.orientation;
            newDoorway.doorPrefab = doorway.doorPrefab;
            newDoorway.isConnected = doorway.isConnected;
            newDoorway.isUnavailable = doorway.isUnavailable;
            newDoorway.doorwayStartCopyPosition = doorway.doorwayStartCopyPosition;
            newDoorway.doorwayCopyTileHeight = doorway.doorwayCopyTileHeight;
            newDoorway.doorwayCopyTileWidth = doorway.doorwayCopyTileWidth;

            newDoorwayList.Add(doorway);
        }
        return newDoorwayList;
    }

    /// <summary>
    /// Create deep copy of string list
    /// </summary>
    private List<string> CopyStringList(List<string> oldStringList)
    {
        List<string> newStringList = new List<string>();

        foreach (string stringValue in oldStringList)
        {
            newStringList.Add(stringValue);
        }
        return newStringList;
    }
}
