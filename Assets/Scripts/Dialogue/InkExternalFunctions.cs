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

        story.BindExternalFunction("resetAreaTrigger", (bool state) => {
            Debug.Log(state);
            switch (state)
            {
                case true: //In here goes script to push the player back so the area trigger can occur again
                break;

                case false: //In here goes script to disable the area trigger
                break;
            }
        });
    }

    public void Unbind(Story story)
    {
        story.UnbindExternalFunction("endGame");
    }
}
