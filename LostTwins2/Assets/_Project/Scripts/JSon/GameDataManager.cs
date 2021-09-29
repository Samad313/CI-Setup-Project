using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Events;
using System.Linq;

public class GameDataManager : MonoBehaviour
{
    public static GameDataManager instance;

    [SerializeField] private string csvPath = "";

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            Initialize();
        }
    }

    private void Initialize()
    {
    }

    public string[] CSVInitializer(int fileNumber)
    {
        string concatPath = "";
        concatPath = csvPath + fileNumber.ToString();
        string[,] rawData = CSVFileReader.GetFileData(concatPath);
        return GetScenes(rawData);
    }

    public string[] GetScenes(string[,] rawData)
    {
        string[] levelScenes = new string[rawData.GetLength(1) - 2];
        for (int i = 0; i < 1; i++)
        {
            for (int j = 1; j < rawData.GetLength(1) - 1; j++)
            {
                levelScenes[j - 1] = rawData[i, j];
                //levels.Add(rawData[i, j]);
                //Debug.Log("<color=yellow> Levels In CSV: </color>" + rawData[i, j]);
            }
        }
        return levelScenes;
    }

    public void MarkFTUEAsComplete()
    {
        JsonReaderWriter.UpdateFtueCompleteStatus(true);
        Debug.Log("<color=cyan> FTUE is Marked As Completed </color>");
    }
}


