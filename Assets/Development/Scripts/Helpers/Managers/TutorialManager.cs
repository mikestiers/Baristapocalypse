using System.Collections;
using System.Collections.Generic;
using System.Threading;
using Unity.VisualScripting;
using UnityEngine;

public class TutorialManager : Singleton<TutorialManager>
{
    public bool tutorialEnabled;
    public bool firstOrderTaken;
    public bool firstBrewStarted;
    public bool firstIngredientSelection;
    public bool secondIngredientSelection;
    public bool thirdIngredientSelection;
    public bool firstDrinkReady;
    public bool firstDrinkDelivered;
    public bool customerServed;
    public bool customerLeftReview;
    public bool addIngredient;
    public bool drinkThreshold;
    public bool loitering;
    public bool gravityStorm;
    public bool radioComplaint;
    public bool tutorialComplete;

    public void Start()
    {
        StartCoroutine(WaitAndExecute(12));
    }

    IEnumerator WaitAndExecute(int seconds)
    {
        yield return new WaitForSeconds(seconds);
        TakeFirstOrder();
    }

    public void Update()
    {
        if (!firstBrewStarted) MakeFirstBrew();
        if (!firstIngredientSelection) MakeFirstIngredientSelection();
        if (!secondIngredientSelection) MakeSecondIngredientSelection();
        if (!thirdIngredientSelection) MakeThirdIngredientSelection();
        if (!firstDrinkReady) FirstDrinkReady();
        
        // When the task to set order state = finished is complete this week, continue...
        //if (!firstDrinkDelivered) FirstDrinkDelivered();
    }

    void TakeFirstOrder()
    {
        AISupervisor.Instance.SupervisorMessageToDisplay("Customer! Wait at the counter and take their order!");
        firstOrderTaken = true;
    }

    public void MakeFirstBrew()
    {
        Debug.Log(OrderManager.Instance.GetFirstOrder());
        if (OrderManager.Instance.GetFirstOrder() == null)
            return;

        Debug.Log("Can make first brew");
        Debug.Log(OrderManager.Instance.GetFirstOrder().State);
        if (OrderManager.Instance.GetFirstOrder().State == Order.OrderState.Brewing)
        {
            AISupervisor.Instance.SupervisorMessageToDisplay("Ok....Pretty neat ingredient stations. Wonder what those do");
            firstBrewStarted = true;
        }
        
    }

    public void MakeFirstIngredientSelection()
    {
        BrewingStation[] brewingStations = FindObjectsOfType<BrewingStation>();
        foreach (BrewingStation brewingStation in brewingStations)
        {
            Debug.Log($"BrewingIngredients: {brewingStation.ingredientSOList.Count}");
            if (brewingStation.ingredientSOList.Count == 1)
            {
                AISupervisor.Instance.SupervisorMessageToDisplay("That better be the right ingredient! Now add the next!");
                firstIngredientSelection = true;
                return;
            }
        }
    }

    public void MakeSecondIngredientSelection()
    {
        BrewingStation[] brewingStations = FindObjectsOfType<BrewingStation>();
        foreach (BrewingStation brewingStation in brewingStations)
        {
            Debug.Log($"BrewingIngredients: {brewingStation.ingredientSOList.Count}");
            if (brewingStation.ingredientSOList.Count == 2)
            {
                AISupervisor.Instance.SupervisorMessageToDisplay("Don't mess this up! Add more!");
                secondIngredientSelection = true;
                return;
            }
        }
    }

    public void MakeThirdIngredientSelection()
    {
        BrewingStation[] brewingStations = FindObjectsOfType<BrewingStation>();
        foreach (BrewingStation brewingStation in brewingStations)
        {
            Debug.Log($"BrewingIngredients: {brewingStation.ingredientSOList.Count}");
            if (brewingStation.ingredientSOList.Count == 3)
            {
                AISupervisor.Instance.SupervisorMessageToDisplay("MOAR INGREDIENTS");
                thirdIngredientSelection = true;
                return;
            }
        }
    }

    public void FirstDrinkReady()
    {
        BrewingStation[] brewingStations = FindObjectsOfType<BrewingStation>();
        foreach (BrewingStation brewingStation in brewingStations)
        {
            if (brewingStation.ingredientSOList.Count == 4)
            {
                AISupervisor.Instance.SupervisorMessageToDisplay("Grab the drink! Bring it to the customer!");
                firstDrinkReady = true;
                return;
            }
        }
    }

    public void FirstDrinkDelivered()
    {
        AISupervisor.Instance.SupervisorMessageToDisplay("Now do this forever until you rust and fall apart");
        firstDrinkDelivered = true;
    }
}
