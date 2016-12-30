using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GenerateRooms : MonoBehaviour
{
    public GameObject[,] rooms = new GameObject[4,4];
    public GameObject[] roomTypes;
    public int roomSize = 16;
    int numberOfRooms = 16;
    public int sqrtNumberOfRooms = 4;
    int roomsCount = 0;
    int reverseDirection = 1;
    int type5roomCount = 0;

    public GameObject border;
    public GameObject borderBg;
    public GameObject[] obstacles;
    public GameObject[] monsters;
    public float[] chanceToSpawnMonster;
    public int[] monstersValues;
    public GameObject test;

    public float chanceToSpawnObstacles;

    List<Vector2> emptySpaces = new List<Vector2>();
    HashSet<Vector2> emptySpacesHash = new HashSet<Vector2>();
    HashSet<Vector2> exceptionFieldsHash = new HashSet<Vector2>();

    [Header("")]
    public bool debug;

    void Awake()
    {
        numberOfRooms = (int) Mathf.Pow(sqrtNumberOfRooms, 2);
        rooms = new GameObject[sqrtNumberOfRooms, sqrtNumberOfRooms];
        GenerateRoom(new int[] { 1, 2 }, 0, 0);
        SpawnRooms();
        CreateBorder(-15);
        SpawnObstaclesAndMonsters();
    }
    
    void SpawnObstaclesAndMonsters()
    {        
        for (int i = 0; i < rooms.GetLength(0); i++)
        {
            for (int j = 0; j < rooms.GetLength(1); j++)
            {
                emptySpacesHash.Clear();
                exceptionFieldsHash.Clear();
                emptySpacesHash.UnionWith(rooms[i, j].GetComponent<Room>().emptySpaces);
                exceptionFieldsHash.UnionWith(rooms[i, j].GetComponent<Room>().exceptionFields);
                emptySpaces = rooms[i, j].GetComponent<Room>().emptySpaces;

                #region Obstacles

                for (int k = 0; k < emptySpaces.Count; k++)
                {
                    if (debug)
                    {
                        GameObject clone = Instantiate(test, emptySpaces[k], Quaternion.identity, transform.FindChild("Debug"));
                    }

                    // If there are no obstacles assigned to the array, break the loop
                    if (obstacles.Length <= 0)
                    {
                        if (debug)
                            Debug.LogWarning("No obstacles to spawn.");
                        break;
                    }

                    // "Roll the dice" to check if should spawn the obstacle
                    float roll = Random.value;
                    bool spawn = Random.value <= chanceToSpawnObstacles / 100f;

                    if (debug)
                        Debug.Log("Obstacle Roll: " + (int)(roll * 100));

                    if (spawn)
                    {
                        if (!emptySpacesHash.Contains(new Vector2(emptySpaces[k].x, emptySpaces[k].y - 1)) && !exceptionFieldsHash.Contains(new Vector2(emptySpaces[k].x, emptySpaces[k].y - 1)))
                        {
                            GameObject clone = Instantiate(obstacles[Random.Range(0, obstacles.Length)], emptySpaces[k], Quaternion.identity, transform.FindChild("Obstacles"));
                        }
                    }
                }

                #endregion

                #region Monsters

                int monstersPerRoom = 100;

                do
                {
                    for (int k = 0; k < emptySpaces.Count; k++)
                    {
                        // If there are no monsters assigned, break the loop
                        if (monsters.Length <= 0)
                        {
                            if (debug)
                                Debug.LogWarning("No obstacles to spawn.");
                            break;
                        }

                        if (monstersPerRoom <= 0)
                        {
                            if (debug)
                                Debug.LogWarning("Monsters per this room exhausted.");
                            break;
                        }

                        int rolledMonster = Random.Range(0, monsters.Length);

                        // "Roll the dice" to check if should spawn the monster
                        float roll = Random.value;
                        bool spawn = Random.value <= chanceToSpawnMonster[rolledMonster] / 100f;

                        if (debug)
                            Debug.Log("Obstacle Roll: " + (int)(roll * 100));

                        if (spawn)
                        {
                            if (!emptySpacesHash.Contains(new Vector2(emptySpaces[k].x, emptySpaces[k].y - 1)) && !exceptionFieldsHash.Contains(new Vector2(emptySpaces[k].x, emptySpaces[k].y - 1)))
                            {
                                GameObject clone = Instantiate(monsters[rolledMonster], emptySpaces[k], Quaternion.identity, transform.FindChild("Mobs"));
                                monstersPerRoom -= monstersValues[rolledMonster] > 0 ? monstersValues[rolledMonster] : 20;
                            }
                        }
                    }
                }
                while (monstersPerRoom > 10);

                #endregion
            }
        }                
    }

    void SpawnRooms()
    {
        for (int y = 0; y < rooms.GetLength(1); y++)
        {
            for (int x = 0; x < rooms.GetLength(0); x++)
            {
                Room thisRoom = rooms[x, y].GetComponent<Room>();

                if (y < rooms.GetLength(1) - 1)
                {                    
                    if (thisRoom.roomType == 2 || thisRoom.roomType == 4)
                    {
                        Room roomAbove = rooms[x, y + 1].GetComponent<Room>();

                        if (roomAbove.roomType != 3 && roomAbove.roomType != 4)
                        {
                            GameObject roomType = roomTypes[Random.Range(3, 5) - 1];

                            if (y == rooms.GetLength(1) - 1)
                                roomType = roomTypes[3 - 1];

                            rooms[x, y + 1] = roomType;                            
                        }
                    }
                }

                Vector2 roomPos = new Vector2(x, y) * roomSize;
                rooms[x, y] = Instantiate(rooms[x, y], roomPos, Quaternion.identity, transform.FindChild("Rooms"));
                rooms[x, y].name = "R[" + x + ", " + y + "]" + " T[" + thisRoom.roomType + "]";
            }
        }
    }
    
    void GenerateRoom(int[] prefferedRoomType, int column, int row)
    {        
        Vector2 roomNumber = new Vector2(column, row);
        Vector2 roomPos = roomNumber * roomSize;

        if(roomNumber.y >= sqrtNumberOfRooms)
        {
            if(debug)
                Debug.Log("Core path completed.");
            FillWithAdditionalRooms();
            return;
        }

        int nextRoomRow = 0;
        int nextRoomColumn = 0;
        int[] nextRoomPreferredType = new int[0];

        roomsCount++;
        if (roomsCount > numberOfRooms)
            return;        

        int roomType = prefferedRoomType[Random.Range(0, prefferedRoomType.Length)];
        GameObject roomToSpawn = roomTypes[roomType - 1];

        rooms[(int)roomNumber.x, (int)roomNumber.y] = roomToSpawn;

        #region Spawning

        // Choose to go up if roomType has North exit
        bool goVertical = false;
        if(roomType != 1 && roomType != 3)
            goVertical = Random.Range(0, 2) == 1;

        // If goVertical go up
        if(goVertical)
        {
            nextRoomRow = (int)roomNumber.y + 1;
            nextRoomColumn = (int)roomNumber.x;
        }      
        // Else proceed along X axis
        else
        {
            nextRoomRow = (int)roomNumber.y;
            nextRoomColumn = (int)roomNumber.x + reverseDirection;            
        }

        #endregion

        #region Choosing next room type

        // Choose preffered roomTypes for next room
        switch (roomType)
        {
            case 1:
                {
                    nextRoomPreferredType = roomNumber.y < rooms.GetLength(1) - 1 ? new int[] { 1, 2 } : new int[] { 1 };
                    break;
                }
            case 2:
                {
                    nextRoomPreferredType = goVertical ? new int[] { 3, 4 } : roomNumber.y < rooms.GetLength(1) - 1 ? new int[] { 1, 2 } : new int[] { 1 };
                    break;
                }
            case 3:
                {
                    nextRoomPreferredType = roomNumber.y < rooms.GetLength(1) - 1 ? new int[] { 1, 2 } : new int[] { 1 };
                    break;
                }
            case 4:
                {
                    nextRoomPreferredType = goVertical ? new int[] { 3, 4 } : roomNumber.y < rooms.GetLength(1) - 1 ? new int[] { 1, 2 } : new int[] { 1 };
                    break;
                }
        }

        if(roomNumber.y < rooms.GetLength(1))
        {
            // If it's last room in row go up
            if ((roomNumber.x == sqrtNumberOfRooms - 1 && reverseDirection == 1) || (roomNumber.x == 0 && reverseDirection == -1))
            {
                nextRoomRow = (int)roomNumber.y + 1;
                nextRoomColumn = (int)roomNumber.x;
                if(roomNumber.y == rooms.GetLength(1) - 2)
                    nextRoomPreferredType = new int[] { 3 };
                else
                    nextRoomPreferredType = new int[] { 3, 4 };
                reverseDirection *= -1;
            }
            // If it's one before last room, make sure to spawn room with North exit
            if ((roomNumber.x == sqrtNumberOfRooms - 2 && reverseDirection == 1) || (roomNumber.x == 1 && reverseDirection == -1))
            {
                nextRoomRow = (int)roomNumber.y;
                nextRoomColumn = (int)roomNumber.x + reverseDirection;
                nextRoomPreferredType = new int[] { 2 };
            }
        }

        #endregion

        if (debug)
            Debug.Log("Row: " + nextRoomRow + " Column: " + nextRoomColumn);

        GenerateRoom(nextRoomPreferredType, nextRoomColumn, nextRoomRow);
    }

    void FillWithAdditionalRooms()
    {
        for (int i = 0; i < rooms.GetLength(0); i++)
        {
            for (int j = 0; j < rooms.GetLength(1); j++)
            {
                if (rooms[i, j] == null)
                {
                    Vector2 roomNumber = new Vector2(i, j);
                    Vector2 roomPos = roomNumber * roomSize;
                    int roomType = 5;
                    GameObject roomToSpawn = roomTypes[roomType - 1];
                    rooms[(int)roomNumber.x, (int)roomNumber.y] = roomToSpawn;

                    type5roomCount++;
                }
            }
        }
    }


    void CreateBorder(int offset)
    {
        int size = sqrtNumberOfRooms * roomSize - offset;

        for (int i = offset; i < size; i++)
        {
            for (int j = offset; j < size; j++)
            {
                if ((i > offset && i < 0) || (i >= size + offset && i < size))
                {
                    if ((i == -1 || i == size + offset) && j >= -1 && j <= size + offset)
                    {
                        GameObject clone = Instantiate(border);
                        clone.transform.parent = transform.FindChild("Border");
                        clone.transform.position = new Vector3(i, j, 0);
                    }
                    else
                    {
                        GameObject clone = Instantiate(borderBg);
                        clone.transform.parent = transform.FindChild("Border");
                        clone.transform.position = new Vector3(i, j, 0);
                    }
                }

                if ((i >= 0 && i < size + offset))
                {
                    if (j < 0 || (j >= size + offset && j < size))
                    {
                        if (j == -1 || j == size + offset)
                        {
                            GameObject clone = Instantiate(border);
                            clone.transform.parent = transform.FindChild("Border");
                            clone.transform.position = new Vector3(i, j, 0);
                        }
                        else
                        {
                            GameObject clone = Instantiate(borderBg);
                            clone.transform.parent = transform.FindChild("Border");
                            clone.transform.position = new Vector3(i, j, 0);
                        }
                    }
                }
            }
        }
    }
}
