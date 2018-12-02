using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.XR;

namespace Graphene.VRUtils.StaticNavigation
{
	public class ShowLogoScreen : MonoBehaviour
	{

		private GameObject logoPanel;

		private void Awake()
		{
			XRSettings.enabled = false;
	
			logoPanel = GameObject.Find("MainMenuCanvas/LogoPanel");
		}

		private IEnumerator Start()
		{
			yield return new WaitForSeconds(1.5f);

			foreach (Graphic graphic in logoPanel.GetComponentsInChildren<Graphic>())
			{
				graphic.CrossFadeAlpha(0f, 0.5f, false);
			}
			logoPanel.GetComponent<Image>().CrossFadeAlpha(0f, 1f, false);

			yield return new WaitForSeconds(1f);

			logoPanel.SetActive(false);
		}
	}
}
