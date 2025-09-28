using System;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Tilemaps;
using static UnityEngine.RuleTile.TilingRuleOutput;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField] AttributeSet player;
    [SerializeField] private float movementSpeed;
    AttributeModifier hurt;
    private bool isRepeatedMovement = false;
    private bool huurt = false;
    public bool moving = false;
    public GridManager gridManager;
    private float gridSize;
    private GameManager gameManager;
    private float lastMoveTime = 0f;


    void Start()
    {
        movementSpeed = player.GetCurrentAttributeValue(player.GetAttributeType("Move Speed"));
        rb2d = GetComponent<Rigidbody2D>();
        gridSize = 1;
        gameManager = FindFirstObjectByType<GameManager>();
        gridManager = FindFirstObjectByType<GridManager>();
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
            else if (inputFunction(KeyCode.E) && huurt == false)
            {
                hurt = new AttributeModifier() //deals 2 damage
                {
                    attribute = player.GetAttributeType("HP"),
                    operation = AttributeModifier.Operator.Subtract,
                    attributeModifierValue = 2
                };
                player.ApplyInstantModifier(hurt);

            }
        }
    }

    //movement between grid spaces
    private IEnumerator<Vector2> Move(Vector2 direction)
    {
        moving = true;
        Vector2 startPosition = transform.position;
        Vector2 target = startPosition + (direction * gridSize);
        Vector2Int endPosition = gridManager.GetCellPosition(target);
        if (!gridManager.TraversableCheck(endPosition))
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
            transform.position = Vector2.MoveTowards(startPosition, gridManager.GetTileCenter(endPosition), movementSpeed);
        }
        transform.position = target;
        while (gameManager.globalTimer < lastMoveTime + movementSpeed)
        {
            gameManager.incrementTime();
        }
        lastMoveTime = gameManager.globalTimer;
        Debug.Log("Start Pos:" + startPosition);
        Debug.Log("End Pos:" + endPosition);
        moving = false;
    }


    void AStarMovement()
    {

    }
    
    

}