using System.Collections.Generic;
using UnityEngine;
using static GridManager;

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
            /*
            else if (inputFunction(KeyCode.E))
            {
                hurt = new AttributeModifier() //deals 2 damage
                {
                    attribute = player.att.GetAttributeType("HP"),
                    operation = AttributeModifier.Operator.Subtract,
                    attributeModifierValue = 2
                };
                player.att.ApplyInstantModifier(hurt);

            }*/
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
            else if (inputFunction(KeyCode.Mouse0))
            {
                //Get distance from player
                Vector2Int targetCenter = player.gridManager.MouseToGrid();
                //Find the Dijkstra's node at this point
                //Debug.Log("Found " + targetCenter);
                //Debug.Log("Found node. Distance = " + player.gridManager.ManhattanDistanceToTile(player.gridManager.GetCellPosition(player.gameObject.transform.position), targetCenter) + " " + player.gridManager.map[targetCenter].occupied);
                TileInfo tile;
                player.gridManager.map.TryGetValue(targetCenter, out tile);
                //player.gridManager.ReturnTileData(targetCenter);
                Debug.Log("Traversability = " + tile.traversable + " LoS = " + tile.LoS + " Wall = " + tile.wall + " VisionBlocking = " + tile.isVisionBlocking + " occupied = " + tile.occupied + " Visible = " + tile.visible);
                //Debug.Log("Wall Check = " + tile.wall);
                //Debug.Log("Visibility Check = " + tile.visible);
                //print out node info
            }
            else if (inputFunction(KeyCode.I))
            {
                /*

                foreach (ItemSO item in player.inventory)
                {
                    if (item != null)
                    {
                        applyItemEffect(player, item);
                    }
                }
                 */
            }
            else if (Input.GetButtonDown("Inventory"))
            {
                if (player.inventoryManager.InventoryCheck())
                {
                    player.SwitchState(player.inventoryState);
                }
            }
            else if (Input.GetButtonDown("Dialogue")) //This should check if you can initiate dialogue
            {
                if (DialogueManager.instance.dialogueCheck())
                {
                    player.SwitchState(player.dialogueState);
                }
            }
        }
    }

    private static void applyItemEffect(Entity owner, ItemSO item)
    {
        foreach (var effect in item.effects)
        {
            owner.att.GetAttributeType(effect.attributeName);
            if (owner.att.GetAttributeType(effect.attributeName) == null)
            {
                Debug.Log("Attribute " + effect.attributeName + " not found on " + owner.name);
                continue;
            }
            else
            {
                AttributeModifier applyItemModifier = new AttributeModifier()
                {
                    attribute = owner.att.GetAttributeType(effect.attributeName),
                    operation = (AttributeModifier.Operator)effect.operation,
                    attributeModifierValue = effect.modifierValue
                };
                owner.att.ApplyInstantModifier(applyItemModifier);
                Debug.Log("Applied " + effect.operation + " " + effect.modifierValue + " to " + effect.attributeName);
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
        // Debug.Log(player.gridManager.map.TryGetValue(player.gridManager.GetCellPosition(startPosition), out TileInfo tile));
        /*
        if (player.gridManager.map.TryGetValue(player.gridManager.GetCellPosition(target), out GridCell cell))
        {
                moving = false;
                yield break;
        }
         */
        Vector2Int endPosition = player.gridManager.GetCellPosition(target);

        //sets a cell location for the player to move to
        if (player.gridManager.map[endPosition].occupied)
        {//HAVE TO FINISH THIS
            Collider2D[] colliders = Physics2D.OverlapCircleAll(player.gridManager.GetTileCenter(endPosition), 0.1f);
            for (int i = 0; i < colliders.Length; i++)
            {
                Entity entity = colliders[i].GetComponent<Entity>();
                if (entity != null)
                {
                    entity.ReceiveEffect(player.spell0.ability.abilityEffects[0], player, player.spell0.ability);
                }
                if (entity = null)
                {
                    Debug.Log("nothing to interact with");
                }
            }
            moving = false;
            yield break;
        }
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
        player.transform.position = player.gridManager.GetTileCenter(endPosition);
        player.gridManager.MapMoveEntity(player, player.gridManager.GetCellPosition(startPosition), endPosition);
        //player.transform.position = new Vector3(endPosition.x, endPosition.y, 0);
        //sets tile player is on to occupied
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

