using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices.ComTypes;
using UnityEngine;

public class CursorProjector : MonoBehaviour
{
    public GameObject m_ReticlePrefab;
    public float m_ReticleMaxDistance = 3f;
    
    private Camera m_Camera;
    private GameObject m_Reticle;

    private void Awake()
    {
        m_Camera = GetComponent<Camera>();
    }

    private void Start()
    {
        m_Reticle = Instantiate(m_ReticlePrefab);
    }

    public Vector3 GetScreenProjectPosition(Vector2 mousePosition) 
    {
        RaycastHit hit;
        Ray ray = m_Camera.ScreenPointToRay(Input.mousePosition);

        int mask = 1 << LayerMask.NameToLayer("Ground");

        if (Physics.Raycast(ray, out hit, 100f, mask))
        {
            float yHeight = hit.collider.transform.position.y;
            Vector3 raisedPosition = new Vector3(hit.point.x, yHeight + 1.0f, hit.point.z); 
            return raisedPosition;
        }
        else
        {
            return Vector3.zero;
        }
    }

    public void SetReticlePosition(Vector3 robotPosition, Vector3 mousePosition) 
    {
        float distance = Vector3.Distance(robotPosition, mousePosition);

        //if distance is greater than max, lock it to max range
        if (distance >= m_ReticleMaxDistance) 
        {
            Vector3 direction = mousePosition - robotPosition;
            direction = direction.normalized;

            Vector3 position = robotPosition + direction * m_ReticleMaxDistance;
            m_Reticle.transform.position = position; 
        }
        else 
        {
            m_Reticle.transform.position = mousePosition;
        }
    }
}
