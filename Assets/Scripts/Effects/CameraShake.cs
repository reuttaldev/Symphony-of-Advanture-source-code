using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
using static Unity.Burst.Intrinsics.X86.Avx;
public class CameraShake : MonoBehaviour
{

    CinemachineVirtualCamera cam;
    CinemachineBasicMultiChannelPerlin cmcp;
    [SerializeField]
    float shakeIntensity = 3, shakeDuration = 2f;
    private void Start()
    {
        cam = GetComponent<CinemachineVirtualCamera>();
        cmcp = cam.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>();    
    }

    public void ShakeCamera()
    {
        StartCoroutine(ShakeCameraCoroutine());
    }
    IEnumerator ShakeCameraCoroutine()
    {
        cmcp.m_AmplitudeGain = shakeIntensity;  
        yield return new WaitForSeconds(shakeDuration);
        cmcp.m_AmplitudeGain = 0;  
    }
}
