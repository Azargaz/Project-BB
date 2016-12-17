using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomObject : MonoBehaviour
{
    public static int width = 16;
    public static int height = 16;
    public GameObject[] backgroundTiles;
    public GameObject[] groundTiles;
    public List<Exit> exits = new List<Exit>();
    public List<Exit> localExits = new List<Exit>();
    public List<Vector3> occupiedTiles = new List<Vector3>();

    public void Awake()
    {
        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < height; j++)
            {
                Transform spawnedTile = Instantiate(backgroundTiles[Random.Range(0, backgroundTiles.Length)], transform).transform;
                spawnedTile.localPosition = new Vector2(i, j);
            }
        }
    }

    public void RefreshExits()
    {
        exits = new List<Exit>();
        localExits = new List<Exit>();
        occupiedTiles = new List<Vector3>();

        foreach (Transform child in transform)
        {
            occupiedTiles.Add(child.position);

            if (child.name.Contains("Exit"))
            {
                Exit.Direction tileDir;

                if (child.localPosition.x == 0)
                    tileDir = Exit.Direction.West;
                else if (child.localPosition.x == width - 1)
                    tileDir = Exit.Direction.East;
                else if (child.localPosition.y == 0)
                    tileDir = Exit.Direction.South;
                else
                    tileDir = Exit.Direction.North;

                exits.Add(new Exit { pos = child.position, dir = tileDir, exit = child.GetChild(0) });
                localExits.Add(new Exit { pos = child.localPosition, dir = tileDir });
            }
        }
    }

    [System.Serializable]
    public class Exit
    {
        public Vector3 pos;        
        public enum Direction { North, South, East, West };        
        public Direction dir;
        public Transform exit;

        public void BlockExit()
        {
            Debug.Log(exit.name);
            exit.gameObject.SetActive(true);
            Debug.Log(exit.gameObject.activeSelf);
        }
    }
}
