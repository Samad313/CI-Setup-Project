using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Text.RegularExpressions;
using TMPro;
public static class Localization
{
    private static Dictionary<string, string> allStrings;
    private static bool initCalled = false;

    public static bool GetInitCalled()
    {
        return initCalled;
    }

    public static void Init()
    {
        allStrings = new Dictionary<string, string>();
        TextAsset theFile = Resources.Load<TextAsset>("CSVs/AllStrings-EN");
        string[,] fetchedRawData = CSVReader.SplitCsvGrid(theFile.text);

        for (int i = 0; i < fetchedRawData.GetLength(1) - 1; i++)
        {
            if (fetchedRawData[0, i] != "")
                allStrings.Add(fetchedRawData[0, i], fetchedRawData[1, i]);
        }

        initCalled = true;
    }

    public static string GetText(string key)
    {
        string value = "";
        if (!allStrings.TryGetValue(key, out value))
        {
            return "";
        }
        if (value.Contains("##"))
        {
            value = value.Replace("##", "\n");
        }

        return value;
    }

    public static string RemoveBetween(string s, char begin, char end)
    {
        Regex regex = new Regex(string.Format("\\{0}.*?\\{1}", begin, end));
        return regex.Replace(s, string.Empty);
    }


    public static void SetMysteryBoxLocalizationValues(Transform canvas)
    {
        canvas.Find("Bg/BackGround/MysteryBoxContainer/Rare/text").GetComponent<TextMeshProUGUI>().text = Localization.GetText("ShopMystery_MysteryBoxRareText");
    }

    public static string GetText(string key, string[] arguments)
    {
        string value = "";
        if (!allStrings.TryGetValue(key, out value))
        {
            return "";
        }
        if (value.Contains("##"))
        {
           value = value.Replace("##", "\n");
        }

        Regex regex = new Regex(@"\[.*?\]");
        MatchCollection matches;
        matches = regex.Matches(value);

        for (int i = 0; i < matches.Count; i++)
        {
            //Debug.Log(matches[i].Value + "   "+i + "   " +arguments[i]);

            //if(value.Contains(matches[i].Value))
            //{
            //    Debug.Log("Inside Check::"+matches[i].Value); 
            //}
            value = value.Replace(matches[i].Value, arguments[i]);

        }

        return value;
    }
}
