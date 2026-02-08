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
    }

    public void Unbind(Story story)
    {
        //story.UnbindExternalFunction("endGame");
    }
}
