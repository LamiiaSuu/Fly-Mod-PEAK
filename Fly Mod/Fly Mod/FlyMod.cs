using BepInEx;
using BepInEx.Logging;
using HarmonyLib;
using Photon.Pun;
using UnityEngine;
using static System.Runtime.CompilerServices.RuntimeHelpers;

[BepInPlugin("com.lamia.flymod", "FlyMod", "1.0.0")]
public class FlyMod : BaseUnityPlugin
{
    internal static ManualLogSource Log;

    private void Awake()
    {
        Log = Logger;
        Log.LogInfo("FlyMod loaded.");
        Harmony harmony = new Harmony("com.lamia.flymod");
        harmony.PatchAll();
    }
}

[HarmonyPatch(typeof(Character), "Awake")]
public static class PalThrow
{
    [HarmonyPostfix]
    public static void AwakePatch(Character __instance)
    {
        if (!__instance.IsLocal)
            return;
        if ((Object)(object)((Component)__instance).GetComponent<FlyModPatch>() == (Object)null)
        {
            ((Component)__instance).gameObject.AddComponent<FlyModPatch>();
            FlyMod.Log.LogInfo((object)("FlyModPatch added to: " + ((Object)__instance).name));
        }
    }
}

public class FlyModPatch : MonoBehaviourPun
{
    private Character character;
    private CharacterMovement charMovement;
    private float maxGravity;
    private float flyMaxGravity = -1f;
    private void Start()
    {
        character = ((Component)this).GetComponent<Character>();
        charMovement = this.GetComponent<CharacterMovement>();
        maxGravity = charMovement.maxGravity;
    }

    private void Update()
    {
        if (!character.IsLocal)

            return;
        if (Input.GetKey(KeyCode.Mouse4))
        {
            charMovement.maxGravity = flyMaxGravity;
            Vector3 flyForce = character.data.lookDirection * 100f;

            if (Input.GetKey(KeyCode.W))
                flyForce *= 2f;
            if (Input.GetKey(KeyCode.S))
                flyForce *= -1f;

            flyForce.x = Mathf.Clamp(flyForce.x, -600f, 600f);
            flyForce.y = Mathf.Clamp(flyForce.y, -600f, 600f);
            flyForce.z = Mathf.Clamp(flyForce.z, -600f, 600f);
            foreach (var part in character.refs.ragdoll.partList)
            {
                part.AddForce(flyForce, ForceMode.Force);
            }
            FlyMod.Log.LogInfo("Flew with " + flyForce);

        }
        else
        {
            charMovement.maxGravity = maxGravity;
        }

    }
}
