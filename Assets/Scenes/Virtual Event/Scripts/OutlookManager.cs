using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

// [Noah]
// this is all yours, good luck!
public class OutlookManager : MonoBehaviour
{
	public GameObject UI;

	public bool debugOnlyUpdateOutlook = false;
	public bool debugOnlySetOutlook = false;
	private void Update() {
		if(debugOnlyUpdateOutlook) {
			// randomly update 1 part
			MainNetworkPlayer.UpdateMainPlayerOutlook(Random.Range(0,10), Random.Range(0,3));
			debugOnlyUpdateOutlook = false;
			Debug.Log("UpdateOutlook");
			UI.SetActive(true);
		}
		if(debugOnlySetOutlook) {
			// set all values in the list
			MainNetworkPlayer.SetMainPlayerOutlook(new List<int>{0,1,3,4,5});
			debugOnlySetOutlook = false;
			Debug.Log("UpdateOutlook");
		}
	}
}
