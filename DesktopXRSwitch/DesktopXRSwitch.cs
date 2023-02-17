using NAK.Melons.DesktopXRSwitch.Patches;
using System.Collections;
using UnityEngine;
using UnityEngine.XR.Management;

namespace NAK.Melons.DesktopXRSwitch;

public class DesktopXRSwitch : MonoBehaviour
{
    //Settings
    public bool _reloadLocalAvatar = true;

    //Internal Stuff
    private bool _switchInProgress = false;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.F6) && Input.GetKey(KeyCode.LeftControl))
        {
            SwitchXRMode();
        }
    }

    public void SwitchXRMode()
    {
        if (_switchInProgress) return;
        if (!IsInXR())
        {
            StartCoroutine(StartXRSystem());
        }
        else
        {
            StartCoroutine(StopXR());
        }
    }

    public bool IsInXR() => XRGeneralSettings.Instance.Manager.activeLoader != null;

    private IEnumerator StartXRSystem()
    {
        BeforeXRModeSwitch(true);
        yield return null;
        yield return XRGeneralSettings.Instance.Manager.InitializeLoader();
        if (XRGeneralSettings.Instance.Manager.activeLoader == null)
        {
            DesktopXRSwitchMod.Logger.Error("Initializing XR Failed. Is there no XR device connected?");
        }
        else
        {
            DesktopXRSwitchMod.Logger.Msg("Starting XR...");
            XRGeneralSettings.Instance.Manager.StartSubsystems();
            yield return null;
            AfterXRModeSwitch(true);
        }
        yield break;
    }

    private IEnumerator StopXR()
    {
        BeforeXRModeSwitch(false);
        yield return null;
        if (XRGeneralSettings.Instance.Manager.isInitializationComplete)
        {
            XRGeneralSettings.Instance.Manager.StopSubsystems();
            XRGeneralSettings.Instance.Manager.DeinitializeLoader();
            yield return null;
            AfterXRModeSwitch(false);
        }
        yield break;
    }

    //one frame before switch attempt
    public void BeforeXRModeSwitch(bool enterXR)
    {
        //let tracked objects know we are attempting to switch
        VRModeSwitchTracker.PreVRModeSwitch(enterXR);
    }

    //one frame after switch attempt
    public void AfterXRModeSwitch(bool enterXR)
    {
        //these two must come first
        TryCatchHell.SetCheckVR(enterXR);
        TryCatchHell.SetMetaPort(enterXR);

        //the bulk of funni changes
        TryCatchHell.RepositionCohtmlHud(enterXR);
        TryCatchHell.UpdateHudOperations(enterXR);
        TryCatchHell.DisableMirrorCanvas();
        TryCatchHell.SwitchActiveCameraRigs(enterXR);
        TryCatchHell.ResetCVRInputManager();
        TryCatchHell.UpdateRichPresence();
        TryCatchHell.UpdateGestureReconizerCam();

        //let tracked objects know we switched
        VRModeSwitchTracker.PostVRModeSwitch(enterXR);

        //reload avatar by default, optional for debugging
        if (_reloadLocalAvatar)
        {
            TryCatchHell.ReloadLocalAvatar();
        }

        _switchInProgress = false;
    }
}

