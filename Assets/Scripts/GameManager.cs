using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

//enum to match colored ring on handheld device to colored ring on tabletop device
public class PlayerColor {
	public enum color {white=0, purple, brown, orange, teal, green, yellow, violet, black, blue}
}

public class GameManager : MonoBehaviour {

	//<----------> Variables that give us access to GameManager components <---------->//
	PrizmRecordGroup recordGroup;
	ClientInitialization clientInit;
	BootstrapHH bootstrap;

	public List<string> debugList;
	bool production = false;

	//<----------> Variables for dealing cards to the player <---------->//
	public GameObject card1;	//GameObject used for Instantiating new cards that come into player's hand
	PlayingCard playingCard1;	//PlayingCard component of Instantiated card
	GameObject hand;			//Area that is parent to cards that player has
	bool cardInPosition1 = true;	//determines which position an incoming card is dealt to
	
	//set all variables
	void Awake() {
		recordGroup = GameObject.Find ("GameManager").GetComponent<PrizmRecordGroup> ();
		clientInit = GameObject.Find ("GameManager").GetComponent<ClientInitialization> ();
		bootstrap = GameObject.Find ("GameManager").GetComponent<BootstrapHH> ();
		hand = GameObject.Find ("Hand");
		debugList = new List<string> ();
	}

	void Update() {
		//exits the app if back button is pressed on android
		if (Input.GetKeyDown (KeyCode.Escape))
			StartCoroutine (ExitApplication ());
	}

	//closes player's record on the database, exits android app
	IEnumerator ExitApplication() {
		//Debug.LogError ("in ExitApplication()");
		var methodCall = Meteor.Method<ChannelResponse>.Call ("removePlayerRecord", clientInit.playerID);
		
		yield return (Coroutine)methodCall;
		
		if (methodCall.Response.success) {
			Debug.LogError ("call to removePlayerRecord SUCCEEDED!, response: " + methodCall.Response.message);
		} else {
			Debug.LogError("call to 'removePlayerRecord' did not succeed.");
		}

		Application.Quit ();
	}



	//manages if a card is dealt to the player OR if the tabletop device recalls cards
	public void HandleDidChangeRecord(string arg1, DatabaseEntry arg2, IDictionary arg3, string[] arg4) {
		Debug.Log ("GameObject Record Change Detected");
		//if the card is dealt to us
		if (arg2.location == clientInit.playerName) {
			GameObject newCard = Instantiate (card1, new Vector3 (0f, 4.2f, -11.0f), card1.transform.rotation) as GameObject;
			newCard.name = "PlayingCard";
			newCard.transform.Rotate (new Vector3 (0, 0, 180));				//keep the card face down
			GameObject.Find("Hand").GetComponent<HandManager>().resetShowingCards();	//reset the hand so that cards are facing down by default
			playingCard1 = newCard.GetComponent<PlayingCard> ();			//set the PlayingCard component

			//sets the variables to a local copy of the card
			playingCard1.dbEntry.location = arg2.location;
			playingCard1.dbEntry.back = arg2.back;
			playingCard1.dbEntry.number = arg2.number;
			playingCard1.dbEntry.suit = arg2.suit;
			playingCard1.dbEntry._id = arg2._id;

			//keep track of this card and enable us to manipulate it
			playingCard1.AddToRecordGroup ();

			//pick the right face of a card to show
			string cardName = arg2.back + "" + arg2.number + "" + arg2.suit;
			foreach (Transform child in newCard.transform) {
				child.gameObject.GetComponent<MeshRenderer> ().enabled = false;
				child.gameObject.SetActive (false);
				if (child.gameObject.name == cardName) {
					child.gameObject.GetComponent<MeshRenderer> ().enabled = true;
					child.gameObject.SetActive (true);
				}
			}

			//move the card to the appropriate location
			if (cardInPosition1) {
				newCard.GetComponent<physicalCard> ().moveToTarget (GameObject.Find ("Hand/CardLocation1"));
				cardInPosition1 = false;
			} else {
				newCard.GetComponent<physicalCard> ().moveToTarget (GameObject.Find ("Hand/CardLocation2"));
				cardInPosition1 = true;
			}

			//set the parent to "Hand" so that "Hand can easily control the cards
			newCard.transform.parent = GameObject.Find ("Hand").gameObject.transform;

			//if the tabletop device is recalling the cards (to reshuffle)
		} else if (arg2.location == "deck") {
			//moves the cards to the discard pile on mobile screen
			//and re-syncs the database that the cards are at location 'deck'
			hand.GetComponent<HandManager>().Recall();
		}
		//the card is neither being passed to us nor being recalled by the tabletop
		else {
			Debug.LogError ("this card does not belong to us, it belongs to: " + arg2.location);
			if (!production) {
				debugList.Add(arg2.location + arg2.number + arg2.suit);
				//Debug.Log ("added to list: " + arg2.location + arg2.number + arg2.suit);
				//clientInit.createMsgLog(arg2.location + "\n" + arg2.number + "\n" + arg2.suit, 1f);
			}
		}
	}

	public void ShowDebugCards (object sender, System.EventArgs e)
	{
		Debug.Log ("Debug Handler Tapped, list is long: " + debugList.Count);
		string bigString = "";
		if (debugList.Count > 8) {
			for (int i = 0; i < 8; i++) {
				bigString += debugList[debugList.Count - i - 1];
				bigString += "\n";
			}
			Debug.Log(bigString);
			clientInit.createMsgLog (bigString, 3);
		}
	}

}
