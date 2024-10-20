using Cinemachine;
using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
public class CameraShake : MonoBehaviour
{

    CinemachineImpulseSource impulseSource;
    [SerializeField]
     float force = 0.4f;
    private void Awake()
    {
        impulseSource = GetComponent<CinemachineImpulseSource>();

    }

    public void Update()
    {
        if (Keyboard.current.vKey.wasPressedThisFrame)
            ShakeCamera();
    }
    public void ShakeCamera()
    {
        impulseSource.GenerateImpulse(force);
    }
}
