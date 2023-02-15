using ABI_RC.Core.Player;
using ABI_RC.Core.Savior;
using HarmonyLib;
using RootMotion.FinalIK;
using UnityEngine;
using cohtml.Net;

namespace NAK.Melons.DesktopVRSwitch.HarmonyPatches;

internal class PlayerSetupPatches
{
    [HarmonyPostfix]
    [HarmonyPatch(typeof(PlayerSetup), "Start")]
    private static void Postfix_PlayerSetup_AutoDetectReferences(ref PlayerSetup __instance)
    {
        __instance.gameObject.AddComponent<DesktopVRSwitch>();
    }
}