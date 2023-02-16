using ABI_RC.Core.Util.Object_Behaviour;
using UnityEngine;

namespace NAK.Melons.DesktopXRSwitch.Patches;

public class CameraFacingObjectFix : VRModeSwitchTracker
{
    public CameraFacingObject cameraFacingObject;
    void Start()
    {
        cameraFacingObject = GetComponent<CameraFacingObject>();
    }

    public override void PostVRModeSwitch(Camera activeCamera)
    {
        cameraFacingObject.m_Camera = activeCamera;
    }
}