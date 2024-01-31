using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class RankPanel : MonoBehaviour
{
    RankLine[] rankLines;

    int[] highScores;

    string[] rankerNames;

    const int rankCount = 5;

    TMP_InputField inputField;

    int updatedIndex = -1;

    private void Awake()
    {
        rankLines = GetComponentsInChildren<RankLine>(true);
        highScores = new int[rankCount];
        rankerNames = new string[rankCount];

        inputField = GetComponentInChildren<TMP_InputField>(true);
        inputField.onEndEdit.AddListener(OnNameInputEnd);
    }

    private void Start()
    {
        Player player = GameManager.Instance.Player;
        player.onDie += UpdateRankData;

        LoadRankData();
    }

    void SetDefaultData()
    {
        for(int i = 0; i < rankCount; i++)
        {
            char temp = 'A';
            temp = (char)((byte)temp + (byte)i);
            rankerNames[i] = $"{temp}{temp}{temp}";   

            int score = 10;
            for(int j=rankCount-i; j>0; j--)
            {
                score *= 10;
            }
            highScores[i] = score;
        }

        RefreshRankLines();
    }

    void SaveRankData()
    {
        SaveData data = new SaveData();  
        data.rankerNames = rankerNames;  
        data.highScores = highScores;
        string jsonText = JsonUtility.ToJson(data);     

        string path = $"{Application.dataPath}/Save/";
        if( !System.IO.Directory.Exists(path))          
        {
            System.IO.Directory.CreateDirectory(path);  
        }

        string fullPath = $"{path}Save.json";           
        System.IO.File.WriteAllText(fullPath, jsonText);
    }

    bool LoadRankData()
    {
        bool result = false;

        string path = $"{Application.dataPath}/Save/";
        if (System.IO.Directory.Exists(path))           
        {
            string fullPath = $"{path}Save.json";       
            if (System.IO.File.Exists(fullPath))
            {
                string json = System.IO.File.ReadAllText(fullPath);

                SaveData loadedData = JsonUtility.FromJson<SaveData>(json);

                rankerNames = loadedData.rankerNames;
                highScores = loadedData.highScores;

                result = true ;
            }
        }

        if(!result)
        {            
            if (!System.IO.Directory.Exists(path))          
            {
                System.IO.Directory.CreateDirectory(path);  
            }
            SetDefaultData();
        }

        RefreshRankLines(); 

        return result;
    }

    void UpdateRankData(int score)
    {
        for(int i=0;i<rankCount;i++)
        {
            if (highScores[i] < score)
            {
                for(int j = rankCount-1; j > i; j--)
                {
                    highScores[j] = highScores[j - 1];
                    rankerNames[j] = rankerNames[j - 1];
                    rankLines[j].SetData(rankerNames[j], highScores[j]);
                }
                highScores[i] = score;                    
                rankLines[i].SetData("새 랭커", score);   
                updatedIndex = i;                         

                Vector3 newPos = inputField.transform.position;
                newPos.y = rankLines[i].transform.position.y;
                inputField.transform.position = newPos;
                inputField.gameObject.SetActive(true);         

                break;
            }
        }
    }

    void RefreshRankLines()
    {
        for(int i=0;i < rankCount; i++)
        {
            rankLines[i].SetData(rankerNames[i], highScores[i]);
        }
    }

    private void OnNameInputEnd(string text)
    {
        inputField.gameObject.SetActive(false);
        rankerNames[updatedIndex] = text;      
        RefreshRankLines();                    
        SaveRankData();                        
    }
#if UNITY_EDITOR
    public void Test_DefaultRankPanel()
    {
        SetDefaultData();
        RefreshRankLines();
    }

    public void Test_SaveRankPanel()
    {
        SaveRankData();
    }

    public void Test_LoadRankPanel()
    {
        LoadRankData();
    }

    public void Test_UpdateRankPanel(int score)
    {
        UpdateRankData(score);
    }
#endif
}