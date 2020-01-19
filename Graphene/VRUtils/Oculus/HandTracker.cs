using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Graphene.VRUtils.Oculus
{
    public class HandTracker : MonoBehaviour
    {
#if OCULUS_QUEST
        public OVRPlugin.Hand HandType = OVRPlugin.Hand.None;
        private bool _isInitialized;
        private bool _isTracked;
        private float _confidence;
        private OVRPlugin.HandState _currentState;

        private Transform _pointer;
        
        private List<Transform> _bones;
        private GameObject _skeleton;
        private IList<Transform> _readOnlyBones;
        
        
        public bool IsInitialized
        {
            get
            {
                return (_isInitialized);
            }
            private set
            {
                _isInitialized = value;
            }
        }

        public bool IsTracked
        {
            get
            {
                return _isTracked;
            }
            private set
            {
                _isTracked = value;
            }
        }

        public float Confidence
        {
            get
            {
                return _confidence;
            }
            private set
            {
                _confidence = value;
            }
        }
        
        public IList<Transform> Bones
        {
            get
            {
                return _readOnlyBones;
            }
        }

        
        #region MonoBehaviour
        
        private void Awake()
        {
            _currentState = new OVRPlugin.HandState();
            CreatePointer();
            SetupSkeleton();

            IsInitialized = true;
        }
        
        private void Start()
        {
            StartCoroutine(InitializeSkeleton());
        }

        private void FixedUpdate()
        {
            if (IsInitialized)
            {
                UpdatePose(OVRPlugin.Step.Physics);
            }
        }

        private void Update()
        {
            if (IsInitialized)
            {
                UpdatePose(OVRPlugin.Step.Render);
            }
        }

        #endregion
        
        
        #region Initialization

        private void SetupSkeleton()
        {
            if (!_skeleton)
            {
                _skeleton = new GameObject("Skeleton");
                _skeleton.transform.SetParent(transform);
                _skeleton.transform.position = Vector3.zero;
                _skeleton.transform.rotation = Quaternion.identity;
            }
        }

        private IEnumerator InitializeSkeleton()
        {
            bool success = false;
            while (!success)
            {
                var skeleton = new OVRPlugin.Skeleton();
                if (OVRPlugin.GetSkeleton(GetSkeletonTypeFromHandType(HandType), out skeleton))
                {
                    success = InitializeSkeleton(ref skeleton);
                }
                yield return null;
            }
            IsInitialized = true;
        }
        
        public bool InitializeSkeleton(ref OVRPlugin.Skeleton skeleton)
        {
            _bones = new List<Transform>(new Transform[(int)OVRPlugin.BoneId.Max]);
            _readOnlyBones = _bones.AsReadOnly();

            for (int i = 0; i < skeleton.NumBones && i < skeleton.NumBones; ++i)
            {
                var boneGO = new GameObject((skeleton.Bones[i].Id).ToString());
                if (((OVRPlugin.BoneId)skeleton.Bones[i].ParentBoneIndex) == OVRPlugin.BoneId.Invalid)
                {
                    boneGO.transform.SetParent(_skeleton.transform);
                }
                else if (_bones.Count > skeleton.Bones[i].ParentBoneIndex &&
                         _bones[skeleton.Bones[i].ParentBoneIndex])
                {
                    boneGO.transform.SetParent(_bones[skeleton.Bones[i].ParentBoneIndex].transform);
                }
                else
                {
                    Debug.LogError("Cannot find bone parent");
                }
                boneGO.transform.localPosition = skeleton.Bones[i].Pose.Position.FromFlippedZVector3f();
                boneGO.transform.localRotation = skeleton.Bones[i].Pose.Orientation.FromFlippedZQuatf();

                _bones[i] = boneGO.transform;
            }
            return true;
        }

        private void CreatePointer()
        {
            _pointer = new GameObject("Pointer").transform;
            _pointer.SetParent(transform);
            _pointer.position = Vector3.zero;
            _pointer.rotation = Quaternion.identity;
        }

        #endregion

        
        #region Tools

        public static OVRPlugin.SkeletonType GetSkeletonTypeFromHandType(OVRPlugin.Hand hand)
        {
            return hand == OVRPlugin.Hand.HandLeft ?
                OVRPlugin.SkeletonType.HandLeft :
                hand == OVRPlugin.Hand.HandRight ?
                    OVRPlugin.SkeletonType.HandRight : OVRPlugin.SkeletonType.None;
        }
        
        #endregion

        
        private void UpdatePose(OVRPlugin.Step renderStep)
        {
            if (!OVRPlugin.GetHandState(renderStep, HandType, ref _currentState))
            {
                IsTracked = false;
                Confidence = 0;
            }
            else
            {
                IsTracked = (_currentState.Status & OVRPlugin.HandStatus.HandTracked) == OVRPlugin.HandStatus.HandTracked;
                if (IsTracked)
                {
                    Confidence = _currentState.HandConfidence == OVRPlugin.TrackingConfidence.High ? 1 : 0.5f;
                }
                else
                {
                    Confidence = 0;
                }
                
                // Update Pointer
                _pointer.position = _currentState.PointerPose.Position.FromFlippedZVector3f();
                _pointer.rotation = _currentState.PointerPose.Orientation.FromFlippedZQuatf();
                // _pointer.PointerStatusValid = 
                //     (_currentState.Status & OVRPlugin.HandStatus.InputStateValid) == OVRPlugin.HandStatus.InputStateValid;
            }

            UpdateSkeletonPose(_currentState);
        }

        private void UpdateSkeletonPose(OVRPlugin.HandState pose)
        {
            if (IsTracked)
            {
                transform.position = pose.RootPose.Position.FromFlippedZVector3f();
                transform.rotation = pose.RootPose.Orientation.FromFlippedZQuatf();
                for (var i = 0; i < _bones.Count; ++i)
                {
                    _bones[i].localRotation = pose.BoneRotations[i].FromFlippedZQuatf();
                }
                transform.localScale = new Vector3(pose.HandScale, pose.HandScale, pose.HandScale);
            }
        }
        
        public float PinchStrength(OVRPlugin.HandFinger finger)
        {
            if (IsTracked && _currentState.PinchStrength != null && 
                _currentState.PinchStrength.Length > (int)finger)
            {
                return _currentState.PinchStrength[(int)finger];
            }
            else
            {
                return 0.0f;
            }
        }
        
#endif
    }
}