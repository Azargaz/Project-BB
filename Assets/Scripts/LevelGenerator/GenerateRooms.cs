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
    public int numberOfWeaponRooms = 1;
    int _numberOfWeaponRooms;
    int roomsCount = 0;
    int leftRightDirection = 1;
    int type5roomCount = 0;

    public GameObject border;
    public GameObject borderBg;
    public GameObject questboard;
    public GameObject test;
    public Obstacle[] obstacles;
    public Monster[] monsters;
    public Monster[] minibosses;

    public float chanceToSpawnObstacles;
    public float chanceToSpawnMonsters;
    public float chanceToSpawnQuestboard;
    public int questboardsPerLevel = 1;

    Vector2 exitRoom;
    [HideInInspector]
    public List<Vector2> allEmptySpaces = new List<Vector2>();
    [HideInInspector]
    public List<Vector2> allPlatformFields = new List<Vector2>();
    [HideInInspector]
    public List<Vector2> allPedestals = new List<Vector2>();
    [HideInInspector]
    public List<Vector2> allSpikeFields = new List<Vector2>();
    List<Vector2> emptySpaces = new List<Vector2>();
    HashSet<Vector2> emptySpacesHash = new HashSet<Vector2>();
    HashSet<Vector2> ignoreFieldsHash = new HashSet<Vector2>();
    HashSet<Vector2> platformFieldsHash = new HashSet<Vector2>();

    [System.Serializable]
    public class Obstacle
    {
        public string name;
        public GameObject[] objects;
        public bool limit = false;
        public int maxPerLevel;
        public int chanceToSpawnWeight;
    }

    [System.Serializable]
    public class Monster : Obstacle
    {
        public int value;
    }

    ImportRoomLayouts RT;
    public static GenerateRooms GR;

    [Header("")]
    public bool debug;

    void Awake()
    {
        GR = this;
        RT = GameObject.FindGameObjectWithTag("RoomLayouts").GetComponent<ImportRoomLayouts>();

        StartGeneration();            
    }

    void StartGeneration()
    {
        numberOfRooms = (int)Mathf.Pow(sqrtNumberOfRooms, 2);
        rooms = new GameObject[sqrtNumberOfRooms, sqrtNumberOfRooms];

        GenerateRoom(new int[] { 1, 2 }, 0, 0);
        SpawnRooms();
        CreateBorder(-15);

        SpawnObstaclesAndMonsters();
        AstarPath.active.Scan();
    }

    int RollWithWeights(Obstacle[] array)
    {
        int summedWeights = 0;
        int returnInt = 0;

        if (array.Length <= 1)
            return 0;

        for (int x = 0; x < array.Length; x++)
        {
            if (array[x].limit && array[x].maxPerLevel <= 0)
                continue;

            summedWeights += array[x].chanceToSpawnWeight;
        }

        for (int x = 0; x < array.Length; x++)
        {
            int random = Random.Range(0, summedWeights);
            random -= array[x].chanceToSpawnWeight;

            if (random <= 0)
            {
                returnInt = x;
                break;
            }                
        }

        return returnInt;
    }
    
    void SpawnObstaclesAndMonsters()
    {        
        for (int i = 0; i < rooms.GetLength(0); i++)
        {
            for (int j = 0; j < rooms.GetLength(1); j++)
            {
                if (i == 0 && j == 0)
                    continue;

                #region Hashsets and Lists

                List<Vector2> spawnPoints = new List<Vector2>();

                emptySpacesHash.Clear();
                ignoreFieldsHash.Clear();
                platformFieldsHash.Clear();

                emptySpacesHash.UnionWith(rooms[i, j].GetComponent<Room>().emptySpaces);
                ignoreFieldsHash.UnionWith(rooms[i, j].GetComponent<Room>().ignoreFields);
                platformFieldsHash.UnionWith(rooms[i, j].GetComponent<Room>().platformsFields);

                emptySpaces = rooms[i, j].GetComponent<Room>().emptySpaces;
                spawnPoints = rooms[i, j].GetComponent<Room>().spawnPoints;

                #endregion

                #region Obstacles

                for (int k = 0; k < emptySpaces.Count; k++)
                {
                    float roll = Random.value;
                    if (Random.value > chanceToSpawnObstacles / 100f && chanceToSpawnObstacles > 0)
                        continue;
                    
                    if (debug)
                    {
                        Instantiate(test, emptySpaces[k], Quaternion.identity, transform.Find("Debug"));
                    }

                    // If there are no obstacles assigned to the array, break the loop
                    if (obstacles.Length <= 0)
                    {
                        if (debug)
                            Debug.LogWarning("No obstacles to spawn.");
                        break;
                    }

                    int obstacleToSpawnID = RollWithWeights(obstacles);
                    GameObject obstacleToSpawn = obstacles[obstacleToSpawnID].objects[Random.Range(0, obstacles[obstacleToSpawnID].objects.Length)];

                    if (obstacleToSpawn != null && !emptySpacesHash.Contains(new Vector2(emptySpaces[k].x, emptySpaces[k].y - 1)) && !ignoreFieldsHash.Contains(new Vector2(emptySpaces[k].x, emptySpaces[k].y - 1)))
                    {
                        Instantiate(obstacleToSpawn, emptySpaces[k], Quaternion.identity, transform.Find("Obstacles"));
                        obstacles[obstacleToSpawnID].maxPerLevel--;
                    }
                    
                }

                #endregion

                #region Monsters

                int monstersPerRoom = 100;

                int infinityBreak = 0;

                HashSet<int> occupiedSpawnPoints = new HashSet<int>();

                do
                {
                    #region INFINITY BREAK

                    infinityBreak++;
                    if (infinityBreak > 20)
                    {
                        Debug.LogError("Couldn't spawn monsters, infinite loop in SpawnObstaclesAndMonsters()");
                        break;
                    }

                    #endregion

                    #region Break loop

                    // If there are no monsters assigned, break the loop
                    if (monsters.Length <= 0)
                    {
                        if (debug)
                            Debug.LogWarning("No monsters to spawn.");
                        break;
                    }

                    if (monstersPerRoom <= 0)
                    {
                        if (debug)
                            Debug.LogWarning("Monsters per this room exhausted.");
                        break;
                    }

                    #endregion

                    if (occupiedSpawnPoints.Count == spawnPoints.Count)
                        occupiedSpawnPoints.Clear();

                    int spawnPointNumber = 0;

                    do
                    {
                        spawnPointNumber = Random.Range(0, spawnPoints.Count);
                    }
                    while(occupiedSpawnPoints.Contains(spawnPointNumber));

                    int monsterToSpawnID = RollWithWeights(monsters);

                    GameObject monsterToSpawn = monsters[monsterToSpawnID].objects[Random.Range(0, monsters[monsterToSpawnID].objects.Length)];

                    if (
                        monsterToSpawn != null
                        )
                    {
                        GameObject clone = Instantiate(monsterToSpawn, spawnPoints[spawnPointNumber], Quaternion.identity, transform.Find("Mobs"));
                        clone.name = monsterToSpawn.name;

                        monstersPerRoom -= monsters[monsterToSpawnID].value;
                        occupiedSpawnPoints.Add(spawnPointNumber);
                    }
                    
                }
                while (monstersPerRoom > 10);

                #endregion

                #region Questboard

                if(questboard != null && questboardsPerLevel > 0)
                {
                    float rollQuestboard = 0;
                    rollQuestboard = Random.value;

                    if (rollQuestboard <= chanceToSpawnQuestboard / 100f)
                    {
                        int spawnPointID = 0;
                        spawnPointID = Random.Range(0, spawnPoints.Count);

                        Instantiate(questboard, spawnPoints[spawnPointID], Quaternion.identity, transform);
                        questboardsPerLevel--;
                    }
                }

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

                #region Changing room types

                if (y < rooms.GetLength(1) - 1)
                {
                    if(thisRoom.roomType == 2 || thisRoom.roomType == 4 || thisRoom.roomType == 5 || thisRoom.roomType == 6)
                    {
                        Room aboveRoom = rooms[x, y + 1].GetComponent<Room>();

                        if(aboveRoom.roomType < 3)
                        {
                            if (y + 1 == rooms.GetLength(1) - 1)
                            {
                                rooms[x, y + 1] = roomTypes[3 - 1];
                            }
                            else if(y < rooms.GetLength(1) - 2)
                            {
                                rooms[x, y + 1] = roomTypes[Random.Range(3, 5) - 1];

                                Room aboveAboveRoom = rooms[x, y + 2].GetComponent<Room>();
                                if (aboveAboveRoom.roomType >= 3)
                                {
                                    rooms[x, y + 1] = roomTypes[Random.Range(4, 5) - 1];
                                }
                            }
                        }
                    }
                }

                if (thisRoom.roomType == 4 && y == rooms.GetLength(1) - 1)
                {
                    rooms[x, y] = roomTypes[3 - 1];
                    if(debug)
                        Debug.Log("TYPE 4 TO 3 AT[" + x + ", " + y + "]");
                }

                if (thisRoom.roomType == 2 && y == rooms.GetLength(1) - 1)
                {
                    rooms[x, y] = roomTypes[1 - 1];
                    if (debug)
                        Debug.Log("TYPE 2 TO 1 AT[" + x + ", " + y + "]");
                }

                #endregion

                thisRoom = rooms[x, y].GetComponent<Room>();
                Vector2 roomPos = new Vector2(x, y) * roomSize;
                rooms[x, y].GetComponent<Room>().RT = RT;
                rooms[x, y] = Instantiate(rooms[x, y], roomPos, Quaternion.identity, transform.Find("Rooms"));                
                rooms[x, y].name = "R[" + x + ", " + y + "]" + " T[" + thisRoom.roomType + "]";
                thisRoom = rooms[x, y].GetComponent<Room>();

                allEmptySpaces.AddRange(thisRoom.emptySpaces);
                allPlatformFields.AddRange(thisRoom.platformsFields);
                allSpikeFields.AddRange(thisRoom.spikeFields);

                if(thisRoom.weaponPedestals.Count > 0)
                    allPedestals.AddRange(thisRoom.weaponPedestals);

                if (new Vector2(x, y) == exitRoom)
                {
                    rooms[x, y].GetComponent<Room>().SpawnExit();
                }
            }
        }
    }
    
    void GenerateRoom(int[] prefferedRoomType, int X, int Y)
    {        
        Vector2 roomNumber = new Vector2(X, Y);

        if(roomNumber.y >= sqrtNumberOfRooms)
        {
            if(debug)
                Debug.Log("Core path completed.");

            exitRoom = new Vector2(X, Y - 1);
            type5roomCount = 0;
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

        #region Choosing next room's direction

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
            nextRoomColumn = (int)roomNumber.x + leftRightDirection;            
        }

        #endregion

        #region Choosing next room's type

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
                    nextRoomPreferredType = goVertical ? new int[] { 3, 4 } : (roomNumber.y < rooms.GetLength(1) - 1 ? new int[] { 1, 2 } : new int[] { 1 });
                    break;
                }
        }

        if(roomNumber.y < rooms.GetLength(1))
        {
            // If it's one before last room, make sure to spawn room with North exit
            if ((roomNumber.x == sqrtNumberOfRooms - 2 && leftRightDirection == 1) || (roomNumber.x == 1 && leftRightDirection == -1))
            {
                nextRoomRow = (int)roomNumber.y;
                nextRoomColumn = (int)roomNumber.x + leftRightDirection;
                if(roomNumber.y == 0)
                    nextRoomPreferredType = new int[] { 2 };
                else
                    nextRoomPreferredType = new int[] { 2, 4 };
            }

            // If it's last room in row go up
            if ((roomNumber.x == sqrtNumberOfRooms - 1 && leftRightDirection == 1) || (roomNumber.x == 0 && leftRightDirection == -1))
            {
                nextRoomRow = (int)roomNumber.y + 1;
                nextRoomColumn = (int)roomNumber.x;
                if(roomNumber.y >= rooms.GetLength(1) - 2)
                    nextRoomPreferredType = new int[] { 3 };
                else
                    nextRoomPreferredType = new int[] { 3, 4 };
                leftRightDirection *= -1;
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
                    int roomType = 5;
                    if (j == 0)
                        roomType = 2;
                    if (numberOfWeaponRooms > 0)
                        roomType = 6;

                    GameObject roomToSpawn = roomTypes[roomType - 1];
                    rooms[i, j] = roomToSpawn;

                    if (roomToSpawn.GetComponent<Room>().roomType == 5)
                        type5roomCount++;
                    else if (roomToSpawn.GetComponent<Room>().roomType == 6)
                    {
                        if(debug)
                            Debug.Log("ROOMTYPE: " + roomType + " POS[" + i + "," + j + "]");
                        numberOfWeaponRooms--;
                    }                        
                }
            }
        }

        if(numberOfWeaponRooms > 0)
        {
            for (int i = 0; i < rooms.GetLength(0); i++)
            {
                for (int j = 0; j < rooms.GetLength(1); j++)
                {
                    if (rooms[i, j].GetComponent<Room>().roomType == 4 || rooms[i, j].GetComponent<Room>().roomType == 5 || rooms[i, j].GetComponent<Room>().roomType == 3)
                    {
                        int roomType = 6;
                        GameObject roomToSpawn = roomTypes[roomType - 1];
                        rooms[i, j] = roomToSpawn;
                        Debug.Log("ROOMTYPE: " + roomType + " POS[" + i + "," + j + "]");
                        numberOfWeaponRooms--;
                        return;
                    }
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
                        clone.transform.parent = transform.Find("Border");
                        clone.transform.position = new Vector3(i, j, 0);
                    }
                    else
                    {
                        GameObject clone = Instantiate(borderBg);
                        clone.transform.parent = transform.Find("Border");
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
                            clone.transform.parent = transform.Find("Border");
                            clone.transform.position = new Vector3(i, j, 0);
                        }
                        else
                        {
                            GameObject clone = Instantiate(borderBg);
                            clone.transform.parent = transform.Find("Border");
                            clone.transform.position = new Vector3(i, j, 0);
                        }
                    }
                }
            }
        }
    }
}
