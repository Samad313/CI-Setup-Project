using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ElementsManager : MonoBehaviour
{
    private List<Transform> allElements;
    private List<Transform> allElementsParents;

    [SerializeField]
    private Transform[] extraElements = default;

    public void Init()
    {
        allElements = new List<Transform>();
        allElementsParents = new List<Transform>();
        foreach (Transform child in transform)
        {
            allElements.Add(child);
            allElementsParents.Add(child.parent);

            if(child.GetComponent<Water>())
            {
                GameplayManager.instance.ContainsWaterInThisLevel();
            }
        }

        for (int i = 0; i < extraElements.Length; i++)
        {
            allElements.Add(extraElements[i]);
            allElementsParents.Add(extraElements[i].parent);
        }

        //if(GameplayManager.instance.ZoomedInStart==false)
        //{
        //    AddPhoenixToElementsList();
        //}
    }

    public void MakeChildrenOfPieces()
    {
        for (int i = 0; i < allElements.Count; i++)
        {
            allElements[i].parent = GameplayManager.instance.GetPieceHoverTransformFromPosition(new Vector2(allElements[i].position.x, allElements[i].position.y));
        }
    }

    public void MakeChildrenOfElementsGroup()
    {
        for (int i = 0; i < allElements.Count; i++)
        {
            allElements[i].parent = allElementsParents[i];
        }
    }

    public void ElementDestroyed(Transform elementToDestroy)
    {
        for (int i = 0; i < allElements.Count; i++)
        {
            if(allElements[i]== elementToDestroy)
            {
                allElements.RemoveAt(i);
                allElementsParents.RemoveAt(i);
                break;
            }
        }
    }

    public void SetActiveStatusOfAllMovingObjects(bool value)
    {
        for (int i = 0; i < allElements.Count; i++)
        {
            //Piece myPiece = PiecesManager.instance.GetPieceFromPosition(new Vector2(allElements[i].position.x, allElements[i].position.y));
            //if(myPiece.GetConnectedState())
            //{
            if (allElements[i].GetComponent<PushableBox>())
                    allElements[i].GetComponent<PushableBox>().SetActive(value);
            else if (allElements[i].GetComponent<BigFlyingCreature>())
                allElements[i].GetComponent<BigFlyingCreature>().SetActive(value);

            if (allElements[i].Find("Collider"))
            {
                if (allElements[i].Find("Collider").GetComponent<Rope>())
                {
                    allElements[i].Find("Collider").GetComponent<Rope>().SetActive(value);
                }
            }

            if (allElements[i].GetComponent<BrokenDynamicPiece>())
                allElements[i].GetComponent<BrokenDynamicPiece>().SetActive(value);
        }
    }

    public void ZoomingInOut(float t, Vector3 cameraPosition)
    {
        float adjustedT = Mathf.InverseLerp(0.3f, 1.0f, t);
        for (int i = 0; i < allElements.Count; i++)
        {
            Vector3 startingPosition = Vector3.zero;
            float zOffset = 6.0f;

            Transform iconTransform = null;
            if (allElements[i].GetComponent<ZoomInOutIcon>())
            {
                iconTransform = allElements[i].GetComponent<ZoomInOutIcon>().IconTransform;
                startingPosition = allElements[i].GetComponent<ZoomInOutIcon>().CenterPosition;
                zOffset = allElements[i].GetComponent<ZoomInOutIcon>().ZOffset;
                adjustedT *= allElements[i].GetComponent<ZoomInOutIcon>().ZoomOutAlpha;
            }
            //if (allElements[i].GetComponent<PressureButton>())
            //{
            //    iconTransform = allElements[i].GetComponent<PressureButton>().GetIconTransform();
            //    startingPosition = allElements[i].GetComponent<PressureButton>().GetCenterPosition();
            //}

            //else if (allElements[i].GetComponent<PushableBox>())
            //    iconTransform = allElements[i].GetComponent<PushableBox>().GetIconTransform();
            //else if (allElements[i].GetComponent<FinalGate>())
            //{
            //    iconTransform = allElements[i].GetComponent<FinalGate>().GetIconTransform();
            //    startingPosition = allElements[i].GetComponent<FinalGate>().GetCenterPosition();

            //    zOffset = 5.8f;
            //}

            //else if (allElements[i].GetComponent<Star>())
            //    iconTransform = allElements[i].GetComponent<Star>().GetIconTransform();
            //else if (allElements[i].GetComponent<Fragile>())
            //{
            //    iconTransform = allElements[i].GetComponent<Fragile>().GetIconTransform();
            //    startingPosition = allElements[i].GetComponent<Fragile>().GetCenterPosition();
            //}
            //else if (allElements[i].GetComponent<Gate>())
            //{
            //    iconTransform = allElements[i].GetComponent<Gate>().GetIconTransform();
            //    startingPosition = allElements[i].GetComponent<Gate>().GetCenterPosition();
            //}
            //else if (allElements[i].GetComponent<BrokenDynamicPiece>())
            //{
            //    iconTransform = allElements[i].GetComponent<BrokenDynamicPiece>().GetIconTransform();
            //    startingPosition = allElements[i].GetComponent<BrokenDynamicPiece>().GetCenterPosition();
            //}
            //else if (allElements[i].GetComponent<Phoenix>())
            //{
            //    iconTransform = allElements[i].GetComponent<Phoenix>().GetIconTransform();
            //    startingPosition = allElements[i].GetComponent<Phoenix>().GetCenterPosition();
            //    zOffset = 5.9f;
            //}

            //if (allElements[i].Find("Visual"))
            //{
            //    if (allElements[i].Find("Visual").GetComponent<Ladder>())
            //    {
            //        iconTransform = allElements[i].Find("Visual").GetComponent<Ladder>().GetIconTransform();
            //        startingPosition = allElements[i].Find("Visual").GetComponent<Ladder>().GetCenterPosition();
            //    }
            //}


            if (allElements[i].Find("Collider"))
            {
                if (allElements[i].Find("Collider").GetComponent<Rope>())
                {
                    iconTransform = allElements[i].Find("Collider").GetComponent<Rope>().GetIconTransform();
                    startingPosition = allElements[i].Find("Collider").GetComponent<Rope>().GetCenterPosition();
                }
            }  

            if(iconTransform)
            {
                Vector3 positionToSet = ZoomInOutIconSetter.GetIconPositionFromCamera(cameraPosition, startingPosition, zOffset);

                iconTransform.position = positionToSet;
                iconTransform.GetComponent<Renderer>().material.color = new Color(1, 1, 1, adjustedT);
            }
        }
    }

    public Transform[] GetAllWaterObjects()
    {
        List<Transform> waterObjects = new List<Transform>();

        for (int i = 0; i < allElements.Count; i++)
        {
            if (allElements[i].GetComponent<PushableBox>())
                waterObjects.Add(allElements[i]);
        }

        return waterObjects.ToArray();
    }

    public void AddPhoenixToElementsList()
    {
        if(!allElements.Contains(GameplayManager.instance.Phoenix.transform))
        {
            allElements.Add(GameplayManager.instance.Phoenix.transform);
            allElementsParents.Add(null);
        }
        
    }
}
