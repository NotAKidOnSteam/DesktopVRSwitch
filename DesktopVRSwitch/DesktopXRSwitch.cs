using ABI_RC.Core.Player;
using ABI_RC.Core.Savior;
using ABI_RC.Systems.IK;
using ABI_RC.Systems.MovementSystem;
using NAK.Melons.DesktopVRSwitch.Patches;
using System.Collections;
using UnityEngine;
using UnityEngine.XR.Management;

//toggling cameras refreshes aspect ratio
//it is likely best to switch the active cameras first before calling StartXR

namespace NAK.Melons.DesktopVRSwitch;

public class DesktopXRSwitch : MonoBehaviour
{
    //Settings
    public bool _reloadLocalAvatar = true;

    //Internal Stuff
    private bool _switchInProgress = false;
    private Vector3 _initialSwitchPosition;
    private Quaternion _initialSwitchRotation;

    void Start()
    {
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

    public bool IsInXR() => CheckVR.Instance.hasVrDeviceLoaded;

    private IEnumerator StartXRSystem()
    {
        BeforeXRModeSwitch(true);
        yield return null;
        yield return XRGeneralSettings.Instance.Manager.InitializeLoader();
        if (XRGeneralSettings.Instance.Manager.activeLoader == null)
        {
            DesktopVRSwitchMod.Logger.Error("Initializing XR Failed. Check Editor or Player log for details.");
        }
        else
        {
            DesktopVRSwitchMod.Logger.Msg("Starting XR...");
            XRGeneralSettings.Instance.Manager.StartSubsystems();
            yield return null;
            AfterXRModeSwitch(true);
        }
        yield break;
    }

    private IEnumerator StopXR()
    {
        BeforeXRModeSwitch(true);
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

    public void BeforeXRModeSwitch(bool enterXR)
    {
        _initialSwitchPosition = PlayerSetup.Instance._avatar.transform.position;
        _initialSwitchRotation = PlayerSetup.Instance._avatar.transform.rotation;
    }

    public void AfterXRModeSwitch(bool enterXR)
    {
        TryCatchHell.SetCheckVR(enterXR);
        TryCatchHell.SetMetaPort(enterXR);
        TryCatchHell.RepositionCohtmlHud(enterXR);
        TryCatchHell.UpdateHudOperations(enterXR);
        TryCatchHell.DisableMirrorCanvas();
        TryCatchHell.SwitchActiveCameraRigs(enterXR);
        TryCatchHell.ResetCVRInputManager();
        TryCatchHell.UpdateRichPresence();
        TryCatchHell.UpdateGestureReconizerCam();

        //custom patch script to swap pickup grips
        CVRPickupObjectTracker.OnVRModeSwitch();

        //lazy way of correcting Desktop & VR offset issue
        MovementSystem.Instance.TeleportToPosRot(_initialSwitchPosition, _initialSwitchRotation, false);
        if (!enterXR) MovementSystem.Instance.UpdateColliderCenter(MovementSystem.Instance.transform.position);
        IKSystem.Instance.ResetIK();

        //reload avatar by default, optional for debugging
        if (_reloadLocalAvatar)
        {
            TryCatchHell.ReloadLocalAvatar();
        }

        _switchInProgress = false;
    }
}

