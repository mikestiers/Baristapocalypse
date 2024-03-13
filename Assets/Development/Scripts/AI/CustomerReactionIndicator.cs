using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.VFX;

public class CustomerReactionIndicator : MonoBehaviour
{
    [SerializeField] private GameObject AngryParticles;
    [SerializeField] private GameObject CryingParticles;
    [SerializeField] private GameObject Failparticles;
    [SerializeField] private GameObject SucceseParticles;
    [SerializeField] private float fadeInTime = 0.5f;
    private bool hasDrink;
    private void Awake()
    {
      
       DeActiveEffects();
    }

    private void DeActiveEffects()
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
        hasDrink = b;
        StartCoroutine(CustomercryingEffect(hasDrink));
    }

    private IEnumerator CustomercryingEffect(bool hasDrink)
    {
        if (hasDrink == true)
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

    public void IsLoitering()
    {
        AngryParticles.SetActive(false);
        CryingParticles.SetActive(false);
        SucceseParticles.SetActive(false);
        Failparticles.SetActive(false);
    }
}
