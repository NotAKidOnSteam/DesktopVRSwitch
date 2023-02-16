using ABI_RC.Core.Player;
using UnityEngine;

namespace NAK.Melons.DesktopXRSwitch.Patches;

public class VRModeSwitchTracker : MonoBehaviour
{
    public static List<VRModeSwitchTracker> allTrackedComponents = new List<VRModeSwitchTracker>();
    public static void PostVRModeSwitch()
    {
        Camera activeCamera = PlayerSetup.Instance.GetActiveCamera().GetComponent<Camera>();
        for (int i = 0; i < allTrackedComponents.Count; i++)
        {
            allTrackedComponents[i]?.PostVRModeSwitch(activeCamera);
        }
    }

    void Awake()
    {
        allTrackedComponents.Add(this);
    }

    public virtual void PostVRModeSwitch(Camera activeCamera)
    {
    }

    public virtual void OnDestroy()
    {
        allTrackedComponents.Remove(this);
    }
}