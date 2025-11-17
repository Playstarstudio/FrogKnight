using UnityEngine;

[CreateAssetMenu(fileName = "Modifier", menuName = "Scriptable Objects/Modifier")]
public class Modifier : ScriptableObject
{
    public enum Operator
    {
        Add,
        Subtract,
        Multiply,
        Divide,
        Set
    }
    public string attributeName;
    public static AttributeType attName;
    public float modifierValue;
    public Operator operation;
    public static AttributeModifier.Operator _operation;
    public void Awake()
    {
        _operation = (AttributeModifier.Operator)operation;
    }
}
