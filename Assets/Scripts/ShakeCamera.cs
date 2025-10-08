using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShakeCamera : MonoBehaviour
{
    private static ShakeCamera instance;
    CinemachineVirtualCamera virtualCamera;
    CinemachineBasicMultiChannelPerlin perlin;
    // Start is called before the first frame update
    void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(this.gameObject);
        }

        virtualCamera = GetComponent<CinemachineVirtualCamera>();
        perlin = virtualCamera.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    /// <summary>
    /// 강도,빈도,시간
    /// </summary>
    /// <param name="ampitudeGain"></param>
    /// <param name="FrequencyGain"></param>
    /// <param name="duringTime"></param>
    /// <returns></returns>
    public IEnumerator Shake(float ampitudeGain, float FrequencyGain, float duringTime)
    {
        perlin.m_AmplitudeGain = ampitudeGain;
        perlin.m_FrequencyGain = FrequencyGain;
        yield return new WaitForSeconds(duringTime);
        perlin.m_AmplitudeGain = 0;
        perlin.m_FrequencyGain = 0;
    }

    public void Shaking(float ampitudeGain, float FrequencyGain, float duringTime)
    {
        StartCoroutine(Shake(ampitudeGain, FrequencyGain, duringTime));
    }

    public static ShakeCamera Instance
    {
        get
        {
            if (instance == null)
            {
                return null;
            }
            return instance;
        }
    }
}
