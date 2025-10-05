using System.Drawing;
using System.Runtime.InteropServices.WindowsRuntime;
using UnityEngine;
using UnityEngine.TextCore.Text;

[CreateAssetMenu(fileName = "Ability", menuName = "Scriptable Objects/Ability")]
public class Ability : ScriptableObject
{
    public string abilityName;
    public string description;
    public float manaCost;
    public int range;
    public float speed;
    public float coolDown;

    public virtual bool CastAbility(P_StateManager player, Vector2 targetPosition)
    {
        // Base spell logic (e.g., reduce mana)
        player.castSuccess = false;
        float mana = player.p_Att.GetBaseAttributeValue(player.p_Att.GetAttributeType("MP"));
        if (mana >= manaCost)
        {
            AttributeModifier manaCostModifier = new AttributeModifier()
            {
                attribute = player.p_Att.GetAttributeType("MP"),
                operation = AttributeModifier.Operator.Subtract,
                attributeModifierValue = manaCost
            };
            player.p_Att.ApplyInstantModifier(manaCostModifier);
            Debug.Log($"{abilityName} cast towards {targetPosition}");
            getTargets(targetPosition);

            // create new timed effect for cooldown here
            // find player's readytime
            //affect player's ready time based on spell speed
            //
            // find final readytime for this spell to be available again
            player.castSuccess = true;
            return true;
        }
        else
        {
            Debug.Log("Not enough MP to cast the ability.");
            player.castSuccess = false;
            return false;
        }
    }
    void getTargets(Vector2 center)
    {
        Collider2D[] colliders = Physics2D.OverlapPointAll(center);
        foreach (Collider2D collider in colliders)
        {
            Debug.Log("Found an entity at " + center + ": " + collider.gameObject.name);
        }
        foreach (Collider2D collider in colliders)
        {
            if(collider.gameObject.CompareTag("Entity"))
            {
                Destroy(collider.gameObject);
            }
        }
        if(colliders.Length == 0)
        {
            Debug.Log("No targets hit.");
        }
    }
}
