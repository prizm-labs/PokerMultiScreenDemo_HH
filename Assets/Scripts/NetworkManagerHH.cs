using UnityEngine;
using System.Collections;
using System.Net;

public class NetworkManagerHH : MonoBehaviour {
	private const string typeName = "PrizmPokerMultiscreen";
	
	private HostData[] hostList;
	
	private void RefreshHostList()
	{
		MasterServer.RequestHostList(typeName);
	}
	
	void OnMasterServerEvent(MasterServerEvent msEvent)
	{
		if (msEvent == MasterServerEvent.HostListReceived)
			hostList = MasterServer.PollHostList();
	}
	
	private string GetIP(){
		string strHostName = "";
		strHostName = System.Net.Dns.GetHostName ();
		
		IPHostEntry ipEntry = System.Net.Dns.GetHostEntry (strHostName);
		
		IPAddress[] addr = ipEntry.AddressList;
		
		return addr [addr.Length - 1].ToString ();
	}
	
	void OnGUI()
	{
		if (!Network.isClient && !Network.isServer)
		{
			
			if (GUI.Button(new Rect(100, 100, 250, 100), "Refresh Hosts"))
				RefreshHostList();
			
			if (hostList != null)
			{
				for (int i = 0; i < hostList.Length; i++)
				{
					if (GUI.Button(new Rect(400, 100 + (110 * i), 300, 100), hostList[i].gameName))
						JoinServer(hostList[i]);
				}
			}
		}
	}
	
	private void JoinServer(HostData hostData)
	{
		Debug.Log ("joining server: ");
		Network.Connect(hostData);
		foreach (var host in hostData.ip) {
			Debug.Log (host + ":" + hostData.port + " " );
		}

		//saves the meteor DB url
		GameObject.Find ("GameManager").GetComponent<ClientInitialization> ().setMeteorURL (hostData.ip [0]);
		GameObject.Find ("ChooseSessionCanvas").SetActive (false);
	}
	
	void OnConnectedToServer()
	{
		Debug.Log("Server Joined" + ", our local IP: " + GetIP());
	}
}
