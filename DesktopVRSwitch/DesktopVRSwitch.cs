using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.XR.Management;
using ABI_RC.Core.Savior;
using ABI_RC.Core.Player;

//toggling cameras refreshes aspect ratio
//it is likely best to switch the active cameras first before calling StartXR

namespace NAK.Melons.DesktopVRSwitch;

public class DesktopVRSwitch : MonoBehaviour
{
    public bool _isInXR = false;
    public bool _awaitingSwitch = false;
    public bool _reloadLocalAvatar = true;
    public bool _xrRuntimeError = false;

    void Start()
    {
        DesktopVRSwitchMod.Logger.Msg("Message");
    }

    void Update()
    {
        //DebugSwitch();
    }

    private void DebugEnterXR()
    {
        if (_awaitingSwitch) return;
        StartCoroutine(AttemptXRSwitch(true));
    }

    private void DebugExitXR()
    {
        if (_awaitingSwitch) return;
        StartCoroutine(AttemptXRSwitch(false));
    }

    private IEnumerator StartXRSystem()
    {
        yield return XRGeneralSettings.Instance.Manager.InitializeLoader();

        if (XRGeneralSettings.Instance.Manager.activeLoader == null)
        {
            _xrRuntimeError = true;
            DesktopVRSwitchMod.Logger.Error("Initializing XR Failed. OpenXR runtime is not running or headset not found!");
            yield break;
        }

        DesktopVRSwitchMod.Logger.Msg("Starting XR...");
        XRGeneralSettings.Instance.Manager.StartSubsystems();
        yield break;
    }

    private IEnumerator StopXRSystem()
    {
        if (XRGeneralSettings.Instance.Manager.isInitializationComplete)
        {
            XRGeneralSettings.Instance.Manager.StopSubsystems();
            XRGeneralSettings.Instance.Manager.DeinitializeLoader();
        }
        else
        {
            _xrRuntimeError = true;
            DesktopVRSwitchMod.Logger.Error("StopXRSystem was called before isInitializationComplete!");
        }
        yield break;
    }

    private IEnumerator AttemptXRSwitch(bool enterXR = false)
    {
        _awaitingSwitch = true;

        //start transition

        if (enterXR)
        {
            //Switch to VR
            yield return StartXRSystem();
        }
        else
        {
            //Switch to Desktop
            yield return StopXRSystem();
        }

        if (_xrRuntimeError)
        {
            _awaitingSwitch = false;
            _xrRuntimeError = false;
            DesktopVRSwitchMod.Logger.Error("Resetting DesktopVRSwitch...");
            yield break;
        }

        //disable cohtml rendering to prevent native crash
        TryCatchHell.PauseCohtmlViews(true);
        //pause basic interactions
        TryCatchHell.PauseInputInteractions(true);

        //basic chillout requirements
        TryCatchHell.SetCheckVR(enterXR);
        TryCatchHell.SetMetaPort(enterXR);

        //stuff that needs help switching
        TryCatchHell.RepositionCohtmlHud(enterXR);
        TryCatchHell.SwitchActiveCameraRigs(enterXR);
        //openxr sets fov & aspect ratio of all Camera.main cams
        TryCatchHell.ResetDesktopCamera();

        if (_reloadLocalAvatar)
        {
            //should wait until loaded to finish
            TryCatchHell.ReloadLocalAvatar();
        }

        //unpause basic interactions
        TryCatchHell.PauseInputInteractions(false);
        //unpause cohtml view rendering
        TryCatchHell.PauseCohtmlViews(false);

        //reset input manager so we can move
        TryCatchHell.ResetCVRInputManager();

        _awaitingSwitch = false;

        yield break;
    }
}

