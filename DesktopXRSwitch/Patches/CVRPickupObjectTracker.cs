using ABI.CCK.Components;
using UnityEngine;

//Thanks Ben! I was scared of transpiler so I reworked a bit...

namespace NAK.Melons.DesktopXRSwitch.Patches;

public class CVRPickupObjectTracker : VRModeSwitchTracker
{
    public static Dictionary<CVRPickupObject, Transform> storedGripOrigins = new();

    public CVRPickupObject pickupObject;

    public static void StoreInitialGripOrigin(CVRPickupObject pickupObject, Transform otherOrigin)
    {
        if (!storedGripOrigins.ContainsKey(pickupObject))
        {
            storedGripOrigins.Add(pickupObject, otherOrigin);
        }
    }

    public override void PostVRModeSwitch(Camera activeCamera)
    {
        if (pickupObject != null)
        {
            if (pickupObject._controllerRay != null) pickupObject._controllerRay.DropObject(true);
            (storedGripOrigins[pickupObject], pickupObject.gripOrigin) = (pickupObject.gripOrigin, storedGripOrigins[pickupObject]);
        }
    }

    public override void OnDestroy()
    {
        base.OnDestroy();
        if (storedGripOrigins.ContainsKey(pickupObject))
        {
            storedGripOrigins.Remove(pickupObject);
        }
    }
}