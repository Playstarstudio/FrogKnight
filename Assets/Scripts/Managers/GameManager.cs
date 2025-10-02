using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using static UnityEngine.EventSystems.EventTrigger;

public class GameManager : MonoBehaviour
{
    public float globalTimer;
    private GridManager gridManager;
    public GameObject[] timedEntities;
    [SerializeField] private List<TimedEntity> sortedTimedEntities = new List<TimedEntity>();

    [System.Serializable]
    public class TimedEntity : IComparable<TimedEntity>
    {
        public GameObject entity;
        public float readyTime;
        public int CompareTo(TimedEntity other)
        {
            return readyTime.CompareTo(other.readyTime);
        }
    }

    private void Start()
    {
        globalTimer = 0f;
        gridManager = FindFirstObjectByType<GridManager>();
        InitializeTimedEntities();
    }

    private void InitializeTimedEntities()
    {
        timedEntities = GameObject.FindGameObjectsWithTag("Entity");
        sortedTimedEntities = timedEntities.Select(go =>
        {
            var enemy = go.GetComponent<Enemy>();
            return new TimedEntity
            {
                entity = go,
                readyTime = enemy != null ? enemy.readyTime : int.MaxValue
            };
        }).ToList();
        sortedTimedEntities.Sort();
    }

    public void CheckAndActivateEntities()
    {
        bool needsResort = false;
        for (int i = sortedTimedEntities.Count - 1; i >= 0; i--)
        {
            var timedEntity = sortedTimedEntities[i];
            var enemy = timedEntity.entity.GetComponent<Enemy>();
            if (timedEntity.readyTime <= globalTimer)
            {
                timedEntity.readyTime = timedEntity.readyTime + 0f; // Example: set it to the entity's action.
                ActivateEntity(timedEntity.entity);
                timedEntity.readyTime = enemy.readyTime;
                needsResort = true;
            }
            else
            {
                break;
            }
        }
        if (needsResort)
        {
            UpdateTimedEntitiesList();
        }
    }

    private void ActivateEntity(GameObject entity)
    {
        var enemy = entity.GetComponent<Enemy>();
        if (enemy != null)
        {
            enemy.move = true;
        }

        Debug.Log($"Activated entity: {entity.name} at time: {globalTimer}");
    }
    public void UpdateTimedEntitiesList()
    {
        foreach (var timedEntity in sortedTimedEntities)
        {
            var enemy = timedEntity.entity.GetComponent<Enemy>();
            if (enemy != null)
            {
                timedEntity.readyTime = enemy.readyTime;
            }
        }
        sortedTimedEntities.Sort();
    }
    public void OnEntityReadyTimeChanged(GameObject entity)
    {
        var existing = sortedTimedEntities.FirstOrDefault(te => te.entity == entity);
        if (existing != null)
        {
            var enemy = entity.GetComponent<Enemy>();
            if (enemy != null)
            {
                existing.readyTime = enemy.readyTime;
            }
            sortedTimedEntities.Sort();
        }
    }

    public void AddTimedEntity(GameObject entity)
    {
        var enemy = entity.GetComponent<Enemy>();
        if (enemy != null)
        {
            var newTimedEntity = new TimedEntity
            {
                entity = entity,
                readyTime = enemy.readyTime
            };
            sortedTimedEntities.Add(newTimedEntity);
            sortedTimedEntities.Sort();
        }
    }
    public float incrementTime()
    {
        globalTimer += 0.01f;
        gridManager.PlayerDijkstras();
        return globalTimer;
    }
}

/*
public class TimedEntity
{
    public GameObject TimedEntityID;
    public float ReadyTime;
}

public class TimedEntityID
{
    GameObject EntityID;

}

public class ReadyTime
{
    float Time;
}

// this list shows up in the Inspector and will be populated with default entities (by Reset function)
[SerializeField]
private List<TimedEntity> TimeList = new List<TimedEntity>();

private Dictionary<TimedEntityID, ReadyTime> _TimedEntityDictionary = new Dictionary<TimedEntityID, ReadyTime>();
public Dictionary<TimedEntityID, ReadyTime> TimedEntityDictionary { get => _TimedEntityDictionary; set => _TimedEntityDictionary = value; }
public void Reset()
{
    // clear any existing values
    TimeList.Clear();

    // Iterate through every AttributeType and create a new Attribute instance for each
    foreach (TimedEntityID EntityID in Resources.LoadAll<TimedEntityID>(""))
    {
        TimeList.Add(new TimedEntity { TimedEntityID = EntityID, ReadyTime = new ReadyTime(type.DefaultValue) });
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
    _TimedEntityDictionary = new Dictionary<AttributeType, Attribute>();
    foreach (var entry in attributes)
    {
        // This check prevents duplicate keys in case of user error.
        if (!TimedEntityDictionary.ContainsKey(entry.type))
        {
            TimedEntityDictionary.Add(entry.type, entry.value);
        }
    }
}

 */
