using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CustomerReactionIndicator : MonoBehaviour
{
    
    [SerializeField] private Image happyImage;
    [SerializeField] private Image sadImage;
    [SerializeField] private Image angryImage;

    private void Awake()
    {
        CustomerHappy();
    }

    public void CustomerHappy() 
   {
      happyImage.gameObject.SetActive(true);

      StartCoroutine(DeactivateImage()); 
   }


   private System.Collections.IEnumerator DeactivateImage()
   {
       // Wait for 3 seconds
       yield return new WaitForSeconds(3f);

       // Deactivate the happy image
       happyImage.gameObject.SetActive(false);
       sadImage.gameObject.SetActive(false);
       angryImage.gameObject.SetActive(false);
   }
   public void CustomerSad() 
   {
        sadImage.gameObject.SetActive(true);

        StartCoroutine(DeactivateImage());
    }

   public void CustomerAngry() 
   {
      angryImage.gameObject.SetActive(true);

      StartCoroutine(DeactivateImage());
   }

}
