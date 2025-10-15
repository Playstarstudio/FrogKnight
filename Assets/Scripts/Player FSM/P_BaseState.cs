using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.EventSystems.EventTrigger;

public class P_BaseState : P_State
{
    private bool moving;
    private AttributeModifier hurt;


    public override void EnterState(P_StateManager player)
    {
        player.gridManager.map[player.gridManager.GetCellPosition(player.transform.position)].occupied = true;
        player.movementSpeed = player.att.GetBaseAttributeValue(player.att.GetAttributeType("Move Speed"));
        inputFunction = Input.GetKeyDown;
        player.casting = null;
    }

    public override void UpdateState(P_StateManager player)
    {
        if (!moving)
        {
            // try moving
            if (inputFunction(KeyCode.W))
            {
                player.StartCoroutine(Move(Vector2.up, player));
            }
            else if (inputFunction(KeyCode.S))
            {
                player.StartCoroutine(Move(Vector2.down, player));
            }
            else if (inputFunction(KeyCode.A))
            {
                player.StartCoroutine(Move(Vector2.left, player));
            }
            else if (inputFunction(KeyCode.D))
            {
                player.StartCoroutine(Move(Vector2.right, player));
            }
            //hurts self 
            else if (inputFunction(KeyCode.E))
            {
                hurt = new AttributeModifier() //deals 2 damage
                {
                    attribute = player.att.GetAttributeType("HP"),
                    operation = AttributeModifier.Operator.Subtract,
                    attributeModifierValue = 2
                };
                player.att.ApplyInstantModifier(hurt);

            }
            // keyboard casting ability 1-4
            else if (inputFunction(KeyCode.Alpha1))
            {
                player.casting = player.spell0;
                if (player.att.GetBaseAttributeValue(player.att.GetAttributeType("MP")) >= player.casting.ability.manaCost)
                {
                    player.SwitchState(player.abilityState);
                }
                else
                {
                    Debug.Log("Not enough MP");
                }
            }
            else if (inputFunction(KeyCode.Alpha2))
            {
                player.casting = player.spell1;
                if (player.att.GetBaseAttributeValue(player.att.GetAttributeType("MP")) >= player.casting.ability.manaCost)
                {
                    player.SwitchState(player.abilityState);
                }
                else
                {
                    Debug.Log("Not enough MP");
                }
            }
            else if (inputFunction(KeyCode.Alpha3))
            {
                player.casting = player.spell2;
                if (player.att.GetBaseAttributeValue(player.att.GetAttributeType("MP")) >= player.casting.ability.manaCost)
                {
                    player.SwitchState(player.abilityState);
                }
                else
                {
                    Debug.Log("Not enough MP");
                }
            }
            else if (inputFunction(KeyCode.Alpha4))
            {
                player.casting = player.spell3;
                if (player.att.GetBaseAttributeValue(player.att.GetAttributeType("MP")) >= player.casting.ability.manaCost)
                {
                    player.SwitchState(player.abilityState);
                }
                else
                {
                    Debug.Log("Not enough MP");
                }
            }
            else if(inputFunction(KeyCode.Mouse0))
            {
                //Get distance from player
                Vector2Int targetCenter = player.gridManager.MouseToGrid();
                //Find the Dijkstra's node at this point
                    Debug.Log("Found node " + player.gridManager.PlayerDijkstra[targetCenter].rawDist);
                //print out node info
               
            }
        }
    }



    public override void ExitState(P_StateManager player)
    {

    }

    private IEnumerator<Vector2> Move(Vector2 direction, P_StateManager player)
    {
        //function for moving on a cadence
        moving = true;
        //takes current position
        Vector2 startPosition = player.transform.position;
        Vector2 target = startPosition + (direction * player.gridSize);
        Vector2Int endPosition = player.gridManager.GetCellPosition(target);
        //sets a cell location for the player to move to
        if (!player.gridManager.TraversableCheck(endPosition))
        {
            moving = false;
            yield break;
        }
        //sets a timer for movement speed, this prevents the player from moving every frame
        float elapsedTime = 0;
        while (elapsedTime < player.movementSpeed)
        {
            elapsedTime += Time.deltaTime;
            float percent = elapsedTime / player.movementSpeed;
            player.transform.position = Vector2.MoveTowards(startPosition, player.gridManager.GetTileCenter(endPosition), player.movementSpeed);
        }
        //sets player position to the center of the target tile
        player.transform.position = target;
        //sets tile player is on to occupied
        player.gridManager.map[endPosition].occupied = true;
        //sets the previous tile to unoccupied
        player.gridManager.map[player.gridManager.GetCellPosition(startPosition)].occupied = false;
        //sends over to the game manager to increment time based on movement speed
        player.gameManager.PlayerAction(player, player.movementSpeed);
        /*
        Debug.Log("Start Pos:" + startPosition);
        Debug.Log("End Pos:" + endPosition);
         */
        moving = false;
        // player.gameManager.CheckAndActivateEntities();
        // player.gameManager.UpdateTimedEntitiesList();
    }

    void AStarMovement()
    {
        throw new System.NotImplementedException();
    }
}

