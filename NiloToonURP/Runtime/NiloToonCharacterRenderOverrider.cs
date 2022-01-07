using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;

namespace NiloToon.NiloToonURP
{
    [ExecuteAlways]
    public class NiloToonCharacterRenderOverrider : MonoBehaviour
    {
        public List<NiloToonPerCharacterRenderController> targets = new List<NiloToonPerCharacterRenderController>();

        public enum SettingSource
        {
            TargetCharacterRenderController,
            CustomSettings
        }
        [Header("Perspective Removal")]
        public bool ShouldOverridePerspectiveRemovalSettings = true;
        public SettingSource settingSource = SettingSource.TargetCharacterRenderController;


        [Header("For Source = TargetCharacterRenderController")]
        public NiloToonPerCharacterRenderController targetCharacterRenderController;

        [Header("For Source = CustomSettings")]
        [Range(0,1)]
        public float CustomPerspectiveRemovalAmount = 0;

        public float CustomPerspectiveRemovalRadius = 1;

        [Space(10)]
        [Range(0,1)]
        public float CustomPerspectiveRemovalWorldSpaceHeightFadeoutUsage = 0;
        public float CustomPerspectiveRemovalStartHeightWorldSpace = 0;
        public float CustomPerspectiveRemovalEndHeightWorldSpace = 1;

        [Space(10)]
        public bool CustomDisablePerspectiveRemovalInXR = true;
        public Transform CustomPerspectiveRemovalOverridedCenterPosWS;

        private void OnValidate()
        {
            // if null, auto assign self 
            if (CustomPerspectiveRemovalOverridedCenterPosWS == null)
                CustomPerspectiveRemovalOverridedCenterPosWS = transform;

            CustomPerspectiveRemovalRadius = Mathf.Max(0, CustomPerspectiveRemovalRadius); // prevent negative number
        }
        private void OnEnable()
        {
            // required to trigger update in editor scene
            LateUpdate();
        }
        private void LateUpdate()
        {
            foreach(var target in targets)
            {
                if(target)
                    target.ExternalRenderOverrider = this;
            }
        }
        private void OnDisable()
        {
            foreach (var target in targets)
            {
                if(target)
                    target.ExternalRenderOverrider = null;
            }
        }


        public bool ShouldOverridePerspectiveRemoval()
        {
            if (this.enabled && ShouldOverridePerspectiveRemovalSettings)
            {
                switch (settingSource)
                {
                    case SettingSource.TargetCharacterRenderController:
                        return targetCharacterRenderController; // return true if not null
                    case SettingSource.CustomSettings:
                        return CustomPerspectiveRemovalOverridedCenterPosWS; // return true if not null
                    default:
                        break;
                }
            }

            return false;
        }

        public float GetPerspectiveRemovalOverridedAmount()
        {
            switch (settingSource)
            {
                case SettingSource.TargetCharacterRenderController:
                    {
                        // XR check
                        if (targetCharacterRenderController.disablePerspectiveRemovalInXR && XRSettings.isDeviceActive)
                        {
                            return 0; // disable in VR, because PerspectiveRemoval looks weird in VR when camera rotate a lot
                        }
                        return targetCharacterRenderController.perspectiveRemovalAmount;
                    }
                case SettingSource.CustomSettings:
                    {
                        // XR check
                        if (CustomDisablePerspectiveRemovalInXR && XRSettings.isDeviceActive)
                        {
                            return 0; // disable in VR, because PerspectiveRemoval looks weird in VR when camera rotate a lot
                        }
                        return CustomPerspectiveRemovalAmount;
                    }
                default:
                    throw new System.NotImplementedException();
            }
        }
        public float GetPerspectiveRemovalOverridedRadius()
        {
            switch (settingSource)
            {
                case SettingSource.TargetCharacterRenderController:
                    return targetCharacterRenderController.perspectiveRemovalRadius;
                case SettingSource.CustomSettings:
                    return CustomPerspectiveRemovalRadius;
                default:
                    throw new System.NotImplementedException();
            }
        }
        public float GetPerspectiveRemovalOverridedStartHeight()
        {
            switch (settingSource)
            {
                case SettingSource.TargetCharacterRenderController:
                    return targetCharacterRenderController.perspectiveRemovalStartHeight;
                case SettingSource.CustomSettings:
                    return CustomPerspectiveRemovalStartHeightWorldSpace;
                default:
                    throw new System.NotImplementedException();
            }            
        }
        public float GetPerspectiveRemovalOverridedEndHeight()
        {
            switch (settingSource)
            {
                case SettingSource.TargetCharacterRenderController:
                    return targetCharacterRenderController.perspectiveRemovalEndHeight;
                case SettingSource.CustomSettings:
                    return CustomPerspectiveRemovalEndHeightWorldSpace;
                default:
                    throw new System.NotImplementedException();
            }            
        }
        public Vector3 GetPerspectiveRemovalOverridedCenterPosWS()
        {
            switch (settingSource)
            {
                case SettingSource.TargetCharacterRenderController:
                    return targetCharacterRenderController.GetSelfPerspectiveRemovalCenter();
                case SettingSource.CustomSettings:
                    return CustomPerspectiveRemovalOverridedCenterPosWS.position;
                default:
                    throw new System.NotImplementedException();
            }
        }
    }
}