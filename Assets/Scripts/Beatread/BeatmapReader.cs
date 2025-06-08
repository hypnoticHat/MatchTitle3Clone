using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public static class BeatmapReader
{
    public static List<(float time, int[] lanes)> ReadBeatmapWithTime(string filename)
    {
        List<(float, int[])> result = new List<(float, int[])>();
        TextAsset textAsset = Resources.Load<TextAsset>(filename);

        if (textAsset == null)
        {
            Debug.LogError("Beatmap file not found in Resources: " + filename);
            return result;
        }

        string[] lines = textAsset.text.Split('\n');

        foreach (string line in lines)
        {
            if (string.IsNullOrWhiteSpace(line)) continue;

            string[] tokens = line.Trim().Split(',');

            if (tokens.Length < 2) continue;

            if (!float.TryParse(tokens[0], out float time)) continue;

            int[] row = new int[tokens.Length - 1];
            for (int i = 1; i < tokens.Length; i++)
            {
                int.TryParse(tokens[i], out row[i - 1]);
            }

            result.Add((time, row));
        }

        return result;
    }


    public static List<BeatTileInfo> ConvertToBeatTileInfo(List<(float time, int[] row)> mapData)
    {
        List<BeatTileInfo> result = new List<BeatTileInfo>();
        int numRows = mapData.Count;
        int numLanes = mapData[0].row.Length;

        for (int row = 0; row < numRows; row++)
        {
            for (int lane = 0; lane < numLanes; lane++)
            {
                int val = mapData[row].row[lane];

                if (val == 1)
                {
                    result.Add(new BeatTileInfo
                    {
                        time = mapData[row].time,
                        lane = lane,
                        isLong = false,
                        lengthY = 1f
                    });
                }
                else if (val == 2)
                {
                    bool isStart = row == 0 || mapData[row - 1].row[lane] != 2;

                    if (isStart)
                    {
                        int length = 1;
                        while (row + length < numRows && mapData[row + length].row[lane] == 2)
                        {
                            length++;
                        }

                        result.Add(new BeatTileInfo
                        {
                            time = mapData[row].time,
                            lane = lane,
                            isLong = true,
                            lengthY = 0.5f * length
                        });
                    }
                }
            }
        }

        return result.OrderBy(t => t.time).ToList();
    }


}
