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
    public GameObject spikes;
    public string[] layout = new string[16];
    public List<Vector2> emptySpaces = new List<Vector2>();
    public List<Vector2> exceptionFields = new List<Vector2>();

    void Awake ()
    {
        Load("assets/_roomlayouts/type" + roomType + "/" + Random.Range(0, roomLayoutsCount) + ".txt");        
	}

    void SpawnRoom(string[] _layout)
    {
        for (int i = 0; i < layout.Length; i++)
        {
            if (layout[i] == null)
                continue;

            for (int j = 0; j < layout[i].Length; j++)
            {               
                if(layout[i][j] == '0' && background != null)
                {
                    GameObject bg = Instantiate(background);
                    bg.transform.parent = transform;
                    bg.transform.localPosition = new Vector2(j, layout.Length - i - 1);

                    emptySpaces.Add(bg.transform.position);
                }
                if(layout[i][j] == '1' && ground != null)
                {
                    GameObject clone = Instantiate(ground);
                    clone.transform.parent = transform;
                    clone.transform.localPosition = new Vector2(j, layout.Length - i - 1);
                }
                else if(layout[i][j] == '2' && platform != null)
                {
                    GameObject clone = Instantiate(platform);
                    clone.transform.parent = transform;
                    clone.transform.localPosition = new Vector2(j, layout.Length - i - 1);

                    if(background != null)
                    {
                        GameObject bg = Instantiate(background);
                        bg.transform.parent = transform;
                        bg.transform.localPosition = new Vector2(j, layout.Length - i - 1);
                    }                    
                }
                else if(layout[i][j] == '3' && spikes != null && background != null)
                {
                    GameObject clone = Instantiate(spikes);
                    clone.transform.parent = transform;
                    clone.transform.localPosition = new Vector2(j, layout.Length - i - 1);

                    exceptionFields.Add(clone.transform.position);

                    if (background != null)
                    {
                        GameObject bg = Instantiate(background);
                        bg.transform.parent = transform;
                        bg.transform.localPosition = new Vector2(j, layout.Length - i - 1);
                    }
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
            SpawnRoom(layout);
            return true;
        }
    }
}

