﻿using NAK.Melons.DesktopXRSwitch.Patches;
using System.Collections;
using UnityEngine;
using Unity​Engine.XR.Management;
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
        PreXRModeSwitch(true);
        yield return null;
        yield return XRGeneralSettings.Instance.Manager.InitializeLoader();
        if (!XRGeneralSettings.Instance.Manager.activeLoader == null)
        {
            DesktopXRSwitchMod.Logger.Msg("Starting OpenXR...");
            XRGeneralSettings.Instance.Manager.StartSubsystems();
            yield return null;
            PostXRModeSwitch(true);
            yield break;
        }
        DesktopXRSwitchMod.Logger.Error("Initializing XR Failed. Is there no XR device connected?");
        FailedVRModeSwitch(true);
        yield break;
    }

    private IEnumerator StopXR()
    {
        PreXRModeSwitch(false);
        yield return null;
        if (XRGeneralSettings.Instance.Manager.isInitializationComplete)
        {
            XRGeneralSettings.Instance.Manager.StopSubsystems();
            XRGeneralSettings.Instance.Manager.DeinitializeLoader();
            yield return null;
            PostXRModeSwitch(false);
            yield break;
        }
        DesktopXRSwitchMod.Logger.Error("Attempted to exit VR without a VR device loaded.");
        FailedVRModeSwitch(true);
        yield break;
    }

    //one frame after switch attempt
    public void FailedVRModeSwitch(bool isXR)
    {
        //let tracked objects know a switch failed
        XRModeSwitchTracker.FailXRModeSwitch(isXR);
    }

    //one frame before switch attempt
    public void PreXRModeSwitch(bool isXR)
    {
        //let tracked objects know we are attempting to switch
        XRModeSwitchTracker.PreXRModeSwitch(isXR);
    }

    //one frame after switch attempt
    public void PostXRModeSwitch(bool isXR)
    {
        //close the menus
        TryCatchHell.CloseCohtmlMenus();

        //the base of VR checks
        TryCatchHell.SetCheckVR(isXR);
        TryCatchHell.SetMetaPort(isXR);

        //game basics for functional gameplay post switch
        TryCatchHell.RepositionCohtmlHud(isXR);
        TryCatchHell.UpdateHudOperations(isXR);
        TryCatchHell.DisableMirrorCanvas();
        TryCatchHell.SwitchActiveCameraRigs(isXR);
        TryCatchHell.ResetCVRInputManager();
        TryCatchHell.UpdateRichPresence();
        TryCatchHell.UpdateGestureReconizerCam();

        //let tracked objects know we switched
        XRModeSwitchTracker.PostXRModeSwitch(isXR);

        //reload avatar by default, optional for debugging
        if (_reloadLocalAvatar)
        {
            TryCatchHell.ReloadLocalAvatar();
        }

        _switchInProgress = false;
    }
}

