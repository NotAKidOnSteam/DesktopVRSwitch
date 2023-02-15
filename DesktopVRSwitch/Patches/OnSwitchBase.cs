using ABI_RC.Core.Player;
using UnityEngine;

namespace NAK.Melons.DesktopXRSwitch.Patches;

public class VRModeSwitchTracker : MonoBehaviour
{
    public static List<VRModeSwitchTracker> allTrackedObjects = new List<VRModeSwitchTracker>();
    public static void OnVRModeSwitch()
    {
        Camera activeCamera = PlayerSetup.Instance.GetActiveCamera().GetComponent<Camera>();
        for (int i = 0; i < allTrackedObjects.Count; i++)
        {
            allTrackedObjects[i]?.OnSwitch(activeCamera);
        }
    }

    void Awake()
    {
        allTrackedObjects.Add(this);
    }

    public virtual void OnSwitch(Camera activeCamera)
    {
    }

    public virtual void OnDestroy()
    {
        allTrackedObjects.Remove(this);
    }
}