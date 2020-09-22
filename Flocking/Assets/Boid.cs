using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Boid : MonoBehaviour
{
    public Spawner spawner;

    public Rigidbody2D rb;

    private Vector2 velocity;

    public int id;
    // Start is called before the first frame update
    void Start()
    {
        rb = gameObject.GetComponent<Rigidbody2D>();
        rb.velocity = new Vector2(Random.Range(-spawner.maxVelocity, spawner.maxVelocity), Random.Range(-spawner.maxVelocity, spawner.maxVelocity));
    }

    private float Distance(GameObject boid)
    {
        //Debug.Log("B1  " + transform.position.x + "  " + transform.position.y);
        //Debug.Log("B2  " + boid.transform.position.x + "  " + boid.transform.position.y);
        return Mathf.Abs(Vector2.Distance(boid.transform.position, transform.position));
    }

    private void MoveCloser(List<GameObject> boids)
    {
        if (boids.Count < 1) return;

        Vector2 avg = new Vector2(0, 0);

        foreach(GameObject boid in boids)
        {
            avg += (Vector2)transform.position - (Vector2)boid.transform.position;
        }

        avg /= boids.Count;

        rb.velocity -= avg / spawner.moveCloser;
    }

    private void MoveWith(List<GameObject> boids)
    {
        if (boids.Count < 1) return;

        Vector2 avg = new Vector2(0, 0);

        foreach (GameObject boid in boids)
        {
            avg += (Vector2)transform.position - (Vector2)boid.transform.position;
        }

        avg /= boids.Count;

        rb.velocity += avg / spawner.moveWith;
    }

    private void MoveAway(List<GameObject> boids,float minDistance)
    {
        if (boids.Count < 1) return;

        Vector2 distance = new Vector2(0, 0);
        int numClose = 0;
        foreach (GameObject boid in boids)
        {
            float dist = Distance(boid);
            if(dist < minDistance)
            {
                numClose++;
                float xdiff = transform.position.x - boid.transform.position.x;
                float ydiff = transform.position.y - boid.transform.position.y;

                if (xdiff >= 0)
                    xdiff = Mathf.Sqrt(minDistance) - xdiff;
                else
                    xdiff = -Mathf.Sqrt(minDistance) - xdiff;

                if (ydiff >= 0)
                    ydiff = Mathf.Sqrt(minDistance) - ydiff;
                else
                    ydiff = -Mathf.Sqrt(minDistance) - ydiff;

                distance.x += xdiff;
                distance.y += ydiff;
            }
        }
        if (numClose == 0) return;

        rb.velocity -= distance / spawner.moveAway;
    }

    private void OutOfBounds()
    {
        if (transform.position.x > 19 || transform.position.y > 19 || transform.position.x < 1 || transform.position.y < 1)
            rb.velocity = new Vector2(-rb.velocity.x*2, -rb.velocity.y*2);
    }
    // Update is called once per frame
    void Update()
    {
        //velocity = new Vector2(0, 0);

        if(Mathf.Abs(rb.velocity.x) > spawner.maxVelocity || Mathf.Abs(rb.velocity.y) > spawner.maxVelocity)
        {
            float scaleFactor = spawner.maxVelocity / Mathf.Max(Mathf.Abs(rb.velocity.x), Mathf.Abs(rb.velocity.y));
            rb.velocity *= scaleFactor;
        }



        GameObject[] allBoids = GameObject.FindGameObjectsWithTag("Boid");

        List<GameObject> nearbyBoids = new List<GameObject>();

        foreach(GameObject boid in allBoids)
        {
            if (boid.GetComponent<Boid>().id == id)
                continue;

            if(Distance(boid) <= spawner.neighborDist)
            {
                //Debug.Log(Distance(boid));
                nearbyBoids.Add(boid);
            }
        }
        MoveCloser(nearbyBoids);
        MoveWith(nearbyBoids);
        MoveAway(nearbyBoids,spawner.minDist);
        OutOfBounds();
        //rb.velocity = velocity;
    }
}
