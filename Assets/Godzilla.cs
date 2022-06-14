using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Godzilla : MonoBehaviour
{
	public float intensity = 10f;
    // Start is called before the first frame update

	public void FootLandingEffect()
	{
		CameraFxController.only.ScreenShake(intensity);
	}
}
