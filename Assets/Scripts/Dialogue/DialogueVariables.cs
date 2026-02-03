using UnityEngine;
using Ink.Runtime;
using System.Collections.Generic;
public class DialogueVariables
{
    public Dictionary<string, Ink.Runtime.Object> variables;
    public Story globalVariablesStory;
    
    public DialogueVariables(TextAsset loadGlobalsJSON) 
    {
        // Initializes the dictionary of variables
        globalVariablesStory = new Story(loadGlobalsJSON.text);
        variables = new Dictionary<string, Ink.Runtime.Object>();
        foreach (string name in globalVariablesStory.variablesState)
        {
            Ink.Runtime.Object value = globalVariablesStory.variablesState.GetVariableWithName(name);
            variables.Add(name, value);
            Debug.Log("Initialized global dialogue variable: " + name + " = " + value);
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

    private void VariableChanged(string name, Ink.Runtime.Object value)
    {
        // only maintain variables that were initialized from the globals ink file
        if (variables.ContainsKey(name))
        {
            variables.Remove(name);
            variables.Add(name, value);
        }
    }

    private void VariablesToStory(Story story) //pushes variable changes to global ink file
    {
        foreach(KeyValuePair<string, Ink.Runtime.Object> variable in variables)
        {
            story.variablesState.SetGlobal(variable.Key, variable.Value);
        }
    }
}
