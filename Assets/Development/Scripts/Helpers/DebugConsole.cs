using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using static CustomerBase;

public class DebugConsole : MonoBehaviour
{
    public InputField debugInput;
    public Ingredient coffeeCup;
    public PlayerController player;

    public void TryCommand()
    {
        if (debugInput.text.StartsWith("coffee")) //  coffee 1 2 3 4 5
        {
            string[] args = debugInput.text.Split(' ');
            try
            {
                if (player.GetNextHoldPoint())
                {
                    //Ingredient i = Ingredient.SpawnIngredient(coffeeCup.IngredientSO, player);
                    //CoffeeAttributes coffeeAttributes = i.GetComponent<CoffeeAttributes>();
                    //coffeeAttributes.AddSweetness(int.Parse(args[1]));
                    //coffeeAttributes.AddBitterness(int.Parse(args[2]));
                    //coffeeAttributes.AddStrength(int.Parse(args[3]));
                    //coffeeAttributes.AddTemperature(int.Parse(args[4]));
                    //coffeeAttributes.AddSpiciness(int.Parse(args[5]));
                    //player.GetNumberOfIngredients();
                }
            }
            catch { }
            finally
            {
                debugInput.text = "";
            }
        }
        if (debugInput.text.StartsWith("leave"))
        {
            try
            {
                CustomerBase[] customers = Object.FindObjectsOfType<CustomerBase>();
                foreach (CustomerBase customer in customers)
                {
                    customer.CustomerLeave();
                }
            }
            catch { }
            finally
            {
                debugInput.text = "";
            }
        }
        if (debugInput.text.StartsWith("order"))
        {
            try
            {
                CustomerBase[] customers = Object.FindObjectsOfType<CustomerBase>();
                foreach (CustomerBase customer in customers)
                {
                    if (customer.GetCustomerState() == CustomerState.Waiting || customer.GetCustomerState() == CustomerState.Ordering)
                    {
                        customer.agent.isStopped = true;
                        customer.StartOrderTimer();
                        customer.SetCustomerStateServerRpc(CustomerState.Ordering);
                        customer.customerNumberCanvas.enabled = true;
                        //UIManager.Instance.ShowCustomerUiOrder(customer);
                        CustomerManager.Instance.barFloor.TrySendToChair(customer);
                    }
                }
            }
            catch { }
            finally
            {
                debugInput.text = "";
            }
        }
    }
}
