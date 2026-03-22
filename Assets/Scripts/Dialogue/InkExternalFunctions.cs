using UnityEngine;
using Ink.Runtime;
using UnityEngine.SceneManagement;

public class InkExternalFunctions //Reads and calls functions from Ink dialogue
{
    public void Bind(Story story, GameObject dialogueNPC) //Listens for dialogue functions and executes them
    {
        /* The way each of these functions are set up are as follows:
           The quotations are the name of the Ink function being called
           It REQUIRES some variable to be passed through. If you just want to trigger
           an event, use a boolean and in the Ink dialogue call the function as true.

           Use the variables to then execute whatever Unity code you want. Just make sure
           to include it with the format provided below:

           story.BindExternalFunction("NameOfInkFunction", (varType varName) =>{ 
           //Insert code here
           });
        */

        //Function to end the game
        story.BindExternalFunction("endGame", (bool state) => {
            //Debug.Log(state);
            if (state == true) {SceneManager.LoadScene("Credits");}
        });

        //Function to control whether area trigger is deactivated or not after dialogue ends
        story.BindExternalFunction("resetAreaTrigger", (bool state) => {
            //Debug.Log(state);
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
