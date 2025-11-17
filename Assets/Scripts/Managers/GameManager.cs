using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using static UnityEngine.EventSystems.EventTrigger;

public class GameManager : MonoBehaviour
{
    public float globalTimer;
    /*
    public P_StateManager player {get; private set; }
     */

    private GridManager gridManager;

    // this is my list of timed entities that will be populated, and the sorted list kept separately.
    public GameObject[] timedEntities;
    [SerializeField] private List<TimedEntity> sortedTimedEntities = new List<TimedEntity>();

    [System.Serializable]
    //created a list of timed entities as a comparable list for sorting purposes.
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
        gridManager.PlayerDijkstras();
    }
    // goes out and finds all entity objects. I will probably redo this so that it grabs all entities of all kinds
    // TODO this doesn't account for entities being added or removed during gameplay.
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

    //CALL THIS EVERY TIME AN ACTION OCCURS
    public void PlayerAction(Entity player, float _time)
    {
        player.gridManager.PlayerDijkstras();
        player.lastMoveTime += _time;
        globalTimer = player.lastMoveTime;
        CheckAndActivateEntities(player, _time);
    }
    public void TimeTracker(GameObject gameObject, float time)
    {
        if (gameObject.tag == "Entity")
        {
            Enemy enemy = gameObject.GetComponent<Enemy>();
            enemy.readyTime += time;
            OnEntityReadyTimeChanged(enemy);
        }
    }

    // iterates through sorted list and finds entities that are ready to go
    // TODO this doesn't account for if an enemy gets to do two actions before another enemy gets to do one.
    public void CheckAndActivateEntities(Entity player, float _time)
    {
        bool needsResort = false;
        for (int i = 0; i <= sortedTimedEntities.Count - 1; i++)
        {
            //Debug.Log($"Checking entity: {sortedTimedEntities[i].entity.name} with ready time: {sortedTimedEntities[i].readyTime} against global time: {globalTimer}");
            var timedEntity = sortedTimedEntities[i];
            var enemy = timedEntity.entity.GetComponent<Enemy>();
            if (enemy.readyTime <= globalTimer)
            {
                player.gridManager.PlayerDijkstras();
                ActivateEntity(timedEntity.entity, _time, globalTimer);
                timedEntity.readyTime = enemy.readyTime;
                // timedEntity.readyTime = timedEntity.readyTime + 0f; // Example: set it to the entity's action.
                needsResort = true;
            }
            else
            {
                // Since the list is sorted, we can break early
                break;
            }
        }
        if (needsResort)
            UpdateTimedEntitiesList(player,_time);
    }

    //sends over to the enemy script and does what it's supposed to do.
    private void ActivateEntity(GameObject entity, float _time, float _globalTimer)
    {
        var enemy = entity.GetComponent<Enemy>();
        if (enemy != null)
        {
            enemy.Activate(_time, _globalTimer);
        }
        // Debug.Log($"Activated entity: at time: {globalTimer}");
    }

    //resort the list and realign readytimes.
    public void UpdateTimedEntitiesList(Entity player, float _time)
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
        CheckAndActivateEntities(player, _time);
    }

    /*
    // this is for incrementing time, is kind of out of date.
    public float incrementTime(P_StateManager player)
    {
        globalTimer = player.lastMoveTime;
        gridManager.PlayerDijkstras();
        CheckAndActivateEntities(player);
        return globalTimer;
    }
    */
    // if an entity's ready time changes, this updates the list and resorts it.
    // this is used when the player stuns an entity or slows it down.
    public void OnEntityReadyTimeChanged(Enemy entity)
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
    // adding a timed entity to the list - this is for spawning or slow spells that travel over time.
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

    public void RemoveTimedEntity(GameObject entity)
    {
        var existing = sortedTimedEntities.FirstOrDefault(te => te.entity == entity);
        if (existing != null)
        {
            sortedTimedEntities.Remove(existing);
        }
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
