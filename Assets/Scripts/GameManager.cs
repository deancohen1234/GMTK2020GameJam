using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public enum GameState {GameNotStarted, GameInited, CountdownRunning, GameComplete, GameRunning}
public class GameManager : MonoBehaviour
{
    public static GameManager m_Singleton = null;

    public UIManager m_UIManager;
    public GrassManager m_GrassManager;
    public EffectsManager m_EffectsManager;

    public float m_CountdownTime = 3.0f;

    private GameState m_GameState = GameState.GameNotStarted;

    private float m_StartGameTime;

    private bool m_GameStarted;
    private bool m_CountdownRunning;
    private bool m_GameComplete;

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
        Time.timeScale = 0;
    }

    // Update is called once per frame
    void Update()
    {

        switch(m_GameState)
        {
            case GameState.GameNotStarted:
                if (Input.GetKeyDown(KeyCode.Space))
                {
                    InitGame();
                }
                break;

            case GameState.CountdownRunning:

                float timeLeft = Mathf.Clamp(m_CountdownTime - (Time.unscaledTime - m_StartGameTime), 0, m_CountdownTime);
                m_UIManager.SetCountdownTime(timeLeft);

                if (timeLeft == 0)
                {
                    StartGame();
                }
                break;

            case GameState.GameComplete:
                if (Input.GetKeyDown(KeyCode.Space))
                {
                    RestartGame();
                }
                break;
        }
        
    }

    public void InitGame() 
    {
        m_StartGameTime = Time.unscaledTime;
        m_GameState = GameState.CountdownRunning;
    }

    public void StartGame()
    {
        m_GameState = GameState.GameRunning;
        m_UIManager.HideStartMenu();
        Time.timeScale = 1;
    }

    public void EndGame()
    {
        m_GameState = GameState.GameComplete;

        int grassCut = m_GrassManager.GetCutGrass();
        int totalGrass = m_GrassManager.GetTotalGrass();

        Time.timeScale = 0;
        m_UIManager.DisplayEndScreen(grassCut, totalGrass);
    }

    public void RestartGame()
    {
        Time.timeScale = 1;
        SceneManager.LoadScene("Lawn Mover");
    }
}
