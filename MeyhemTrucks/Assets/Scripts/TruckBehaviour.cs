using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TruckBehaviour : MonoBehaviour
{

    public TruckConfig truckInfo;
    public FloatData fuelSupply;
    private WaitForSeconds wfsObj;
    
    IEnumerator Start()
    {
        wfsObj = new WaitForSeconds(0.5f);
        
        while (true)
        {
            yield return wfsObj;
            ExpendFuel();
        }
    }

    private void ExpendFuel()
    {
        fuelSupply.UpdateValue(truckInfo.fuelExpenseRate);
    }
    
    // Update is called once per frame
    void Update()
    {
        
    }
}
