using UnityEngine;

public class GameManager : SingletonManager<GameManager>
{
    [Header("UI")]
    [SerializeField] private PointToClick m_pointToClickPrefab;   

    public Unit ActiveUnit;

    private Vector2 m_InitialTouchPosition; 
    public bool HasActiveUnit => ActiveUnit != null;    
    private void Update()
    {
        Vector2 inputPosition = Input.touchCount > 0 ? Input.GetTouch(0).position : Input.mousePosition;

        if (Input.GetMouseButtonUp(0) || (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began))
        {
            DetectClick(inputPosition);
            m_InitialTouchPosition = inputPosition;     
        }
        if (Input.GetMouseButtonUp(0) || (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Ended))
        {
            if(Vector2.Distance(m_InitialTouchPosition, inputPosition) < 10f)
            { 
                DetectClick(inputPosition); 
            }

        }
    }

    void DetectClick(Vector2 inputPosition)
    {
           
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

    bool HasClickedOnActiveUnit(Unit ClickedUnit)

    {
        return ClickedUnit == ActiveUnit;   
    }

    void HandleClickOnUnit(Unit unit)
    {
        if(HasActiveUnit)
        {
            if(HasClickedOnActiveUnit(unit))
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
    }

    void CancelActiveUnit()
    {
        ActiveUnit.DeSelect();
        ActiveUnit = null;   
    }
    void DisplayClickEffect(Vector2 WorldPoint)
    {
        Instantiate(m_pointToClickPrefab, (Vector3)WorldPoint, Quaternion.identity);
    }

    bool IsHumanoid(Unit unit)
    {
        return unit is HumanoidUnit;
    }
}
 