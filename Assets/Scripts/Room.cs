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
    public GameObject ground;
    public GameObject platform;
    string[] layout = new string[24];

    void Start ()
    {
        Load("assets/roomlayouts/type" + roomType + "_" + Random.Range(0, roomLayoutsCount) + ".txt");
        SpawnRoom(layout);
	}

    void SpawnRoom(string[] _layout)
    {
        for (int i = 0; i < layout.Length; i++)
        {
            for (int j = 0; j < layout[i].Length; j++)
            {
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
        // Create a new StreamReader, tell it which file to read and what encoding the file
        // was saved as
        StreamReader theReader = new StreamReader(fileName, Encoding.Default);
        // Immediately clean up the reader after this block of code is done.
        // You generally use the "using" statement for potentially memory-intensive objects
        // instead of relying on garbage collection.
        // (Do not confuse this with the using directive for namespace at the 
        // beginning of a class!)
        using (theReader)
        {
            // While there's lines left in the text file, do this:
            do
            {
                line = theReader.ReadLine();

                if (line != null)
                {
                    // Do whatever you need to do with the text line, it's a string now
                    // In this example, I split it into arguments based on comma
                    // deliniators, then send that array to DoStuff()
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
            // Done reading, close the reader and return true to broadcast success    
            theReader.Close();
            return true;
        }
    }
}

