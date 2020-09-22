using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spawner : MonoBehaviour
{
    public GameObject floorSprite;

    public GameObject boidSPrite;

    public int boidsCount;

    [Range(0.1f,40)]
    public float maxVelocity;

    public int moveCloser;

    public int moveWith;

    public float moveAway;

    [Range(0.1f,10)]
    public float neighborDist;

    [Range(0.1f,2)]
    public float minDist;

    // Start is called before the first frame update
    void Start()
    {
        CreateGrid();
        CreateBoids();
    }
    private void CreateGrid()
    {
        for (int i = 0; i < 20; i++)
        {
            for (int j = 0; j < 20; j++)
            {
                GameObject tile;
                tile = Instantiate(floorSprite, transform);
                tile.transform.position = new Vector2(i, j);
            }
        }
    }

    private void CreateBoids()
    {
        for(int i= 0; i<boidsCount; i++)
        {
            GameObject boid = Instantiate(boidSPrite, transform);
            boid.transform.position = new Vector2(Random.Range(1, 19), Random.Range(1, 19));
            boid.GetComponent<Boid>().spawner = this;
            boid.GetComponent<Boid>().id = i;
        }
    }
    // Update is called once per frame
    void Update()
    {
        
    }
}
