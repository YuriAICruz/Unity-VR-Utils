using System;
using UnityEngine;

namespace Graphene.VRUtils
{
    public class ResetHeadPosition : MonoBehaviour
    {
        public event Action OnReset;
        
        public Transform Target;
        private Vector3 _offset;

        public Vector3 OffsetPosition
        {
            get
            {
                if (PlayerPrefs.HasKey("HeadPositionOffsetX"))
                    _offset = new Vector3(
                        PlayerPrefs.GetFloat("HeadPositionOffsetX"),
                        PlayerPrefs.GetFloat("HeadPositionOffsetY"),
                        PlayerPrefs.GetFloat("HeadPositionOffsetZ")
                    );

                return Target.position + _offset;
            }
            set
            {
                _offset = value - Target.position;

                PlayerPrefs.SetFloat("HeadPositionOffsetX", _offset.x);
                PlayerPrefs.SetFloat("HeadPositionOffsetY", _offset.y);
                PlayerPrefs.SetFloat("HeadPositionOffsetZ", _offset.z);
            }
        }

        private void Start()
        {
            ResetPosition();
        }

        void Update()
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                ResetPosition();
            }
        }

        public void ResetPosition()
        {
            transform.position = OffsetPosition - (-transform.position + Camera.main.transform.position);

            OnReset?.Invoke();
        }
    }
}