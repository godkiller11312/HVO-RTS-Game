using UnityEngine;

public class PlacementProcess 
{
    private GameObject m_PlacementOutLine;  
    private BuildActionSO m_BuildAction;
    public PlacementProcess(BuildActionSO buildAction)
    {
        m_BuildAction = buildAction;    
    }

    public void Update()
    {
      
        if(HvoUtils.TryGetHoldPosition(out Vector3 worldPosition))
        {
            m_PlacementOutLine.transform.position = SnaptoGrid(worldPosition);  
        }  
          
    }

    public void ShowPlacementOutLine()
    {
        m_PlacementOutLine = new GameObject("Placement OutLine");
        var renderer = m_PlacementOutLine.AddComponent<SpriteRenderer>();
        renderer.sortingOrder = 999;
        renderer.color = new Color(1f, 1f, 1f, 0.5f);
        renderer.sprite = m_BuildAction.PlacementSprite;
    }

    Vector3 SnaptoGrid(Vector3 worldPosition)
    {
        return new Vector3(Mathf.FloorToInt(worldPosition.x), Mathf.FloorToInt(worldPosition.y), 0);
    }   
}
