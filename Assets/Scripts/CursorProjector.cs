using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CursorProjector : MonoBehaviour
{
    private Camera m_Camera;

    private void Awake()
    {
        m_Camera = GetComponent<Camera>();
    }

    public Vector3 GetScreenProjectPosition(Vector2 mousePosition) 
    {
        RaycastHit hit;
        Ray ray = m_Camera.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out hit, 100f, ~LayerMask.NameToLayer("Ground")))
        {
            return hit.point;
        }
        else
        {
            return Vector3.zero;
        }
    }
}
