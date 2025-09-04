using System;
using UnityEngine;
using UnityEngine.Tilemaps;

// Cdde is good for now but Im not sure if we will want to change the movement system later to allow for a star based movement using a mouse click
public class PlayerController : MonoBehaviour
{
    // Time for the sprite to move from one grid point to the next
    public float moveSpeed = 5f;

    // The transform representing the target point to move towards
    public Transform movePoint;

    // Last direction pressed for buffering diagonal movement
    private Vector3 lastDirection = Vector3.zero;

    // Time to detect second key press for diagonal movement
    private float bufferTime = 0.15f;

    // Timer to track buffer duration
    private float bufferTimer = 0f;
    private bool isMoving = false;


    void Start()
    {
        // Endsure the movePoint is not a child of the player
        movePoint.parent = null;
        // Move movePoint to the player's starting position
        movePoint.position = transform.position;
    }

    void Update()
    {
        // Check if player has reached the movePoint
        if (isMoving && Vector3.Distance(transform.position, movePoint.position) <= 0.05f)
        {
            isMoving = false;
        }

        // Move the player towards the movePoint
        transform.position = Vector3.MoveTowards(transform.position, movePoint.position, moveSpeed * Time.deltaTime);

        // Only process input if not currently moving
        if (!isMoving && Vector3.Distance(transform.position, movePoint.position) <= 0.05f)
        {
            Vector3 direction = Vector3.zero;

            if (Input.GetKeyDown(KeyCode.W)) direction += Vector3.up;
            if (Input.GetKeyDown(KeyCode.S)) direction += Vector3.down;
            if (Input.GetKeyDown(KeyCode.A)) direction += Vector3.left;
            if (Input.GetKeyDown(KeyCode.D)) direction += Vector3.right;

            if (direction != Vector3.zero)
            {
                // If a key was just pressed start buffer
                if (bufferTimer > 0f && lastDirection != Vector3.zero)
                {
                    // Combine with last direction for diagonal
                    direction += lastDirection;
                    bufferTimer = 0f;
                    lastDirection = Vector3.zero;
                }
                else
                {
                    // Start buffer for possible diagonal
                    bufferTimer = bufferTime;
                    lastDirection = direction;
                    return;
                }
            }
            // If bufferTimer is still active  
            else if (bufferTimer > 0f)
            {
                // Tick down buffer time when player can move diagonally 
                bufferTimer -= Time.deltaTime;

                // If timer expires move in the last given direction
                if (bufferTimer <= 0f && lastDirection != Vector3.zero)
                {
                    // Players current cell
                    Vector2Int currentCell = GridManager.Instance.GetCellPosition(movePoint.position);

                    // Get the next cell by adding the direction of the given inputs  
                    Vector2Int nextCell = currentCell + new Vector2Int((int)lastDirection.x, (int)lastDirection.y);
                    Vector3 nextCellWorld = GridManager.Instance.GetWorldPosition(nextCell);

                    if (!GridManager.Instance.IsNonTraversable(nextCellWorld))
                    {
                        movePoint.position = nextCellWorld;
                        isMoving = true;
                    }
                    else
                    {
                        Debug.Log("Blocked by non-traversable tile!");
                    }
                }
                return;
            }
            if (direction != Vector3.zero)
            {
                // Players current cell
                Vector2Int currentCell = GridManager.Instance.GetCellPosition(movePoint.position);

                // Get the next cell by adding the direction of the given inputs  
                Vector2Int nextCell = currentCell + new Vector2Int((int)direction.x, (int)direction.y);
                Vector3 nextCellWorld = GridManager.Instance.GetWorldPosition(nextCell);

                if (!GridManager.Instance.IsNonTraversable(nextCellWorld))
                {
                    movePoint.position = nextCellWorld;
                    isMoving = true;
                }
                else
                {
                    Debug.Log("Blocked by non-traversable tile!");
                }
            }
        }
    }
}