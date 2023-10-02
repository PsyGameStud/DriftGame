using DriftGame.Managers;
using DriftGame.UI;
using Photon.Pun;
using System;
using UnityEngine;

namespace DriftGame.Car
{
    [RequireComponent(typeof(Rigidbody))]
    public class CarController : MonoBehaviour
    {
        [SerializeField] private CarData _carData;
        [SerializeField] private PhotonView _photonView;

        [Header("WHEELS")]
        [SerializeField] private GameObject _frontLeftMesh;
        [SerializeField] private WheelCollider _frontLeftCollider;
        [Space(10)]
        [SerializeField] private GameObject _frontRightMesh;
        [SerializeField] private WheelCollider _frontRightCollider;
        [Space(10)]
        [SerializeField] private GameObject _rearLeftMesh;
        [SerializeField] private WheelCollider _rearLeftCollider;
        [Space(10)]
        [SerializeField] private GameObject _rearRightMesh;
        [SerializeField] private WheelCollider _rearRightCollider;

        [Space(20)]
        [Header("EFFECTS")]
        [Space(10)]
        [SerializeField] private bool _useEffects = false;
        [SerializeField] private ParticleSystem _leftWheelEffect;
        [SerializeField] private ParticleSystem _rightWgeelEffect;
        [Space(10)]
        [SerializeField] private TrailRenderer _leftWheelLine;
        [SerializeField] public TrailRenderer _rightWheelLine;

        private float _carSpeed;
        private bool _isDrifting;
        private bool _isTractionLocked;

        private Rigidbody _carRigidbody;
        private float _steeringAxis;
        private float _throttleAxis;
        private float _driftingAxis;
        private float _localVelocityZ;
        private float _localVelocityX;
        private bool _deceleratingCar;
        private int _driftPoints;
        private int _additionalSpeed;
        private int _additionalAcceleration;

        private WheelFrictionCurve _forrwardLeftWheelFriction;
        private float _forwardLeftWextremumSlip;
        private WheelFrictionCurve _forwardRightWheelFriction;
        private float _forwardRightWextremumSlip;
        private WheelFrictionCurve _rearLeftWheelFriction;
        private float _rearLeftWextremumSlip;
        private WheelFrictionCurve _rearRightwheelFriction;
        private float _rearRightWextremumSlip;

        private void Start()
        {
            _carRigidbody = gameObject.GetComponent<Rigidbody>();
            _carRigidbody.centerOfMass = _carData.BodyMassCenter;

            _forrwardLeftWheelFriction = new WheelFrictionCurve();
            _forrwardLeftWheelFriction.extremumSlip = _frontLeftCollider.sidewaysFriction.extremumSlip;
            _forwardLeftWextremumSlip = _frontLeftCollider.sidewaysFriction.extremumSlip;
            _forrwardLeftWheelFriction.extremumValue = _frontLeftCollider.sidewaysFriction.extremumValue;
            _forrwardLeftWheelFriction.asymptoteSlip = _frontLeftCollider.sidewaysFriction.asymptoteSlip;
            _forrwardLeftWheelFriction.asymptoteValue = _frontLeftCollider.sidewaysFriction.asymptoteValue;
            _forrwardLeftWheelFriction.stiffness = _frontLeftCollider.sidewaysFriction.stiffness;

            _forwardRightWheelFriction = new WheelFrictionCurve();
            _forwardRightWheelFriction.extremumSlip = _frontRightCollider.sidewaysFriction.extremumSlip;
            _forwardRightWextremumSlip = _frontRightCollider.sidewaysFriction.extremumSlip;
            _forwardRightWheelFriction.extremumValue = _frontRightCollider.sidewaysFriction.extremumValue;
            _forwardRightWheelFriction.asymptoteSlip = _frontRightCollider.sidewaysFriction.asymptoteSlip;
            _forwardRightWheelFriction.asymptoteValue = _frontRightCollider.sidewaysFriction.asymptoteValue;
            _forwardRightWheelFriction.stiffness = _frontRightCollider.sidewaysFriction.stiffness;

            _rearLeftWheelFriction = new WheelFrictionCurve();
            _rearLeftWheelFriction.extremumSlip = _rearLeftCollider.sidewaysFriction.extremumSlip;
            _rearLeftWextremumSlip = _rearLeftCollider.sidewaysFriction.extremumSlip;
            _rearLeftWheelFriction.extremumValue = _rearLeftCollider.sidewaysFriction.extremumValue;
            _rearLeftWheelFriction.asymptoteSlip = _rearLeftCollider.sidewaysFriction.asymptoteSlip;
            _rearLeftWheelFriction.asymptoteValue = _rearLeftCollider.sidewaysFriction.asymptoteValue;
            _rearLeftWheelFriction.stiffness = _rearLeftCollider.sidewaysFriction.stiffness;

            _rearRightwheelFriction = new WheelFrictionCurve();
            _rearRightwheelFriction.extremumSlip = _rearRightCollider.sidewaysFriction.extremumSlip;
            _rearRightWextremumSlip = _rearRightCollider.sidewaysFriction.extremumSlip;
            _rearRightwheelFriction.extremumValue = _rearRightCollider.sidewaysFriction.extremumValue;
            _rearRightwheelFriction.asymptoteSlip = _rearRightCollider.sidewaysFriction.asymptoteSlip;
            _rearRightwheelFriction.asymptoteValue = _rearRightCollider.sidewaysFriction.asymptoteValue;
            _rearRightwheelFriction.stiffness = _rearRightCollider.sidewaysFriction.stiffness;

            if (!_useEffects)
            {
                if (_leftWheelEffect != null)
                {
                    _leftWheelEffect.Stop();
                }
                if (_rightWgeelEffect != null)
                {
                    _rightWgeelEffect.Stop();
                }
                if (_leftWheelLine != null)
                {
                    _leftWheelLine.emitting = false;
                }
                if (_rightWheelLine != null)
                {
                    _rightWheelLine.emitting = false;
                }
            }

            TimerManager.Instance.OnComplete += DisableCar;

            GameplayEntryPoint.Instance.SaveSystem.Load<GarageData>("GARAGE_DATA", data =>
            {
                _additionalSpeed = data.AdditionalSpeed;
                _additionalAcceleration = data.AdditionalAcceleration;
            });
        }

        private void DisableCar()
        {
            _carSpeed = 0;
            Brakes();
            SoundManager.Instance.StopSound();
            enabled = false;
        }

        private void Update()
        {
            _carSpeed = (2 * Mathf.PI * _frontLeftCollider.radius * _frontLeftCollider.rpm * 60) / 1000;
            _localVelocityX = transform.InverseTransformDirection(_carRigidbody.velocity).x;
            _localVelocityZ = transform.InverseTransformDirection(_carRigidbody.velocity).z;
            GameWindow.Instance.SetSpeed(_carSpeed);

            if (PhotonNetwork.LocalPlayer.UserId == _photonView.Owner.UserId)
            {
                if (Input.GetKey(KeyCode.W))
                {
                    DecelerateCar(true);
                    _deceleratingCar = false;
                    GoForward();
                }
                if (Input.GetKey(KeyCode.S))
                {
                    DecelerateCar(true);
                    _deceleratingCar = false;
                    GoReverse();
                }

                if (Input.GetKey(KeyCode.A))
                {
                    TurnLeft();
                }
                if (Input.GetKey(KeyCode.D))
                {
                    TurnRight();
                }
                if (Input.GetKey(KeyCode.Space))
                {
                    DecelerateCar(true);
                    _deceleratingCar = false;
                    Handbrake();
                }
                if (Input.GetKeyUp(KeyCode.Space))
                {
                    RecoverTraction();
                }
                if ((!Input.GetKey(KeyCode.S) && !Input.GetKey(KeyCode.W)))
                {
                    ThrottleOff();
                }
                if ((!Input.GetKey(KeyCode.S) && !Input.GetKey(KeyCode.W)) && !Input.GetKey(KeyCode.Space) && !_deceleratingCar)
                {
                    DecelerateCar();
                    _deceleratingCar = true;
                }
                if (!Input.GetKey(KeyCode.A) && !Input.GetKey(KeyCode.D) && _steeringAxis != 0f)
                {
                    ResetSteeringAngle();
                }

                SoundManager.Instance.CarSounds(_isDrifting, _isTractionLocked, _carSpeed, _carRigidbody);
                AnimateWheelMeshes();
            }   
        }

        private void TurnLeft()
        {
            _steeringAxis = _steeringAxis - (Time.deltaTime * 10f * _carData.SteeringSpeed);
            if (_steeringAxis < -1f)
            {
                _steeringAxis = -1f;
            }
            var steeringAngle = _steeringAxis * _carData.MaxSteeringAngle;
            _frontLeftCollider.steerAngle = Mathf.Lerp(_frontLeftCollider.steerAngle, steeringAngle, _carData.SteeringSpeed);
            _frontRightCollider.steerAngle = Mathf.Lerp(_frontRightCollider.steerAngle, steeringAngle, _carData.SteeringSpeed);
        }

        private void TurnRight()
        {
            _steeringAxis = _steeringAxis + (Time.deltaTime * 10f * _carData.SteeringSpeed);
            if (_steeringAxis > 1f)
            {
                _steeringAxis = 1f;
            }
            var steeringAngle = _steeringAxis * _carData.MaxSteeringAngle;
            _frontLeftCollider.steerAngle = Mathf.Lerp(_frontLeftCollider.steerAngle, steeringAngle, _carData.SteeringSpeed);
            _frontRightCollider.steerAngle = Mathf.Lerp(_frontRightCollider.steerAngle, steeringAngle, _carData.SteeringSpeed);
        }

        private void ResetSteeringAngle()
        {
            if (_steeringAxis < 0f)
            {
                _steeringAxis = _steeringAxis + (Time.deltaTime * 10f * _carData.SteeringSpeed);
            }
            else if (_steeringAxis > 0f)
            {
                _steeringAxis = _steeringAxis - (Time.deltaTime * 10f * _carData.SteeringSpeed);
            }
            if (Mathf.Abs(_frontLeftCollider.steerAngle) < 1f)
            {
                _steeringAxis = 0f;
            }
            var steeringAngle = _steeringAxis * _carData.MaxSteeringAngle;
            _frontLeftCollider.steerAngle = Mathf.Lerp(_frontLeftCollider.steerAngle, steeringAngle, _carData.SteeringSpeed);
            _frontRightCollider.steerAngle = Mathf.Lerp(_frontRightCollider.steerAngle, steeringAngle, _carData.SteeringSpeed);
        }

        private void AnimateWheelMeshes()
        {
            try
            {
                Quaternion FLWRotation;
                Vector3 FLWPosition;
                _frontLeftCollider.GetWorldPose(out FLWPosition, out FLWRotation);
                _frontLeftMesh.transform.position = FLWPosition;
                _frontLeftMesh.transform.rotation = FLWRotation;

                Quaternion FRWRotation;
                Vector3 FRWPosition;
                _frontRightCollider.GetWorldPose(out FRWPosition, out FRWRotation);
                _frontRightMesh.transform.position = FRWPosition;
                _frontRightMesh.transform.rotation = FRWRotation;

                Quaternion RLWRotation;
                Vector3 RLWPosition;
                _rearLeftCollider.GetWorldPose(out RLWPosition, out RLWRotation);
                _rearLeftMesh.transform.position = RLWPosition;
                _rearLeftMesh.transform.rotation = RLWRotation;

                Quaternion RRWRotation;
                Vector3 RRWPosition;
                _rearRightCollider.GetWorldPose(out RRWPosition, out RRWRotation);
                _rearRightMesh.transform.position = RRWPosition;
                _rearRightMesh.transform.rotation = RRWRotation;
            }
            catch (Exception ex)
            {
                Debug.LogWarning(ex);
            }
        }

        private void GoForward()
        {
            if (Mathf.Abs(_localVelocityX) > 2.5f)
            {
                _isDrifting = true;
                DriftCarPS();
            }
            else
            {
                _isDrifting = false;
                DriftCarPS();
            }

            _throttleAxis = _throttleAxis + (Time.deltaTime * 3f);
            if (_throttleAxis > 1f)
            {
                _throttleAxis = 1f;
            }

            if (_localVelocityZ < -1f)
            {
                Brakes();
            }
            else
            {
                if (Mathf.RoundToInt(_carSpeed) < _carData.MaxSpeed + _additionalSpeed)
                {
                    _frontLeftCollider.brakeTorque = 0;
                    _frontLeftCollider.motorTorque = (_carData.AccelerationMultiplier * 50f + _additionalAcceleration) * _throttleAxis;
                    _frontRightCollider.brakeTorque = 0;
                    _frontRightCollider.motorTorque = (_carData.AccelerationMultiplier * 50f + _additionalAcceleration) * _throttleAxis;
                    _rearLeftCollider.brakeTorque = 0;
                    _rearLeftCollider.motorTorque = (_carData.AccelerationMultiplier * 50f + _additionalAcceleration) * _throttleAxis;
                    _rearRightCollider.brakeTorque = 0;
                    _rearRightCollider.motorTorque = (_carData.AccelerationMultiplier * 50f + _additionalAcceleration) * _throttleAxis;
                }
                else
                {
                    _frontLeftCollider.motorTorque = 0;
                    _frontRightCollider.motorTorque = 0;
                    _rearLeftCollider.motorTorque = 0;
                    _rearRightCollider.motorTorque = 0;
                }
            }
        }

        private void GoReverse()
        {
            if (Mathf.Abs(_localVelocityX) > 2.5f)
            {
                _isDrifting = true;
                DriftCarPS();
            }
            else
            {
                _isDrifting = false;
                DriftCarPS();
            }
            _throttleAxis = _throttleAxis - (Time.deltaTime * 3f);
            if (_throttleAxis < -1f)
            {
                _throttleAxis = -1f;
            }

            if (_localVelocityZ > 1f)
            {
                Brakes();
            }
            else
            {
                if (Mathf.Abs(Mathf.RoundToInt(_carSpeed)) < _carData.MaxReverseSpeed)
                {
                    _frontLeftCollider.brakeTorque = 0;
                    _frontLeftCollider.motorTorque = (_carData.AccelerationMultiplier * 50f + _additionalAcceleration) * _throttleAxis;
                    _frontRightCollider.brakeTorque = 0;
                    _frontRightCollider.motorTorque = (_carData.AccelerationMultiplier * 50f + _additionalAcceleration) * _throttleAxis;
                    _rearLeftCollider.brakeTorque = 0;
                    _rearLeftCollider.motorTorque = (_carData.AccelerationMultiplier * 50f + _additionalAcceleration) * _throttleAxis;
                    _rearRightCollider.brakeTorque = 0;
                    _rearRightCollider.motorTorque = (_carData.AccelerationMultiplier * 50f + _additionalAcceleration) * _throttleAxis;
                }
                else
                {
                    _frontLeftCollider.motorTorque = 0;
                    _frontRightCollider.motorTorque = 0;
                    _rearLeftCollider.motorTorque = 0;
                    _rearRightCollider.motorTorque = 0;
                }
            }
        }

        private void ThrottleOff()
        {
            _frontLeftCollider.motorTorque = 0;
            _frontRightCollider.motorTorque = 0;
            _rearLeftCollider.motorTorque = 0;
            _rearRightCollider.motorTorque = 0;
        }

        private void DecelerateCar(bool isStopping = false)
        {
            if (isStopping)
            {
                return;
            }

            if (Mathf.Abs(_localVelocityX) > 2.5f)
            {
                _isDrifting = true;
                DriftCarPS();
            }
            else
            {
                _isDrifting = false;
                DriftCarPS();
            }

            if (_throttleAxis != 0f)
            {
                if (_throttleAxis > 0f)
                {
                    _throttleAxis = _throttleAxis - (Time.deltaTime * 10f);
                }
                else if (_throttleAxis < 0f)
                {
                    _throttleAxis = _throttleAxis + (Time.deltaTime * 10f);
                }
                if (Mathf.Abs(_throttleAxis) < 0.15f)
                {
                    _throttleAxis = 0f;
                }
            }

            _carRigidbody.velocity = _carRigidbody.velocity * (1f / (1f + (0.025f * _carData.DecelerationMultiplier)));

            _frontLeftCollider.motorTorque = 0;
            _frontRightCollider.motorTorque = 0;
            _rearLeftCollider.motorTorque = 0;
            _rearRightCollider.motorTorque = 0;

            if (_carRigidbody.velocity.magnitude < 0.25f)
            {
                _carRigidbody.velocity = Vector3.zero;
                DecelerateCar(true);
            }
        }

        private void Brakes()
        {
            _frontLeftCollider.brakeTorque = _carData.BrakeForce;
            _frontRightCollider.brakeTorque = _carData.BrakeForce;
            _rearLeftCollider.brakeTorque = _carData.BrakeForce;
            _rearRightCollider.brakeTorque = _carData.BrakeForce;
        }

        private void Handbrake()
        {
            CancelInvoke("RecoverTraction");

            _driftingAxis = _driftingAxis + (Time.deltaTime);
            float secureStartingPoint = _driftingAxis * _forwardLeftWextremumSlip * _carData.HandbrakeDriftMultiplier;

            if (secureStartingPoint < _forwardLeftWextremumSlip)
            {
                _driftingAxis = _forwardLeftWextremumSlip / (_forwardLeftWextremumSlip * _carData.HandbrakeDriftMultiplier);
            }
            if (_driftingAxis > 1f)
            {
                _driftingAxis = 1f;
            }

            if (Mathf.Abs(_localVelocityX) > 2.5f)
            {
                _isDrifting = true;
            }
            else
            {
                _isDrifting = false;
            }

            if (_driftingAxis < 1f)
            {
                _forrwardLeftWheelFriction.extremumSlip = _forwardLeftWextremumSlip * _carData.HandbrakeDriftMultiplier * _driftingAxis;
                _frontLeftCollider.sidewaysFriction = _forrwardLeftWheelFriction;

                _forwardRightWheelFriction.extremumSlip = _forwardRightWextremumSlip * _carData.HandbrakeDriftMultiplier * _driftingAxis;
                _frontRightCollider.sidewaysFriction = _forwardRightWheelFriction;

                _rearLeftWheelFriction.extremumSlip = _rearLeftWextremumSlip * _carData.HandbrakeDriftMultiplier * _driftingAxis;
                _rearLeftCollider.sidewaysFriction = _rearLeftWheelFriction;

                _rearRightwheelFriction.extremumSlip = _rearRightWextremumSlip * _carData.HandbrakeDriftMultiplier * _driftingAxis;
                _rearRightCollider.sidewaysFriction = _rearRightwheelFriction;
            }

            _isTractionLocked = true;
            DriftCarPS();
        }

        private void DriftCarPS()
        {
            if (_useEffects)
            {
                try
                {
                    if (_isDrifting)
                    {
                        _leftWheelEffect.Play();
                        _rightWgeelEffect.Play();
                        _driftPoints++;
                        GameWindow.Instance.SetDriftPoint(_driftPoints);
                    }
                    else if (!_isDrifting)
                    {
                        _leftWheelEffect.Stop();
                        _rightWgeelEffect.Stop();
                        GameWindow.Instance.UpdateTotalDriftPoints(_driftPoints);
                        _driftPoints = 0;
                        GameWindow.Instance.SetDriftPoint(_driftPoints);
                    }
                }
                catch (Exception ex)
                {
                    Debug.LogWarning(ex);
                }

                try
                {
                    if ((_isTractionLocked || Mathf.Abs(_localVelocityX) > 5f) && Mathf.Abs(_carSpeed) > 12f)
                    {
                        _leftWheelLine.emitting = true;
                        _rightWheelLine.emitting = true;
                    }
                    else
                    {
                        _leftWheelLine.emitting = false;
                        _rightWheelLine.emitting = false;
                    }
                }
                catch (Exception ex)
                {
                    Debug.LogWarning(ex);
                }
            }
            else if (!_useEffects)
            {
                if (_leftWheelEffect != null)
                {
                    _leftWheelEffect.Stop();
                }
                if (_rightWgeelEffect != null)
                {
                    _rightWgeelEffect.Stop();
                }
                if (_leftWheelLine != null)
                {
                    _leftWheelLine.emitting = false;
                }
                if (_rightWheelLine != null)
                {
                    _rightWheelLine.emitting = false;
                }
            }
        }

        private void RecoverTraction()
        {
            _isTractionLocked = false;
            _driftingAxis = _driftingAxis - (Time.deltaTime / 1.5f);
            if (_driftingAxis < 0f)
            {
                _driftingAxis = 0f;
            }

            if (_forrwardLeftWheelFriction.extremumSlip > _forwardLeftWextremumSlip)
            {
                _forrwardLeftWheelFriction.extremumSlip = _forwardLeftWextremumSlip * _carData.HandbrakeDriftMultiplier * _driftingAxis;
                _frontLeftCollider.sidewaysFriction = _forrwardLeftWheelFriction;

                _forwardRightWheelFriction.extremumSlip = _forwardRightWextremumSlip * _carData.HandbrakeDriftMultiplier * _driftingAxis;
                _frontRightCollider.sidewaysFriction = _forwardRightWheelFriction;

                _rearLeftWheelFriction.extremumSlip = _rearLeftWextremumSlip * _carData.HandbrakeDriftMultiplier * _driftingAxis;
                _rearLeftCollider.sidewaysFriction = _rearLeftWheelFriction;

                _rearRightwheelFriction.extremumSlip = _rearRightWextremumSlip * _carData.HandbrakeDriftMultiplier * _driftingAxis;
                _rearRightCollider.sidewaysFriction = _rearRightwheelFriction;

                Invoke("RecoverTraction", Time.deltaTime);

            }
            else if (_forrwardLeftWheelFriction.extremumSlip < _forwardLeftWextremumSlip)
            {
                _forrwardLeftWheelFriction.extremumSlip = _forwardLeftWextremumSlip;
                _frontLeftCollider.sidewaysFriction = _forrwardLeftWheelFriction;

                _forwardRightWheelFriction.extremumSlip = _forwardRightWextremumSlip;
                _frontRightCollider.sidewaysFriction = _forwardRightWheelFriction;

                _rearLeftWheelFriction.extremumSlip = _rearLeftWextremumSlip;
                _rearLeftCollider.sidewaysFriction = _rearLeftWheelFriction;

                _rearRightwheelFriction.extremumSlip = _rearRightWextremumSlip;
                _rearRightCollider.sidewaysFriction = _rearRightwheelFriction;

                _driftingAxis = 0f;
            }
        }
    }
}
