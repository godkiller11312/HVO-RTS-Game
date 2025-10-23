using UnityEngine;

public class GameManager : SingletonManager<GameManager>
{
    private Vector2 m_InitialTouchPosition; 
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
        Debug.Log(inputPosition);
    }
}
