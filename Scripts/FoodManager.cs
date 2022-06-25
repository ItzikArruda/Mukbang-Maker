using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FoodManager : Manager<FoodManager>
{
    [Space]
    public GameObject[] FoodPrefabs;

    // Start is called before the first frame update
    void Awake()
    {
        Init(this);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
