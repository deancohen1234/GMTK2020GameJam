using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public Image m_EnergyBar;
    public float m_SmoothSpeed;

    public GameObject m_EndScreen;
    public Text m_GrassCutText;

    public GameObject m_StartScreen;
    public Text m_TimeText;

    private float m_EnergyBarStart = 1.0f;
    private float m_EnergyBarTarget = 1.0f;
    private float m_LerpTime;

    private void Start()
    {
        m_EndScreen.SetActive(false);
        m_StartScreen.SetActive(true);
    }
    // Update is called once per frame
    void Update()
    {
        m_EnergyBar.fillAmount = Mathf.Lerp(m_EnergyBarStart, m_EnergyBarTarget, m_LerpTime);
        m_LerpTime = Mathf.Clamp01(m_LerpTime + Time.deltaTime * m_SmoothSpeed);
    }

    //run as update
    public void SetEnergyBarValue(float value) 
    {
        m_LerpTime = 0;
        m_EnergyBarStart = m_EnergyBarTarget;
        m_EnergyBarTarget = value;
    }

    public void DisplayEndScreen(int grassCut, int numberOfPenalties, int totalGrass) 
    {
        int intPercentage = Mathf.FloorToInt((float)grassCut / (float)totalGrass * 100.0f);

        m_EndScreen.SetActive(true);
        string endScreenText = "Mission Synopsis:";
        endScreenText += "\nPercentage of Grass Eliminated: " + intPercentage.ToString() + "%";
        endScreenText += "\nUnexpected Casualties: " + numberOfPenalties.ToString();
        m_GrassCutText.text = endScreenText;
    }

    public void SetCountdownTime(float time) 
    {
        m_TimeText.text = time.ToString("F");
    }

    public void HideStartMenu()
    {
        m_StartScreen.SetActive(false);
    }
}
