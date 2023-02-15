using ABI.CCK.Components;
using ABI_RC.Core.Base;
using ABI_RC.Core.Player;
using ABI_RC.Core.Savior;
using HarmonyLib;
using NAK.Melons.DesktopVRSwitch.Patches;
using UnityEngine;

namespace NAK.Melons.DesktopVRSwitch.HarmonyPatches;

internal class PlayerSetupPatches
{
    [HarmonyPostfix]
    [HarmonyPatch(typeof(PlayerSetup), "Start")]
    private static void Postfix_PlayerSetup_Start()
    {
        CheckVR.Instance.gameObject.AddComponent<DesktopXRSwitch>();
    }
}

internal class CVRPickupObjectPatches
{
    [HarmonyPrefix]
    [HarmonyPatch(typeof(CVRPickupObject), "Start")]
    private static void Prefix_CVRPickupObject_Start(ref CVRPickupObject __instance)
    {
        Transform vrOrigin = __instance.gripOrigin;
        Transform desktopOrigin = __instance.gripOrigin.Find("[Desktop]");
        if (vrOrigin != null && desktopOrigin != null)
        {
            var tracker = __instance.AddComponentIfMissing<CVRPickupObjectTracker>();
            tracker.pickupObject = __instance;
            CVRPickupObjectTracker.StoreInitialGripOrigin(__instance, (!MetaPort.Instance.isUsingVr) ? vrOrigin : desktopOrigin);
        }
    }
}

internal class CVRWorldPatches
{
    [HarmonyPostfix]
    [HarmonyPatch(typeof(CVRWorld), "SetDefaultCamValues")]
    private static void CVRWorld_SetDefaultCamValues_Postfix()
    {
        ReferenceCameraFix.OnWorldLoad();
    }

    [HarmonyPostfix]
    [HarmonyPatch(typeof(CVRWorld), "CopyRefCamValues")]
    private static void CVRWorld_CopyRefCamValues_Postfix()
    {
        ReferenceCameraFix.OnWorldLoad();
    }
}