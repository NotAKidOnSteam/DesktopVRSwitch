using NAK.Melons.DesktopXRSwitch.Patches;
using System.Collections;
using UnityEngine;
using Valve.VR;

namespace NAK.Melons.DesktopXRSwitch;

public class DesktopXRSwitch : MonoBehaviour
{
    //Settings
    public bool _reloadLocalAvatar = true;

    //Internal Stuff
    private bool _switchInProgress = false;

    void Start()
    {
        //do not pause game, this breaks dynbones & trackers
        SteamVR_Settings.instance.pauseGameWhenDashboardVisible = false;
    }

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
        PreVRModeSwitch(true);
        yield return null;
        yield return XRGeneralSettings.Instance.Manager.InitializeLoader();
        if (!XRGeneralSettings.Instance.Manager.activeLoader == null)
        {
            DesktopXRSwitchMod.Logger.Msg("Starting OpenXR...");
            XRGeneralSettings.Instance.Manager.StartSubsystems();
            yield return null;
            PostVRModeSwitch(true);
            yield break;
        }
        DesktopXRSwitchMod.Logger.Error("Initializing XR Failed. Is there no XR device connected?");
        FailedVRModeSwitch(true);
        yield break;
    }

    private IEnumerator StopXR()
    {
        PreVRModeSwitch(false);
        yield return null;
        if (XRGeneralSettings.Instance.Manager.isInitializationComplete)
        {
            XRGeneralSettings.Instance.Manager.StopSubsystems();
            XRGeneralSettings.Instance.Manager.DeinitializeLoader();
            yield return null;
            PostVRModeSwitch(false);
            yield break;
        }
        DesktopXRSwitchMod.Logger.Error("Attempted to exit VR without a VR device loaded.");
        FailedVRModeSwitch(true);
        yield break;
    }

    //one frame after switch attempt
    public void FailedVRModeSwitch(bool enterVR)
    {
        //let tracked objects know a switch failed
        VRModeSwitchTracker.FailVRModeSwitch(enterVR);
    }

    //one frame before switch attempt
    public void PreVRModeSwitch(bool enterVR)
    {
        //let tracked objects know we are attempting to switch
        VRModeSwitchTracker.PreVRModeSwitch(enterVR);
    }

    //one frame after switch attempt
    public void PostVRModeSwitch(bool enterVR)
    {
        //close the menus
        TryCatchHell.CloseCohtmlMenus();

        //the base of VR checks
        TryCatchHell.SetCheckVR(enterVR);
        TryCatchHell.SetMetaPort(enterVR);

        //game basics for functional gameplay post switch
        TryCatchHell.RepositionCohtmlHud(enterVR);
        TryCatchHell.UpdateHudOperations(enterVR);
        TryCatchHell.DisableMirrorCanvas();
        TryCatchHell.SwitchActiveCameraRigs(enterVR);
        TryCatchHell.ResetCVRInputManager();
        TryCatchHell.UpdateRichPresence();
        TryCatchHell.UpdateGestureReconizerCam();

        //let tracked objects know we switched
        VRModeSwitchTracker.PostVRModeSwitch(enterVR);

        //reload avatar by default, optional for debugging
        if (_reloadLocalAvatar)
        {
            TryCatchHell.ReloadLocalAvatar();
        }

        _switchInProgress = false;
    }
}

