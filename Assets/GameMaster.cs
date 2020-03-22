using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System;
using UnityEngine.SceneManagement;

public class GameMaster : MonoBehaviour {

	public GameObject text;
	public float timeForFirstSpawn = 2;
	public float timeBetweenSpawns = 2;
	float nextSpawn;
	public string nextScene;

	public AudioClip[] sounds;

	List<string> words;

	string fileName = "Assets/poems.txt";
	string begin = "#";
	string end = "|";
	Dictionary<string , List<string>> ngrams = new Dictionary<string , List<string>>();

	string currentWord;
	string result;

	bool spawing;

	// Use this for initialization
	void Start () {
		nextSpawn = Time.time + timeForFirstSpawn;

		TrainFromFile ();
		currentWord = begin;
		RefreshWords ();

		spawing = true;
	}

	void Update(){
		if (Time.time >= nextSpawn && spawing == true) {
			nextSpawn = Time.time + timeBetweenSpawns;

			string word = PickWord ();
			if (word != null) {
				SpawnText (word);	
			}
			else {
				print ("Out of words");
			}
		}

	}

	void SpawnText(string word){
		Vector3 spawnPosition = new Vector3 (transform.position.x, transform.position.y, transform.position.z);
		spawnPosition.x += UnityEngine.Random.Range (0f, 20f);
		if (word != "|") {
			//print (word);
			GameObject textInstance = Instantiate (text, spawnPosition, Quaternion.identity);
			textInstance.GetComponent<TextMesh> ().text = word;
			textInstance.GetComponent<TextController> ().textActivation += OnTextActivation;
		} else {
			GameObject textInstance = Instantiate (text, spawnPosition, Quaternion.identity);
			textInstance.GetComponent<TextMesh> ().text = "The End.";
			textInstance.GetComponent<TextController> ().textActivation += OnTextActivation;
		}
	}

	void RefreshWords(){
		List<string> possibilities = ngrams [currentWord];
		if (possibilities.Count == 0) {
			words = new List<string> ();
			words.Add (end);
		} else {
			words = possibilities;
		}
	}

	string PickWord(){
		return words[UnityEngine.Random.Range (0, words.Count)];
	}



	void OnTextActivation(string word){
		//print ("Text Activation");
		if (word != "The End.") {

			GetComponent<AudioSource>().PlayOneShot(sounds[UnityEngine.Random.Range(1,sounds.Length)]);

			result += word + " ";
			currentWord = word;
			RefreshWords ();

			GameObject[] texts = GameObject.FindGameObjectsWithTag ("Text");
			foreach (GameObject t in texts) {
				TextMesh tm = t.GetComponent<TextMesh> ();
				if (tm != null) {
					string pos = PickWord ();
					if(pos == "|"){
						pos = "The End.";
						tm.color = Color.red;
					}
					tm.text = pos;

				} else {
					//print ("NULL");
				}
			}
		} else {
			GetComponent<AudioSource>().PlayOneShot(sounds[0]);

			spawing = false;

			GameObject[] texts = GameObject.FindGameObjectsWithTag ("Text");
			foreach (GameObject t in texts) {
				t.GetComponent<TextController>().Die ();
			}
			float fadeTime = GameObject.Find ("Fade").GetComponent<Fading> ().BeginFade (1);
			fadeTime = 1 / fadeTime;
			StartCoroutine (LoadNewLevel(fadeTime));
		}
	}

	IEnumerator LoadNewLevel(float waitTime){
		ApplicationModel.Instance.poem = "\""+result+"\"";
		yield return new WaitForSeconds (waitTime);
		SceneManager.LoadScene (nextScene);
	}

	bool TrainFromFile(){
		try
		{
			string line;

			FileStream stream = new FileStream(fileName , FileMode.Open);
			StreamReader theReader = new StreamReader(stream);

			using(theReader){
				// While there's lines left in the text file, do this:
				do
				{
					line = theReader.ReadLine();
					if (line != null)
					{
						line = begin+" "+line+" "+end;
						var words = line.Split(null);
						Train(words);
					}
				}
				while (line != null);
				// Done reading, close the reader and return true to broadcast success    
				theReader.Close();
				return true;
			}
		}
		// If anything broke in the try block, we throw an exception with information
		// on what didn't work
		catch (Exception e)
		{
			Console.WriteLine("{0}\n", e.Message);
			return false;
		}
	}

	void Train(String[] words){
		for (int i = 0; i < words.Length; i++) {
			string gram = words [i];
			if (!ngrams.ContainsKey (gram)) {
				ngrams.Add (gram, new List<string> ());
			}
			//print (gram);
			if(i < words.Length-1)
				ngrams [gram].Add (words[i+1]);
		}

	}
}
