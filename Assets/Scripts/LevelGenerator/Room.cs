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
    public GameObject weaponPedestal;
    public GameObject exit;
    bool exitSpawned;
    public string[] layout = new string[16];
    public List<Vector2> emptySpaces = new List<Vector2>();
    public List<Vector2> ignoreFields = new List<Vector2>();
    public List<Vector2> platformsFields = new List<Vector2>();
    bool blocked;
    List<GameObject> blockade = new List<GameObject>();

    void Awake ()
    {
        Load("assets/_roomlayouts/type" + roomType + "/" + Random.Range(0, roomLayoutsCount) + ".txt");        
	}

    public void BlockRoom()
    {
        if (blocked)
            return;

        blocked = true;

        for (int i = 0; i < roomHeight; i++)
        {
            for (int j = 0; j < roomHeight; j++)
            {
                if((i == 0 || i == roomHeight - 1) && (j != 0 && j != roomHeight - 1))
                {
                    GameObject clone = Instantiate(ground, transform);
                    clone.transform.localPosition = new Vector2(i, j);

                    blockade.Add(clone);
                }

                if ((j == 0 || j == roomHeight - 1) && (i != 0 && i != roomHeight - 1))
                {
                    GameObject clone = Instantiate(ground, transform);
                    clone.transform.localPosition = new Vector2(i, j);

                    blockade.Add(clone);
                }
            }
        }
    }

    public void UnblockRoom()
    {
        if (!blocked)
            return;

        for (int i = 0; i < blockade.Count; i++)
        {
            Destroy(blockade[i]);
        }

        blockade.Clear();
    }

    public void SpawnExit()
    {
        if (exit == null)
        {
            Debug.LogError("No exit assigned to the room object!");
            return;
        }

        do
        {
            for (int i = 0; i < emptySpaces.Count; i++)
            {
                if(emptySpaces.Contains(new Vector2(emptySpaces[i].x, emptySpaces[i].y - 1)) || ignoreFields.Contains(new Vector2(emptySpaces[i].x, emptySpaces[i].y - 1)))
                    continue;

                bool spawnExit = Random.value > 0.5f;

                if (spawnExit)
                {
                    exitSpawned = true;
                    GameObject clone = Instantiate(exit, transform);
                    clone.transform.position = emptySpaces[i];
                    return;
                }
            }
        }
        while (!exitSpawned);
    }

    void SpawnRoom(string[] _layout)
    {
        for (int i = 0; i < layout.Length; i++)
        {
            if (layout[i] == null)
                continue;

            for (int j = 0; j < layout[i].Length; j++)
            {          
                // SPAWNING JUST BACKGROUNDS     
                if(layout[i][j] == '0' && background != null)
                {
                    GameObject bg = Instantiate(background);
                    bg.transform.parent = transform;
                    bg.transform.localPosition = new Vector2(j, layout.Length - i - 1);

                    emptySpaces.Add(bg.transform.position);
                }
                // SPAWNING NORMAL TILES
                if (layout[i][j] == '1' && ground != null)
                {
                    GameObject clone = Instantiate(ground);
                    clone.transform.parent = transform;
                    clone.transform.localPosition = new Vector2(j, layout.Length - i - 1);
                }
                // SPAWNING PLATFORMS WITH BACKGROUNDS   
                else if (layout[i][j] == '2' && platform != null)
                {
                    GameObject clone = Instantiate(platform);
                    clone.transform.parent = transform;
                    clone.transform.localPosition = new Vector2(j, layout.Length - i - 1);

                    platformsFields.Add(clone.transform.position);

                    if (background != null)
                    {
                        GameObject bg = Instantiate(background);
                        bg.transform.parent = transform;
                        bg.transform.localPosition = new Vector2(j, layout.Length - i - 1);
                    }                    
                }
                // SPAWNING SPIKES WITH BACKGROUNDS 
                else if (layout[i][j] == '3' && spikes != null)
                {
                    GameObject clone = Instantiate(spikes);
                    clone.transform.parent = transform;
                    clone.transform.localPosition = new Vector2(j, layout.Length - i - 1);

                    ignoreFields.Add(clone.transform.position);

                    if (background != null)
                    {
                        GameObject bg = Instantiate(background);
                        bg.transform.parent = transform;
                        bg.transform.localPosition = new Vector2(j, layout.Length - i - 1);
                    }
                }
                // SPAWNING WEAPON PEDESTALS WITH BACKGROUNDS   
                else if (layout[i][j] == '4' && weaponPedestal != null)
                {
                    GameObject clone = Instantiate(weaponPedestal);
                    clone.transform.parent = transform;
                    clone.transform.localPosition = new Vector2(j, layout.Length - i - 1);

                    ignoreFields.Add(clone.transform.position);

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

