using UnityEngine;
using Ink.Runtime;
using System.Collections.Generic;
public class DialogueVariables : MonoBehaviour
{
    public Dictionary<string, Ink.Runtime.Object> variables;
    public Story globalVariablesStory;
    
    public DialogueVariables(TextAsset loadGlobalsJSON)  //ENABLE THIS IF THE GLOBAL FILE IS CAUSING ISSUES (and disable it as a main file in the editor)
    {
        // Compiles the global variables ink  script
        /*string inkFileContents = File.ReadAllText(globalsFilePath);
        Ink.Compiler compiler = new Ink.Compiler(inkFileContents);
        Story globalVariablesStory = compiler.Compile();*/

        // Initializes the dictionary
        Story globalVariablesStory = new Story(loadGlobalsJSON.text);
        variables = new Dictionary<string, Ink.Runtime.Object>();
        variables = new Dictionary<string, Ink.Runtime.Object>();
        foreach (string name in globalVariablesStory.variablesState)
        {
            Ink.Runtime.Object value = globalVariablesStory.variablesState.GetVariableWithName(name);
            variables.Add(name, value);
            Debug.Log("Initialized global dialogue variable: " + name + " = " + value);
        }
    }

    public void StartListening(Story story)
    {
        VariablesToStory(story);
        story.variablesState.variableChangedEvent += VariableChanged;
    }
    public void StopListening(Story story)
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

    private void VariablesToStory(Story story)
    {
        foreach(KeyValuePair<string, Ink.Runtime.Object> variable in variables)
        {
            story.variablesState.SetGlobal(variable.Key, variable.Value);
        }
    }
}
