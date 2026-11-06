using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UnityEngine.InputSystem.UI;
using System.Collections.Generic;

public class ButtonEventDebugger : MonoBehaviour, IPointerClickHandler, IPointerDownHandler, IPointerEnterHandler
{
    private Button btn;
    private Graphic graphic;

    void Awake()
    {
        btn = GetComponent<Button>();
        graphic = GetComponent<Graphic>();
    }

    void Start()
    {
        Debug.Log($"<color=#66CCFF>[DEBUG INIT] Button '{gameObject.name}' initialized.</color>");
        Debug.Log($" - Button Component Present: {btn != null}, Interactable: {(btn != null ? btn.interactable : false)}");
        Debug.Log($" - Graphic Present: {graphic != null}, RaycastTarget: {(graphic != null ? graphic.raycastTarget : false)}");

        if (btn != null)
        {
            btn.onClick.AddListener(() =>
            {
                Debug.Log($"<color=#33FF66>[SUCCESS] Unity Button.onClick FIRED for '{gameObject.name}'!</color>");
            });
        }
    }

    void Update()
    {
        // Auto-heal EventSystem if it ever gets destroyed or nullified
        if (EventSystem.current == null)
        {
            Debug.LogError("<color=red>[AUTO-FIX TRIGGERED] EventSystem.current was NULL! Restoring an active EventSystem...</color>");
            EventSystem existing = FindAnyObjectByType<EventSystem>(FindObjectsInactive.Include);
            if (existing != null)
            {
                existing.gameObject.SetActive(true);
                existing.enabled = true;
                EventSystem.current = existing;
            }
            else
            {
                GameObject esObj = new GameObject("EventSystem_AutoRestored", typeof(EventSystem), typeof(InputSystemUIInputModule));
                DontDestroyOnLoad(esObj);
            }
        }

        // Global mouse click inspection
        if (Input.GetMouseButtonDown(0))
        {
            if (EventSystem.current == null)
            {
                Debug.LogError("<color=red>[CRITICAL CLICK] EventSystem.current remains NULL even after restore attempt!</color>");
                return;
            }

            PointerEventData pointerData = new PointerEventData(EventSystem.current)
            {
                position = Input.mousePosition
            };

            List<RaycastResult> results = new List<RaycastResult>();
            EventSystem.current.RaycastAll(pointerData, results);

            if (results.Count > 0)
            {
                Debug.Log($"<color=yellow>[SCREEN CLICK at {Input.mousePosition}] Top Raycast Hit: '{results[0].gameObject.name}' (Total hits: {results.Count})</color>");
                for (int i = 0; i < results.Count; i++)
                {
                    Debug.Log($"   [Hit {i}] '{results[i].gameObject.name}' (Canvas/Module: {results[i].module?.gameObject.name}, Depth: {results[i].depth})");
                }
            }
            else
            {
                Debug.LogWarning($"<color=orange>[SCREEN CLICK at {Input.mousePosition}] 0 UI Raycast hits! Mouse clicked through to empty space.</color>");
            }
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        Debug.Log($"<color=#FFD94D>[HOVER] Mouse entered: '{gameObject.name}'</color>");
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        Debug.Log($"<color=#33FF66>[POINTER DOWN] Physical mouse press registered on: '{gameObject.name}'</color>");
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        Debug.Log($"<color=#33FF66>[POINTER CLICK] IPointerClickHandler executed on: '{gameObject.name}'</color>");
    }
}