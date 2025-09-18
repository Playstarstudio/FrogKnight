using System.Security.Cryptography;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public float globalTimer;

    private void Start()
    {
        globalTimer = 0f;
    }


    public float incrementTime()
    {
        globalTimer += 0.01f;
        return globalTimer;
    }
}
