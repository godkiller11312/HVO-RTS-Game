using UnityEngine;

public class PointToClick : MonoBehaviour
{
    [SerializeField] private float m_Duration  = 2f;
    [SerializeField] private float m_Timer = 0f;
    [SerializeField] private SpriteRenderer m_SpriteRenderer;
    [SerializeField] private AnimationCurve m_scaleCurve;

    private float m__FreqTimer;
    private Vector3 m_InitialScale;


    private void Start()
    {
        m_InitialScale = transform.localScale;
    
    }

    private void Update()
    {
        m_Timer += Time.deltaTime;
        m__FreqTimer += Time.deltaTime;
        m__FreqTimer %= 1f;

        float scaleMultiplier = m_scaleCurve.Evaluate(m__FreqTimer);    
        transform.localScale = m_InitialScale * scaleMultiplier;
        if (m_Timer >=m_Duration *0.9f)
        {
            float fadeProgress = (m_Timer - m_Duration * 0.9f) / (m_Duration * 0.1f);
            m_SpriteRenderer.color = new Color(1f, 1f, 1f, 1 - fadeProgress);
        }

        if (m_Timer >= m_Duration)
        {
            Destroy(gameObject);
        }
    }
}
