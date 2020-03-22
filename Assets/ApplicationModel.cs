using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ApplicationModel {
	public string poem;

	private static ApplicationModel _instance;

	public static ApplicationModel Instance{
		get{
			if (_instance == null) {
				_instance = new ApplicationModel ();
			}
			return _instance;
		}
	}

	void Start(){
		Instance.poem = "TESST";
	}


}
