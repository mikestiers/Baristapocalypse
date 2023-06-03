using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

enum milkBarColor
{
    green,
    red,
    brown,
    pink
}

public class MilkBarUI : MonoBehaviour
{
    [SerializeField] private GameObject hasProgressGameObject;
    [SerializeField] private Image milkBar;

    private IHasProgress hasProgress;

    // Start is called before the first frame update
    private void Start()
    {
        hasProgress = hasProgressGameObject.GetComponent<IHasProgress>();
        if(hasProgress != null)
        {
            hasProgress.OnProgressChanged += HasProgress_OnProgressChanged;
            milkBar.color = Color.green;
        }
        
    }

    private void HasProgress_OnProgressChanged(object sender, IHasProgress.OnProgressChangedEventArgs e)
    {
        switch (e.progressNormalized)
        {
            case (int)milkBarColor.green:
                milkBar.color = Color.green;
                break;
            case (int)milkBarColor.red:
                milkBar.color = Color.red;
                break;
            case (int)milkBarColor.brown:
                milkBar.color = Color.blue;
                break;
            case (int)milkBarColor.pink:
                milkBar.color = Color.yellow;
                break;
            default:
                milkBar.color = Color.white;
                break;

        }
    }

}
