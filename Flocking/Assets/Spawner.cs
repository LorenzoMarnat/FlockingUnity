using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spawner : MonoBehaviour
{
    public GameObject floorSprite;

    //public GameObject wallSprite;

    public GameObject boidSprite;

    public int boidsCount;

    [Range(0.1f,10)]
    public float maxVelocity;

    [Range(.1f, .5f)]
    public float maxForce = .03f;

    /*public int moveCloser;

    public int moveWith;

    public float moveAway;*/
    [Range(0, 3)]
    public float separationAmount = 1f;

    [Range(0, 3)]
    public float cohesionAmount = 1f;

    [Range(0, 3)]
    public float alignmentAmount = 1f;

    [Range(0.1f,10)]
    public float neighborDist;

    /*[Range(0.1f,2)]
    public float minDist;*/

    // Start is called before the first frame update
    void Start()
    {
        CreateGrid();
        CreateBoids();
    }
    private void CreateGrid()
    {
        for (int i = -7; i < 27; i++)
        {
            for (int j = 0; j < 20; j++)
            {
                GameObject tile;
                tile = Instantiate(floorSprite, transform);
                tile.transform.position = new Vector2(i, j);
            }
        }
        /*for(int i = 0;i<20;i++)
        {
            GameObject tile;
            tile = Instantiate(wallSprite, transform);
            tile.transform.position = new Vector2(i, -1);

            tile = Instantiate(wallSprite, transform);
            tile.transform.position = new Vector2(i, 20);

            tile = Instantiate(wallSprite, transform);
            tile.transform.position = new Vector2(-1, i);

            tile = Instantiate(wallSprite, transform);
            tile.transform.position = new Vector2(20, i);
        }*/
    }

    private void CreateBoids()
    {
        for(int i= 0; i<boidsCount; i++)
        {
            GameObject boid = Instantiate(boidSprite, transform);
            boid.transform.position = new Vector2(Random.Range(1, 19), Random.Range(1, 19));
            boid.transform.rotation = Quaternion.identity;
            boid.GetComponent<Boid>().spawner = this;
            boid.GetComponent<Boid>().id = i;
        }
    }
    // Update is called once per frame
    void Update()
    {
        
    }
}
