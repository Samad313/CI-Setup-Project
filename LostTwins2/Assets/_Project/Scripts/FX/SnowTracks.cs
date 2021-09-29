using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SnowTracks : MonoBehaviour
{
    private Texture2D decal;
    private Color[] decalColors;

    [SerializeField]
    private int decalResolutionX = 128;
    [SerializeField]
    private int decalResolutionY = 128;

    [SerializeField]
    private string decalProperty = "_BumpMap";

    // Start is called before the first frame update
    void Start()
    {
        decal = new Texture2D(decalResolutionX, decalResolutionY);
        GetComponent<Renderer>().material.SetTexture(decalProperty, decal);

        decalColors = new Color[decalResolutionX * decalResolutionY];
        for (int i = 0; i < decalColors.Length; i++)
        {
            decalColors[i] = new Color(0, 0, 0, 0);
        }
        decal.SetPixels(decalColors);
        decal.Apply();
    }

    // Update is called once per frame
    void Update()
    {
        //decalColors[Random.Range(0, decalColors.Length)] = Random.ColorHSV();
        decal.SetPixels(decalColors);
        decal.Apply();
    }

    void OnCollisionStay(Collision col)
    {
        if(col.gameObject.layer==14)
        {
            Debug.Log(col.contactCount);
            Vector2[] uvs = new Vector2[col.contactCount];
            ContactPoint[] contactPoints = col.contacts;
            
            for (int i = 0; i < contactPoints.Length; i++)
            {
                uvs[i] = GetUVFromPoint(contactPoints[i].point);
                
                if(uvs[i].x>=0)
                {
                    int colorIndexY = decalResolutionX * Mathf.RoundToInt(((decalResolutionY - 1.0f) * uvs[i].y));
                    int colorIndexX = Mathf.RoundToInt( uvs[i].x * (decalResolutionX - 1.0f) );

                    //Debug.Log(uvs[i].x + ":" + uvs[i].y + " - " + colorIndexY + ":" + colorIndexX);
                    ColorThisPixel(colorIndexX+ colorIndexY);
                }
            }
        }
    }

    private Vector2 GetUVFromPoint(Vector3 point)
    {
        RaycastHit hit;
        if(Physics.Raycast(point + new Vector3(0, 5.0f, 0), new Vector3(0, -1.0f, 0), out hit, 20.0f, 1<<15))
        {
            return hit.textureCoord;
        }

        return new Vector2(-1, -1);
    }

    private void ColorThisPixel(int index)
    {
        Color centerColor = new Color(0.1f, 0.1f, 0.1f, 0.5f);
        decalColors[index] = centerColor;

        Color fadedColor = new Color(0.1f, 0.1f, 0.1f, 0.5f);

        if (index % decalResolutionX != 0 && index!=0)
            decalColors[index - 1] = fadedColor;

        if ((index+1) % decalResolutionX != 0 && index < decalColors.Length-1)
            decalColors[index + 1] = fadedColor;

        if(index>=decalResolutionX)
            decalColors[index - decalResolutionX] = fadedColor;

        //if (index >= decalResolutionX)
            decalColors[index + decalResolutionX] = fadedColor;

        decalColors[index - decalResolutionX - 1] = fadedColor;
        decalColors[index - decalResolutionX + 1] = fadedColor;
        decalColors[index + decalResolutionX - 1] = fadedColor;
        decalColors[index + decalResolutionX + 1] = fadedColor;
    }
}
