using UnityEngine;

[CreateAssetMenu]
public class TruckConfig : ScriptableObject
{
    public Sprite 
        body, 
        frontWheel, 
        rearWheel, 
        backAccessory, 
        frontAccessory, 
        sticker;

    public float
        topSpeed,
        weight;

    public WheelConfig wheelAttributes;
    
    public struct DriveTrain
    {
        
    }
    
    
}