using UnityEngine;
using System.Collections;

public class BackToIndexScript : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    public void BackToIndex()
    {
        Application.LoadLevel(0);
    }
}
