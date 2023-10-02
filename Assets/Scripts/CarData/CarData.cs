using UnityEngine;

namespace DriftGame.Car
{
    [CreateAssetMenu(fileName = "car_data", menuName = "Create Car Data")]
    public class CarData : ScriptableObject
    {
        [Range(20, 190)]
        [SerializeField] private int _maxSpeed = 190;
        [Range(10, 120)]
        [SerializeField] private int _maxReverseSpeed = 120; 
        [Range(1, 15)]
        [SerializeField] private int _accelerationMultiplier = 10; 
        [Space(10)]
        [Range(10, 45)]
        [SerializeField] private int _maxSteeringAngle = 35; 
        [Range(0.1f, 1f)]
        [SerializeField] private float _steeringSpeed = 1f; 
        [Space(10)]
        [Range(100, 600)]
        [SerializeField] private int _brakeForce = 350;
        [Range(1, 10)]
        [SerializeField] private int _decelerationMultiplier = 10; 
        [Range(1, 10)]
        [SerializeField] private int _handbrakeDriftMultiplier = 10;
        [Space(10)]
        [SerializeField] private Vector3 _bodyMassCenter;

        public int MaxSpeed => _maxSpeed;
        public int MaxReverseSpeed => _maxReverseSpeed;
        public int AccelerationMultiplier => _accelerationMultiplier;
        public int MaxSteeringAngle => _maxSteeringAngle;
        public float SteeringSpeed => _steeringSpeed;
        public int BrakeForce => _brakeForce;
        public int DecelerationMultiplier => _decelerationMultiplier;
        public int HandbrakeDriftMultiplier => _handbrakeDriftMultiplier;
        public Vector3 BodyMassCenter => _bodyMassCenter;
    }
}