using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEditor;


[System.Serializable]
public class SurveyData
{

    public string question;
    public int score;
    public int userID;

    public SurveyData(string inQuestion, int inScore, int inUserID)
    {

        question = inQuestion;
        score = inScore;
        userID = inUserID;
    }

}

[System.Serializable]
public class SurveyDataWithTwoQ
{

    public string question, question2;
    public int score, score2;
    public int userID;

    public SurveyDataWithTwoQ(string inQuestion, int inScore, string inQuestion2, int inScore2, int inUserID)
    {
        question = inQuestion;
        score = inScore;
        question2 = inQuestion2;
        score2 = inScore2;
        userID = inUserID;
    }

}


public class Serialization : MonoBehaviour
{
    

    // Start is called before the first frame update

    public void SaveFile(SurveyData surveyData, int fileID, string sceneName)
    {
        string destination = Application.dataPath + "/SurveyData_1/" + sceneName + "/ID_" + fileID + ".dat";


        Debug.Log(destination);

        FileStream file;

        if (File.Exists(destination)) 
            file = File.OpenWrite(destination);
        else 
            file = File.Create(destination);

        
        BinaryFormatter bf = new BinaryFormatter();
        bf.Serialize(file, surveyData);
        file.Close();
    }

    public void SaveFile(SurveyDataWithTwoQ surveyData, int fileID, string sceneName)
    {
        string destination = Application.dataPath +  "/SurveyData_2/" + sceneName + "/ID_" + fileID + ".dat";


        Debug.Log(destination);

        FileStream file;

        if (File.Exists(destination))
            file = File.OpenWrite(destination);
        else
            file = File.Create(destination);


        BinaryFormatter bf = new BinaryFormatter();
        bf.Serialize(file, surveyData);
        file.Close();
    }

    public void LoadFile(SurveyData surveyData)
    {
        string destination = Application.dataPath + "/SurveyData_1/ID_0.dat";
        FileStream file;

        if (File.Exists(destination))
        {
            file = File.OpenRead(destination);
        }
        else
        {
            Debug.LogError("File not found");
            return;
        }

        BinaryFormatter bf = new BinaryFormatter();
        SurveyData temp = (SurveyData)bf.Deserialize(file);
        surveyData.question = temp.question;
        surveyData.score = temp.score;
        surveyData.userID = temp.userID;
        file.Close();
        
    }

    public void LoadFile(SurveyDataWithTwoQ surveyData)
    {
        string destination = Application.dataPath + "/SurveyData_2/save.dat";
        FileStream file;

        if (File.Exists(destination))
        {
            file = File.OpenRead(destination);
        }
        else
        {
            Debug.LogError("File not found");
            return;
        }

        BinaryFormatter bf = new BinaryFormatter();
        surveyData = (SurveyDataWithTwoQ)bf.Deserialize(file);
        file.Close();

    }

}

