using ABI_RC.Systems.IK;
using ABI_RC.Systems.IK.SubSystems;
using ABI_RC.Systems.IK.TrackingModules;
using HarmonyLib;
using UnityEngine;

namespace NAK.Melons.DesktopXRSwitch.Patches;

public class IKSystemTracker : MonoBehaviour
{
    public IKSystem ikSystem;
    public Traverse _traverseModules;

    void Start()
    {
        ikSystem = GetComponent<IKSystem>();
        _traverseModules = Traverse.Create(ikSystem).Field("_trackingModules");
        VRModeSwitchTracker.OnPostVRModeSwitch += PostVRModeSwitch;
    }
    void OnDestroy()
    {
        VRModeSwitchTracker.OnPostVRModeSwitch -= PostVRModeSwitch;
    }

    public void PostVRModeSwitch(bool enterXR, Camera activeCamera)
    {
        var _trackingModules = _traverseModules.GetValue<List<TrackingModule>>();
        OpenXRTrackingModule openXRTrackingModule = _trackingModules.FirstOrDefault(m => m is OpenXRTrackingModule) as OpenXRTrackingModule;
        if (openXRTrackingModule != null)
        {
            openXRTrackingModule.ModuleDestroy();
        }
        else
        {
            //enable arm tracking for VR
            BodySystem.TrackingLeftArmEnabled = true;
            BodySystem.TrackingRightArmEnabled = true;
            //add back tracking module for FBT
            OpenXRTrackingModule inputModule = new OpenXRTrackingModule();
            ikSystem.AddTrackingModule(inputModule);
        }
    }
}