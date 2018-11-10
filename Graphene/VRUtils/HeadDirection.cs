using System;
using UnityEngine;
using UnityEngine.UI;

namespace Graphene.VRUtils
{
    public class HeadDirection : MonoBehaviour
    {
        private Camera _camera;

        public Vector3 GizmoPos;
        private LayerMask _mask;

        public Action<float> Counter;

        public float CounterDuration = 1.2f;

        private float _time;
        private GameObject _lastHit;

        private void Awake()
        {
            _camera = Camera.main;

            _mask = 1 << LayerMask.NameToLayer("UI");
        }

        private void Update()
        {
            var center = _camera.ScreenToWorldPoint(new Vector3(Screen.width / 2f, Screen.height / 2f, _camera.nearClipPlane));

            RaycastHit hit;
            if (Physics.Raycast(center, _camera.transform.forward, out hit, 15, _mask))
            {
                _time += Time.deltaTime;
                Debug.DrawRay(center, _camera.transform.forward * hit.distance, Color.red);

                GizmoPos = hit.point;

                if (_time / CounterDuration >= 1 && hit.transform.gameObject != _lastHit)
                {
                    _lastHit = hit.transform.gameObject;
                    hit.transform.GetComponent<Button>()?.onClick.Invoke();
                }
            }
            else
            {
                _time = 0;
                Debug.DrawRay(center, _camera.transform.forward * 15, Color.gray);

                GizmoPos = center + _camera.transform.forward * 15;

                _lastHit = null;
            }

            Counter?.Invoke(_time / CounterDuration);
        }
    }
}