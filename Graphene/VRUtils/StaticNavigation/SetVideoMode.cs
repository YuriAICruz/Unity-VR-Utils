using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.XR;

namespace Graphene.VRUtils.StaticNavigation
{
	public class SetVideoMode : MonoBehaviour
	{
		public static bool STEREO_MODE = false;

		private GameObject logoPanel;
		private GameObject instructionsPanel;
		private GameObject holder;

		Button continueBt;


		private void Awake()
		{
			XRSettings.enabled = false;
	
			logoPanel = GameObject.Find("MainMenuCanvas/LogoPanel");

			instructionsPanel = GameObject.Find("MainMenuCanvas/InstructionsPanel");

			continueBt = instructionsPanel.GetComponentInChildren<Button>();
			continueBt.interactable = false;

			instructionsPanel.SetActive(false);

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

		public void SetMonoscopic()
		{
			STEREO_MODE = false;
			StartCoroutine(WaitForClickToContinue());
		}

		public void SetStereoscopic()
		{
			STEREO_MODE = true;
			StartCoroutine(WaitForClickToContinue());
		}

		private IEnumerator WaitForClickToContinue()
		{
			string help = "Instruções\n\n";
			help += STEREO_MODE ? "Coloque os óculos de VR e vire a cabeça para ver as cenas em 360°." : "Deslize o dedo na tela para girar a cena.";
			help += " Para mudar de sala centralize o cursor por alguns segundos no círculo com o nome do ambiente escolhido.";
			help += "\n\nNecessário ter conexão de boa velocidade (Wi-Fi).";
			string lastLine = "\n\n...";

			Text txt = instructionsPanel.GetComponentInChildren<Text>(true);

			txt.text = help + lastLine;

			logoPanel.SetActive(false);
			holder.SetActive(false);
			instructionsPanel.SetActive(true);

			yield return new WaitForSeconds(3); // wait at least 5 seconds

			txt.text = help + "\n\nToque na tela para iniciar...";
			continueBt.interactable = true;
		}
	}
}