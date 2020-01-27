using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Highscore : MonoBehaviour
{
    // Start is called before the first frame update

    public GameObject entryPrefab;
    public InputField playerNameInput;
    public GameObject inputFieldPanel;
    public Text scoreText;

    public GameObject buttonExit;

    private List<KeyValuePair<string, int>> highscoreList;

    void Start()
    {
        // TODO get player prefs

        highscoreList = new List<KeyValuePair<string, int>>();


        string highscorePrefs = PlayerPrefs.GetString("Highscore");

        string[] entries = highscorePrefs.Split(new char[] { '\n' });

        for (int i = 0; i < entries.Length; i++)
        {

            var entry = entries[i];

            string[] tokens = entry.Split(new char[] { ';' });

            if (i < entries.Length - 1)
            {
                if (tokens.Length != 2)
                    continue;

                highscoreList.Add(new KeyValuePair<string, int>(tokens[0], int.Parse(tokens[1])));
            }
            else
            {
                highscoreList.Add(new KeyValuePair<string, int>("", int.Parse(tokens[1])));

                scoreText.text = string.Format("Your score is\n\n{0}\n\nEnter your name: ", int.Parse(tokens[1]));
            }
            
        }
    }

    public void BackToMainMenu()
    {
        SceneManager.LoadScene(0);
    }

    public void ShowScore()
    {
        inputFieldPanel.SetActive(false);

        var entry = highscoreList[highscoreList.Count - 1];

        entry = new KeyValuePair<string, int>(playerNameInput.text, entry.Value);

        highscoreList[highscoreList.Count - 1] = entry;

        var highscoreCollectible = highscoreList.OrderByDescending(o => o.Value);

        int idx = 0;

        string highscorePrefs = "";

        foreach (var pair in highscoreCollectible)
        {
            if (idx < 6)
            {
                GameObject entryObject = Instantiate(entryPrefab, this.transform);

                GameObject nameLabel = entryObject.transform.GetChild(0).gameObject;
                nameLabel.GetComponent<Text>().text = pair.Key;

                GameObject scoreLabel = entryObject.transform.GetChild(1).gameObject;
                scoreLabel.GetComponent<Text>().text = pair.Value.ToString();
            }

            highscorePrefs += pair.Key + ";" + pair.Value.ToString() + "\n";

            idx++;


        }

        PlayerPrefs.SetString("Highscore", highscorePrefs);

        PlayerPrefs.Save();

        buttonExit.SetActive(true);
    }
}
