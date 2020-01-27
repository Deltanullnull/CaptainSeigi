using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    // Start is called before the first frame update

    public GameObject title;
    public GameObject mainButtons;
    public GameObject instructionMenu;

    void Start()
    {
        StartCoroutine(SpawnFirework());
    }

    IEnumerator SpawnFirework()
    {
        while (true)
        {
            float rX = Random.Range(-1f, 1f) * 10f;
            float rY = Random.Range(-1f, 1f) * 10f;
            float rZ = Random.Range(0.1f, 1f) * 10f;

            GameObject fireWork= Instantiate(Resources.Load("Prefabs/Firework") as GameObject, new Vector3(rX, rY, rZ), Quaternion.identity);

            Destroy(fireWork, 1f);

            yield return new WaitForSeconds(2f);
        }
        
    }

    // Update is called once per frame
    public void OpenInstructions(bool open)
    {
        instructionMenu.SetActive(open);

        mainButtons.SetActive(!open);
        title.SetActive(!open);
    }



    public void StartGame()
    {
        SceneManager.LoadScene(1);
    }

    public void ExitGame()
    {
        Application.Quit();
    }
}
