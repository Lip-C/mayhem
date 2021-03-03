using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class TruckBehaviour : MonoBehaviour
{
    public TruckConfig truckInfo;
    public FloatData fuelSupply, boostPower;
    public GameAction startGameAction, horizontalInput, endGameAction, winGameAction;
    public WheelJoint2D[] wheels;

    private float inputSpeed, boostLevel = 1f;
    private WaitForSeconds wfsObj;
    private readonly WaitForFixedUpdate wffuObj;
    private JointMotor2D newMotor, backMotor2D;
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
        wheels = GetComponents<WheelJoint2D>();
        foreach (var wheel in wheels)
        {
            print(wheel.connectedBody.name);
        }
    }

    private IEnumerator StartDriving()
    {
        horizontalInput.raise += RunInput;
        endGameAction.raiseNoArgs += EndControl;
        winGameAction.raiseNoArgs += WinGameHandler;
        
        newMotor = wheels[0].motor;
        
        wfsObj = new WaitForSeconds(0.1f);

        run = true;
        StartCoroutine(Drive());
        
        while (fuelSupply.value > 0 || run)
        {
            yield return wfsObj;
            ExpendFuel();
        }
    }

    private void OnDestroy()
    {
        startGameAction.raiseNoArgs = null;
        horizontalInput.raise = null;
        endGameAction.raiseNoArgs = null;
        winGameAction.raiseNoArgs = null;
    }

    private void RunMotors(float speed, float torque)
    {
        newMotor.motorSpeed = speed;
        backMotor2D.motorSpeed = speed * 0.3f;
        newMotor.maxMotorTorque = torque;
        backMotor2D.maxMotorTorque = torque;
        foreach (var wheel in wheels)
        {
            //wheel.motor = newMotor;
        }
        wheels[1].motor = newMotor;
        wheels[0].motor = backMotor2D;
    }

    private void WinGameHandler()
    {
        StopCoroutine(StartDriving());
        EndControl();
        RunMotors(0, 100);
    }

    private void StartTruck()
    {
        StartCoroutine(StartDriving());
    }

    private void EndControl()
    {
        horizontalInput.raise = null;
        inputSpeed = 0;
        run = false;
        RunMotors(0,0);
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
            print(boostPower.value);
        }
        boostLevel = 1;
    }

    private IEnumerator Drive()
    {
        boostLevel = 1f;
        float speed;
        float torque;
        
        while (run)
        {
            if (Input.GetKeyDown(KeyCode.Space) && boostPower.value > 0)
            {
                StopCoroutine(Boost());
                StartCoroutine(Boost());
            }
            
            if(inputSpeed != 0)
                fuelSupply.UpdateValue(-0.0001f);
        
            speed = truckInfo.topSpeed * -10 * inputSpeed * boostLevel;
            if (inputSpeed < 0)
                inputSpeed *= -0.1f;
            torque = truckInfo.topRPM * inputSpeed;
        
            if (Input.GetKey(KeyCode.B))
            {
                speed = 0;
                torque = 1000;
            }
            
            RunMotors(speed, torque);
            yield return wffuObj;
        }
    }
}
