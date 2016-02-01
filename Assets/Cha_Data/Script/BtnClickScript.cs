using UnityEngine;
using System.Collections;

public class BtnClickScript : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    public void ClickRi()
    {
        StartCoroutine(LoadScene(1));
    }

    public void ClickTu()
    {
        StartCoroutine(LoadScene(2));
    }

    public void ClickAl()
    {
        StartCoroutine(LoadScene(3));
    }

    public void ClickRitual()
    {
        StartCoroutine(LoadScene(4));
    }

    IEnumerator LoadScene(int sceneIndex)
    {
       AsyncOperation async = Application.LoadLevelAsync(sceneIndex);

        yield return async;
    }
}
