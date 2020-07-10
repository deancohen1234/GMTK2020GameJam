using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class OverloadController : MonoBehaviour
{
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

    }

    private void StartSpeedBoost() 
    {
        m_IsBoosting = true;

        m_OnSpeedBoostStart?.Invoke();

        m_LastSpeedBoostTime = Time.time;
        m_SpeedBoostDelay = Random.Range(m_SpeedBoostTimeMinTime, m_SpeedBoostTimeMaxTime);
    }

    private void EndSpeedBoost() 
    {
        m_IsBoosting = false;

        m_OnSpeedBoostEnd?.Invoke();
    }
}
