using UnityEngine;
using UnityEngine.Tilemaps;

public class PlacementProcess
{
    private GameObject m_PlacementOutline;
    private BuildActionSO m_BuildAction;

    private Vector3Int[] m_HightlightPositions;

    private Tilemap m_WalkableTilemap;
    private Tilemap m_OverlayTilemap;
    private Tilemap[] m_UnreachableTilemaps;
    private Sprite m_PlaceHolderTileSprite;
    private Color m_HighlightColor = new Color(0f, 0.8f, 1f, 0.4f);
    private Color m_BlockedColor = new Color(0f, 0.2f, 0f, 0.8f);
    public PlacementProcess(BuildActionSO buildAction, Tilemap walkableTilemap, Tilemap overlayTilemap, Tilemap[] unreachableTilemaps)
    {
        m_PlaceHolderTileSprite = Resources.Load<Sprite>("Images/PlaceHolderTileSprite");
        m_BuildAction = buildAction;
        m_WalkableTilemap = walkableTilemap;
        m_OverlayTilemap = overlayTilemap;
        m_UnreachableTilemaps = unreachableTilemaps;    
    }

    public void Update()
    {
        if (m_PlacementOutline != null)
        {
            HighlightTiles(m_PlacementOutline.transform.position);
        }

        if (HvoUtils.TryGetHoldPosition(out Vector3 worldPosition))
        {
            m_PlacementOutline.transform.position = SnaptoGrid(worldPosition);
        }

    }

    public void ShowPlacementOutLine()
    {
        m_PlacementOutline = new GameObject("Placement OutLine");
        var renderer = m_PlacementOutline.AddComponent<SpriteRenderer>();
        renderer.sortingOrder = 999;
        renderer.color = new Color(1f, 1f, 1f, 0.5f);
        renderer.sprite = m_BuildAction.PlacementSprite;
    }

    Vector3 SnaptoGrid(Vector3 worldPosition)
    {
        return new Vector3(Mathf.FloorToInt(worldPosition.x), Mathf.FloorToInt(worldPosition.y), 0);
    }

    void HighlightTiles(Vector3 outlinePosition)
    {
        Vector3Int buildingSize = m_BuildAction.BuildingSize; // Giả sử kích thước tòa nhà là 2x2
        
        Vector3 pivotPosition = outlinePosition + m_BuildAction.OriginOffset;
        ClearHighlight();
        m_HightlightPositions = new Vector3Int[buildingSize.x * buildingSize.y];
        for (int x = 0; x < buildingSize.x; x++)
        {
            for (int y = 0; y < buildingSize.y; y++)
            {
                m_HightlightPositions[x + y * buildingSize.x] = new Vector3Int((int)pivotPosition.x + x, (int)pivotPosition.y + y, 0);
            }
        }

        foreach (var tilePosition in m_HightlightPositions)
        {
            var tile = ScriptableObject.CreateInstance<Tile>();
            tile.sprite = m_PlaceHolderTileSprite;  
            if (CanPlaceTile(tilePosition))
            {
                tile.color = m_HighlightColor;
            }
               
            else
            {
                tile.color = m_BlockedColor;
            }
            
            m_OverlayTilemap.SetTile(tilePosition, tile);
            //m_overlayTilemap.SetTileFlags(tilePosition, TileFlags.None);       
            //m_overlayTilemap.SetColor(tilePosition, Color.green);
        }
    }

    void ClearHighlight()
    {
        if (m_HightlightPositions == null) return;

        foreach (var tilePosition in m_HightlightPositions)
        {
            m_OverlayTilemap.SetTile(tilePosition, null);
        }
    }

    bool CanPlaceTile(Vector3Int tilePosition)
    {
        return m_WalkableTilemap.HasTile(tilePosition) && !IsUnreachableTilemap(tilePosition) && !IsBlockedByGameobject(tilePosition);  
    }   

    bool IsUnreachableTilemap(Vector3Int tilePosition)
    {
        foreach (var tilemap in m_UnreachableTilemaps)
        {
            if (tilemap.HasTile(tilePosition))
            {
                return true;
            }
        }
        return false;
    }   
    bool IsBlockedByGameobject(Vector3Int tilePosition)
    {
        Vector3 tileSize = m_WalkableTilemap.cellSize;  
        Collider2D[] colliders = Physics2D.OverlapBoxAll(tilePosition + tileSize / 2, tileSize *0.5f, 0);
     
        foreach (var collider in colliders)
        {
            var layer = collider.gameObject.layer;  
            if(layer == LayerMask.NameToLayer("Player"))
            {
                return true;    
            }   
           
        }
        return false;
    }
}

