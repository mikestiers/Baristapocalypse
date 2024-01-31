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
    private float cafeCleanliness;
    private float cafeCrowdedness;
    private float timeToServe;
    private float drinkScore;
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

    //Calculates the score for the drink giving to the customer (Max 20)
    public float OrderScore(CustomerBase customer)
    {
        string difficulty = GameManager.Instance.currentDifficulty.difficultyString;
        float orderScore = 0;

        if (difficulty == "Medium")
        {

        }
        else if (difficulty == "Hard")
        {

        }
        else //default or easy setting
        {
            float score = 0;

            float tempDif = customer.coffeeAttributes.GetTemperature() - customer.GetIngredient().GetComponent<CoffeeAttributes>().GetTemperature();
            float sweetDif = customer.coffeeAttributes.GetSweetness() - customer.GetIngredient().GetComponent<CoffeeAttributes>().GetSweetness();
            float spicyDif = customer.coffeeAttributes.GetSpiciness() - customer.GetIngredient().GetComponent<CoffeeAttributes>().GetSpiciness();
            float strengthDif = customer.coffeeAttributes.GetStrength() - customer.GetIngredient().GetComponent<CoffeeAttributes>().GetStrength();

            Debug.Log("Temp dif: " + tempDif + ", Sweet dif: " + sweetDif + ", Spicy dif: " + spicyDif + ", Strength dif: " + strengthDif);

            //Calculate Temperture difference
            if (tempDif == 0)
                score += 5;
            else if (tempDif <= 1 && tempDif >= -1)
                score += 4;
            else if (tempDif <= 2 && tempDif >= -2)
                score += 3;
            else if (tempDif <= 3 && tempDif >= -3)
                score += 2;
            else if (tempDif <= 4 && tempDif >= -4)
                score += 1;
            else
            {
                //do nothing
            }

            //Calculate Sweetness difference
            if (sweetDif == 0)
                score += 5;
            else if (sweetDif <= 1 && sweetDif >= -1)
                score += 4;
            else if (sweetDif <= 2 && sweetDif >= -2)
                score += 3;
            else if (sweetDif <= 3 && sweetDif >= -3)
                score += 2;
            else if (sweetDif <= 4 && sweetDif >= -4)
                score += 1;
            else
            {
                //do nothing
            }

            //Calculate Spicyiness difference
            if (spicyDif == 0)
                score += 5;
            else if (spicyDif <= 1 && spicyDif >= -1)
                score += 4;
            else if (spicyDif <= 2 && spicyDif >= -2)
                score += 3;
            else if (spicyDif <= 3 && spicyDif >= -3)
                score += 2;
            else if (spicyDif <= 4 && spicyDif >= -4)
                score += 1;
            else
            {
                //do nothing
            }

            //Calculate Strength difference
            if (strengthDif == 0)
                score += 5;
            else if (strengthDif <= 1 && strengthDif >= -1)
                score += + 4;
            else if (strengthDif <= 2 && strengthDif >= -2)
                score += 3;
            else if (strengthDif <= 3 && strengthDif >= -3)
                score += 2;
            else if (strengthDif <= 4 && strengthDif >= -4)
                score += 1;
            else
            {
                //do nothing
            }
            orderScore = score;
        }
        Debug.Log("OrderScore: " + orderScore);
        return orderScore;
    }
    public void GenerateReview(CustomerBase customer)
    {
        orderOwner = customer;
        drinkScore = OrderScore(customer);
        cafeCleanliness = GetCafeCleanliness();
        cafeCrowdedness = GetCafeCrowd();
        timeToServe = GetTimeToServe();
        reviewScore = CalculateReviewScore(cafeCleanliness, cafeCrowdedness, timeToServe, drinkScore);
    }

    //Calculates cafe cleanliness and assigns a score (Max 20)
    private float GetCafeCleanliness()
    {
        float cleanlinessScore = 0;
        // Did we run out of tags?
        //GameObject[] garbageToCleanUp = GameObject.FindGameObjectsWithTag("Garbage");
        GameObject[] spillsToCleanUp = GameObject.FindGameObjectsWithTag("Mess");
        //GameObject[] itemsToCleanUp = spillsToCleanUp.Concat(garbageToCleanUp).ToArray();
        cafeCleanliness = spillsToCleanUp.Length;
        //Adjust score based on cleanliness
        switch (cafeCleanliness)
        {
            case 0:
                cleanlinessScore += 20;
                break;
            case 1:
                cleanlinessScore += 15;
                break;
            case 2:
                cleanlinessScore += 10;
                break;
            case 3:
                cleanlinessScore += 5;
                break;
            default:
                if (cafeCleanliness >= 4)
                    cleanlinessScore += 0; // Not necessary as it doesn't change the score, but kept for clarity
                break;
        }
        return cleanlinessScore;
    }

    //Calculates a score based on amount of customer/total customers (max 20)
    private float GetCafeCrowd()
    {
        float crowdednessScore = 0;
        float customerCount = CustomerManager.Instance.TotalCustomers();
        float maxCustomers = CustomerManager.Instance.TotalMaxCapacity();
        cafeCrowdedness = customerCount / maxCustomers * 100;
        Debug.Log("Crowd %: " +cafeCrowdedness);
        if(cafeCrowdedness >= 80)
        {
            crowdednessScore = 0;
        }
        else if (cafeCrowdedness >= 60)
        {
            crowdednessScore = 5;
        }
        else if (cafeCrowdedness >= 40)
        {
            crowdednessScore = 10;
        }
        else if (cafeCrowdedness >= 20)
        {
            crowdednessScore = 15;
        }
        else
        {
            crowdednessScore = 20;
        }
        return crowdednessScore;
    }

    //Caculates a score based on how long it took the customer to get served (max 20)
    private float GetTimeToServe()
    {
        float timeScore = 0;
        float timeToServe = orderOwner.orderTimer != null ? (float)orderOwner.orderTimer : 0;
        Debug.Log("ServeTime: " + timeToServe);
        if (timeToServe <= 20)
        {
            timeScore = 20;
        }
        else if (timeToServe <= 40)
        {
            timeScore = 15;
        }
        else if (timeToServe <= 60)
        {
            timeScore = 10;
        }
        else if (timeToServe <= 90)
        {
            timeScore = 5;
        }
        else
        {
            timeScore = 0;
        }
        return timeScore;
    }

    private int CalculateReviewScore(float cafeCleanliness, float cafeCrowdedness, float timeToServe, float drinkScore)
    {
        int reviewScore = 0; // Initialize the local variable
        float totalScore = 0;
        Debug.Log($"Cleanliness: {cafeCleanliness} Crowdedness: {cafeCrowdedness} Time: {timeToServe} Drink: {drinkScore}");

        //find the lowest score of all scores
        List<float> scoreList = new List<float> { cafeCleanliness, cafeCrowdedness, timeToServe, drinkScore};
        float minScore = scoreList.Min();

        //check for multiple of the lowest score
        List<int> minIndices = new List<int>();
        for (int i = 0; i < scoreList.Count; i++)
        {
            if (scoreList[i] == minScore)
            {
                minIndices.Add(i);
            }
        }

        //if there are multiple with the same score it will choose randomly between them
        System.Random random = new System.Random();
        int chosenIndex = minIndices[random.Next(minIndices.Count)];
        float chosenScore = scoreList[chosenIndex];

        Debug.Log($"The minimum score is {chosenScore} at index {chosenIndex}");
        //choose review text here 
        if (chosenIndex == 0)
        {
            if (chosenScore <= 5)
                reviewText = "SO GROSS!!! Ew! This place is nasty!";
            else if (chosenScore <= 10)
                reviewText = "Acceptable I guess, could use a mopping once and a while.";
            else if (chosenScore <= 15)
                reviewText = "Meh, could be cleaner tbh smh";
            else if (chosenScore < 20)
                reviewText = "Nice and tidy. No complaints";
            else
                reviewText = "Sparkling clean! Could see myself in the reflection of the floor!";
        }
        else if (chosenIndex == 1)
        {
            if (chosenScore <= 5)
                reviewText = "No good seats. Too many liotering youngsters!";
            else if (chosenScore <= 10)
                reviewText = "Hard to find a seat. No one seems to be leaving?";
            else if (chosenScore <= 15)
                reviewText = "A very busy place but I managed to find a good seat!";
            else if (chosenScore < 20)
                reviewText = "This place always has hundreds of people at the front but lots of room?? How?";
            else
                reviewText = "Plenty of room to enjoy my morning brew! Nice work Milky Way!";
        }
        else if (chosenIndex == 2)
        {
            if (chosenScore <= 5)
                reviewText = "Ugh!! I was completely forgotten about!";
            else if (chosenScore <= 10)
                reviewText = "Took the hunk-a-junk forever just to make ONE drink for me...";
            else if (chosenScore <= 15)
                reviewText = "These robots take so long! Aren't they supposed to be faster?";
            else if (chosenScore < 20)
                reviewText = "Quick service, very in and out!";
            else
                reviewText = "Wow! It's like they had it premade, just for meeee :)";
        }
        else if (chosenIndex == 3)
        {
            if (chosenScore <= 5)
                reviewText = "Not what I wanted at all!! Ruined my day!1!1!!";
            else if (chosenScore <= 10)
                reviewText = "I mean I guess this is my drink?? Smells weird tho-";
            else if (chosenScore <= 15)
                reviewText = "Tastes good, not a bad flavour.";
            else if (chosenScore < 20)
                reviewText = "What a brilliant combination! They should make this a permanent menu item.";
            else
                reviewText = "Exactly what I was looking for!! Perfect perfect perfect!!";
        }

        //add up scores for final total score and convert into a star amount (max 80 points, max 5 star)
        totalScore = cafeCleanliness + cafeCrowdedness + timeToServe + drinkScore;
        if (totalScore == 80)
        {
            reviewScore = 5;
        }
        else if(totalScore >= 60)
        {
            reviewScore = 4;
        }
        else if(totalScore >= 40)
        {
            reviewScore = 3;
        }
        else if(totalScore >= 20)
        {
            reviewScore = 2;
        }
        else
        {
            reviewScore = 1;
        }
        numReviews++;
        averageReviewScore += reviewScore;
        //add gain money
        GameManager.Instance.moneySystem.AdjustMoneyByAmount(GameManager.Instance.moneySystem.ComputeMoney(reviewScore), true);
        GameManager.Instance.moneySystem.SetAverageReviewRating(GetAverageReviewScore());
        return reviewScore;
    }

    /*private string GenerateReviewText(int reviewScore)
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
    }*/

    public static int GetAverageReviewScore()
    {
        if (numReviews != 0)
            return averageReviewScore / numReviews;
        else
            return 1;
    }
}
