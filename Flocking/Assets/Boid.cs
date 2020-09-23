using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Boid : MonoBehaviour
{
    public Spawner spawner;

    public Vector2 velocity;
    public Vector2 acceleration;

    public int id;
    // Start is called before the first frame update
    void Start()
    {
        float angle = Random.Range(0, 2 * Mathf.PI);
        transform.rotation = Quaternion.Euler(new Vector3(0, 0, angle));
        velocity = new Vector2(Mathf.Cos(angle)*3, Mathf.Sin(angle)*3);
    }

    private float Distance(GameObject boid)
    {
        return Vector2.Distance(boid.transform.position, transform.position);
    }
    private void OutOfBounds()
    {
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
                nearbyBoids.Add(boid);
        }

        GameObject[] allWalls = GameObject.FindGameObjectsWithTag("Wall");

        List<GameObject> nearbyWalls = new List<GameObject>();

        foreach (GameObject wall in allWalls)
            if (Distance(wall) <= 2)
                nearbyWalls.Add(wall);

        bool nearPlayer = false;
        if (Distance(spawner.player) < spawner.playerDist)
            nearPlayer = true;

        Flock(nearbyBoids,nearbyWalls,nearPlayer);

        UpdateBoid();

        OutOfBounds();
    }

    private void UpdateBoid()
    {
        velocity += acceleration;
        velocity = LimitMagnitude(velocity, spawner.maxVelocity);

        transform.position += (Vector3)(velocity * Time.deltaTime);

        float angle = Mathf.Atan2(velocity.y, velocity.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(new Vector3(0, 0, angle));
    }

    private void Flock(List<GameObject> boids, List<GameObject> walls, bool player)
    {
        Vector2 avoidWalls = AvoidWalls(walls);

        // Si l'état est Roam
        if (spawner.state.GetState() == 0)
        {
            acceleration = avoidWalls * spawner.avoidWalls;
        }
        else
        {
            Vector2 alignment = Alignment(boids);
            Vector2 separation = Separation(boids);
            Vector2 cohesion = Cohesion(boids);
            // Si l'état est Follow
            if (spawner.state.GetState() == 2)
            {
                Vector2 alignmentPlayer = AlignmentPlayer(player);
                Vector2 cohesionPlayer = CohesionPlayer(player);
                acceleration = spawner.alignment * alignment + spawner.cohesion * cohesion + spawner.separation * separation + avoidWalls * spawner.avoidWalls + spawner.alignmentPlayer * alignmentPlayer + spawner.cohesionPlayer * cohesionPlayer;
            }
            // Si l'état est Flock
            else
                acceleration = spawner.alignment * alignment + spawner.cohesion * cohesion + spawner.separation * separation + avoidWalls * spawner.avoidWalls;
        }
    }

    private Vector2 AvoidWalls(List<GameObject> walls)
    {
        if(walls.Count < 1) return Vector2.zero;

        Vector2 direction = Vector2.zero;

        foreach (GameObject wall in walls)
        {
            Vector2 difference = (Vector2)transform.position - (Vector2)wall.transform.position;
            direction += difference.normalized / difference.magnitude;
        }
        direction /= walls.Count;

        Vector2 steer = Steer(direction.normalized * spawner.maxVelocity);
        return steer;

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
        direction /= sepBoids.Count;

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

    private Vector2 AlignmentPlayer(bool player)
    {
        if (!player) return Vector2.zero;

        Vector2 velocity = Vector2.zero;

        velocity += spawner.player.GetComponent<Rigidbody2D>().velocity;

        Vector2 steer = Steer(velocity.normalized * spawner.maxVelocity);
        return steer;
    }

    private Vector2 CohesionPlayer(bool player)
    {
        if (!player) return Vector2.zero;

        Vector2 direction = (Vector2)spawner.player.transform.position - (Vector2)transform.position;

        Vector2 steer = Steer(direction.normalized * spawner.maxVelocity);
        return steer;
    }
}
