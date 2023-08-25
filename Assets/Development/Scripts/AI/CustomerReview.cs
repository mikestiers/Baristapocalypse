using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CustomerReview : MonoBehaviour
{
    private int cafeCleanliness;
    private string customerMood; // use customerBase.customerMood when implemented
    private bool favouriteIngredientServed;
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
        Debug.Log("GenerateReview()");
        orderOwner = customer;
        cafeCleanliness = GetCafeCleanliness();
        customerMood = GetCustomerMood();
        favouriteIngredientServed = GetCustomerFavouriteIngredient();
        reviewScore = CalculateReviewScore(cafeCleanliness, customerMood, favouriteIngredientServed);
        reviewText = GenerateReviewText(reviewScore);
        Debug.Log("this is review text");
    }

    private int GetCafeCleanliness()
    {
        GameObject[] itemsToCleanUp = GameObject.FindGameObjectsWithTag("Player"); //change to "cleanup" or whatever when messes are ready
        cafeCleanliness = itemsToCleanUp.Length;
        return cafeCleanliness;
    }

    private string GetCustomerMood()
    {
        //customerMood = customerBase.GetCustomerMood(); // when GetCustomerMood() is implemented
        customerMood = "mad";
        return customerMood;
    }

    private bool GetCustomerFavouriteIngredient()
    {
        //favouriteIngredientServed = customerBase.GetFavouriteIngredient(); // when GetFavouriteIngredient is implemented
        favouriteIngredientServed = true;
        return favouriteIngredientServed;
    }

    private int CalculateReviewScore(int cafeCleanliness, string customerMood, bool favouriteIngredientServed)
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

        // Adjust score based on customerMood
        switch (customerMood.ToLower()) // Case insensitive match
        {
            case "mad":
                reviewScore += 0; // Not necessary as it doesn't change the score, but kept for clarity
                break;
            case "happy":
                reviewScore += 1;
                break;
            default:
                // Optionally handle unexpected mood values here
                break;
        }

        // Adjust score based on favouriteIngredientServed
        if (favouriteIngredientServed)
            reviewScore += 1;

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
}
