using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class WorkerUnit : HumanoidUnit
{
    public void OnBuildingFinished() => ResetState();

    protected override void OnSetDestination() => ResetState();

    protected override void UpdateBehaviour()
    {
        if (CurrentTask == UnitTask.Build && HasTarget)
        {
            CheckForConstruction();
        }

    }

    public void SendToBuild(StructureUnit structure)
    {
        MoveTo(structure.transform.position);
        SetTarget(structure);
        SetTask(UnitTask.Build);
    }

    void CheckForConstruction()
    {
        var distanceToConstruction = Vector3.Distance(transform.position, Target.transform.position);
        if (distanceToConstruction <= m_ObjectDetectionRadius && CurrentState == UnitState.Idle)
        {
            StartBuilding(Target as StructureUnit);
        }
        
    }

    void StartBuilding(StructureUnit structure)
    {
        SetState(UnitState.Building);
        m_Animator.SetBool("IsBuilding", true); 
        structure.AssignWorkerToBuildProcess(this); 

    }

    void ResetState()
    {
        SetTask(UnitTask.None);
        if (HasTarget) CleanupTarget();
        m_Animator.SetBool("IsBuilding", false);
    }

    void CleanupTarget()
    {
        if (Target is StructureUnit structure)
        {
            structure.UnassignWorkerFromBuildProcess();
        }
        
        SetTarget(null);    
    }

 
}


//private void CheckForCloseObjects()
//{
//    Debug.Log("checking");
//    var hits = RunProximityObjectDetection();
//    foreach (var hit in hits)
//    {
//        if (hit.gameObject == this.gameObject) continue;
//        Debug.Log(hit.gameObject.name);

//        if (CurrentTask == UnitTask.Build && Target != null && hit.gameObject == Target.gameObject)

//        {
//            if (hit.TryGetComponent<StructureUnit>(out var unit))
//            {
//                StartBuilding(unit);
//            }
//        }
//    }
//}