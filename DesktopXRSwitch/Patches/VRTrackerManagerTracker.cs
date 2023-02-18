using ABI_RC.Core.Player;
using HarmonyLib;
using UnityEngine;

namespace NAK.Melons.DesktopXRSwitch.Patches;

public class VRTrackerManagerTracker : MonoBehaviour
{
    public VRTrackerManager vrTrackerManager;
    public Traverse _hasCheckedForKnucklesTraverse;
    public Traverse _posesTraverse;

    void Start()
    {
        vrTrackerManager = GetComponent<VRTrackerManager>();
        _posesTraverse = Traverse.Create(vrTrackerManager).Field("poses");
        _hasCheckedForKnucklesTraverse = Traverse.Create(vrTrackerManager).Field("hasCheckedForKnuckles");
        XRModeSwitchTracker.OnPostXRModeSwitch += PostXRModeSwitch;
    }
    void OnDestroy()
    {
        XRModeSwitchTracker.OnPostXRModeSwitch -= PostXRModeSwitch;
    }

    public void PostXRModeSwitch(bool isXR, Camera activeCamera)
    {
        //force the VRTrackerManager to reset anything its stored
        //this makes it get correct Left/Right hand if entering VR with different controllers
        //or if you restarted SteamVR and controllers are now in swapped index
        vrTrackerManager.leftHand = null;
        vrTrackerManager.rightHand = null;
        _posesTraverse.SetValue(null);
        _hasCheckedForKnucklesTraverse.SetValue(false);
    }
}