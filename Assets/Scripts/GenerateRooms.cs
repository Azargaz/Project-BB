using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GenerateRooms : MonoBehaviour
{
    public GameObject[,] rooms = new GameObject[4,4];
    public GameObject[] roomTypes;
    int roomsCount = 0;
    int reverseDirection = 1;

    void Awake()
    {
        GenerateRoom(new int[] { 1, 2 }, 0, 0);
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
                    Vector2 roomPos = roomNumber * 16;
                    int roomType = 5;
                    GameObject roomToSpawn = roomTypes[roomType - 1];

                    rooms[(int)roomNumber.x, (int)roomNumber.y] = Instantiate(roomToSpawn, roomPos, Quaternion.identity);
                    rooms[(int)roomNumber.x, (int)roomNumber.y].name = "R[" + (int)roomNumber.x + ", " + (int)roomNumber.y + "]" + " T[" + roomType + "]";

                }
            }
        }
    }

    void GenerateRoom(int[] prefferedRoomType, int column, int row)
    {        
        Vector2 roomNumber = new Vector2(column, row);
        Vector2 roomPos = roomNumber * 16;

        if(roomNumber.y >= 4)
        {
            Debug.LogWarning("Core path completed.");
            FillWithAdditionalRooms();
            return;
        }

        int nextRoomRow = 0;
        int nextRoomColumn = 0;
        int[] nextRoomPreferredType = new int[0];

        roomsCount++;
        if (roomsCount > 16)
            return;

        int roomType = prefferedRoomType[Random.Range(0, prefferedRoomType.Length)];
        GameObject roomToSpawn = roomTypes[roomType - 1];

        rooms[(int)roomNumber.x, (int)roomNumber.y] = Instantiate(roomToSpawn, roomPos, Quaternion.identity);
        rooms[(int)roomNumber.x, (int)roomNumber.y].name = "R[" + (int)roomNumber.x + ", " + (int)roomNumber.y + "]" + " T[" + roomType + "]";

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
        
        // Choose preffered roomTypes for next room
        switch(roomType)
        {
            case 1:
                {
                    nextRoomPreferredType = new int[] { 1, 2, 3, 4 };
                    break;
                }
            case 2:
                {
                    nextRoomPreferredType = goVertical ? new int[] { 3, 4 } : new int[] { 1, 2, 3, 4 };
                    break;
                }
            case 3:
                {
                    nextRoomPreferredType = new int[] { 1, 2, 3, 4 };
                    break;
                }
            case 4:
                {
                    nextRoomPreferredType = goVertical ? new int[] { 3, 4 } : new int[] { 1, 2, 3, 4 };
                    break;
                }
        }

        // If it's last room in row go up
        if ((roomNumber.x == 3 && reverseDirection == 1) || (roomNumber.x == 0 && reverseDirection == -1))
        {
            nextRoomRow = (int)roomNumber.y + 1;
            nextRoomColumn = (int)roomNumber.x;
            nextRoomPreferredType = new int[] { 3, 4 };
            reverseDirection *= -1;
        }
        // If it's one before last room, make sure to spawn room with North exit
        if((roomNumber.x == 2 && reverseDirection == 1) || (roomNumber.x == 1 && reverseDirection == -1))
        {
            nextRoomRow = (int)roomNumber.y;
            nextRoomColumn = (int)roomNumber.x + reverseDirection;
            nextRoomPreferredType = new int[] { 2, 4 };
        }

        Debug.Log("Row: " + nextRoomRow + " Column: " + nextRoomColumn);

        GenerateRoom(nextRoomPreferredType, nextRoomColumn, nextRoomRow);
    }
}
