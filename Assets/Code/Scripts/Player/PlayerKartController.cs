using System.Collections.Generic;
using BoschingMachine.Kart;
using UnityEngine;
using UnityEngine.InputSystem;
using Cinemachine;

namespace BoschingMachine
{
    [SelectionBase]
    [DisallowMultipleComponent]
    [RequireComponent(typeof(KartBody))]
    public class PlayerKartController : MonoBehaviour
    {
        [SerializeField] private InputActionAsset inputAsset;
        [SerializeField] private float mouseSensitivity;

        [Space]
        [SerializeField] CinemachineVirtualCamera carCam;
        [SerializeField] float amplitudeScaling;
        
        private KartBody kart;

        private InputActionMap kartMap;
        private InputAction throttle;
        private InputAction steer;
        private InputAction brake;

        CinemachineBasicMultiChannelPerlin noiseMachine;

        public static List<PlayerKartController> Players { get; } = new();

        private void Awake()
        {
            kart = GetComponent<KartBody>();

            kartMap = inputAsset.FindActionMap("kart");
            
            throttle = kartMap.FindAction("throttle");
            steer = kartMap.FindAction("steer");
            brake = kartMap.FindAction("brake");

            noiseMachine = carCam.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>();
        }

        private void OnEnable()
        {
            Register();
        }

        private void Register()
        {
            if (!Players.Contains(this)) Players.Add(this);

            Cursor.lockState = CursorLockMode.Locked;
        }

        private void OnDisable()
        {
            Deregister();
        }

        private void OnDestroy()
        {
            Deregister();
        }

        private void Deregister()
        {
            Players.Remove(this);

            Cursor.lockState = CursorLockMode.None;
        }

        private void Update()
        {
            kartMap.Enable();

            kart.ThrottlePercent = throttle.ReadValue<float>();
            if (Mouse.current != null) kart.SteerAngle += Mouse.current.delta.ReadValue().x / Screen.width * mouseSensitivity;
            
            kart.Steer = Mathf.RoundToInt(steer.ReadValue<float>());
            kart.SteerHeld = kart.Steer != 0;
            
            kart.BrakePercent = brake.ReadValue<float>();

            var speed = Mathf.Abs(Vector3.Dot(transform.forward, kart.Rigidbody.velocity));
            noiseMachine.m_AmplitudeGain = speed * speed * amplitudeScaling * amplitudeScaling;
        }
    }
}
