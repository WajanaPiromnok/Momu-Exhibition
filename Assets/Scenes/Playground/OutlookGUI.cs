using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class OutlookGUI : MonoBehaviour
{
	public InputField iBaseModel;
	public InputField iTop;
	public InputField iBody;
	public InputField iBottom;
    public void ChangeOutlookValues(){
		ModifiedMovementInput.Main.CmdSetOutlook(int.Parse(iBaseModel.text), int.Parse(iTop.text), int.Parse(iBody.text), int.Parse(iBottom.text));
	}
}
