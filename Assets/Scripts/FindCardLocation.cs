using UnityEngine;
using System.Collections;
using TouchScript;
using TouchScript.Gestures;

public class FindCardLocation : MonoBehaviour {

	GameObject GMReference;
	// Use this for initialization
	void Awake () {
		GMReference = GameObject.Find ("GameManager");
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	void OnEnable() {
		GetComponent<TapGesture>().Tapped += GMReference.GetComponent<GameManager>().ShowDebugCards;
	}


	void OnDisable() {
		GetComponent<TapGesture>().Tapped -= GMReference.GetComponent<GameManager>().ShowDebugCards;
	}
}
