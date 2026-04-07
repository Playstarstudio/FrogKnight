using UnityEngine;

public class P_DeathState : P_State
{
    DeathScreen deathScreen;
    public override void EnterState(P_StateManager player)
    {
        //deathScreen = UnityEngine.Object.FindFirstObjectByType<DeathScreen>();
        //GameObject deathScreen = GameObject.Find("DeathScreenUI");
        DeathScreen deathScreen= Object.FindFirstObjectByType<DeathScreen>(FindObjectsInactive.Include);
        deathScreen.gameObject.SetActive(true);
    }

    public override void UpdateState(P_StateManager player)
    {
    }
}