using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Robot : MonoBehaviour
{
    public CursorProjector m_CursorProjector;
    public float m_BaseSpeed = 2.0f;

    private Rigidbody m_Rigidbody;

    private void Awake()
    {
        m_Rigidbody = GetComponent<Rigidbody>();
    }

    void Update()
    {
        Vector3 position = m_CursorProjector.GetScreenProjectPosition(Input.mousePosition);
        MoveToPoint(position);

        m_CursorProjector.SetReticlePosition(transform.position, position);
    }

    void MoveToPoint(Vector3 point) 
    {
        Vector3 direction = point - transform.position;
        direction = direction.normalized;

        m_Rigidbody.velocity = direction * m_BaseSpeed;

        if (point == Vector3.zero) 
        {
            Debug.Log(point);
        }
    }
}
