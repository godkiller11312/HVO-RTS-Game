using JetBrains.Annotations;
using System.Collections.Generic;
using UnityEngine;

public abstract class Unit : MonoBehaviour
{

    [SerializeField] private ActionSO[] m_Actions;

    public bool IsMoving;
    public bool IsTargeted; 
    protected Animator m_Animator;
    protected AiPawn   m_AiPawn;        
    protected SpriteRenderer m_SpriteRenderer;
    protected Material m_OriginalMaterial;
    protected Material m_HighlightedMaterial;
    public ActionSO[] Actions => m_Actions; 

    protected void Awake()
    {
  

        if (TryGetComponent<Animator> (out var animator))
        {
            m_Animator = animator;
        }

        if(TryGetComponent<AiPawn>(out var aiPawn))
        {
            m_AiPawn = aiPawn;
        }   

      m_SpriteRenderer = GetComponent<SpriteRenderer>();    
        m_OriginalMaterial = m_SpriteRenderer.material; 
        m_HighlightedMaterial = Resources.Load<Material>("Material/Outline");      
    }

    public void MoveTo(Vector3 Destination)
    {
        var direction = (Destination - transform.position).normalized;
        m_SpriteRenderer.flipX = direction.x < 0;       
        m_AiPawn.SetDestination(Destination);       

    }

    public void Select()
    {
        HighLight();
        IsTargeted = true;
    }
    
    public void DeSelect()
    {
        UnHighLight (); 
        IsTargeted = false;  
    }

    void HighLight()
    {
        m_SpriteRenderer.material = m_HighlightedMaterial;
    }

    void UnHighLight()
    {
        m_SpriteRenderer.material = m_OriginalMaterial; 
    }
}
