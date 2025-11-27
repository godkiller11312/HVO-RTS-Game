using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class ConfirmationBar : MonoBehaviour
{
    [SerializeField] private ResourceRequirementsDisplay m_ResourceDisplay;
    [SerializeField] private Button m_ConfirmationButton;
    [SerializeField] private Button m_CancelButton;
    public void Show(int gold, int wood)
    {
        gameObject.SetActive(true);
        m_ResourceDisplay.Show(gold, wood);
    }
    public void Hide ()
    { 
        gameObject.SetActive(false);    
    }

    void OnDisable()
    {
        m_ConfirmationButton.onClick.RemoveAllListeners();
        m_CancelButton.onClick.RemoveAllListeners();
    }   

    public void SetupHooks(UnityAction onConfirm, UnityAction onCancel)
    {
        m_ConfirmationButton.onClick.RemoveAllListeners();
        m_ConfirmationButton.onClick.AddListener(onConfirm);
        m_CancelButton.onClick.RemoveAllListeners();
        m_CancelButton.onClick.AddListener(onCancel);
    }
}
