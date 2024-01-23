using UnityEngine;
using UnityEngine.UI;

[CreateAssetMenu()]
public class IngredientSO : ScriptableObject
{
    public GameObject prefab;
    public string objectName;
    public string objectTag;
    public int temperature;
    public int sweetness;
    public int spiciness;
    public int strength;
    //public Image icon;
}