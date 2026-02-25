using UnityEngine;
using Ink.Runtime;
using UnityEngine.SceneManagement;

public class InkExternalFunctions
{
    public void Bind(Story story, GameObject dialogueNPC)
    {
        story.BindExternalFunction("endGame", (bool state) => {
            Debug.Log(state);
            if (state == true) {SceneManager.LoadScene("Credits");}
        });

        story.BindExternalFunction("resetAreaTrigger", (bool state) => {
            Debug.Log(state);
            switch (state)
            {
                case true: 
                //In here goes script to push the player back so the area trigger can occur again & player can't advance
                break;

                case false: 
                //In here goes script to disable the area trigger
                dialogueNPC.SetActive(false);
                break;
            }
        });
    }

    public void Unbind(Story story) //stops the listening of the external functions
    {
        story.UnbindExternalFunction("endGame");
        story.UnbindExternalFunction("resetAreaTrigger");
    }
}
