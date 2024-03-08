using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class RadioMiniGame : MonoBehaviour
{
    [SerializeField] private Slider slider;
    [SerializeField] private float moveSpeed = 1f; // Speed at which the slider moves
    [SerializeField] private BoxCollider box;

    [System.Serializable]
    public class TextPair
    {
        public TextMeshProUGUI text1;
        public TextMeshProUGUI text2;
        public float goalValue; // Goal value associated with this pair

        // Constructor to initialize the pair with specific values
        public TextPair(TextMeshProUGUI t1, TextMeshProUGUI t2, float goal)
        {
            text1 = t1;
            text2 = t2;
            goalValue = goal;
        }
    }

    public List<TextPair> textPairs = new List<TextPair>();

    void Start()
    {
        
        

        // Change text color to red for a randomly selected pair of TextMeshProUGUI objects
        ChangeRandomTextColorPair();
    }

    void Update()
    {
        // Check if the player is within range of a hitbox
        if (Input.GetKeyDown(KeyCode.Q))
        {
            MoveSlider(-moveSpeed); // Move slider left
        }
        else if (Input.GetKeyDown(KeyCode.E))
        {
            MoveSlider(moveSpeed); // Move slider right
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            // Move slider to the goal value associated with this pair
            foreach (TextPair pair in textPairs)
            {
                if (Mathf.Approximately(slider.value, pair.goalValue))
                {
                    // Turn text color white for the pair
                    pair.text1.color = Color.white;
                    pair.text2.color = Color.white;
                }
            }
        }
    }

    void ChangeRandomTextColorPair()
    {
        if (textPairs.Count > 0)
        {
            // Choose a random index within the list
            int randomIndex = Random.Range(0, textPairs.Count);

            // Change the text color of both TextMeshProUGUI objects in the randomly selected pair to red
            textPairs[randomIndex].text1.color = Color.red;
            textPairs[randomIndex].text2.color = Color.red;
        }
        else
        {
            Debug.LogError("Text Pair List is empty!");
        }
    }

    void MoveSlider(float direction)
    {
        slider.value = Mathf.Clamp01(slider.value + direction);
    }
}