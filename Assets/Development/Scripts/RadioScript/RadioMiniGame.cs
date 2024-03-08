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
    [SerializeField] private Image rotatingImage;
    [SerializeField] private float minRotationAngle = -47.6f; // Minimum rotation angle
    [SerializeField] private float maxRotationAngle = 229.1f; // Maximum rotation angle

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
        // Set up slider
        slider.minValue = 0f;
        slider.maxValue = 25f;
        slider.wholeNumbers = true;
        slider.value = 0f; // Start at 0

        // Change text color to red for a randomly selected pair of TextMeshProUGUI objects
        ChangeRandomTextColorPair();
    }

    void Update()
    {
        // Check if the player hits Q or E to increment or decrement the slider value
        if (Input.GetKeyDown(KeyCode.Q))
        {
            MoveSlider(-1);
        }
        else if (Input.GetKeyDown(KeyCode.E))
        {
            MoveSlider(1);
        }

        // Update image rotation based on slider value
        float rotationAngle = Mathf.Lerp(minRotationAngle, maxRotationAngle, slider.normalizedValue);
        rotatingImage.transform.rotation = Quaternion.Euler(0f, 0f, rotationAngle);

        // Check if the slider value matches any of the goal values
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

    void MoveSlider(int direction)
    {
        slider.value = Mathf.Clamp(slider.value + direction, slider.minValue, slider.maxValue);
    }
}