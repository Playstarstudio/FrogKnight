using NUnit.Framework.Interfaces;
using System.Collections.Generic;
using System.Security.Cryptography;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public float globalTimer;
    private GridManager gridManager;
    /*
    public class TimedEntity
    {
        public GameObject TimedEntityID;
        public float ReadyTime;
    }

    // this list shows up in the Inspector and will be populated with default entities (by Reset function)
    [SerializeField]
    private List<TimedEntity> TimeList = new List<TimedEntity>();

    private Dictionary<TimedEntityID, ReadyTime> _TimedEntityDictionary = new Dictionary<EntityID, TimedEntity, ReadyTime>();
    public Dictionary<AttributeType, Attribute> TimedEntityDictionary { get => _TimedEntityDictionary; set => _TimedEntityDictionary = value; }
    public void Reset()
    {
        // clear any existing values
        TimeList.Clear();

        // Iterate through every AttributeType and create a new Attribute instance for each
        foreach (AttributeType type in Resources.LoadAll<AttributeType>(""))
        {
            TimeList.Add(new AttributeEntry { type = type, value = new Attribute(type.DefaultValue) });
        }
    }
    private void Awake()
    {
        // ensure the dictionary is populated before gameplay.
        InitializeTimedEntityDictionary();
    }

    /// <summary>
    /// Initializes the dictionary from the serialized list.
    /// </summary>
    public void InitializeTimedEntityDictionary()
    {
        _attributeDictionary = new Dictionary<AttributeType, Attribute>();
        foreach (var entry in attributes)
        {
            // This check prevents duplicate keys in case of user error.
            if (!attributeDictionary.ContainsKey(entry.type))
            {
                attributeDictionary.Add(entry.type, entry.value);
            }
        }
    }

     */

    private void Start()
    {
        globalTimer = 0f;
        gridManager = FindFirstObjectByType<GridManager>();
    }


    public float incrementTime()
    {
        globalTimer += 0.01f;
        gridManager.PlayerDijkstras();
        return globalTimer;
    }
}
