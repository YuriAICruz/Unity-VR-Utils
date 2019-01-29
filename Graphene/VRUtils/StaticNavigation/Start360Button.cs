using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Graphene.UiGenerics;
using UnityEngine.SceneManagement;

namespace Graphene.VRUtils.StaticNavigation
{
	public class Start360Button : ButtonView
	{

		public string tourScene = "Demo";
		public string mainMenuScene = "MainMenu";

		protected override void OnClick()
		{
			StartCoroutine(LoadScene(tourScene));
		}

		protected IEnumerator LoadScene(string scene)
		{
			AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(scene);

			while (!asyncLoad.isDone)
			{
				yield return null;
			}

		}

		void Update()
		{
			if (Input.GetKeyDown(KeyCode.Escape))
			{
				Screen.orientation = ScreenOrientation.LandscapeLeft;
				//StartCoroutine(LoadScene(mainMenuScene));
				SceneManager.LoadScene(mainMenuScene);
			}
		}
	}
}
