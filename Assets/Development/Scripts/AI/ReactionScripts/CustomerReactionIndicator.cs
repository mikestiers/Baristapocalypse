using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public class CustomerReactionIndicator : MonoBehaviour
{ 
    //Text
    public CustomerReactionTextHappySO[] allReactionTextHappySO;
    public CustomerReactionTextSadSO[] allReactionTextSadSO;
    public CustomerReactionTextAngrySO[] allReactionTextAngrySO;
    //Images
    public CustomerReactionHappySO[] allReactionHappySO;
    public CustomerReactionSadSO[] allReactionSadSO;
    public CustomerReactionAngrySO[] allReactionAngrySO;
    [Header("ReactionSlots")]
    public Image happyImageSlot;
    public Image sadImageSlot;
    public Image angryImageSlot;
    [Header("TextReactionSlots")]
    public Text TextSlot;
    //public Text sadTextSlot;
    //public Text angryTextSlot;
    private void Start()
    {
        // Find all instances of CustomerReaction in the project
        allReactionTextHappySO = FindAllScriptableObjects<CustomerReactionTextHappySO>();
        allReactionTextSadSO = FindAllScriptableObjects<CustomerReactionTextSadSO>();
        allReactionTextAngrySO = FindAllScriptableObjects<CustomerReactionTextAngrySO>();
        allReactionHappySO = FindAllScriptableObjects<CustomerReactionHappySO>();
        allReactionSadSO = FindAllScriptableObjects<CustomerReactionSadSO>();
        allReactionAngrySO = FindAllScriptableObjects<CustomerReactionAngrySO>();

       // CustomerAngry();
       //CustomerHappy();
       // CustomerSad();
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
        CustomerReactionHappySO customerReactionHSO = allReactionHappySO[Random.Range(0,allReactionHappySO.Length)];
        happyImageSlot.sprite = customerReactionHSO.Image;
        happyImageSlot.gameObject.SetActive(true);
        CustomerReactionTextHappySO customerReactionHTSO = allReactionTextHappySO[Random.Range(0,allReactionTextHappySO.Length)];
        TextSlot.text = customerReactionHTSO.Text;
       // TextSlot.gameObject.SetActive(true);

        StartCoroutine(DeactivateImage()); 
    }


   private IEnumerator DeactivateImage()
   {
       // Wait for 3 seconds
       yield return new WaitForSeconds(3f);

        // Deactivate the images
        happyImageSlot.gameObject.SetActive(false);
        sadImageSlot.gameObject.SetActive(false);
        angryImageSlot.gameObject.SetActive(false);
        // Deactivate the text
       // TextSlot.gameObject.SetActive(false);
       // sadTextSlot.gameObject.SetActive(false);
       // angryTextSlot.gameObject .SetActive(false);
      
   }
   public void CustomerSad() 
   {
        CustomerReactionSadSO customerReactionSSO = allReactionSadSO[Random.Range(0, allReactionSadSO.Length)];
        sadImageSlot.sprite = customerReactionSSO.Image;
        sadImageSlot.gameObject.SetActive(true);
        CustomerReactionTextSadSO customerReactionSTSO = allReactionTextSadSO[Random.Range(0, allReactionTextSadSO.Length)];
        TextSlot.text = customerReactionSTSO.Text;
       // TextSlot.gameObject.SetActive(true);

        StartCoroutine(DeactivateImage());
    }

   public void CustomerAngry() 
   {
        CustomerReactionAngrySO customerReactionASO = allReactionAngrySO[Random.Range(0, allReactionAngrySO.Length)];
        angryImageSlot.sprite = customerReactionASO.Image;
        angryImageSlot.gameObject.SetActive(true);
        CustomerReactionTextAngrySO customerReactionATSO = allReactionTextAngrySO[Random.Range(0, allReactionTextAngrySO.Length)];
        TextSlot.text = customerReactionATSO.Text;
       // TextSlot.gameObject.SetActive(true);

        StartCoroutine(DeactivateImage());
    }

}
