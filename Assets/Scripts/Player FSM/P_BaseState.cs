using System.Collections.Generic;
using UnityEngine;

public class P_BaseState : P_State
{
    private bool moving;
    private AttributeModifier hurt;

    public override void EnterState(P_StateManager player)
    {
        player.movementSpeed = player.p_Att.GetBaseAttributeValue(player.p_Att.GetAttributeType("Move Speed"));
        inputFunction = Input.GetKeyDown;
        player.casting = null;
    }

    public override void UpdateState(P_StateManager player)
    {
        if (!moving)
        {
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
            else if (inputFunction(KeyCode.E))
            {
                hurt = new AttributeModifier() //deals 2 damage
                {
                    attribute = player.p_Att.GetAttributeType("HP"),
                    operation = AttributeModifier.Operator.Subtract,
                    attributeModifierValue = 2
                };
                player.p_Att.ApplyInstantModifier(hurt);

            }
            else if (inputFunction(KeyCode.Alpha1))
            {
                player.casting = player.spell0;
                if (player.p_Att.GetBaseAttributeValue(player.p_Att.GetAttributeType("MP")) >= player.casting.ability.manaCost)
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
                if (player.p_Att.GetBaseAttributeValue(player.p_Att.GetAttributeType("MP")) >= player.casting.ability.manaCost)
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
                if (player.p_Att.GetBaseAttributeValue(player.p_Att.GetAttributeType("MP")) >= player.casting.ability.manaCost)
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
                if (player.p_Att.GetBaseAttributeValue(player.p_Att.GetAttributeType("MP")) >= player.casting.ability.manaCost)
                {
                    player.SwitchState(player.abilityState);
                }
                else
                {
                    Debug.Log("Not enough MP");
                }
            }
        }
    }



    public override void ExitState(P_StateManager player)
    {

    }


    private IEnumerator<Vector2> Move(Vector2 direction, P_StateManager player)
    {
        moving = true;
        Vector2 startPosition = player.transform.position;
        Vector2 target = startPosition + (direction * player.gridSize);
        Vector2Int endPosition = player.gridManager.GetCellPosition(target);
        if (!player.gridManager.TraversableCheck(endPosition))
        {
            moving = false;
            yield break;
        }
        float elapsedTime = 0;
        while (elapsedTime < player.movementSpeed)
        {
            elapsedTime += Time.deltaTime;
            float percent = elapsedTime / player.movementSpeed;
            player.transform.position = Vector2.MoveTowards(startPosition, player.gridManager.GetTileCenter(endPosition), player.movementSpeed);
        }
        player.transform.position = target;
        player.lastMoveTime = player.lastMoveTime + player.movementSpeed;
        while (player.gameManager.globalTimer < player.lastMoveTime)
        {
            player.gameManager.incrementTime(player);
            player.gameManager.CheckAndActivateEntities();
            player.gameManager.UpdateTimedEntitiesList();
        }
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

    }
}

