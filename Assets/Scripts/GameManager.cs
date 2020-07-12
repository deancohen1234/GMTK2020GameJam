using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager m_Singleton = null;

    public UIManager m_UIManager;
    public GrassManager m_GrassManager;

    private void Awake()
    {
        if (m_Singleton == null) 
        {
            m_Singleton = this;
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space)) 
        {
            EndGame();
        }
    }

    public void EndGame()
    {
        int grassCut = m_GrassManager.GetCutGrass();
        int totalGrass = m_GrassManager.GetTotalGrass();

        Time.timeScale = 0;
        m_UIManager.DisplayEndScreen(grassCut, totalGrass);
    }
}
