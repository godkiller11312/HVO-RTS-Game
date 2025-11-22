using System.Collections.Generic;
using UnityEngine;

public class HumanoidUnit : Unit
{
    protected Vector2 m_Velocity;
    protected Vector3 m_LastPosition;

    public float CurrentSpeed => m_Velocity.magnitude;

    private void Start()
    {
        m_LastPosition = transform.position;        
    }
    protected void Update()
    {
        m_Velocity = new Vector2(
            (transform.position.x - m_LastPosition.x) / Time.deltaTime,
            (transform.position.z - m_LastPosition.z) / Time.deltaTime
        );
        m_LastPosition = transform.position;
        IsMoving = m_Velocity.magnitude > 0f;
        m_Animator.SetFloat("Speed", Mathf.Clamp01(CurrentSpeed));
    }

}

