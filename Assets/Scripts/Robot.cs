using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Robot : MonoBehaviour
{
    public UIManager m_UIHandler;

    public float m_TotalEnergy = 100f;
    public float m_LossTickRate = 1.0f; //in seconds
    public float m_EnergyLossPerTick = 10.0f;

    [Header("Speed Controls")]
    public float m_BaseSpeed = 2.0f;
    public float m_BaseTurnSpeed = 1.0f;
    public float m_TurnDampening = 1.0f;
    public float m_SpeedBoostSpeed = 30.0f;

    private float m_GameTimeKeeper;
    private OverloadController m_OverloadController;
    private Rigidbody m_Rigidbody;

    private float m_Speed;
    private float m_CurrentEnergy;

    private void Awake()
    {
        m_Rigidbody = GetComponent<Rigidbody>();
        m_OverloadController = GetComponent<OverloadController>();
    }

    private void Start()
    {
        m_CurrentEnergy = m_TotalEnergy;
        m_Speed = m_BaseSpeed;
    }

    private void OnEnable()
    {
        m_OverloadController.m_OnSpeedBoostStart.AddListener(OnSpeedBoostStart);
        m_OverloadController.m_OnSpeedBoostEnd.AddListener(OnSpeedBoostEnd);
    }

    private void OnDisable()
    {
        m_OverloadController.m_OnSpeedBoostStart.RemoveAllListeners();
    }

    private void Update()
    {
        CalculateEnergy();
    }

    void FixedUpdate()
    {
        Move();
    }

    void Move() 
    {
        if (Input.GetKey(KeyCode.LeftArrow)) 
        {
            transform.rotation = Quaternion.Lerp(transform.rotation, transform.rotation * Quaternion.Euler(0, -m_BaseTurnSpeed * m_Speed, 0), Time.deltaTime * m_TurnDampening);
        }

        if (Input.GetKey(KeyCode.RightArrow))
        {
            transform.rotation = Quaternion.Lerp(transform.rotation, transform.rotation * Quaternion.Euler(0, m_BaseTurnSpeed * m_Speed, 0), Time.deltaTime * m_TurnDampening);
        }

        m_Rigidbody.velocity = transform.forward * m_Speed;
    }

    void OnSpeedBoostStart() 
    {
        Debug.Log("Speeding Up!!!");
        m_Speed = m_SpeedBoostSpeed;
    }

    void OnSpeedBoostEnd() 
    {
        Debug.Log("It chill now");
        m_Speed = m_BaseSpeed;
    }

    void CalculateEnergy() 
    {
        m_GameTimeKeeper += Time.deltaTime;

        if (m_GameTimeKeeper >= m_LossTickRate) 
        {
            m_GameTimeKeeper = 0;
            m_CurrentEnergy = Mathf.Clamp(m_CurrentEnergy - m_EnergyLossPerTick, 0, m_TotalEnergy);

            //update ui not every frame
            float normalizedEnergy = DeanUtils.Map(m_CurrentEnergy, 0, m_TotalEnergy, 0.0f, 1.0f);
            m_UIHandler.SetEnergyBarValue(normalizedEnergy);
        }
    }
}
