using JetBrains.Annotations;
using System.Collections.Generic;
using System.Security.Cryptography.X509Certificates;
using UnityEngine;

public enum UnitState
{
    Idle, Moving, Attacking, Chopping, Mining, Building
}

public enum UnitTask
{
    None, Build, Chop, Mine, Attack 
}

public abstract class Unit : MonoBehaviour
{

    [SerializeField] private ActionSO[] m_Actions;
    [SerializeField] protected float m_ObjectDetectionRadius = 3f;

    public UnitState CurrentState { get; protected set; }  =UnitState.Idle;
    public UnitTask CurrentTask { get; protected set; } = UnitTask.None;    
    public bool IsTargeted; 
    public Unit Target { get;protected set; }       
    protected Animator m_Animator;
    protected AiPawn   m_AiPawn;        
    protected SpriteRenderer m_SpriteRenderer;
    protected Material m_OriginalMaterial;
    protected Material m_HighlightedMaterial;
    public ActionSO[] Actions => m_Actions; 
    public bool HasTarget => Target != null;
    public SpriteRenderer Renderer => m_SpriteRenderer;
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
        OnSetDestination();
    }

    public void MoveTo(Vector3 Destination)
    {
        var direction = (Destination - transform.position).normalized;
        m_SpriteRenderer.flipX = direction.x < 0;       
        m_AiPawn.SetDestination(Destination);
        OnSetDestination();

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

    protected virtual void OnSetTask(UnitTask oldTask, UnitTask newTask)
    {
         CurrentTask = newTask;
    }

    protected virtual void OnSetState(UnitState oldState, UnitState newState)
    {
        CurrentState = newState;
    }

    protected Collider2D[] RunProximityObjectDetection() {
        return Physics2D.OverlapCircleAll(transform.position, m_ObjectDetectionRadius);
    }

    void HighLight()
    {
        m_SpriteRenderer.material = m_HighlightedMaterial;
    }

    void UnHighLight()
    {
        m_SpriteRenderer.material = m_OriginalMaterial; 
    }

    public void SetTask(UnitTask task)
    {
        OnSetTask(CurrentTask, task);
    }

    public void SetState(UnitState state)
    {
        OnSetState(CurrentState, state);                
    }

    public void SetTarget(Unit target)
    {
        Target = target;    
    }

    protected virtual void OnSetDestination()
    {

    }



        void OnDrawGizmos()
    {
        Gizmos.color = new Color(0, 0, 1, 0.3f);
        Gizmos.DrawSphere(transform.position, m_ObjectDetectionRadius);
    }
}
