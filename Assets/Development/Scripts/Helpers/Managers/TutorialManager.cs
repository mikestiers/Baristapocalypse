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
    public bool firstIngredientSelected;
    public bool secondIngredientSelected;
    public bool thirdIngredientSelected;
    public bool fourthIngredientSelected;
    public bool firstDrinkReady;
    public bool firstDrinkDelivered;

    // not implemented yet
    public bool drinkThreshold;
    public bool loitering;
    public bool gravityStorm;
    public bool radioComplaint;
    public bool tutorialComplete;

    public void Start()
    {
        if (BaristapocalypseMultiplayer.playMultiplayer)
        {
            tutorialEnabled = false;
            Destroy(this);
        }
    }

    public void TakeFirstOrder()
    {
        AISupervisor.Instance.SupervisorMessageToDisplay("Customer! Wait at the counter and take their order!");
        firstOrderTaken = true;
    }

    public void StartFirstBrew(Order order)
    {
        Debug.Log($"cusorder: {order.coffeeAttributes.GetSweetness()}");
        AISupervisor.Instance.SupervisorMessageToDisplay($"Look at their drink order. They want {order.coffeeAttributes.GetSweetness()} sweetness");
        AISupervisor.Instance.SupervisorMessageToDisplay($"Go over to the sweetener station and pick the right ingredient");
        firstBrewStarted = true;
    }

    public void MadeFirstIngredientSelection()
    {
        AISupervisor.Instance.SupervisorMessageToDisplay("Your ingredient was added to the brewing station. Now add more");
        firstIngredientSelected = true;
    }

    public void MadeSecondIngredientSelection()
    {
        AISupervisor.Instance.SupervisorMessageToDisplay("Excellent. They need 4 ingredients. Add from another ingredient station");
        secondIngredientSelected = true;
    }

    public void MadeThirdIngredientSelection()
    {
        AISupervisor.Instance.SupervisorMessageToDisplay("One more ingredient type to go");
        thirdIngredientSelected = true;
    }

    public void MadeFourthIngredientSelection()
    {
        AISupervisor.Instance.SupervisorMessageToDisplay("Go activate the brewing station and see how you did");
        fourthIngredientSelected = true;
    }

    public void FirstDrinkReady()
    {
        AISupervisor.Instance.SupervisorMessageToDisplay("Bring it to the customer!");
        firstDrinkReady = true;
    }

    public void FirstDrinkDelivered()
    {
        AISupervisor.Instance.SupervisorMessageToDisplay("Now do this forever until you rust and fall apart");
        firstDrinkDelivered = true;
    }
}
