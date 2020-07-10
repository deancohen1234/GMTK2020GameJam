using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Robot : MonoBehaviour
{
    public float m_TotalEnergy = 100f;

    [Header("Speed Controls")]
    public float m_BaseSpeed = 2.0f;
    public float m_BaseTurnSpeed = 1.0f;
    public float m_SpeedBoostSpeed = 30.0f;

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

    void Update()
    {
        Move();
    }

    void Move() 
    {
        if (Input.GetKey(KeyCode.LeftArrow)) 
        {
            transform.Rotate(0, -0.5f, 0);
        }

        if (Input.GetKey(KeyCode.RightArrow))
        {
            transform.Rotate(0, 0.5f, 0);
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
}
