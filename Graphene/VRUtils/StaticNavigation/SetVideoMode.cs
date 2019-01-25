using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.XR;
using UnityEngine.SceneManagement;

namespace Graphene.VRUtils.StaticNavigation
{
	public class SetVideoMode : MonoBehaviour
	{
		public static bool STEREO_MODE = false;

		private GameObject instructionsPanel;
		private GameObject holder;

		Button continueBt;

		private void Awake()
		{
			Screen.orientation = ScreenOrientation.LandscapeLeft;

			XRSettings.enabled = false;
	
			instructionsPanel = GameObject.Find("MainMenuCanvas/InstructionsPanel");

			continueBt = instructionsPanel.GetComponentInChildren<Button>();
			continueBt.interactable = false;

			instructionsPanel.SetActive(false);

			holder = GameObject.Find("Holder");
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
			string help_pt = "Instruções\n\n";
			help_pt += STEREO_MODE ? "Coloque os óculos de VR e vire a cabeça para ver as cenas em 360°." : "Deslize o dedo na tela para girar a cena.";
			help_pt += " Para mudar de sala centralize o cursor por alguns segundos no círculo com o nome do ambiente escolhido.";
			help_pt += "\n\nNecessário ter conexão de boa velocidade (Wi-Fi).";

			string help_es = "Instrucciones\n\n";
			help_es += STEREO_MODE ? "Coloque las gafas de VR y gire la cabeza para ver las escenas en 360°." : "Deslice el dedo en la pantalla para girar la escena.";
			help_es += " Para cambiar de sala centraliza el cursor por unos segundos en el círculo con el nombre del ambiente elegido.";
			help_es += "\n\nEs necesario tener conexión de buena velocidad (Wi-Fi).";

			string lastLine = "\n\n...";

			string touchTheScreen_pt = "\n\nToque na tela para iniciar...";
			string touchTheScreen_es = "\n\nToque la pantalla para iniciar...";

			string help = help_pt;
			string touchTheScreen = touchTheScreen_pt;
			
			if (PlayerPrefs.GetString("language", "PT") == "ES")
			{
				help = help_es;
				touchTheScreen = touchTheScreen_es;
			}

			Text txt = instructionsPanel.GetComponentInChildren<Text>(true);

			txt.text = $"{help}{lastLine}";

			holder.SetActive(false);
			instructionsPanel.SetActive(true);

			yield return new WaitForSeconds(3); // wait at least 5 seconds

			txt.text = $"{help}{touchTheScreen}";
			continueBt.interactable = true;
		}

		void Update()
		{
			if (Input.GetKeyDown(KeyCode.Escape))
			{
				SceneManager.LoadScene("AppMenu");
			}
		}
	}
}