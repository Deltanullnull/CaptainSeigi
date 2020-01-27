using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyTrigger : MonoBehaviour {

	// Use this for initialization
	void Start () {
		
	}

    void OnTriggerEnter(Collider collider)
    {
        if (collider.gameObject.layer == LayerMask.NameToLayer("Player"))
        {
            //Debug.Log("Close to player");

            this.transform.parent.GetComponent<EnemyMovement>().closeToPlayer = true;
        }
    }

    void OnTriggerExit(Collider collider)
    {
        if (collider.gameObject.layer == LayerMask.NameToLayer("Player"))
        {
            //Debug.Log("Not Close to player");

            this.transform.parent.GetComponent<EnemyMovement>().closeToPlayer = false;
        }
    }

}
