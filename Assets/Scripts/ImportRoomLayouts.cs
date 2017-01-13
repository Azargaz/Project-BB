using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEngine;

public class ImportRoomLayouts : MonoBehaviour
{
    [Header("Imported Room Types Layouts")]
    [SerializeField]
    public List<Room> roomTypes;

    [Serializable]
    public class Room
    {
        [SerializeField]
        public List<Layout> layouts = new List<Layout>();
        [Serializable]
        public class Layout
        {
            public string[] layout;
        }
        
        public int roomSize = 0;
    }   

    [Header("Number of certain room types")]
    public int[] roomTypesCount;
    string[] _layout = new string[16];    
    int _roomSize = 0;

    [Header("Tiles for rooms")]
    public GameObject background;
    public GameObject ground;
    public GameObject platform;
    public GameObject spikes;
    public GameObject weaponPedestal;
    public GameObject exit;

    public void ImportRooms()
    {
        roomTypes = new List<Room>();
        roomTypes.Clear();
        roomTypes.Add(new Room());

        for (int i = 1; i < roomTypesCount.Length; i++)
        {
            for (int j = 0; j < roomTypesCount[i]; j++)
            {
                _layout = new string[16];

                    Load("assets/editor/_roomlayouts/type" + i + "/" + j + ".txt", j, i);
            }

            //Debug.Log("Room " + i + " variations count: " + roomTypes[i].layout.Count);                
        }

        //for (int i = 0; i < roomTypes[1].layout.Count; i++)
        //{
        //    Debug.Log("RoomType 1 layout #" + i);
        //    for (int j = 0; j < roomTypes[1].layout[i].Length; j++)
        //    {
        //        Debug.Log(roomTypes[1].layout[i][j]);
        //    }
        //}

        Debug.LogWarning("All room layouts were imported correctly!");   
    }

    bool Load(string fileName, int roomNumber, int roomType)
    {
        string line;
        StreamReader theReader = new StreamReader(fileName, Encoding.Default);

        using (theReader)
        {
            do
            {
                line = theReader.ReadLine();

                if (line != null)
                {
                    string[] entries = line.Split(',');
                    if (entries.Length > 0)
                    {
                        for (int i = 0; i < entries.Length; i++)
                        {
                            _layout[_roomSize] += entries[i];
                        }
                    }
                    _roomSize++;
                }
            }
            while (line != null);

            theReader.Close();

            if (roomNumber == 0)
            {
                Debug.Log("Roomtype" + roomType);
                roomTypes.Add(new Room { layouts = new List<Room.Layout>(), roomSize = _roomSize });
            }

            roomTypes[roomType].layouts.Add(new Room.Layout { layout = _layout });           
            _roomSize = 0;
            return true;
        }
    }
}
