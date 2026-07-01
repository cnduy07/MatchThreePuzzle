using UnityEngine;

[ExecuteAlways]
public class SafeAreaRoot : MonoBehaviour
{
    RectTransform m_rectTransform;
    Rect m_lastSafeArea;
    Vector2Int m_lastScreenSize;

    void OnEnable()
    {
        ApplySafeArea();
    }

    void Update()
    {
        if (Screen.safeArea != m_lastSafeArea || Screen.width != m_lastScreenSize.x || Screen.height != m_lastScreenSize.y)
        {
            ApplySafeArea();
        }
    }

    public void ApplySafeArea()
    {
        if (m_rectTransform == null)
        {
            m_rectTransform = GetComponent<RectTransform>();
        }

        if (m_rectTransform == null || Screen.width <= 0 || Screen.height <= 0)
        {
            return;
        }

        Rect safeArea = Screen.safeArea;
        if (safeArea.width <= 0f || safeArea.height <= 0f)
        {
            safeArea = new Rect(0f, 0f, Screen.width, Screen.height);
        }

        Vector2 anchorMin = safeArea.position;
        Vector2 anchorMax = safeArea.position + safeArea.size;
        anchorMin.x /= Screen.width;
        anchorMin.y /= Screen.height;
        anchorMax.x /= Screen.width;
        anchorMax.y /= Screen.height;

        m_rectTransform.anchorMin = anchorMin;
        m_rectTransform.anchorMax = anchorMax;
        m_rectTransform.offsetMin = Vector2.zero;
        m_rectTransform.offsetMax = Vector2.zero;

        m_lastSafeArea = safeArea;
        m_lastScreenSize = new Vector2Int(Screen.width, Screen.height);
    }
}
