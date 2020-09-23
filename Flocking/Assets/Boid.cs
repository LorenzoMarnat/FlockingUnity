using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Boid : MonoBehaviour
{
    public Spawner spawner;

    //public Rigidbody2D rb;

    public Vector2 velocity;
    public Vector2 acceleration;

    public int id;
    // Start is called before the first frame update
    void Start()
    {
        /*
        rb = gameObject.GetComponent<Rigidbody2D>();
        rb.velocity = new Vector2(Random.Range(-spawner.maxVelocity, spawner.maxVelocity), Random.Range(-spawner.maxVelocity, spawner.maxVelocity));*/
        float angle = Random.Range(0, 2 * Mathf.PI);
        transform.rotation = Quaternion.Euler(new Vector3(0, 0, angle));
        velocity = new Vector2(Mathf.Cos(angle), Mathf.Sin(angle));
    }

    private float Distance(GameObject boid)
    {
        //Debug.Log("B1  " + transform.position.x + "  " + transform.position.y);
        //Debug.Log("B2  " + boid.transform.position.x + "  " + boid.transform.position.y);
        return Vector2.Distance(boid.transform.position, transform.position);
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

       // rb.velocity -= avg / spawner.moveCloser;
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

        //rb.velocity += avg / spawner.moveWith;
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

        //rb.velocity -= distance / spawner.moveAway;
    }

    private void OutOfBounds()
    {
        /*if (transform.position.x > 19 || transform.position.y > 19 || transform.position.x < 1 || transform.position.y < 1)
            rb.velocity = new Vector2(-rb.velocity.x*2, -rb.velocity.y*2);*/
        if (transform.position.x < -7.5f)
        {
            transform.position = new Vector2(26, transform.position.y);
        }

        if (transform.position.y < -0.5f)
        {
            transform.position = new Vector2(transform.position.x, 19);
        }

        if (transform.position.x > 26.5f)
        {
            transform.position = new Vector2(-7, transform.position.y);
        }

        if (transform.position.y > 19.5)
        {
            transform.position = new Vector2(transform.position.x, 0);
        }
    }

    /*private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "Wall")
            rb.velocity = new Vector2(-rb.velocity.x * 2, -rb.velocity.y * 2);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Wall")
            rb.velocity = new Vector2(-rb.velocity.x * 2, -rb.velocity.y * 2);
    }*/

    // Update is called once per frame
    void Update()
    {
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
        Flock(nearbyBoids);
        UpdateVelocity();
        UpdatePosition();
        UpdateRotation();
        /*MoveCloser(nearbyBoids);
        MoveWith(nearbyBoids);
        MoveAway(nearbyBoids,spawner.minDist);*/
        OutOfBounds();
    }

    private void Flock(List<GameObject> boids)
    {
        Vector2 alignment = Alignment(boids);
        Vector2 separation = Separation(boids);
        Vector2 cohesion = Cohesion(boids);

        acceleration = spawner.alignmentAmount * alignment + spawner.cohesionAmount * cohesion + spawner.separationAmount * separation;
    }

    private Vector2 Alignment(List<GameObject> boids)
    {
        Vector2 velocity = Vector2.zero;
        if (boids.Count < 1) return velocity;

        foreach (GameObject boid in boids)
        {
            velocity += boid.GetComponent<Boid>().velocity;
        }
        velocity /= boids.Count;

        Vector2 steer = Steer(velocity.normalized * spawner.maxVelocity);
        return steer;
    }

    private Vector2 Cohesion(List<GameObject> boids)
    {
        if (boids.Count < 1) return Vector2.zero;

        Vector2 sumPositions = Vector2.zero;
        foreach (GameObject boid in boids)
        {
            sumPositions += (Vector2)boid.transform.position;
        }
        Vector2 average = sumPositions / boids.Count;
        Vector2 direction = average - (Vector2)transform.position;

        Vector2 steer = Steer(direction.normalized * spawner.maxVelocity);
        return steer;
    }

    private Vector2 Separation(List<GameObject> boids)
    {
        Vector2 direction = Vector2.zero;

        List<GameObject> sepBoids = new List<GameObject>();
        foreach (GameObject boid in boids)
        {
            float dist = Distance(boid);
            if (dist < spawner.neighborDist)
            {
                sepBoids.Add(boid);
            }
        }
        
        if (sepBoids.Count < 1) return direction;

        foreach (GameObject boid in sepBoids)
        {
            Vector2 difference = (Vector2)transform.position - (Vector2)boid.transform.position;
            direction += difference.normalized / difference.magnitude;
        }
        direction /= boids.Count;

        Vector2 steer = Steer(direction.normalized * spawner.maxVelocity);
        return steer;
    }

    private Vector2 Steer(Vector2 desired)
    {
        Vector2 steer = desired - velocity;
        steer = LimitMagnitude(steer, spawner.maxForce);

        return steer;
    }
    private Vector2 LimitMagnitude(Vector2 baseVector, float maxMagnitude)
    {
        if (baseVector.sqrMagnitude > maxMagnitude * maxMagnitude)
        {
            baseVector = baseVector.normalized * maxMagnitude;
        }
        return baseVector;
    }
    public void UpdateVelocity()
    {
        velocity += acceleration;
        velocity = LimitMagnitude(velocity, spawner.maxVelocity);
    }

    private void UpdatePosition()
    {
        transform.position += (Vector3)(velocity * Time.deltaTime);
    }

    private void UpdateRotation()
    {
        float angle = Mathf.Atan2(velocity.y, velocity.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(new Vector3(0, 0, angle) /*+ baseRotation*/);
    }
}
