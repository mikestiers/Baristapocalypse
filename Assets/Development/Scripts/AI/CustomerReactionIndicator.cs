using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CustomerReactionIndicator : MonoBehaviour
{
    
    [SerializeField] private GameObject AngryParticles;
    [SerializeField] private GameObject CryingParticles;
    [SerializeField] private GameObject Failparticles;
    [SerializeField] private GameObject SucceseParticles;
    [SerializeField] private float fadeInTime = 0.5f;
    
    private bool isSad;
    private void Awake()
    {
      
        DeactiveEffects();
    }

    public void DeactiveEffects()
    {
        AngryParticles.SetActive(false);
        CryingParticles.SetActive(false);
        Failparticles.SetActive(false);
        SucceseParticles.SetActive(false);
    }

    public void HappycCustomerEffect()
    {
        
        StartCoroutine(CustomerReceivedDrink());
    }

    private IEnumerator CustomerReceivedDrink()
    {
        SucceseParticles.SetActive(true);

        yield return new WaitForSeconds(7f);
        
        SucceseParticles.SetActive(false);
    }
    public void CustomerSad(bool b)
    {
        isSad = b;
        StartCoroutine(CustomercryingEffect(b));
    }

    private IEnumerator CustomercryingEffect(bool isSad)
    {
        if (isSad == true)
        {
            CryingParticles.SetActive(true);
        }
        else
        {
            CryingParticles.SetActive(false);
        }
        yield return null;
    }
    
    public void CustomerMad()
    {
        StartCoroutine(CustomerMadEffect());
    }

    private IEnumerator CustomerMadEffect()
    {
        AngryParticles.SetActive(true);
        yield return null;
    }
    public void CustomerDrinkFail()
    {
        StartCoroutine(CustomerFailEffect());
        
    }

    private IEnumerator CustomerFailEffect()
    {
        Failparticles.SetActive(true);
        yield return null;
    }



}
