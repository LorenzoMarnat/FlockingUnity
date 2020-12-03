﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

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

    public GameObject text;

    public GameObject player;

    public State state;

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

    [Range(0, 5)]
    public float avoidWalls = 3f;

    [Range(0.1f,10)]
    public float neighborDist = 2;

    [Range(0, 5)]
    public float cohesionPlayer = 3f;

    [Range(0, 5)]
    public float alignmentPlayer = 3f;

    [Range(0, 5)]
    public float avoidPlayer = 3f;

    [Range(0.1f, 10)]
    public float playerDist = 2;

    // Start is called before the first frame update
    void Start()
    {
        state.SetState(0);

        CreateBoids();
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
            text.GetComponent<Text>().text = "Roam";
        }

        // Flock
        if (Input.GetKeyDown(KeyCode.Z))
        {
            state.SetState(1);
            text.GetComponent<Text>().text = "Flock";
        }

        // Follow
        if (Input.GetKeyDown(KeyCode.E))
        {
            state.SetState(2);
            text.GetComponent<Text>().text = "Follow";
        }

        // Fear
        if (Input.GetKeyDown(KeyCode.R))
        {
            state.SetState(3);
            text.GetComponent<Text>().text = "Fear";
        }
    }

}