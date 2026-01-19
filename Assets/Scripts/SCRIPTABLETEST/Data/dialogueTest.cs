using System.Collections.Generic;
using UnityEngine;
using Ink.Runtime;
using System;

[CreateAssetMenu(fileName = "dialogueTest", menuName = "Scriptable Objects/dialogueTest")]
public class dialogueTest : ScriptableObject
{
     public List<dialogueContainer> dialogueList;

     public dialogueContainer GetDialogueContainer(string tag)
    {
        foreach (var item in dialogueList)
        {
            if (item.tag == tag)
            {
                return item;
            } 
        }
        
        Debug.LogError($"{tag} was not found in dialogueList");
        return null;

    }
}

[Serializable]
public class dialogueContainer
{
    public string tag; 
    public TextAsset inkJSON;
    public Sprite portraitSpeaker;
    

}