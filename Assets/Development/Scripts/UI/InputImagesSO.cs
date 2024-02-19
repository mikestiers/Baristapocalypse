using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "InputImagesSO-", menuName = "ScriptableObjects/InputImages")]
public class InputImagesSO : ScriptableObject
{
    public Sprite move;
    public Sprite interact;
    public Sprite interactAlt;
    public Sprite dash;
    public Sprite pickup;
    public Sprite release;
    public Sprite brewingStationSelectLeft;
    public Sprite brewingStationSelectRight;
    public Sprite brewingStationEmpty;
    public Sprite tutorialImage;
}
