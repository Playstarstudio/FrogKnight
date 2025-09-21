using System.Security.Cryptography;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public float globalTimer;
    private GridManager gridManager;

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
