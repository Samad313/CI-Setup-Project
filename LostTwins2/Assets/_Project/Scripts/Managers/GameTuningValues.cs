using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameTuningValues : MonoBehaviour
{
    public static GameTuningValues instance;
    private List<string> testCoolNames;
    private static Dictionary<string, string> testTuningData;

    private int faggyIQLevel;
    private string whatIsSamad;

    // Start is called before the first frame update
    void Start()
    {
        instance = this;

        TextAsset singleColumnFile = Resources.Load<TextAsset>("CSVs/TestCoolNames");
        string[,] fetchedSingleColumnRawData = CSVReader.SplitCsvGrid(singleColumnFile.text);
        testCoolNames = new List<string>();
        for (int i = 0; i < fetchedSingleColumnRawData.GetLength(1) - 1; i++)
        {
            if (fetchedSingleColumnRawData[0, i] != "")
                testCoolNames.Add(fetchedSingleColumnRawData[0, i]);
        }

        testTuningData = new Dictionary<string, string>();
        TextAsset doubleColumnFile = Resources.Load<TextAsset>("CSVs/TestTuningData");
        string[,] fetchedDoubleColumnRawData = CSVReader.SplitCsvGrid(doubleColumnFile.text);

        for (int i = 0; i < fetchedDoubleColumnRawData.GetLength(1) - 1; i++)
        {
            if (fetchedDoubleColumnRawData[0, i] != "")
                testTuningData.Add(fetchedDoubleColumnRawData[0, i], fetchedDoubleColumnRawData[1, i]);
        }

        faggyIQLevel = int.Parse(GetStringFromData("FaggyIQLevel"));
        whatIsSamad = GetStringFromData("WhatIsSamad");
    }

    public static string GetStringFromData(string key)
    {
        string value = "";
        if (!testTuningData.TryGetValue(key, out value))
        {
            return "";
        }

        return value;
    }

    public int GetFaggyIQLevel()
    {
        return faggyIQLevel;
    }

    public string GetWhatIsSamad()
    {
        return whatIsSamad;
    }

    public List<string> GetTestCoolNames()
    {
        return testCoolNames;
    }

}
