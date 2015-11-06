using UnityEngine;
using System.Collections;
using System.Net;

public class NetworkManagerTT : MonoBehaviour {
	private const string typeName = "PrizmPokerMultiscreen";
	public string gameName = "Texas Hold'em (Default)";

	private HostData[] hostList;

	private string GetIP(){
		string strHostName = "";
		strHostName = System.Net.Dns.GetHostName ();
		IPHostEntry ipEntry = System.Net.Dns.GetHostEntry (strHostName);
		IPAddress[] addr = ipEntry.AddressList;
		return addr [addr.Length - 1].ToString ();
	}
	
	private void StartServer()
	{
		Network.InitializeServer(32, 25000, !Network.HavePublicAddress());
		MasterServer.RegisterHost(typeName, gameName);
	}
	
	void OnServerInitialized()
	{
		Debug.Log("Server Initializied");
		Debug.Log ("Server master ip address: " + MasterServer.ipAddress + ", our local IP: " + GetIP());

	}

	void OnMasterServerEvent(MasterServerEvent msEvent)
	{
		if (msEvent == MasterServerEvent.HostListReceived)
			hostList = MasterServer.PollHostList();
	}

	void OnGUI() {
		if (GUI.Button (new Rect (100, 100, 250, 100), GetIP ()))
			Debug.Log ("clicked");
		if (GUI.Button(new Rect(100, 250, 250, 100), "Refresh Hosts"))
			RefreshHostList();
		if (hostList != null)
		{
			for (int i = 0; i < hostList.Length; i++)
			{
				if (GUI.Button(new Rect(400, 100 + (110 * i), 300, 100), hostList[i].gameName))
					Debug.Log(hostList[i]);
			}
		}
	}

	void Start () {
		StartServer ();
	}

	private void RefreshHostList()
	{
		MasterServer.RequestHostList(typeName);
	}
}