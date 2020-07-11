using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public Image m_EnergyBar;
    public float m_SmoothSpeed;

    private float m_EnergyBarStart = 1.0f;
    private float m_EnergyBarTarget = 1.0f;
    private float m_LerpTime;

    // Update is called once per frame
    void Update()
    {
        m_EnergyBar.fillAmount = Mathf.Lerp(m_EnergyBarStart, m_EnergyBarTarget, m_LerpTime);
        m_LerpTime = Mathf.Clamp01(m_LerpTime + Time.deltaTime * m_SmoothSpeed);

        Debug.Log("LerpTime: " + m_LerpTime);
    }

    //run as update
    public void SetEnergyBarValue(float value) 
    {
        m_LerpTime = 0;
        m_EnergyBarStart = m_EnergyBarTarget;
        m_EnergyBarTarget = value;
    }
}
