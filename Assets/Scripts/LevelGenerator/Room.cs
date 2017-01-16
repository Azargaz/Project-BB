using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Room : MonoBehaviour
{
    public int roomType = 1;
    int roomSize = 0;
    public GameObject background;
    GameObject ground;
    GameObject platform;
    GameObject spikes;
    GameObject weaponPedestal;
    public GameObject exit;
    bool exitSpawned;
    public string[] layout = new string[16];
    public List<Vector2> emptySpaces = new List<Vector2>();
    public List<Vector2> ignoreFields = new List<Vector2>();
    public List<Vector2> weaponPedestals = new List<Vector2>();
    public List<Vector2> platformsFields = new List<Vector2>();
    bool blocked;
    List<GameObject> blockade = new List<GameObject>();
    public ImportRoomLayouts RT;

    void Awake()
    {
        if(RT != null)
        {
            background = RT.background;
            ground = RT.ground;
            platform = RT.platform;
            spikes = RT.spikes;
            weaponPedestal = RT.weaponPedestal;
            exit = RT.exit;

            ImportRoomLayouts.Room thisRoomType = RT.roomTypes[roomType];
            roomSize = thisRoomType.roomSize;
            
            layout = thisRoomType.layouts[Random.Range(0, thisRoomType.layouts.Count)].layout;
        }

        SpawnRoom(layout);
    }

    public void BlockRoom()
    {
        if (blocked)
            return;

        blocked = true;

        for (int i = 0; i < roomSize; i++)
        {
            for (int j = 0; j < roomSize; j++)
            {
                if((i == 0 || i == roomSize - 1) && (j != 0 && j != roomSize - 1))
                {
                    GameObject clone = Instantiate(ground, transform);
                    clone.transform.localPosition = new Vector2(i, j);

                    blockade.Add(clone);
                }

                if ((j == 0 || j == roomSize - 1) && (i != 0 && i != roomSize - 1))
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

        int infinityBreak = 0;
        do
        {
            infinityBreak++;
            if (infinityBreak > 10)
            {
                Debug.LogError("Couldn't spawn exit, infinite loop in SpawnExit()");
                break;
            }

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
                    weaponPedestals.Add(clone.transform.position);

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
}