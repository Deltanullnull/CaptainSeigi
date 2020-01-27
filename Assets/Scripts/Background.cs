using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Background : MonoBehaviour
{
    
    float time = 0f;

    float brightness = 1f;

    bool darkening = true;

    // Update is called once per frame
    void Update()
    {
        time += Time.deltaTime;

        if (time > 0.5)
        {
            time = 0;

            if (darkening)
                brightness -= 0.2f;
            else
                brightness += 0.2f;

            if (brightness <= 0.6f && darkening)
                darkening = false;
            else if (brightness >= 1.0f && !darkening)
                darkening = true;

            this.GetComponent<Renderer>().material.SetFloat("_Brightness", brightness);
        }
    }
}
