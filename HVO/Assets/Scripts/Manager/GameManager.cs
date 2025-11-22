using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Tilemaps;

public class GameManager : SingletonManager<GameManager>
{

    [Header("Tilemaps")]
    [SerializeField] private Tilemap m_WalkableTilemap;
    [SerializeField] private Tilemap m_OverlayTilemap;
    [SerializeField] private Tilemap[] m_UnreachableTilemaps;   

    [Header("UI")]
    [SerializeField] private PointToClick m_pointToClickPrefab;   

    public Unit ActiveUnit;
    public ActionBar m_ActionBar; 



  
    
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
            DisplayClickEffect(worldPoint);
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
        }

        SelectNewUnit(unit);
    }

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
    void DisplayClickEffect(Vector2 WorldPoint)
    {
        Instantiate(m_pointToClickPrefab, (Vector3)WorldPoint, Quaternion.identity);
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
   

    public void StartBuildProcess(BuildActionSO buildAction)
    {
      
        m_PlacementProcess = new PlacementProcess(buildAction, m_WalkableTilemap, m_OverlayTilemap, m_UnreachableTilemaps);
        m_PlacementProcess.ShowPlacementOutLine();
    }
}
 