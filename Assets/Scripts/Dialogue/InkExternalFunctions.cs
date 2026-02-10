using UnityEngine;
using Ink.Runtime;

public class InkExternalFunctions
{
    public void Bind(Story story)
    {
        story.BindExternalFunction("endGame", (bool state) => {
            Debug.Log(state);
            if (state == true) {Application.Quit();}
        });

        story.BindExternalFunction("disableAreaTrigger", (bool state) => {
            Debug.Log(state);
            if (state == true) {}//In here goes script to disable the area trigger
        });

        story.BindExternalFunction("resetAreaTrigger", (bool state) => {
            Debug.Log(state);
            if (state == true) {}//In here goes script to push the player back so the area trigger can occur again
        });
    }

    public void Unbind(Story story)
    {
        story.UnbindExternalFunction("endGame");
    }
}
