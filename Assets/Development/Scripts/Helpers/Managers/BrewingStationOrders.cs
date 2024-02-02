using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BrewingStationOrders: MonoBehaviour
{
    [System.Serializable]
    public class BrewingStationOrder
    {
        public BrewingStation brewingStation;
        public OrderStats orderStats;
    }
}
