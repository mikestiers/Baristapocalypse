using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public class CustomerReactionIndicator : MonoBehaviour
{
    public CustomerReactionTextSO[] allReactionTextSO;
    public CustomerReactionSO[] allReactionSO;
    public Image happyImageSlot;
    private void Start()
    {
        // Find all instances of CustomerReaction in the project
        allReactionTextSO = FindAllScriptableObjects<CustomerReactionTextSO>();
        allReactionSO = FindAllScriptableObjects<CustomerReactionSO>();

       
    }

    private T[] FindAllScriptableObjects<T>() where T : ScriptableObject
    {
        string[] guids = AssetDatabase.FindAssets("t:" + typeof(T).Name);
        T[] scriptableObjects = new T[guids.Length];

        for (int i = 0; i < guids.Length; i++)
        {
            string path = AssetDatabase.GUIDToAssetPath(guids[i]);
            scriptableObjects[i] = AssetDatabase.LoadAssetAtPath<T>(path);
        }

        return scriptableObjects;
    }
    public void CustomerHappy() 
    {
        CustomerReactionSO customerReactionSO = allReactionSO[Random.Range(0,allReactionSO.Length)];
        happyImageSlot.sprite = customerReactionSO.Image;
        happyImageSlot.gameObject.SetActive(true);

        StartCoroutine(DeactivateImage()); 
    }


   private System.Collections.IEnumerator DeactivateImage()
   {
       // Wait for 3 seconds
       yield return new WaitForSeconds(3f);

        // Deactivate the happy image

        happyImageSlot.gameObject.SetActive(false);
      
   }
   public void CustomerSad() 
   {
        

        StartCoroutine(DeactivateImage());
    }

   public void CustomerAngry() 
   {
      

      StartCoroutine(DeactivateImage());
   }

}
