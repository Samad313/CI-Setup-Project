using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[ExecuteInEditMode]
public class LevelEditor : MonoBehaviour
{
    [SerializeField]
    private Material onMaterial = default;

    [SerializeField]
    private Material offMaterial = default;

    [SerializeField]
    private Material specialMaterial = default;

    private Vector3 lastMousePosition = default;

    [SerializeField]
    private bool shouldCycleSpecial = false;

    private List<GameObject> currentChangedBoxes; 

    // Start is called before the first frame update
    void Start()
    {
        currentChangedBoxes = new List<GameObject>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButton(0))
        {
            
            Ray ray = Camera.main.ScreenPointToRay(lastMousePosition);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit, 120, 1<<13))
            {
                if(currentChangedBoxes.Contains(hit.collider.gameObject)==false)
                {
                    currentChangedBoxes.Add(hit.collider.gameObject);
                    hit.collider.gameObject.GetComponent<LevelEditorBox>().WasClicked(onMaterial, offMaterial, specialMaterial, shouldCycleSpecial);
                }
                
            }
        }
        else
        {
            currentChangedBoxes.Clear();
        }
#if UNITY_EDITOR
        EditorUtility.SetDirty(gameObject);
#endif
    }

    void OnGUI()
    {
        if (Event.current.type == EventType.Layout || Event.current.type == EventType.Repaint)
        {
#if UNITY_EDITOR
            EditorUtility.SetDirty(this); // this is important, if omitted, "Mouse down" will not be display
#endif
        }
        else if (Event.current.type == EventType.MouseDrag || Event.current.type == EventType.MouseDown)
        {
            lastMousePosition = Event.current.mousePosition;
            lastMousePosition.y = Screen.height - lastMousePosition.y;
        }
    }
}
