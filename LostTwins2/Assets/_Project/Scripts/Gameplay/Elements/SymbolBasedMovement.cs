using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SymbolBasedMovement : MonoBehaviour
{
    #region Legos Class
    [System.Serializable]
    public class MovingLegos
    {
        #region Exposed Variables
        [SerializeField]
        private float yOffset = 0f;

        [SerializeField]
        private int requiredSymbolKeyID = 0; // this is the id which binds with piano key. If this respective key is pressed with piano, further operations will be performed.

        #endregion

        #region Private Variables
        private Transform element;
        private Vector3 startPosition;
        private Vector3 wantedPosition;
        private bool isAlreadyOnTop = false;
        private int keyOctave;

        #endregion

        #region Constructor

        public MovingLegos(Transform element)
        {
            this.element = element;
        }

        #endregion

        #region Getters Setters
        public Transform Element
        {
            get => element;
            set => element = value;
        }

        public int KeyOctave
        {
            get => keyOctave;
            set => keyOctave = value;
        }

        public int RequiredSymbolKeyID
        {
            get => requiredSymbolKeyID;
            set => requiredSymbolKeyID = value;
        }

        public Vector3 StartPosition
        {
            get => startPosition;
            set => startPosition = value;
        }

        public Vector3 WantedPosition
        {
            get => wantedPosition;
            set => wantedPosition = value;
        }

        public bool IsAlreadyOnTop
        {
            get => isAlreadyOnTop;
            set => isAlreadyOnTop = value;
        }

        #endregion

        #region Functions
        public void AddOffset()
        {
            if (!isAlreadyOnTop)
            {
                wantedPosition.z += yOffset;
                isAlreadyOnTop = true;
            }
        }

        public void SetWantedPositionToDefaultPosition()
        {
            wantedPosition = startPosition;
            isAlreadyOnTop = false;
        }
        #endregion
    }
    #endregion

    #region Exposed Variables

    [SerializeField]
    private Transform searchSymbolsFromObject = default;
    
    [SerializeField]
    private List<MovingLegos> legos = new List<MovingLegos>();

    [SerializeField]
    private float moveSpeed = 1f;

    [SerializeField]
    private int pianoOctave = 1;

    [SerializeField]
    private Color dimColor;

    [SerializeField]
    private Color brightColor;

    #endregion

    #region Private Variables
    private int currentLegoIndex = 0;
    private int nextSequenceValue = 0;
    private Vector3 symbolTargetPosition = default;
    private List<int> audioIDs = new List<int>();
    private bool isMusicSymbolAnimationCompleted = false;

    #endregion

    #region Getters

    public Vector3 SymbolTargetPosition
    {
        get => symbolTargetPosition;
    }


    public Color DimColor
    {
        get => dimColor;
    }

    public Color BrightColor
    {
        get => brightColor;
    }



    #endregion


    // Start is called before the first frame update
    void Start()
    {
        Init();
    }


    public void Init()
    {
        for (int i = 0; i < legos.Count; i++)
        {
            if(i == 0)
            {
                nextSequenceValue = legos[i].RequiredSymbolKeyID;
            }
            Transform visual = searchSymbolsFromObject.Find("SymbolBasedLego" + (i + 1));
            legos[i].Element = visual;
            legos[i].StartPosition = legos[i].Element.localPosition;
            legos[i].WantedPosition = legos[i].StartPosition;
            legos[i].KeyOctave = pianoOctave;
            SpawnSymbolOnElement(legos[i].RequiredSymbolKeyID, legos[i].Element, DimColor);
            audioIDs.Add(legos[i].RequiredSymbolKeyID);
        }
    }

    // Update is called once per frame
    void Update()
    {
        for (int i = 0; i < legos.Count; i++)
        {
            legos[i].Element.localPosition = Vector3.MoveTowards(legos[i].Element.localPosition, legos[i].WantedPosition, Time.deltaTime * moveSpeed);

        }

    }

    private void SpawnSymbolOnElement(int musicSymbolID, Transform legoElement, Color targetColor)
    {
        musicSymbolID -= 1;
        GameObject symbol = new GameObject();
        symbol.transform.position = legoElement.position;
        symbol.AddComponent<MeshFilter>();
        symbol.AddComponent<MeshRenderer>();
        symbol.GetComponent<MeshFilter>().mesh = GameData.instance.MusicNotes[musicSymbolID].sharedMesh;
        symbol.GetComponent<MeshRenderer>().material = GameData.instance.MusicSymbol;
        symbol.GetComponent<MeshRenderer>().material.SetColor("_Color", targetColor);
        symbol.transform.localScale = new Vector3(0.05f, 0.05f, 0.05f);
        symbol.transform.rotation = Quaternion.Euler(0f, 180f, 0f);
        symbol.transform.SetParent(legoElement);
        symbol.transform.localPosition = new Vector3(-0.255f, 0.45f, 0.2f);
    }

   

    public void CheckIfAllLegosOnTop()
    {
        if(currentLegoIndex >= legos.Count)
        {
            StopCoroutine("EndSequenceCoroutine");
            StartCoroutine("EndSequenceCoroutine");

            for (int i = 0; i < legos.Count; i++)
            {
                legos[i].Element.GetChild(0).transform.GetComponent<MeshRenderer>().material.SetColor("_Color", DimColor);
            }

        }

    }

    public bool IsKeyMatched(int keyId)
    {
        for (int i = 0; i < legos.Count; i++)
        {
            if(legos[i].RequiredSymbolKeyID == keyId)
            {
                return true;
            }

        }
        return false;
    }


    public void SetTrigger(int keyID, Transform keyPosition)
    {
        MovingLegos targetLego = legos.Find(lego => lego.RequiredSymbolKeyID == keyID);
        if(targetLego != null && !targetLego.IsAlreadyOnTop && targetLego.KeyOctave == pianoOctave && nextSequenceValue == targetLego.RequiredSymbolKeyID)
        {
            isMusicSymbolAnimationCompleted = false;
            currentLegoIndex++;
            CheckIfAllLegosOnTop();
            if (currentLegoIndex < legos.Count)
                nextSequenceValue = legos[currentLegoIndex].RequiredSymbolKeyID;
            symbolTargetPosition = targetLego.Element.transform.position;
           
            StartCoroutine(HandleLego(targetLego));
        }
        else
        {
            symbolTargetPosition = new Vector3(keyPosition.position.x, keyPosition.position.y + 6f, keyPosition.position.z);
            if (currentLegoIndex >= legos.Count)
            {
                return;
            }
           
            currentLegoIndex = 0;
            for (int i = 0; i < legos.Count; i++)
            {
                if (i == 0)
                {
                    nextSequenceValue = legos[i].RequiredSymbolKeyID;
                }

                if (legos[i].IsAlreadyOnTop)
                {
                    legos[i].SetWantedPositionToDefaultPosition();
                    if (FXManager.instance)
                        FXManager.instance.AssignColor(legos[i].Element.transform.GetChild(0).transform, 0.5f, BrightColor, DimColor);
                }

                legos[i].Element.GetChild(0).transform.GetComponent<MeshRenderer>().material.SetColor("_Color", DimColor);

            }
        }

    }

    private IEnumerator EndSequenceCoroutine()
    {
        yield return new WaitForSeconds(1f);
        int currentIndex = 0;

        while (currentIndex < audioIDs.Count)
        {
            AudioManager.instance.PlaySoundEffect("Piano", audioIDs[currentIndex]);

            if (FXManager.instance)
                FXManager.instance.AssignColor(legos[currentIndex].Element.transform.GetChild(0).transform, 2f, DimColor, BrightColor);

            currentIndex++;
            yield return new WaitForSeconds(0.2f);
        }

        yield return null;
    }

    private IEnumerator HandleLego(MovingLegos lego)
    {
        yield return new WaitUntil( ()=> isMusicSymbolAnimationCompleted );

        lego.AddOffset();
        if (FXManager.instance)
            FXManager.instance.AssignColor(lego.Element.transform.GetChild(0).transform, 0.5f, DimColor, BrightColor);

    }

    private void SetBoolTrueAfterMusicSymbolSpawned()
    {
        isMusicSymbolAnimationCompleted = true;
    }

    public void HandleMusicSymbolCallBack(int id)
    {
        SetBoolTrueAfterMusicSymbolSpawned();
    }



}
