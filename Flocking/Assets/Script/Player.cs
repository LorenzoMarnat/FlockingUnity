using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    private Rigidbody2D rb;
    public int force = 5;

    private GameObject text;
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
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
    private void FixedUpdate()
    {
        float x = Input.GetAxis("Horizontal") * Time.fixedDeltaTime;
        float y = Input.GetAxis("Vertical") * Time.fixedDeltaTime;

        Vector2 movement = new Vector2(x*force, y*force);

        rb.transform.Translate(movement);

        OutOfBounds();
    }
}
