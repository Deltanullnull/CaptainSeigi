using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ScoreSystem : MonoBehaviour
{
    public int Score {
        get {
            return score;
        }
    }

    int score = 0;

    public void UpdateScore(int score)
    {
        this.score += score;

        this.GetComponent<Text>().text = "Score: " + this.score.ToString();

        foreach (Transform t in this.transform)
        {
            t.GetComponent<Text>().text = "Score: " + this.score.ToString();
        }
    }
}
