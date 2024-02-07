using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "AISupervisorMessagesScriptableObjects", menuName = "ScriptableObjects/AISupervisorMessasges")]
public class AISupervisorMessagesSO : ScriptableObject
{
    public string tutorialEnabled;
    public string customerOrdered;
    public string customerServed;
    public string customerLeftReview;
    public string addIngredient;
    public string drinkThreshold;
    public string loitering;
    public string gravityStorm;
    public string radioComplaint;
    public string tutorialComplete;
}
