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

		public string Scene = "Demo";

		protected override void OnClick()
		{
			StartCoroutine(LoadScene());
		}

		protected IEnumerator LoadScene()
		{
			AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(Scene);

			while (!asyncLoad.isDone)
			{
				yield return null;
			}

		}

		void Update()
		{
			if (Input.GetKeyDown(KeyCode.Escape))
			{
				SceneManager.LoadScene("MainMenu");
			}
		}
	}
}
