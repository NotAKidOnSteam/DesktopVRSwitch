using ABI_RC.Core;
using ABI_RC.Core.EventSystem;
using ABI_RC.Core.InteractionSystem;
using ABI_RC.Core.Player;
using ABI_RC.Core.Savior;
using ABI_RC.Core.UI;
using ABI_RC.Core.Util.Object_Behaviour;
using ABI_RC.Systems.Camera;
using ABI_RC.Systems.IK.SubSystems;
using ABI_RC.Systems.MovementSystem;
using HarmonyLib;
using MelonLoader;
using System.Collections;
using UnityEngine;
using UnityEngine.XR;
using Valve.VR;
using Object = UnityEngine.Object;

namespace NAK.Melons.DesktopVRSwitch;

internal class TryCatchHell
{
    private static void TryCatchWrapper(Action action, string errorMsg, params object[] msgArgs)
    {
        try
        {
            action();
        }
        catch (Exception ex)
        {
            DesktopVRSwitchMod.Logger.Error(string.Format(errorMsg, msgArgs));
            DesktopVRSwitchMod.Logger.Msg(ex.Message);
        }
    }

    internal static void SwitchActiveCameraRigs(bool isVR)
    {
        TryCatchWrapper(() =>
        {
            DesktopVRSwitchMod.Logger.Msg("Switched active camera rigs.");
            PlayerSetup.Instance.desktopCameraRig.SetActive(!isVR);
            PlayerSetup.Instance.vrCameraRig.SetActive(isVR);
        },
        "Error switching active cameras. Are the camera rigs invalid?");
    }

    internal static void RepositionCohtmlHud(bool isVR)
    {
        TryCatchWrapper(() =>
        {
            DesktopVRSwitchMod.Logger.Msg("Parented CohtmlHud to active camera.");
            CohtmlHud.Instance.gameObject.transform.parent = isVR ? PlayerSetup.Instance.vrCamera.transform : PlayerSetup.Instance.desktopCamera.transform;
            CVRTools.ConfigureHudAffinity();
            CohtmlHud.Instance.gameObject.transform.localScale = new Vector3(1.2f, 1f, 1.2f);
        },
        "Error parenting CohtmlHud to active camera. Is CohtmlHud.Instance invalid?");
    }

    internal static void PauseInputInteractions(bool toggle)
    {
        TryCatchWrapper(() =>
        {
            DesktopVRSwitchMod.Logger.Msg($"Toggling input & interactions to " + !toggle);
            CVRInputManager.Instance.inputEnabled = !toggle;
            CVR_InteractableManager.enableInteractions = !toggle;
        },
        "Toggling input & interactions failed. Is something invalid?");
    }

    internal static void ResetCVRInputManager()
    {
        TryCatchWrapper(() =>
        {
            DesktopVRSwitchMod.Logger.Msg("Enabling CVRInputManager inputEnabled & disabling blockedByUi!");
            //just in case
            CVRInputManager.Instance.blockedByUi = false;
            //sometimes head can get stuck, so just in case
            CVRInputManager.Instance.independentHeadToggle = false;
            //just nice to load into desktop with idle gesture
            CVRInputManager.Instance.gestureLeft = 0f;
            CVRInputManager.Instance.gestureLeftRaw = 0f;
            CVRInputManager.Instance.gestureRight = 0f;
            CVRInputManager.Instance.gestureRightRaw = 0f;
        },
        "Toggling input & interactions failed. Is something invalid?");
    }

    internal static void SetCheckVR(bool isVR)
    {
        TryCatchWrapper(() =>
        {
            DesktopVRSwitchMod.Logger.Msg($"Set CheckVR hasVrDeviceLoaded to {isVR}.");
            CheckVR.Instance.hasVrDeviceLoaded = isVR;
        },
        "Setting CheckVR hasVrDeviceLoaded failed. Is CheckVR.Instance invalid?");
    }

    internal static void SetMetaPort(bool isVR)
    {
        TryCatchWrapper(() =>
        {
            DesktopVRSwitchMod.Logger.Msg($"Set MetaPort isUsingVr to {isVR}.");
            MetaPort.Instance.isUsingVr = isVR;
        },
        "Setting MetaPort isUsingVr failed. Is MetaPort.Instance invalid?");
    }

    internal static void ReloadLocalAvatar()
    {
        TryCatchWrapper(() =>
        {
            DesktopVRSwitchMod.Logger.Msg("Attempting to reload current local avatar from GUID.");
            AssetManagement.Instance.LoadLocalAvatar(MetaPort.Instance.currentAvatarGuid);
        },
        "Failed to reload local avatar. Is MetaPort.Instance or AssetManagment.Instance invalid?");
    }

    internal static void PauseCohtmlViews(bool toggle)
    {
        TryCatchWrapper(() =>
        {
            DesktopVRSwitchMod.Logger.Msg($"Temperarily murdering Cohtml...");

            ViewManager.Instance.quickMenuView.View.EnableRendering(toggle);
            ViewManager.Instance.quickMenuView.View.ContinuousRepaint(toggle);
            ViewManager.Instance.gameMenuView.View.EnableRendering(toggle);
            ViewManager.Instance.gameMenuView.View.ContinuousRepaint(toggle);
        },
        "Failed to murder cohtml. The cohtmlviews are likely invalid.");
    }

    internal static void ResetDesktopCamera()
    {
        TryCatchWrapper(() =>
        {
            DesktopVRSwitchMod.Logger.Msg("Resetting Desktop Camera...");
            CVR_DesktopCameraController.UpdateFov();
            Camera camera = PlayerSetup.Instance.desktopCamera.GetComponent<Camera>();
            camera.ResetAspect();
        },
        "Failed to reset desktop camera... PlayerSetup.Instance.desktopCamera might be doomed.");
    }

}

