using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Robot : MonoBehaviour
{
    public float m_TotalEnergy = 100f;
    public float m_BaseSpeed = 2.0f;
    public bool m_AlternateScheme = false;

    private Rigidbody m_Rigidbody;
    private float m_CurrentEnergy;

    private void Awake()
    {
        m_Rigidbody = GetComponent<Rigidbody>();
    }

    private void Start()
    {
        m_CurrentEnergy = m_TotalEnergy;
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

        m_Rigidbody.velocity = transform.forward * m_BaseSpeed;
    }
}
