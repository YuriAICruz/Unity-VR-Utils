using Graphene.UiGenerics;
using UnityEngine;
using UnityEngine.UI;

namespace Graphene.VRUtils.Presentation
{
    public class OffsetPositionManager : MonoBehaviour
    {
        public InputFieldView PosX, PosY, PosZ, Rotation;
        private ResetHeadPosition _rh;

        private Vector3 _pos;

        private void Start()
        {
            PosX.OnValueChanged += (str) => UpdatePosition(0, str);
            PosY.OnValueChanged += (str) => UpdatePosition(1, str);
            PosZ.OnValueChanged += (str) => UpdatePosition(2, str);

            if (Rotation)
                Rotation.OnValueChanged += UpdateRotation;


            _rh = FindObjectOfType<ResetHeadPosition>();

            _rh.OnReset += UpdateInputFields;

            _pos = _rh.OffsetPosition;

            UpdateInputFields();
        }

        private void UpdateInputFields()
        {
            PosX.InputField.text = (-_pos.x).ToString("0.000");
            PosY.InputField.text = (_pos.y).ToString("0.000");
            PosZ.InputField.text = (-_pos.z).ToString("0.000");
        }

        private void UpdatePosition(int i, string pos)
        {
            float v;

            if (!float.TryParse(pos, out v)) return;

            switch (i)
            {
                case 0:
                    _pos.x = -v;
                    break;
                case 1:
                    _pos.y = v;
                    break;
                case 2:
                    _pos.z = -v;
                    break;
            }

            _rh.OffsetPosition = _pos;
        }

        private void UpdateRotation(string rot)
        {
        }
    }
}