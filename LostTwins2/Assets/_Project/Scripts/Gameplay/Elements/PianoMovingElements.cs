using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PianoMovingElements : MonoBehaviour
{
    #region Legos Class
    [System.Serializable]
    public class MovingElementsStruct
    {
        #region Exposed Variables
        [SerializeField]
        private Transform element;

        [SerializeField]
        private float amplitude = 0f;

        [SerializeField]
        private int elementID = 1;
        #endregion

        #region Private Variables
        private Vector3 startPosition = default;
        #endregion

        #region Constructor
        public MovingElementsStruct(int elementID, Transform element, float amplitude)
        {
            this.elementID = elementID;
            this.element = element;
            this.amplitude = amplitude;
            this.startPosition = element.localPosition;
        }

        #endregion

        #region Getters Setters
        public float Amplitude
        {
            get { return amplitude; }
            set { amplitude = value; }
        }

        public Transform Element
        {
            get { return element; }
        }

        public int ElementID
        {
            get { return elementID; }
            set { elementID = value; }
        }

        public Vector3 StartPosition
        {
            get { return startPosition; }
            set { startPosition = value; }
        }
        #endregion
    }
    #endregion

    #region Wave Data Class
    [System.Serializable]
    public class WaveData
    {
        #region Exposed Variables
        [SerializeField]
        private int elementID = 0;

        [SerializeField]
        private float finalOffset = default;

        [SerializeField]
        private float currentOffset = default;

        #endregion

        #region Private Variables
        private bool isReachedTop = false;
        private float currentTime = 0f;

        #endregion

        #region Getters Setters
        public WaveData(float offset, int ID)
        {
            finalOffset = offset;
            elementID = ID;
        }


        public float CurrentTime
        {
            get { return currentTime; }
            set { currentTime = value; }
        }

        public int ElementID
        {
            get { return elementID; }
            set { elementID = value; }
        }


        public bool IsReachedTop
        {
            get { return isReachedTop; }
            set { isReachedTop = value; }
        }

        public float FinalOffset
        {
            get { return finalOffset; }
            set { finalOffset = value; }
        }

        public float CurrentOffset
        {
            get { return currentOffset; }
            set { currentOffset = value; }
        }
        #endregion

    }
    #endregion

    #region Wave Class
    [System.Serializable]
    public class Wave
    {
        
        [SerializeField]
        private List<WaveData> waveDatas = new List<WaveData>();

        public List<WaveData> WaveDatas
        {
            get => waveDatas;
        }


        public Wave(PianoMovingElements pianoObject ,Transform keyPosition)
        {
            pianoObject.SetInverseLerpValuesBasedOnDistance(keyPosition);

            List<float> angles = pianoObject.SetAngleBasedOnT();

            for (int i = 0; i < pianoObject.MovingElementsData.Count; i++)
            {
                float offset = pianoObject.CalculateOffsetBasedOnAngle(angles[i], pianoObject.MovingElementsData[i].Amplitude);
                waveDatas.Add(new WaveData(offset, pianoObject.MovingElementsData[i].ElementID));
            }

        }

        public float GetElementOffset(int elementID)
        {
            WaveData resultantWave = waveDatas.Find(val => val.ElementID == elementID);

            if (resultantWave != null)
                return resultantWave.CurrentOffset;
            return 0f;
        }

    }
    #endregion


    #region Private Variables
    private List<float> tempValues = new List<float>();
    private List<MovingElementsStruct> movingElementsData = new List<MovingElementsStruct>();
    private List<Wave> waves = new List<Wave>();

    #endregion


    #region Exposed Variables
    [SerializeField]
    private int movingLegosCount = 10;

    [SerializeField]
    private Transform legosParent;

    [SerializeField]
    private float waveLength = 5f;
    [SerializeField]
    private float amplitude = 1f;

    [SerializeField]
    private float elementUpSpeed = 5f;

    [SerializeField]
    private float elementDownSpeed = 8f;
    #endregion

    #region Getters
    public List<MovingElementsStruct> MovingElementsData
    { get => movingElementsData; }
    #endregion


    private void Start()
    {
        ElementsInit();
    }

    private void ElementsInit()
    {
        for (int i = 0; i < movingLegosCount; i++)
        {
            Transform visual = legosParent.Find("MovingLego" + (i + 1));
            movingElementsData.Add( new MovingElementsStruct( i + 1, visual ,amplitude ) );

        }
    }

    public float GetAngleInRadians(float angleInDegrees)
    {
        return angleInDegrees * Mathf.Deg2Rad;
    }

    void Update()
    {
        UpdateLegosCurrentOffsets();
        UpdateLegosPosition();
    }

    private void UpdateLegosCurrentOffsets()
    {
        if (waves.Count > 0)
        {
            for (int i = 0; i < waves.Count; i++)
            {
                for (int j = 0; j < waves[i].WaveDatas.Count; j++)
                {
                    waves[i].WaveDatas[j].CurrentTime += Time.deltaTime;
                    if (!waves[i].WaveDatas[j].IsReachedTop)
                    {
                        waves[i].WaveDatas[j].CurrentOffset = Mathf.Lerp(0f, waves[i].WaveDatas[j].FinalOffset, (waves[i].WaveDatas[j].CurrentTime * elementUpSpeed));
                        //waves[i].WaveDatas[j].CurrentOffset = Mathf.Lerp(0f, waves[i].WaveDatas[j].FinalOffset, MyMath.EaseInQuad( waves[i].WaveDatas[j].CurrentTime * elementUpSpeed));

                        if (waves[i].WaveDatas[j].CurrentOffset >= waves[i].WaveDatas[j].FinalOffset)
                        {
                            waves[i].WaveDatas[j].IsReachedTop = true;
                            waves[i].WaveDatas[j].CurrentTime = 0f;
                        }

                    }
                    else
                    {
                        waves[i].WaveDatas[j].CurrentOffset = Mathf.Lerp(waves[i].WaveDatas[j].FinalOffset, 0f, (waves[i].WaveDatas[j].CurrentTime * elementDownSpeed));
                        //waves[i].WaveDatas[j].CurrentOffset = Mathf.Lerp(waves[i].WaveDatas[j].FinalOffset, 0f, MyMath.EaseInOutQuad(waves[i].WaveDatas[j].CurrentTime * elementDownSpeed));

                        if (waves[i].WaveDatas[j].CurrentOffset <= 0f)
                        {
                            waves[i].WaveDatas.Remove(waves[i].WaveDatas[j]);
                        }

                    }
                }

                if (waves[i].WaveDatas.Count <= 0)
                {
                    waves.Remove(waves[i]);
                }

            }

        }

    }

    private void UpdateLegosPosition()
    {
        if (waves.Count > 0)
        {
            for (int i = 0; i < movingElementsData.Count; i++)
            {
                float elementOffset = 0f;
                for (int j = 0; j < waves.Count; j++)
                {
                    elementOffset += waves[j].GetElementOffset(movingElementsData[i].ElementID);
                    elementOffset = Mathf.Clamp(elementOffset, elementOffset, 1f);
                }
                MoveElement(movingElementsData[i].Element, movingElementsData[i].StartPosition, elementOffset);

            }

        }
    }

    private void MoveElement(Transform element , Vector3 startPositon ,float offset)
    {
        Vector3 currentPos = startPositon;
        currentPos.z += offset;
        element.localPosition = currentPos;
    }
    #region Offset Based On Angle

    public void SetInverseLerpValuesBasedOnDistance(Transform keyPosition)
    {
        tempValues.Clear();
        for (int i = 0; i < movingElementsData.Count; i++)
        {
            float distance = Mathf.Abs(movingElementsData[i].Element.position.x - keyPosition.position.x);
            float value = Mathf.InverseLerp(0f, waveLength, distance);
            tempValues.Add(value);
        }
    }

    public List<float> SetAngleBasedOnT()
    {
        List<float> values = new List<float>();

        for (int i = 0; i < tempValues.Count; i++)
        {
            float val = Mathf.Lerp(90f, 0f, tempValues[i]);
            values.Add(val);
        }
        return values;
    }

    public float CalculateOffsetBasedOnAngle(float angle, float amplitude)
    {
        float offsetValue = Mathf.Sin(GetAngleInRadians(angle)) * amplitude;
        return offsetValue;
    }
    #endregion

    private void GenerateWave(Transform keyTransform)
    {
        waves.Add(new Wave(this , keyTransform));
      
    }

    public void SetTrigger(Transform keyPosition)
    {
        GenerateWave(keyPosition);
    }

}
