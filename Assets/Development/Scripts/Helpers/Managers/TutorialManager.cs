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

    private const string TutorialKey = "FirstTimePlayed";

    public void Start()
    {
        //Checks for PlayersPrefs TutorialKey if it is 0 or 1 and sets the tutorial on for first time players
        if (!PlayerPrefs.HasKey(TutorialKey))
        {
            PlayerPrefs.SetFloat(TutorialKey, 0);
        }
        else
        {
            if (PlayerPrefs.GetFloat(TutorialKey) == 0)
            {
                tutorialEnabled = true;
            }
            else if (PlayerPrefs.GetFloat(TutorialKey) == 1)
            {
                tutorialEnabled = false;
                UIManager.Instance.FlipTutorialText();
            }
        }

        if (BaristapocalypseMultiplayer.playMultiplayer)
        {
            tutorialEnabled = false;
            Destroy(this);
        }

    }

    //Used only to change the PlayPref TutorialKey for the first time players and turns it off the next time game is played
    public void TutorialTurnedOff()
    {
        PlayerPrefs.SetFloat(TutorialKey, 1);
    }

    public void TakeFirstOrder()
    {
        SoundManager.Instance.PlayOneShot(SoundManager.Instance.audioClipRefsSO.tutorialPopOut);
        AISupervisor.Instance.SupervisorMessageToDisplay("Customer! Wait at the counter and take their order!");
        firstOrderTaken = true;
    }

    public void StartFirstBrew(OrderInfo order)
    {
        SoundManager.Instance.PlayOneShot(SoundManager.Instance.audioClipRefsSO.tutorialPopOut);
        AISupervisor.Instance.SupervisorMessageToDisplay($"Pick the right ingredient from the four ingredient stations");
        firstBrewStarted = true;
    }

    public void MadeFirstIngredientSelection()
    {
        SoundManager.Instance.PlayOneShot(SoundManager.Instance.audioClipRefsSO.tutorialPopOut);
        AISupervisor.Instance.SupervisorMessageToDisplay("Your ingredient was added to the brewing station. Try adding more");
        firstIngredientSelected = true;
    }

    public void MadeSecondIngredientSelection()
    {
        SoundManager.Instance.PlayOneShot(SoundManager.Instance.audioClipRefsSO.tutorialPopOut);
        AISupervisor.Instance.SupervisorMessageToDisplay("Excellent. Try adding from another ingredient station");
        secondIngredientSelected = true;
    }

    public void MadeThirdIngredientSelection()
    {
        SoundManager.Instance.PlayOneShot(SoundManager.Instance.audioClipRefsSO.tutorialPopOut);
        AISupervisor.Instance.SupervisorMessageToDisplay("Almost done!");
        thirdIngredientSelected = true;
    }

    public void MadeFourthIngredientSelection()
    {
        SoundManager.Instance.PlayOneShot(SoundManager.Instance.audioClipRefsSO.tutorialPopOut);
        AISupervisor.Instance.SupervisorMessageToDisplay("Go activate the brewing station and see how you did");
        fourthIngredientSelected = true;
    }

    public void FirstDrinkReady()
    {
        SoundManager.Instance.PlayOneShot(SoundManager.Instance.audioClipRefsSO.tutorialPopOut);
        AISupervisor.Instance.SupervisorMessageToDisplay("Bring it to the customer!");
        firstDrinkReady = true;
    }

    public void FirstDrinkDelivered()
    {
        SoundManager.Instance.PlayOneShot(SoundManager.Instance.audioClipRefsSO.tutorialPopOut);
        AISupervisor.Instance.SupervisorMessageToDisplay("Now do this forever until you rust and fall apart");
        firstDrinkDelivered = true;
    }
}
