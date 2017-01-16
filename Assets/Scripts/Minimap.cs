using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Minimap : MonoBehaviour
{
    Texture2D texture;
    Transform playerTransform;
    Vector2 playerCurrentRoom;
    Vector2 lastPlayerPos = new Vector2(999, 999);
    HashSet<Vector2> emptyFields = new HashSet<Vector2>();
    HashSet<Vector2> platformFields = new HashSet<Vector2>();
    HashSet<Vector2> pedestalFields = new HashSet<Vector2>();
    Vector2 savedTilePos;
    Color savedTileColor;
    public Color emptyTiles;
    public Color platforms;
    public Color tiles;
    public Color pedestals;
    public Color player;

	void Start ()
    {
        Texture2D _texture = new Texture2D(32, 32);
        _texture.filterMode = FilterMode.Point;
        GetComponent<RawImage>().texture = _texture;
        texture = _texture;
        playerTransform = GameObject.FindGameObjectWithTag("Player").transform;
	}

	void Update ()
    {
        if(playerTransform == null)
        {
            playerTransform = GameObject.FindGameObjectWithTag("Player").transform;
            return;
        }

        if(emptyFields.Count <= 0)
        {
            emptyFields.Clear();
            emptyFields.UnionWith(GenerateRooms.GR.allEmptySpaces);
        }

        if (platformFields.Count <= 0)
        {
            platformFields.Clear();
            platformFields.UnionWith(GenerateRooms.GR.allPlatformFields);
        }

        if(pedestalFields.Count <= 0)
        {
            pedestalFields.Clear();
            pedestalFields.UnionWith(GenerateRooms.GR.allPedestals);
        }

        if (emptyFields.Count <= 0 || platformFields.Count <= 0 || pedestalFields.Count <= 0)
            return;

        playerCurrentRoom = new Vector2((int) playerTransform.position.x / 16, (int) (playerTransform.position.y + 1) / 16);
        Vector2 roundedPlayerPos = new Vector2((int)playerTransform.position.x, (int)playerTransform.position.y + 1);

        if (playerCurrentRoom.x == 3)
            playerCurrentRoom.x = 2;

        if (playerCurrentRoom.y == 3)
            playerCurrentRoom.y = 2;

        if(playerCurrentRoom.x == 1)
            playerCurrentRoom.x = 0;

        if (playerCurrentRoom.y == 1)
            playerCurrentRoom.y = 0;

        if (lastPlayerPos != roundedPlayerPos)
        {
            for (int y = 0; y < texture.height; y++)
            {
                for (int x = 0; x < texture.width; x++)
                {
                    Vector2 tilePos = new Vector2(playerCurrentRoom.x * 16 + x, playerCurrentRoom.y * 16 + y - 1);
                    Color color = tiles;

                    if (emptyFields.Contains(tilePos))
                        color = emptyTiles;

                    if (platformFields.Contains(tilePos))
                        color = platforms;

                    if (pedestalFields.Contains(tilePos))
                        color = pedestals;

                    if (tilePos == roundedPlayerPos)
                        color = player;

                    texture.SetPixel(x, y, color);
                }
            }            
        }        

        texture.Apply();
        lastPlayerPos = roundedPlayerPos;
	}
}
