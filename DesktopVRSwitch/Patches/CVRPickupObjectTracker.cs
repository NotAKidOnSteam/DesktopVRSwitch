using ABI.CCK.Components;
using UnityEngine;

//Thanks Ben! I was scared of transpiler so I reworked a bit...

namespace NAK.Melons.DesktopVRSwitch.Patches;

public class CVRPickupObjectTracker : MonoBehaviour
{
    public static List<CVRPickupObjectTracker> allTrackedObjects = new List<CVRPickupObjectTracker>();
    public static Dictionary<CVRPickupObject, Transform> storedGripOrigins = new();

    public CVRPickupObject pickupObject;

    public static void StoreInitialGripOrigin(CVRPickupObject pickupObject, Transform otherOrigin)
    {
        if (!storedGripOrigins.ContainsKey(pickupObject))
        {
            storedGripOrigins.Add(pickupObject, otherOrigin);
        }
    }

    public static void OnVRModeSwitch()
    {
        for (int i = 0; i < allTrackedObjects.Count; i++)
        {
            allTrackedObjects[i].OnSwitch();
        }
    }

    private void Awake()
    {
        allTrackedObjects.Add(this);
    }

    public void OnSwitch()
    {
        if (pickupObject != null)
        {
            if (pickupObject._controllerRay != null) pickupObject._controllerRay.DropObject(true);
            (storedGripOrigins[pickupObject], pickupObject.gripOrigin) = (pickupObject.gripOrigin, storedGripOrigins[pickupObject]);
        }
    }

    private void OnDestroy()
    {
        allTrackedObjects.Remove(this);
        if (storedGripOrigins.ContainsKey(pickupObject))
        {
            storedGripOrigins.Remove(pickupObject);
        }
    }
}