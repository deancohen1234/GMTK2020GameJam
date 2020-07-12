using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class OverloadController : MonoBehaviour
{
    public ParticleSystem m_ZapSystem;
    public AudioSource m_Mower;
    public Camera m_MainCamera;

    public float m_SpeedBoostTimeMinTime = 10f;
    public float m_SpeedBoostTimeMaxTime = 20f;
    public float m_BoostTime = 3.0f;

    public UnityEvent m_OnSpeedBoostStart;
    public UnityEvent m_OnSpeedBoostEnd;

    private float m_LastSpeedBoostTime;
    private float m_SpeedBoostDelay;
    private bool m_IsBoosting;

    private void Start()
    {
        m_SpeedBoostDelay = Random.Range(m_SpeedBoostTimeMinTime, m_SpeedBoostTimeMaxTime);
        m_LastSpeedBoostTime = 0;
    }

    private void Update()
    {
        if (Time.time - m_LastSpeedBoostTime > m_SpeedBoostDelay) 
        {
            StartSpeedBoost();
        }

        if (m_IsBoosting) 
        {
            if (Time.time - m_LastSpeedBoostTime > m_BoostTime)
            {
                EndSpeedBoost();
            }
        }

        UpdateZapAmount();
    }

    private void StartSpeedBoost() 
    {
        m_IsBoosting = true;

        m_OnSpeedBoostStart?.Invoke();

        m_LastSpeedBoostTime = Time.time;
        m_SpeedBoostDelay = Random.Range(m_SpeedBoostTimeMinTime, m_SpeedBoostTimeMaxTime);

        GameManager.m_Singleton.m_EffectsManager.ActivateEffect("FireAss");

        m_Mower.pitch = 1.5f;

        //m_MainCamera.fieldOfView = 70;
    }

    public void EndSpeedBoost() 
    {
        m_IsBoosting = false;

        m_OnSpeedBoostEnd?.Invoke();

        GameManager.m_Singleton.m_EffectsManager.DeActivateEffect("FireAss");

        m_Mower.pitch = 1.0f;

        //m_MainCamera.fieldOfView = 60;
    }

    private void UpdateZapAmount() 
    {
        int particleEmmision = Mathf.CeilToInt(DeanUtils.Map(Time.time, m_LastSpeedBoostTime, m_LastSpeedBoostTime + m_SpeedBoostDelay, 0, 7));

        ParticleSystem.EmissionModule module = m_ZapSystem.emission;

        module.rateOverTime = particleEmmision;
    }
}
