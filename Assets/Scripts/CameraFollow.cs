using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public Transform m_FollowTarget;
    public float m_CameraForwardPredictionDistance = 1.0f;
    public Vector3 m_Offset;
    public float m_SmoothSpeed = 1.0f;

    void LateUpdate()
    {
        Vector3 distancedCameraPos = m_FollowTarget.position + m_FollowTarget.forward * m_CameraForwardPredictionDistance + m_Offset;
        Vector3 smoothedNewVector = Vector3.Lerp(transform.position, distancedCameraPos, Time.deltaTime * m_SmoothSpeed);

        transform.position = smoothedNewVector;
    }
}
