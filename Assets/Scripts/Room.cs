using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Text;
using System.IO;

public class Room : MonoBehaviour
{
    public int roomType = 1;
    int roomHeight = 0;
    public int roomLayoutsCount;
    public GameObject background;
    public GameObject ground;
    public GameObject platform;
    public string[] layout = new string[16];

    void Start ()
    {
        Load("assets/roomlayouts/type" + roomType + "_" + Random.Range(0, roomLayoutsCount) + ".txt");
        SpawnRoom(layout);
	}

    void SpawnRoom(string[] _layout)
    {
        for (int i = 0; i < layout.Length; i++)
        {
            if (layout[i] == null)
                continue;

            for (int j = 0; j < layout[i].Length; j++)
            {               
                if(layout[i][j] == '0' || layout[i][j] == '2')
                {
                    GameObject clone = Instantiate(background);
                    clone.transform.parent = transform;
                    clone.transform.localPosition = new Vector2(j, layout.Length - i - 1);
                }
                if(layout[i][j] == '1')
                {
                    GameObject clone = Instantiate(ground);
                    clone.transform.parent = transform;
                    clone.transform.localPosition = new Vector2(j, layout.Length - i - 1);
                }
                else if(layout[i][j] == '2')
                {
                    GameObject clone = Instantiate(platform);
                    clone.transform.parent = transform;
                    clone.transform.localPosition = new Vector2(j, layout.Length - i - 1);
                }
            }
        }
    }

    private bool Load(string fileName)
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
                            layout[roomHeight] += entries[i];
                        }
                    }
                    roomHeight++;
                }
            }
            while (line != null);
   
            theReader.Close();
            return true;
        }
    }
}

