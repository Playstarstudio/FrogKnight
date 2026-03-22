using UnityEngine;
using Ink.Runtime;
using System.Collections.Generic;
public class DialogueVariables //Reads and updates Unity variables to reflect Ink variable changes
{
    public Dictionary<string, Ink.Runtime.Object> variables;
    public Story globalVariablesStory;
    
    public DialogueVariables(TextAsset loadGlobalsJSON) // Initializes the dictionary of variables
    {
        globalVariablesStory = new Story(loadGlobalsJSON.text); //Grabs the story from the JSON
        variables = new Dictionary<string, Ink.Runtime.Object>();
        foreach (string name in globalVariablesStory.variablesState) //iterates and gets each variable
        {
            Ink.Runtime.Object value = globalVariablesStory.variablesState.GetVariableWithName(name);
            variables.Add(name, value);
            //Debug.Log("Initialized global dialogue variable: " + name + " = " + value);
        }
    }

    public void StartListening(Story story) //Enables variable changes
    {
        VariablesToStory(story);
        story.variablesState.variableChangedEvent += VariableChanged;
    }
    public void StopListening(Story story) //Disables variable changes
    {
        story.variablesState.variableChangedEvent -= VariableChanged;
    }

    private void VariableChanged(string name, Ink.Runtime.Object value) //Updates dictionary with new variable values
    {
        if (variables.ContainsKey(name)) //If statement makes it so you only maintain variables that were initialized from the globals ink file
        {
            variables.Remove(name);
            variables.Add(name, value);
        }
    }

    private void VariablesToStory(Story story) //Pushes variable changes to global ink file
    {
        foreach(KeyValuePair<string, Ink.Runtime.Object> variable in variables)
        {
            story.variablesState.SetGlobal(variable.Key, variable.Value);
        }
    }
}
