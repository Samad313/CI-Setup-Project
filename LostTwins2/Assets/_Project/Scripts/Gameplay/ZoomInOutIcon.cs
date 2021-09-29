using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ZoomInOutIcon : MonoBehaviour
{
    [SerializeField] private Texture2D icon = default;
    [SerializeField] private Texture2D onIcon = default;
    [SerializeField] private Vector3 iconScale = default;
    [SerializeField] private Material iconMaterial = default;
    [SerializeField] private bool shouldMakeChildOfCenter = default;
    [SerializeField] private bool shouldSetRotation = false;
    [SerializeField] private float zOffset = 6.0f;
    [SerializeField] private float zoomOutAlpha = 1.0f;

    private float scaleDirection = 1.0f;

    private Transform iconTransform;
    private Transform parentTransform;

    public Transform IconTransform { get { return iconTransform; } }
    public Vector3 CenterPosition { get { return parentTransform.position; } }
    public float ZOffset { get { return zOffset; } }
    public float ZoomOutAlpha { get => zoomOutAlpha; }

    void Awake()
    {
        SpawnIcon();
    }

    private void SpawnIcon()
    {
        iconTransform = Instantiate(GameData.instance.IconTransform, null);

        if (shouldMakeChildOfCenter)
        {
            parentTransform = transform.Find("center");
        }
        else
        {
            parentTransform = transform;
        }

        iconTransform.position = parentTransform.position;
        if(shouldSetRotation)
        {
            iconTransform.localRotation = transform.localRotation;
        }

        if(transform.GetComponent<IOnOffState>()!=null)
        {

        }

        iconTransform.localScale = iconScale;
        if (iconMaterial)
        {
            iconTransform.GetComponent<Renderer>().sharedMaterial = iconMaterial;
        }
        else
        {
            iconTransform.GetComponent<Renderer>().material.mainTexture = icon;
        }
        
        iconTransform.GetComponent<Renderer>().material.color = new Color(1, 1, 1, 0);
        iconTransform.parent = parentTransform;
    }

    public void SetIconAgain()
    {
        if(transform.GetComponent<IOnOffState>().OnState)
        {
            iconTransform.GetComponent<Renderer>().material.mainTexture = onIcon;
        }
        else
        {
            iconTransform.GetComponent<Renderer>().material.mainTexture = icon;
        }
    }

    public void SetDirection(float direction)
    {
        iconTransform.localScale = new Vector3(Mathf.Abs(iconTransform.localScale.x) * direction, iconTransform.localScale.y, iconTransform.localScale.z);
    }
}
