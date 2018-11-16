using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.XR;

namespace Graphene.VRUtils.StaticNavigation
{
	public class SetVideoMode : MonoBehaviour
	{
		public static bool STEREO_MODE = false;

		private void Awake()
		{
			XRSettings.enabled = false;
		}

		public string Scene = "VideoPlayer";

		public void SetMonoscopic()
		{
			STEREO_MODE = false;
			SceneManager.LoadScene(Scene);
		}

		public void SetStereoscopic()
		{
			STEREO_MODE = true;
			SceneManager.LoadScene(Scene);
		}
	}
}
