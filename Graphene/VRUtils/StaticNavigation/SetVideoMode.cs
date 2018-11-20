using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.XR;

namespace Graphene.VRUtils.StaticNavigation
{
	public class SetVideoMode : MonoBehaviour
	{
		public static bool STEREO_MODE = false;

		private GameObject logoPanel;
		private GameObject instructionsPanel;
		private GameObject holder;

		private void Awake()
		{
			XRSettings.enabled = false;
	
			logoPanel = GameObject.Find("MainMenuCanvas/LogoPanel");
			instructionsPanel = GameObject.Find("MainMenuCanvas/InstructionsPanel");
			if (instructionsPanel)
			{
				instructionsPanel.SetActive(false);
			}
			holder = GameObject.Find("Holder");
		}
		
		private IEnumerator Start()
		{
			yield return new WaitForSeconds(1.5f);

			foreach (Graphic graphic in logoPanel.GetComponentsInChildren<Graphic>())
			{
				graphic.CrossFadeAlpha(0f, 0.5f, false);
			}
			logoPanel.GetComponent<Image>().CrossFadeAlpha(0f, 1f, false);
		}

		public string Scene = "Demo";

		public void SetMonoscopic()
		{
			STEREO_MODE = false;
			StartCoroutine(LoadWelcomeScene());
		}

		public void SetStereoscopic()
		{
			STEREO_MODE = true;
			StartCoroutine(LoadWelcomeScene());
		}

		public void ShowInstructions()
		{
			string help = "Instruções\n\n";
			help += STEREO_MODE ? "Coloque os óculos de VR e vire a cabeça para ver as cenas em 360°." : "Deslize o dedo na tela para girar a cena.";
			help += " Para mudar de sala centralize o cursor por alguns segundos no círculo com o nome do ambiente escolhido.";
			help += "\n\nNecessário ter conexão de boa velocidade (Wi-Fi).\n\nCarregando...";

			Text txt = instructionsPanel.GetComponentInChildren<Text>(true);

			if (txt)
			{
				txt.text = help;
			}

			logoPanel.SetActive(false);
			holder.SetActive(false);
			instructionsPanel.SetActive(true);
		}

		private IEnumerator LoadWelcomeScene()
		{
			ShowInstructions();

			yield return new WaitForSeconds(5); // wait at least 2 seconds

			AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(Scene);

			while (!asyncLoad.isDone)
			{
				yield return null;
			}
		}
	}
}