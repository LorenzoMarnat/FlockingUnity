using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spawner : MonoBehaviour
{
    public struct State
    {   
        private int currentState { get; set; }
        public void SetState(int state) 
        {
            currentState = state;
        }
        public int GetState()
        {
            return currentState;
        }
    }
    public State state;

    public GameObject floorSprite;

    public GameObject boidSprite;

    public int boidsCount = 10;

    [Range(0.1f,10)]
    public float maxVelocity;

    [Range(.1f, .5f)]
    public float maxForce = .03f;

    [Range(0, 3)]
    public float separation = 1f;

    [Range(0, 3)]
    public float cohesion = 1f;

    [Range(0, 3)]
    public float alignment = 1f;

    [Range(0, 50)]
    public float avoidWalls = 10f;

    [Range(0.1f,10)]
    public float neighborDist;

    // Start is called before the first frame update
    void Start()
    {
        state.SetState(0);

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

    private void Update()
    {
        // Roam
        if (Input.GetKeyDown(KeyCode.A))
        {
            state.SetState(0);
        }

        // Flock
        if (Input.GetKeyDown(KeyCode.Z))
        {
            state.SetState(1);
        }
    }

}
