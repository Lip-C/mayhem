using System;
using System.Collections;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class TruckBehaviour : MonoBehaviour
{
    public TruckConfig truckInfo;
    public FloatData fuelSupply, boostPower;
    public GameAction startGameAction, horizontalInput, endGameAction, winGameAction;
    public WheelJoint2D frontWheel;

    private float inputSpeed, boostLevel = 1f;
    private WaitForSeconds wfsObj;
    private readonly WaitForFixedUpdate wffuObj;
    private JointMotor2D newMotor;
    private Vector2 truckDirection;
    private bool run;

    public TruckBehaviour(Vector2 truckDirection, WaitForFixedUpdate wffuObj)
    {
        this.truckDirection = truckDirection;
        this.wffuObj = wffuObj;
    }

    private void Awake()
    {
        startGameAction.raiseNoArgs += StartTruck;
    }

    private IEnumerator StartDriving()
    {
        horizontalInput.raise += RunInput;
        endGameAction.raiseNoArgs += EndControl;
        winGameAction.raiseNoArgs += WinGameHandler;
        
        newMotor = frontWheel.motor;
        wfsObj = new WaitForSeconds(0.1f);

        run = true;
        StartCoroutine(Drive());
        
        while (fuelSupply.value > 0)
        {
            yield return wfsObj;
            ExpendFuel();
        }
    }

    private void WinGameHandler()
    {
        StopCoroutine(StartDriving());
        EndControl();
        newMotor.motorSpeed = 0;
        newMotor.maxMotorTorque = 100;
        frontWheel.motor = newMotor;
    }

    public void StartTruck()
    {
        StartCoroutine(StartDriving());
    }

    private void EndControl()
    {
        horizontalInput.raise = null;
        inputSpeed = 0;
        run = false;
        newMotor.motorSpeed = 0;
        newMotor.maxMotorTorque = 0;
        frontWheel.motor = newMotor;
    }
    
    private void RunInput(object obj)
    {
        inputSpeed = obj is float f ? f : 0;
    }

    private void ExpendFuel()
    {
        fuelSupply.UpdateValue(truckInfo.fuelExpenseRate);
    }

    private IEnumerator Boost()
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

    private IEnumerator Drive()
    {
        boostLevel = 1f;
        while (run)
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
            yield return wffuObj;
        }
    }
}
