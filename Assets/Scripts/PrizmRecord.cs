using UnityEngine;
using System.Collections;
using System.Collections.Generic;

//parent class of all game objects that need syncing
public class PrizmRecord : MonoBehaviour{
	//<----------> Keep track of components for convenience <---------->//
	private PrizmRecordGroup recordGroup;

	//<----------> PrizmRecord variables related to database operations <---------->//
	public DatabaseEntry dbEntry;		//DatabaseEntry's fields are determined by the developer
	public bool needsUpdate = false;

	//call this function in any derived classes with base.Awake()
	virtual protected void Awake() {
		dbEntry = new DatabaseEntry ();
		recordGroup = GameObject.Find ("/GameManager").GetComponent<PrizmRecordGroup> ();
	}

	//call this function in any derived classes with base.Start()
	virtual protected void Start() {
		/*
		if (recordGroup.gameObjectCollectionCreated) {
			AddToRecordGroup ();	//adds to PrizmRecordGroup so that we can keep track of it
		} else {
			Debug.Log ("you will have to call AddToRecordGroup() for this object: " + gameObject.name);
		}
		*/
	}
	
	virtual public void OnDestroy() {
		recordGroup.RemoveRecord (this);		//stop tracking this object
	}
	
	public void dbUpdated() {
		needsUpdate = false;
	}

	//add self to PrizmRecordGroup
	public void AddToRecordGroup() 
	{
		//only add the record if it isn't already added
		if (!recordGroup.associates.Contains (this)) {
			recordGroup.AddRecord (this);	
		} else {
			Debug.Log ("this: " + gameObject.name + " is already added to PrizmRecordGroup.associates");
		}
	}
}
