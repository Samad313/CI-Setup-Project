using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

[RequireComponent(typeof(Button))]
public class ButtonDown : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    private Button m_button;

    private void Awake()
    {
        m_button = transform.GetComponent<Button>();
    }

    private void Update()
    {
        //if (m_pointerData != null)
        //{
        //    if (Time.realtimeSinceStartup - m_lastTrigger >= m_repeatDelay)
        //    {
        //        m_lastTrigger = Time.realtimeSinceStartup - (m_repeatDelay - m_repeatRetriggerInterval);
        //        m_button.OnSubmit(m_pointerData);
        //    }
        //}
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        m_button.OnSubmit(eventData);
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        Debug.Log("UP");
    }
}