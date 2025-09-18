using System.Collections;
using UnityEngine;
using static UnityEngine.RuleTile.TilingRuleOutput;

public class PlayerMovement : MonoBehaviour
{
    private PlayerAttributes player;
    private int movementSpeed;
    private bool isRepeatedMovement = false;
    public bool moving = false;
    public GridManager grid;
    private float gridSize;
    private GameManager gameManager;
    private float lastMoveTime = 0f;

    void Start()
    {
        player = GetComponent<PlayerAttributes>();
        movementSpeed = player.speed;
        rb2d = GetComponent<Rigidbody2D>();
        gridSize = 1;
        gameManager = FindFirstObjectByType<GameManager>();
    }

    private Rigidbody2D rb2d;

    void Update()
    {
        if (!moving)
        {
            System.Func<KeyCode, bool> inputFunction;
            inputFunction = Input.GetKeyDown;
            if (inputFunction(KeyCode.W))
            {
                StartCoroutine(Move(Vector2.up));
            }
            else if (inputFunction(KeyCode.S))
            {
                StartCoroutine(Move(Vector2.down));
            }
            else if (inputFunction(KeyCode.A))
            {
                StartCoroutine(Move(Vector2.left));
            }
            else if (inputFunction(KeyCode.D))
            {
                StartCoroutine(Move(Vector2.right));
            }
        }
    }

    //movement between grid spaces
    private IEnumerator Move(Vector2 direction)
    {
        moving = true;
        Vector2 startPosition = transform.position;
        Vector2 target = startPosition + (direction * gridSize);
        Vector2Int endPosition = grid.GetCellPosition(target);
        if(!grid.TraversableCheck(endPosition))
        {
            moving = false;
            //bump
            yield break;
        }
        float elapsedTime = 0;
        while (elapsedTime < movementSpeed)
        {
            elapsedTime += Time.deltaTime;
            float percent = elapsedTime / movementSpeed;
            transform.position = Vector2.MoveTowards(startPosition, grid.GetTileCenter(endPosition), movementSpeed);
        }
        transform.position = target;
        while (gameManager.globalTimer < lastMoveTime + movementSpeed)
        {
            gameManager.incrementTime();
        }
        lastMoveTime = gameManager.globalTimer;
        moving = false;
    }


    void AStarMovement()
    {

    }

}