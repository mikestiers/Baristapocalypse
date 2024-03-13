using UnityEngine;

public class RoombotConsole : Base
{
    [SerializeField] private CleanupBot bot;

    // Start is called before the first frame update
    public void Start()
    {
        //to just check if no roomba get 1 from scene
        if (bot == null) bot = FindObjectOfType<CleanupBot>();
    }

    public override void Interact(PlayerController player)
    {
        if (bot == null) return; //if roomba is not around for some reason
        switch(bot.currentState)
        {
            case CleanupBot.BotState.Empty:
                bot.currentState = CleanupBot.BotState.Roam;
                break;

            case CleanupBot.BotState.Roam:
                bot.currentState = CleanupBot.BotState.Empty;
                break;
        }
    }

}
