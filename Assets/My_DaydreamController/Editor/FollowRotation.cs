using UnityEngine;
using System.Collections;

public class FollowRotation : MonoBehaviour {
	// Update is called once per frame
    public bool PkeyPress = false;
    public bool algDone = false;

	void Update () {
		transform.rotation = SensorManager.rotationRelativeToScreen;
		
	    if (Input.GetKeyUp(KeyCode.P))
	    {
//	        SpacePointAlgorithm.resetRefFlagFunction();
	        PkeyPress = true;
	    }
	    else
	    {
	        algDone = SpacePointAlgorithm.CheckresetRefFlagFunction();
	        if (PkeyPress && algDone)
	        {
	            SensorManager.SetScreenHeading();
	            PkeyPress = false;
	        }
	    }
	}
}
