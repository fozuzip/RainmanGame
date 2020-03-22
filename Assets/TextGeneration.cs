using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.IO;
using System.Text;
using System;
using UnityEngine.SceneManagement;

public class TextGeneration : MonoBehaviour {

	public Text textbox;
	public Button generateButton;
	public string nextScene = "scen2";

	void Start(){
		generateButton.onClick.AddListener (NextLevel);
	}

	public void OnLevelWasLoaded(int level)
	{
		textbox.text = ApplicationModel.Instance.poem;
	}

	void NextLevel(){
		float fadeTime = GameObject.Find ("Fade").GetComponent<Fading> ().BeginFade (1);
		fadeTime = 1 / fadeTime;
		StartCoroutine (LoadLevel(fadeTime));
	}

	IEnumerator LoadLevel(float waitTime){
		yield return new WaitForSeconds (waitTime);
		SceneManager.LoadScene (nextScene);
	}

	string RemoveSymbols(string text){
		string ret = "\"";
		char prevC = 'a';
		foreach(char c in text){
			if (c == '#' || c == '|') {
				prevC = c;
				continue;
			}
			if(c == ' ' && (prevC == '#' || prevC == '|')){
				prevC = c;
				continue;
			}
			ret += c;
		}
		ret += "\"";
		return ret;
	}

	void fetchWord(string currentGram){
		
	}
}
