using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;

public class CustomerReview : MonoBehaviour
{
    private static int averageReviewScore = 0;
    private static int numReviews = 0;
    private int cafeCleanliness;
    private float cafeCrowdedness;
    private float timeToServe;
    private int reviewScore = 0;
    private string reviewText;
    private CustomerBase orderOwner;
    public int ReviewScore
    {
        get { return reviewScore; }
    }

    public string ReviewText
    {
        get { return reviewText; }
    }

    public CustomerBase GetOrderOwner()
    {
        return orderOwner;
    }

    public void GenerateReview(CustomerBase customer)
    {
        orderOwner = customer;
        cafeCleanliness = GetCafeCleanliness();
        cafeCrowdedness = GetCafeCrowd();
        timeToServe = GetTimeToServe();
        reviewScore = CalculateReviewScore(cafeCleanliness, cafeCrowdedness, timeToServe);
        reviewText = GenerateReviewText(reviewScore);
    }

    private int GetCafeCleanliness()
    {
        // Did we run out of tags?
        //GameObject[] garbageToCleanUp = GameObject.FindGameObjectsWithTag("Garbage");
        GameObject[] spillsToCleanUp = GameObject.FindGameObjectsWithTag("Mess");
        //GameObject[] itemsToCleanUp = spillsToCleanUp.Concat(garbageToCleanUp).ToArray();
        cafeCleanliness = spillsToCleanUp.Length;
        return cafeCleanliness;
    }

    private float GetCafeCrowd()
    {
        int customerCount = CustomerManager.Instance.TotalCustomers();
        int maxCustomers = CustomerManager.Instance.TotalMaxCapacity();
        cafeCrowdedness = customerCount / maxCustomers;
        return cafeCrowdedness;
    }

    private float GetTimeToServe()
    {
        timeToServe = orderOwner.orderTimer;
        return timeToServe;
    }

    private int CalculateReviewScore(int cafeCleanliness, float cafeCrowdedness, float timeToServe)
    {
        int reviewScore = 0; // Initialize the local variable

        // Adjust score based on cafeCleanliness
        switch (cafeCleanliness)
        {
            case 0:
                reviewScore += 3;
                break;
            case 1:
                reviewScore += 2;
                break;
            case 2:
                reviewScore += 1;
                break;
            default:
                if (cafeCleanliness >= 3)
                    reviewScore += 0; // Not necessary as it doesn't change the score, but kept for clarity
                break;
        }

        // Adjust score based on cafe crowdedness
        if (cafeCrowdedness > 50.0f)
            reviewScore += 1;
        else if (cafeCrowdedness <= 50.0f)
            reviewScore += 0;

        // Adjust score based on how long it took to serve the customer
        if (timeToServe < 45.0f)
            reviewScore += 1;

        numReviews++;
        averageReviewScore += reviewScore;
        return reviewScore;
    }

    private string GenerateReviewText(int reviewScore)
    {
        string cafeDescription;

        switch (reviewScore)
        {
            case 0:
                cafeDescription = "is terrible";
                break;
            case 1:
                cafeDescription = "kind of sucks";
                break;
            case 2:
                cafeDescription = "is pretty mid bro";
                break;
            case 3:
                cafeDescription = "has potential";
                break;
            case 4:
                cafeDescription = "made my day";
                break;
            default:
                if (reviewScore >= 5)
                    cafeDescription = "is amazing";
                else
                {
                    Debug.Log($"Invalid review score {reviewScore}");
                    cafeDescription = " has an invalid score";
                }
                break;
        }

        string reviewText = $"This place {cafeDescription}";
        return reviewText;
    }

    public static int GetAverageReviewScore()
    {
        if (numReviews != 0)
            return averageReviewScore / numReviews;
        else
            return 1;
    }
}
