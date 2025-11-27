using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Tilemaps;

public enum ClickType
{
     Move ,Attack, Build
}

public class GameManager : SingletonManager<GameManager>
{

    [Header("Tilemaps")]
    [SerializeField] private Tilemap m_WalkableTilemap;
    [SerializeField] private Tilemap m_OverlayTilemap;
    [SerializeField] private Tilemap[] m_UnreachableTilemaps;   

    [Header("UI")]
    [SerializeField] private PointToClick m_pointToMovePrefab;
    [SerializeField] private PointToClick m_pointToBuildPrefab;
    [SerializeField] private ConfirmationBar m_BuildConfirmationBar;     
    public Unit ActiveUnit;
    public ActionBar m_ActionBar;
    private int m_Gold = 1000;
    private int m_Wood = 1000;
    [Header("VFX")]

    [SerializeField] private ParticleSystem m_ConstructionEffectPrefab;

    
    
    
    public int Gold => m_Gold;
    public int Wood => m_Wood;  

    public bool HasActiveUnit => ActiveUnit != null;


    [SerializeField] private PlacementProcess m_PlacementProcess;    

    private void Start()
    {
        Clear_ActionBar_UI();
    }

    private void Update()
    {
        if(m_PlacementProcess != null)
        {
           
            m_PlacementProcess.Update();
            return;
        }
        else if (HvoUtils.TryGetShortLeftClickPosition(out var inputPosition))
        {
            DetectClick(inputPosition);
        }
       



    }

    void DetectClick(Vector2 inputPosition)
    {

        if (HvoUtils.IsPointerOverUIElement())
        {
            return;
        }
        Vector2 worldPoint = Camera.main.ScreenToWorldPoint(inputPosition);
        RaycastHit2D hit = Physics2D.Raycast(worldPoint, Vector2.zero);     
        
        if (HasClickedOnUnit(hit, out var unit))
        {
            HandleClickOnUnit(unit);
        }
        else
        {       
            HandleClickOnGround(worldPoint);
        }       
    }

    void HandleClickOnGround(Vector2 worldPoint)
    {
        if (HasActiveUnit && IsHumanoid(ActiveUnit))
        {
            DisplayClickEffect(worldPoint, ClickType.Move);
            ActiveUnit.MoveTo(worldPoint);
        }
     
    }   

    bool HasClickedOnUnit(RaycastHit2D hit, out Unit unit)
    {
        if(hit.collider != null && hit.collider.TryGetComponent<Unit> (out var clickedUnit))
        {
            unit = clickedUnit;
            return true;
        }

        unit = null;
        return false;
    }

    bool HasClickedOnActiveUnit(Unit clickedUnit)
    {
        
        return clickedUnit == ActiveUnit;
    }
    void HandleClickOnUnit(Unit unit)
    {
        if (HasActiveUnit)
        {
            if (HasClickedOnActiveUnit(unit))
            {
                CancelActiveUnit();
                return;
            }
            else if (WorkerClickedOnUnfinishedBuild(unit))
            {
                DisplayClickEffect(unit.transform.position, ClickType.Build);
               ((WorkerUnit)ActiveUnit).SendToBuild(unit as StructureUnit);
                return;
            }
        }

        SelectNewUnit(unit);
    }

    bool WorkerClickedOnUnfinishedBuild (Unit clickedUnit)
    { return ActiveUnit is WorkerUnit && clickedUnit is StructureUnit structure && structure.IsUnderConstruction; }

    void SelectNewUnit (Unit unit)
    {
        if (ActiveUnit != null)
        {
            ActiveUnit.DeSelect();
        }   
        ActiveUnit = unit;  
        ActiveUnit.Select(); 
        Show_UnitActions(unit);
    }

    void CancelActiveUnit()
    {
        
        ActiveUnit.DeSelect();
        ActiveUnit = null;
        Clear_ActionBar_UI();   
    }
    void DisplayClickEffect(Vector2 WorldPoint, ClickType clickType)
    {
        if(clickType == ClickType.Move)
        {
            Instantiate(m_pointToMovePrefab, (Vector3)WorldPoint, Quaternion.identity);
        }
        else if(clickType == ClickType.Build)
        {
            Instantiate(m_pointToBuildPrefab, (Vector3)WorldPoint, Quaternion.identity);
        }
    }

    bool IsHumanoid(Unit unit)
    {
        return unit is HumanoidUnit;
    }

    void Show_UnitActions(Unit unit)
    {
        Clear_ActionBar_UI();
        if (unit.Actions.Length == 0 )
        {
            m_ActionBar.Hide();
            return;
        }

        m_ActionBar.Show();
        foreach (var action in unit.Actions)
        {
            m_ActionBar.RegisterAction(action.Icon, () => action.Execute(this));
        }   
    }   

    void Clear_ActionBar_UI()
    {
        m_ActionBar.ClearActions();
        m_ActionBar.Hide();
    }
    
    void ConfirmBuildPlacement()
    {
        if(!TryDeductResources(m_PlacementProcess.BuildAction.GoldCost, m_PlacementProcess.BuildAction.WoodCost))
        {
            Debug.Log("Not enough resources to build!");
            return;
        }
        if (m_PlacementProcess.TryFinalizePlacement(out Vector3 buildPosition))
        {
            DisplayClickEffect(buildPosition,ClickType.Build);      
            m_BuildConfirmationBar.Hide();
            new BuildingProcess(m_PlacementProcess.BuildAction, buildPosition, (WorkerUnit)ActiveUnit , m_ConstructionEffectPrefab); 

            ActiveUnit.MoveTo(buildPosition);
            ActiveUnit.SetTask(UnitTask.Build); 
            m_PlacementProcess = null;
        }
        else
        {
                       RevertResources(m_PlacementProcess.BuildAction.GoldCost, m_PlacementProcess.BuildAction.WoodCost);
            Debug.Log("Invalid placement position!");
        }
    }

    void CancelBuildPlacement()
    {

        m_BuildConfirmationBar.Hide();
        m_PlacementProcess.ClearUp();
        m_PlacementProcess = null;  

    }   

    public void StartBuildProcess(BuildActionSO buildAction)
    {
      
        if(m_PlacementProcess != null)
        {
            return;
        }
        m_PlacementProcess = new PlacementProcess(buildAction, m_WalkableTilemap, m_OverlayTilemap, m_UnreachableTilemaps);
        m_PlacementProcess.ShowPlacementOutLine();
        m_BuildConfirmationBar.SetupHooks(ConfirmBuildPlacement, CancelBuildPlacement);
        m_BuildConfirmationBar.Show(buildAction.GoldCost, buildAction.WoodCost);   
       

    }

    bool TryDeductResources(int goldCost, int woodCost)
    {
        if(m_Gold >= goldCost && m_Wood >= woodCost)
        {
            m_Gold -= goldCost;
            m_Wood -= woodCost;
            return true;
        }
        return false;
    }
    void OnGUI()
    {
        GUI.Label(new Rect(20, 40, 200, 20), "Gold: " + m_Gold.ToString(), new GUIStyle { fontSize = 30});
        GUI.Label(new Rect(20, 80, 200, 20), "Gold: " + m_Wood.ToString(), new GUIStyle { fontSize = 30 });
        if (ActiveUnit!= null)
        {
            GUI.Label(new Rect(20, 120, 200, 20), "State: " + ActiveUnit.CurrentState.ToString(), new GUIStyle { fontSize = 30 });
            GUI.Label(new Rect(20, 160, 200, 20), "Task: " + ActiveUnit.CurrentTask.ToString(), new GUIStyle { fontSize = 30 });
        }
    }

    void RevertResources(int gold, int wood)
    {
        m_Gold += gold;
        m_Wood += wood;
    }
}
 