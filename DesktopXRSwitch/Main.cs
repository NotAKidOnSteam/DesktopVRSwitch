using MelonLoader;

/**
    I know the TryCatchHell thing might be a bit exessive, but it is
    built so if a user that happens to have access to a build I do not,
    I will have a good idea of what broke and where, and what to look out
    for when updates/experimentals release. (which has happened a few times)

    It is also just in case other mods break or tweak functionality that
    could fuck with switching. Or if they try to detect switching and break...

    The XRModeSwitchTracker system is also built so I can easily & quickly make adjustments to
    components that may or may not change between builds without breaking the rest of the mod.
**/

namespace NAK.Melons.DesktopXRSwitch;

public class DesktopXRSwitchMod : MelonMod
{
    internal const string SettingsCategory = "DesktopXRSwitch";
    internal static MelonPreferences_Category m_categoryDesktopXRSwitch;
    internal static MelonLogger.Instance Logger;

    public override void OnInitializeMelon()
    {
        Logger = LoggerInstance;
        m_categoryDesktopXRSwitch = MelonPreferences.CreateCategory(SettingsCategory);

        ApplyPatches(typeof(HarmonyPatches.PlayerSetupPatches));
        ApplyPatches(typeof(HarmonyPatches.CVRPickupObjectPatches));
        ApplyPatches(typeof(HarmonyPatches.CVRWorldPatches));
        ApplyPatches(typeof(HarmonyPatches.CameraFacingObjectPatches));
        ApplyPatches(typeof(HarmonyPatches.IKSystemPatches));
        ApplyPatches(typeof(HarmonyPatches.MovementSystemPatches));
    }

    private void ApplyPatches(Type type)
    {
        try
        {
            HarmonyInstance.PatchAll(type);
        }
        catch (Exception e)
        {
            Logger.Msg($"Failed while patching {type.Name}!");
            Logger.Error(e);
        }
    }
}