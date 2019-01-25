using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;
using UnityEngine.XR;
using UnityEngine.SceneManagement;

namespace Graphene.VRUtils.StaticNavigation
{
    public class ToggleVRView : MonoBehaviour
    {
        private float pitch;
        private float yaw;

        private bool isVR;
        private Vector3 _lastTouch;

        public NavigationMap navigationMap;
        public int mainMenuId;

#if UNITY_EDITOR
        public float _speed;
#endif
        
        void Start()
        {
            isVR = SetVideoMode.STEREO_MODE;
            Setup();
        }

        void Setup()
        {
            Debug.Log("Is VR: " + isVR);
            string device = isVR ? "cardboard" : "";

            StartCoroutine(LoadDevice(device));
        }

        void Update()
        {
            if (!isVR)
            {
                TurnAround();
            }

            if (Input.GetKeyDown(KeyCode.Escape))
            {
                if (navigationMap.GetCurrentId() == mainMenuId)
                {
                    // Application.Quit();
		            SceneManager.LoadScene("AppMenu");
                }
                else
                {
                    navigationMap.MoveToRoom(mainMenuId);
                }
            }
        }

        void TurnAround()
        {
#if UNITY_EDITOR
            if (Input.GetMouseButton(0))
            {
                var touch = Input.mousePosition;
                var delta = (_lastTouch - touch);
                if (delta.magnitude > 0.1f)
                {
                    var xt = delta.x;
                    var yt = delta.y;

                    if (Mathf.Abs(xt) >= Mathf.Abs(yt))
                    {
                        yaw += Time.deltaTime * -xt * _speed;
                        yaw = yaw % 360;
                        if (yaw < 0)
                        {
                            yaw += 360;
                        }
                    }
                    else
                    {
                        pitch += Time.deltaTime * -yt * _speed;
                        pitch = Mathf.Clamp(pitch, -90f, 90f);
                    }
                    transform.localEulerAngles = new Vector3(-pitch, yaw, 0f);
                }
                _lastTouch = touch;
            }
            else
            {
                _lastTouch = Input.mousePosition;
            }
#endif

            if (Input.touchCount > 0)
            {
                Touch touch = Input.GetTouch(0);
                if (touch.phase == TouchPhase.Moved)
                {
                    float xt = touch.deltaPosition.x;
                    float yt = touch.deltaPosition.y;

                    if (Mathf.Abs(xt) >= Mathf.Abs(yt))
                    {
                        yaw += Time.deltaTime * -xt;
                        yaw = yaw % 360;
                        if (yaw < 0)
                        {
                            yaw += 360;
                        }
                    }
                    else
                    {
                        pitch += Time.deltaTime * -yt;
                        pitch = Mathf.Clamp(pitch, -90f, 90f);
                    }
                    transform.localEulerAngles = new Vector3(-pitch, yaw, 0f);
                }
            }
        }

        IEnumerator LoadDevice(string newDevice)
        {
            XRSettings.LoadDeviceByName(newDevice);
            yield return null;
            if (isVR)
            {
                XRSettings.enabled = true;
            }
        }
    }
}