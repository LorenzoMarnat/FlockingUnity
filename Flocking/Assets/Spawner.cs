using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spawner : MonoBehaviour
{
    public GameObject floor;
    // Start is called before the first frame update
    void Start()
    {
        CreateGrid();
    }
    private void CreateGrid()
    {
        for (int i = 0; i < 20; i++)
        {
            for (int j = 0; j < 20; j++)
            {
                GameObject tile;
                tile = Instantiate(floor, transform);
                tile.transform.position = new Vector2(i, j);
            }
        }
    }
    // Update is called once per frame
    void Update()
    {
        
    }
}
