using System.Collections;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class TruckBehaviour : MonoBehaviour
{
    public TruckConfig truckInfo;
    public FloatData fuelSupply, boostPower;
    public GameAction horizontalInput, endGameAction;
    public WheelJoint2D frontWheel;

    private float inputSpeed, boostLevel = 1f;
    private WaitForSeconds wfsObj;
    private WaitForFixedUpdate wffuObj;
    private JointMotor2D newMotor;
    private Rigidbody2D truckRigidbody2D;
    private Vector2 truckDirection;
    
    IEnumerator Start()
    {
        horizontalInput.raise += RunInput;
        endGameAction.raise += EndControl;
        
        newMotor = frontWheel.motor;
        truckRigidbody2D = GetComponent<Rigidbody2D>();
        wfsObj = new WaitForSeconds(0.1f);
        
        while (true)
        {
            yield return wfsObj;
            ExpendFuel();
        }
    }

    private void EndControl(object arg0)
    {
        horizontalInput.raise = null;
        inputSpeed = 0;
    }


    private void RunInput(object obj)
    {
        inputSpeed = obj is float f ? f : 0;
    }

    private void ExpendFuel()
    {
        fuelSupply.UpdateValue(truckInfo.fuelExpenseRate);
    }

    IEnumerator Boost()
    {
        boostLevel = truckInfo.boostLevel;
        while (boostPower.value > 0)
        {
            yield return wffuObj;
            boostPower.UpdateValue(-0.0035f);
            fuelSupply.UpdateValue(-0.001f);
        }
        boostLevel = 1;
    }
    
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space) && boostPower.value > 0)
        {
            StopCoroutine(Boost());
            StartCoroutine(Boost());
        }
        if(inputSpeed != 0)
            fuelSupply.UpdateValue(-0.0002f);
        
        newMotor.motorSpeed = truckInfo.topSpeed * -10 * inputSpeed * boostLevel;
        if (inputSpeed < 0)
            inputSpeed *= -0.1f;
        newMotor.maxMotorTorque = truckInfo.topRPM * inputSpeed;
        
        if (Input.GetKey(KeyCode.B))
        {
            newMotor.motorSpeed = 0;
            newMotor.maxMotorTorque = 1000;
        }
        frontWheel.motor = newMotor;
    }
}
