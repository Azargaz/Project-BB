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
    public GameObject test;

    List<string[]> roomLayouts = new List<string[]>();
    List<Vector2>[] emptySpace = new List<Vector2>[16];

    [Header("Debug")]
    public bool debug;

    void Awake()
    {
        numberOfRooms = (int) Mathf.Pow(sqrtNumberOfRooms, 2);
        rooms = new GameObject[sqrtNumberOfRooms, sqrtNumberOfRooms];
        GenerateRoom(new int[] { 1, 2 }, 0, 0);
        CreateBorder(-10);        
    }

    void Start()
    {
        StartCoroutine(LateStart(0.1f));
    }

    IEnumerator LateStart(float waitTime)
    {
        yield return new WaitForSeconds(waitTime);
        GetEmptySpaces();
    }

    void CreateBorder(int offset)
    {
        int size = sqrtNumberOfRooms * roomSize - offset;

        for(int i = offset; i < size; i++)
        {
            for (int j = offset; j < size; j++)
            {
                if((i > offset && i < 0) || (i >= size + offset && i < size))
                {
                    if((i == -1 || i == size + offset) && j >= -1 && j <= size + offset)
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

    void GetEmptySpaces()
    {
        for (int i = 0; i < roomLayouts.Count; i++)
        {
            emptySpace[i] = new List<Vector2>();
            for (int j = 0; j < roomLayouts[i].Length; j++)
            {
                for (int k = 0; k < roomLayouts[i][j].Length; k++)
                {
                    if(roomLayouts[i][j][k] == '1' || roomLayouts[i][j][k] == '2')
                    {
                        if(j + 1 != roomLayouts[i].Length && roomLayouts[i][j + 1][k] == '0')
                        {
                            emptySpace[i].Add(new Vector2(k, j + 1));
                        }
                    }
                }
            }
        }
        
        for (int j = 0; j < emptySpace[0].Count; j++)
        {
            Instantiate(test, emptySpace[0][j], Quaternion.identity);
        }        
    }

    void FillWithAdditionalRooms()
    {
        for (int i = 0; i < rooms.GetLength(0); i++)
        {
            for (int j = 0; j < rooms.GetLength(1); j++)
            {
                if(rooms[i, j] == null)
                {
                    Vector2 roomNumber = new Vector2(i, j);
                    Vector2 roomPos = roomNumber * roomSize;
                    int roomType = 5;
                    GameObject roomToSpawn = roomTypes[roomType - 1];

                    rooms[(int)roomNumber.x, (int)roomNumber.y] = Instantiate(roomToSpawn, roomPos, Quaternion.identity);
                    rooms[(int)roomNumber.x, (int)roomNumber.y].transform.parent = transform;
                    rooms[(int)roomNumber.x, (int)roomNumber.y].name = "R[" + (int)roomNumber.x + ", " + (int)roomNumber.y + "]" + " T[" + roomType + "]";
                    type5roomCount++;
                }
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
                Debug.LogWarning("Core path completed.");
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

        #region Spawning

        rooms[(int)roomNumber.x, (int)roomNumber.y] = Instantiate(roomToSpawn, roomPos, Quaternion.identity);
        rooms[(int)roomNumber.x, (int)roomNumber.y].transform.parent = transform;
        rooms[(int)roomNumber.x, (int)roomNumber.y].name = "R[" + (int)roomNumber.x + ", " + (int)roomNumber.y + "]" + " T[" + roomType + "]";
        roomLayouts.Add(rooms[(int)roomNumber.x, (int)roomNumber.y].GetComponent<Room>().layout);

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
                    nextRoomPreferredType = new int[] { 1, 2 };
                    break;
                }
            case 2:
                {
                    nextRoomPreferredType = goVertical ? new int[] { 3, 4 } : new int[] { 1, 2 };
                    break;
                }
            case 3:
                {
                    nextRoomPreferredType = new int[] { 1, 2 };
                    break;
                }
            case 4:
                {
                    nextRoomPreferredType = goVertical ? new int[] { 3, 4 } : new int[] { 1, 2 };
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
}
