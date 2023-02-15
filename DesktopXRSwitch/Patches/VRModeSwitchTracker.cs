using ABI_RC.Core.Player;
using UnityEngine;

namespace NAK.Melons.DesktopXRSwitch.Patches;

public class VRModeSwitchTracker : MonoBehaviour
{
    public static List<VRModeSwitchTracker> allTrackedComponents = new List<VRModeSwitchTracker>();
    public static void OnVRModeSwitch()
    {
        Camera activeCamera = PlayerSetup.Instance.GetActiveCamera().GetComponent<Camera>();
        for (int i = 0; i < allTrackedComponents.Count; i++)
        {
            allTrackedComponents[i]?.OnSwitch(activeCamera);
        }
    }

    void Awake()
    {
        allTrackedComponents.Add(this);
    }

    public virtual void OnSwitch(Camera activeCamera)
    {
    }

    public virtual void OnDestroy()
    {
        allTrackedComponents.Remove(this);
    }
}