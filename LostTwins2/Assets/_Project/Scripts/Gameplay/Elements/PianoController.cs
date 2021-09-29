using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PianoController : MonoBehaviour
{
    [System.Serializable]
    public class KeysData
    {
        [SerializeField]
        private Transform keyVisual;

        [SerializeField]
        private Collider collider;

        [SerializeField]
        private int keyID = 0;

        private Vector3 colliderDefaultPosition = default;

        private bool isKeyPressed = false;


        public KeysData(int keyID, Transform keyVisual, Collider collider)
        {
            this.keyID = keyID;
            this.keyVisual = keyVisual;
            this.collider = collider;
        }

        public Transform KeyVisual
        {
            get { return keyVisual; }
        }

        public Collider KeyCollider
        {
            get { return collider; }
        }

        public bool IsKeyPressed
        {
            get { return isKeyPressed; }
            set { isKeyPressed = value; }
        }

        public Vector3 ColliderDefaultPosition
        {
            get { return colliderDefaultPosition; }
            set { colliderDefaultPosition = value; }
        }

        public int KeyID
        {
            get { return keyID; }
            set { keyID = value; }
        }

    }
    #region Exposed Variables

    [SerializeField]
    private int keysCount = 17;

    [SerializeField]
    private Transform player;

    [SerializeField]
    private float pianoKeyPressedSpeed = 5f;

    [SerializeField]
    private GameObject[] myTriggers = default;

   
    [SerializeField]
    private bool isPianoEnabled = false;
    [SerializeField]
    private int numButtonsRequiredToEnablePiano = 1;
    #endregion

    #region Private Variables
    private List<KeysData> keysData = new List<KeysData>();
    private float pressedTimeDuration = 0.3f;
    private float unPressedTimeDuration = 1.5f;
    private Quaternion currentKeyRotation = default;
    private Vector3 keyPressedRotation = new Vector3(0f, 3.5f, 0f);
    private Vector3 symbolTargetPosition = default;
    private float keyLength = 5f; //3.74
    private float keyHeight = 0f;
    private Transform[] playerTransforms;
    private List<Transform> myButtons;

    #endregion

    #region Getters

    public Transform Player
    {
        get => player;
    }

    public List<KeysData> GetPianoKeys
    {
        get { return keysData; }
    }

    #endregion


    // Start is called before the first frame update
    void Start()
    {
        PianoInit();
    }

    private void PianoInit()
    {
        KeysInit();
        SaveColliderDefaultPositions();
        if (GameplayManager.instance)
            playerTransforms = GameplayManager.instance.PlayerTransforms;

        myButtons = new List<Transform>();
        if (numButtonsRequiredToEnablePiano <= 0)
        {
            isPianoEnabled = true;
        }

    }

    private void KeysInit()
    {
        for (int i = 0; i < keysCount; i++)
        {
            Transform visual = transform.Find("Key" + (i + 1));
            Collider collider = transform.Find("Colliders").Find("PianoKeyCollider" + (i + 1)).GetComponent<Collider>();
            keysData.Add(new KeysData( i + 1 , visual, collider));
        }
    }

    public void SetTrigger(bool value, Transform inputButton)
    {
        if (value)
        {
            if (myButtons.Contains(inputButton) == false)
            {
                myButtons.Add(inputButton);
            }
            SetPianoState();
        }
        else
        {
            if (myButtons.Contains(inputButton))
            {
                myButtons.Remove(inputButton);
            }
            SetPianoState();

        }
    }

    private void SetPianoState()
    {
        float currentPianoStateValue = (myButtons.Count * 1f / numButtonsRequiredToEnablePiano * 1f);

        if(currentPianoStateValue > 0.95f)
        {
            isPianoEnabled = true;
        }
        else
        {
            isPianoEnabled = false;
        }

    }

    // Update is called once per frame
    void Update()
    {
        for (int i = 0; i < keysData.Count; i++)
        {
            if(IsPlayerCollidingKey(keysData[i].KeyCollider.transform))
            {
                keysData[i].KeyVisual.localRotation = Quaternion.Lerp(keysData[i].KeyVisual.localRotation, Quaternion.Euler(keyPressedRotation), Time.deltaTime * pianoKeyPressedSpeed);

                if (keysData[i].IsKeyPressed)
                    continue;

                if (!keysData[i].IsKeyPressed)
                {
                    keysData[i].IsKeyPressed = true;
                    ActivateTriggers(keysData[i]);
                    StartCoroutine(PressedStateOfCollider(keysData[i].KeyCollider.transform));
                }

            }
            else
            {
                keysData[i].KeyVisual.localRotation = Quaternion.Lerp(keysData[i].KeyVisual.localRotation, Quaternion.identity, Time.deltaTime * 5f);

                if (!keysData[i].IsKeyPressed)
                    continue;

                if (keysData[i].IsKeyPressed)
                {
                    keysData[i].IsKeyPressed = false;
                    StartCoroutine(UnPressedStateOfCollider(keysData[i].KeyCollider.transform, keysData[i].ColliderDefaultPosition));
                }

            }
        }
    }

    private void ActivateTriggers(KeysData keyData)
    {
        if(isPianoEnabled)
        {
            System.Action<int> MusicSymbolCallBack = null;
            symbolTargetPosition = new Vector3(keyData.KeyCollider.transform.position.x, keyData.KeyCollider.transform.position.y + 6f, keyData.KeyCollider.transform.position.z);

            for (int i = 0; i < myTriggers.Length; i++)
            {
                if (myTriggers[i])
                {
                    if (myTriggers[i].GetComponent<PianoMovingElements>())
                    {
                        myTriggers[i].GetComponent<PianoMovingElements>().SetTrigger(keyData.KeyCollider.transform);
                    }

                    if(myTriggers[i].GetComponent<SymbolBasedMovement>())
                    {
                        myTriggers[i].GetComponent<SymbolBasedMovement>().SetTrigger(keyData.KeyID, keyData.KeyCollider.transform);
                        if(myTriggers[i].GetComponent<SymbolBasedMovement>().IsKeyMatched(keyData.KeyID))
                        {
                            symbolTargetPosition = myTriggers[i].GetComponent<SymbolBasedMovement>().SymbolTargetPosition;
                            MusicSymbolCallBack = myTriggers[i].GetComponent<SymbolBasedMovement>().HandleMusicSymbolCallBack;
                        }
                    }
                }

            }
            FXManager.instance.SpawnMusicSymbol(keyData.KeyID, keyData.KeyCollider.transform, symbolTargetPosition, MusicSymbolCallBack);
            AudioManager.instance.PlaySoundEffect("Piano", keyData.KeyID);
        }
        
    }

    private void SaveColliderDefaultPositions()
    {
        for (int i = 0; i < keysData.Count; i++)
        {
            keysData[i].ColliderDefaultPosition = keysData[i].KeyCollider.transform.localPosition;
        }
    }

    public bool IsPlayerCollidingKey(Transform key)
    {
        float keyXScale = key.localScale.x;
        float keyXPosition = key.position.x;

        if(GameplayManager.instance != null)
        {
            for (int i = 0; i < playerTransforms.Length; i++)
            {
                float yDifference = Mathf.Abs(playerTransforms[i].position.y - key.position.y);

                if (Mathf.Abs((keyXPosition - playerTransforms[i].position.x)) < (keyXScale * 0.4f) && (yDifference < 0.5f))
                {
                    return true;
                }
            }
        }
        else
        {
            float yDifference = Mathf.Abs(Player.position.y - key.position.y);

            if (Mathf.Abs((keyXPosition - Player.position.x)) < (keyXScale * 0.4f) && (yDifference < 0.5f))
            {
                return true;
            }
        }
        return false;
  
    }


    public float GetAngleInRadians(float angleInDegrees)
    {
        return angleInDegrees * Mathf.Deg2Rad;
    }

    public float FindHeightOfKey(Transform pressedKey)
    {
        float angleOfDeviation = pressedKey.localRotation.y;
        float height = Mathf.Tan(GetAngleInRadians(angleOfDeviation)) * keyLength; //trignometric formula tan = P / B
        return height;
    }

    public float FindHeightOfKey()
    {
        float angleOfDeviation = keyPressedRotation.y;
        float height = Mathf.Tan(GetAngleInRadians(angleOfDeviation)) * keyLength; //trignometric formula tan = P / B
        return height;
    }

    private IEnumerator PressedStateOfCollider(Transform keyCollider)
    {
        float timeElapsed = 0f;
        Vector3 colliderStartPosition = keyCollider.transform.localPosition;
        Vector3 colliderCurrentPosition = colliderStartPosition;
        Vector3 colliderFinalPosition = colliderStartPosition;

        keyHeight = FindHeightOfKey();
        colliderFinalPosition.z -= keyHeight;

        while (timeElapsed < pressedTimeDuration)
        {

            colliderCurrentPosition = Vector3.Lerp(colliderStartPosition, colliderFinalPosition, timeElapsed / pressedTimeDuration);
            keyCollider.localPosition = colliderCurrentPosition;
            timeElapsed += Time.deltaTime;
            yield return new WaitForEndOfFrame();
        }
    }

    private IEnumerator UnPressedStateOfCollider(Transform keyCollider, Vector3 colliderTargetPosition)
    {
        float timeElapsed = 0f;
        Vector3 colliderStartPosition = keyCollider.transform.localPosition;
        Vector3 colliderCurrentPosition = colliderStartPosition;

        while (timeElapsed < pressedTimeDuration)
        {
            colliderCurrentPosition = Vector3.Lerp(colliderStartPosition, colliderTargetPosition, timeElapsed / pressedTimeDuration);
            keyCollider.localPosition = colliderCurrentPosition;
            timeElapsed += Time.deltaTime;
            yield return new WaitForEndOfFrame();
        }
    }


}
