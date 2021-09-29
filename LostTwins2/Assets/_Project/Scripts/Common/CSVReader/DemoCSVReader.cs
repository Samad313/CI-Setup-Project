using UnityEngine;
using System.Collections;
using System.Collections.Generic;



public class DemoCSVReader : MonoBehaviour {

    public TextAsset csv; 

    void Start () {
        CSVReader.DebugOutputGrid( CSVReader.SplitCsvGrid(csv.text) ); 

        string[,] testString;
        testString = CSVReader.SplitCsvGrid(csv.text);

        //column vs row
        Debug.Log(testString[1,0]);
    }
}