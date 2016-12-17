using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelGenerator : MonoBehaviour
{
    public int rooms;
    public List<GameObject> roomTypes = new List<GameObject>();
    List<RoomObject.Exit> spawnedRoomsExits = new List<RoomObject.Exit>();
    List<Vector3> occupiedTiles = new List<Vector3>();
    GameObject roomToSpawn = null;
    RoomObject lastSpawnedRoom = null;
    bool rerollRoom = false;
    int exitToRemove;
    int infinitLoopBreak = 0;
    public GameObject bgTile;

    void Start()
    {
        GenerateLevels(rooms);
    }

    void GenerateLevels(int rooms)
    {       
        for (int i = 0; i < rooms; i++)
        {
            Vector3 posToSpawnRoom = Vector3.zero;
            Vector3 spawnedRoomOffset = Vector3.zero;
            infinitLoopBreak = 0;

            do
            {
                roomToSpawn = roomTypes[Random.Range(0, roomTypes.Count)];
                lastSpawnedRoom = Instantiate(roomToSpawn).GetComponent<RoomObject>();
                lastSpawnedRoom.name = "Room " + i;
                lastSpawnedRoom.RefreshExits();

                rerollRoom = !ChooseDirection(out posToSpawnRoom, out spawnedRoomOffset);
                lastSpawnedRoom.transform.position = posToSpawnRoom - spawnedRoomOffset;
                
                //Debug.Log(rerollRoom ? "Reroll" : "Don't reroll");
                if (rerollRoom)
                    Destroy(lastSpawnedRoom.gameObject);

                #region Getting off MR. BONES' WILD RIDE

                infinitLoopBreak++;
                if (infinitLoopBreak >= 10)
                {
                    Debug.LogError("I want to get off MR. BONES' WILD RIDE!");
                    return;
                }
                
                #endregion
            }
            while (rerollRoom);

            lastSpawnedRoom.RefreshExits();
            occupiedTiles.AddRange(lastSpawnedRoom.occupiedTiles);
            spawnedRoomsExits.AddRange(lastSpawnedRoom.exits);
            spawnedRoomsExits.RemoveAt(exitToRemove);
        }

        #region Spawning background

        int maxX = (int)occupiedTiles[0].x;
        int minX = (int)occupiedTiles[0].x;
        int maxY = (int)occupiedTiles[0].y;
        int minY = (int)occupiedTiles[0].y;
        
        for (int i = 0; i < occupiedTiles.Count; i++)
        {
            if (occupiedTiles[i].x < minX)
                minX = (int) occupiedTiles[i].x;

            if (occupiedTiles[i].x > maxX)
                maxX = (int)occupiedTiles[i].x;

            if (occupiedTiles[i].y < minY)
                minY = (int)occupiedTiles[i].y;

            if (occupiedTiles[i].y > maxY)
                maxY = (int)occupiedTiles[i].y;
        }

        maxX += 3;
        minX -= 3;
        maxY += 3;
        minY -= 3;

        for (int i = minX; i < maxX; i++)
        {
            for (int j = minY; j < maxY; j++)
            {
                if (!occupiedTiles.Contains(new Vector2(i, j)))
                {
                    GameObject clone = Instantiate(bgTile, transform);
                    clone.transform.position = new Vector2(i, j);
                }
            }
        }

        #endregion
    }

    bool ChooseDirection(out Vector3 posToSpawnRoom, out Vector3 spawnedRoomOffset)
    {
        posToSpawnRoom = Vector3.zero;
        spawnedRoomOffset = Vector3.zero;
        Vector3 posToCheckInHashset = Vector3.zero;

        if (spawnedRoomsExits.Count > 0)
        {
            RoomObject.Exit temp = spawnedRoomsExits[Random.Range(0, spawnedRoomsExits.Count)];
            Debug.Log(temp.dir);

            var exits = lastSpawnedRoom.GetComponent<RoomObject>().exits;
            var localExits = lastSpawnedRoom.GetComponent<RoomObject>().localExits;

            switch (temp.dir)
            {
                case RoomObject.Exit.Direction.North:
                    {
                        posToSpawnRoom = new Vector3(temp.pos.x, temp.pos.y + 1);

                        for (int j = 0; j < localExits.Count; j++)
                        {
                            if (localExits[j].dir == RoomObject.Exit.Direction.South)
                            {
                                spawnedRoomOffset = localExits[j].pos;
                                exitToRemove = j;
                            }
                        }

                        break;
                    }
                case RoomObject.Exit.Direction.South:
                    {
                        posToSpawnRoom = new Vector3(temp.pos.x, temp.pos.y - 1);

                        for (int j = 0; j < localExits.Count; j++)
                        {
                            if (localExits[j].dir == RoomObject.Exit.Direction.North)
                            {
                                spawnedRoomOffset = localExits[j].pos;
                                exitToRemove = j;
                            }
                        }

                        break;
                    }
                case RoomObject.Exit.Direction.East:
                    {
                        posToSpawnRoom = new Vector3(temp.pos.x + 1, temp.pos.y);

                        for (int j = 0; j < localExits.Count; j++)
                        {
                            if (localExits[j].dir == RoomObject.Exit.Direction.West)
                            {
                                spawnedRoomOffset = localExits[j].pos;
                                exitToRemove = j;
                            }
                        }

                        break;
                    }
                case RoomObject.Exit.Direction.West:
                    {
                        posToSpawnRoom = new Vector3(temp.pos.x - 1, temp.pos.y);

                        for (int j = 0; j < localExits.Count; j++)
                        {
                            if (localExits[j].dir == RoomObject.Exit.Direction.East)
                            {
                                spawnedRoomOffset = localExits[j].pos;
                                exitToRemove = j;
                            }
                        }

                        break;
                    }
            }

            posToCheckInHashset = posToSpawnRoom;

            if (occupiedTiles.Contains(posToCheckInHashset))
            {
                Debug.Log("No space for new room");
                return false;
            }

            if (spawnedRoomOffset == Vector3.zero)
            {
                Debug.Log("No matching exit in roomToSpawn");
                return false;
            }
                        
            spawnedRoomsExits.Remove(temp);
            return true;
        }

        posToSpawnRoom = transform.position;
        return true;
    }
}
