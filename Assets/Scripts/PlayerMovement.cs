using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    private PlayerAttributes player;
    private int movementSpeed;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        player = GetComponent<PlayerAttributes> ();
        movementSpeed = player.speed;
        rb2d = GetComponent<Rigidbody2D> ();
    }

    private Rigidbody2D rb2d;


    void Update()
    {
        float moveHorizontal = Input.GetAxis ("Horizontal");
        float moveVertical = Input.GetAxis ("Vertical");

        rb2d.linearVelocity = new Vector2 (moveHorizontal*movementSpeed, moveVertical*movementSpeed);

        // Try out this delta time method??
        //rb2d.transform.position += new Vector3(speed * Time.deltaTime, 0.0f, 0.0f);
    }
}
