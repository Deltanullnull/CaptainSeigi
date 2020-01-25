using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Blood : MonoBehaviour {

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		if (!this.GetComponent<ParticleSystem>().IsAlive())
        {
            Destroy(this.gameObject);
        }
	}
}
