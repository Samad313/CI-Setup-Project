using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public static class CSVFileReader
{

    public static string[,] GetFileData(string filePath)
    {
        Debug.Log("FilePath: " + filePath);
        TextAsset theFile = Resources.Load<TextAsset>(filePath);
        string[,] grid = SplitCsvGrid(theFile.text);
        return grid;
    }


    // splits a CSV file into a 2D string array
    static public string[,] SplitCsvGrid(string csvText)
    {
        string[] lines = csvText.Split("\n"[0]);

        // finds the max width of row
        int width = 0;
        for (int i = 0; i < lines.Length; i++)
        {
            string[] row = SplitCsvLine(lines[i]);
            width = Mathf.Max(width, row.Length);
        }

        // creates new 2D string grid to output to
        string[,] outputGrid = new string[width + 1, lines.Length + 1];
        for (int y = 0; y < lines.Length; y++)
        {
            string[] row = SplitCsvLine(lines[y]);
            for (int x = 0; x < row.Length; x++)
            {
                outputGrid[x, y] = row[x];
                //Debug.Log("<color=green> OutPut Grid Display: </color>" + outputGrid[x, y]);
                //Debug.LogFormat("<color=green> X: {0} , Y:{1} , OutPut: {2}", x, y, outputGrid[x, y]);

                // This line was to replace "" with " in my output. 
                // Include or edit it as you wish.
                outputGrid[x, y] = outputGrid[x, y].Replace("\"\"", "\"");
            }
        }

        return outputGrid;
    }

    // splits a CSV row 
    static public string[] SplitCsvLine(string line)
    {
        return (from System.Text.RegularExpressions.Match m in System.Text.RegularExpressions.Regex.Matches(line,
            @"(((?<x>(?=[,\r\n]+))|""(?<x>([^""]|"""")+)""|(?<x>[^,\r\n]+)),?)",
            System.Text.RegularExpressions.RegexOptions.ExplicitCapture)
                select m.Groups[1].Value).ToArray();
    }
}
