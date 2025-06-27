using BepInEx;
using BepInEx.Configuration;
using BepInEx.Logging;
using HarmonyLib;
using Photon.Pun;
using UnityEngine;
using static System.Runtime.CompilerServices.RuntimeHelpers;

[BepInPlugin("com.lamia.flymod", "FlyMod", "1.0.0")]
public class FlyMod : BaseUnityPlugin
{
    internal static ConfigEntry<float> BaseForce;
    internal static ConfigEntry<float> VerticalForce;
    internal static ConfigEntry<float> SprintMultiplier;
    internal static ConfigEntry<float> GravityPull;
    internal static ConfigEntry<float> MaxClamp;
    internal static ConfigEntry<KeyCode> FlyKey;
    internal static ConfigEntry<bool> ToggleFly;
    internal static ManualLogSource Log;

    private void Awake()
    {
        Log = Logger;
        Log.LogInfo("FlyMod loaded.");
        Harmony harmony = new Harmony("com.lamia.flymod");
        harmony.PatchAll();
        FlyKey = Config.Bind("FlyMod", "FlyKey", KeyCode.V, "Key to activate flying (default: Mouse3/Middle mouse button).");
        ToggleFly = Config.Bind("FlyMod", "ToggleFly", true, "If true, fly is toggled on/off with the key instead of holding.");
        BaseForce = Config.Bind("FlyMod", "BaseForce", 800f, "Base force applied when flying forward/backward/sideways.");
        VerticalForce = Config.Bind("FlyMod", "VerticalForce", 800f, "Force applied when flying up or down.");
        SprintMultiplier = Config.Bind("FlyMod", "SprintMultiplier", 4f, "Multiplier when holding shift.");
        MaxClamp = Config.Bind("FlyMod", "MaxClamp", 3600f, "Clamp for maximum fly force in any direction.");

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
    // private float maxGravity;
    // private float flyMaxGravity = -1f;
    private bool flyActive = false;

    private void Start()
    {
        character = ((Component)this).GetComponent<Character>();
        charMovement = this.GetComponent<CharacterMovement>();
        // maxGravity = charMovement.maxGravity;
    }

    private void Update()
    {
        if (!character.IsLocal)

            return;
        if (FlyMod.ToggleFly.Value)
        {
            if (Input.GetKeyDown(FlyMod.FlyKey.Value))
            {
                flyActive = !flyActive;
            }

        }
        if (Input.GetKey(FlyMod.FlyKey.Value) || flyActive)
        {
            // charMovement.maxGravity = flyMaxGravity;
            character.data.isGrounded = true;


            Vector3 flyForce = character.data.lookDirection;

            if (Input.GetKey(KeyCode.W))
                flyForce *= FlyMod.VerticalForce.Value;

            else if (Input.GetKey(KeyCode.S))
                flyForce *= -FlyMod.VerticalForce.Value;


            Vector3 right = Vector3.Cross(Vector3.up, character.data.lookDirection).normalized * FlyMod.VerticalForce.Value;

            if (Input.GetKey(KeyCode.D))
                flyForce += right; 

            if (Input.GetKey(KeyCode.A))
                flyForce -= right; 


            if (Input.GetKey(KeyCode.Space))
                flyForce += Vector3.up * FlyMod.VerticalForce.Value;


            else if (Input.GetKey(KeyCode.LeftControl))
                flyForce += Vector3.down * FlyMod.VerticalForce.Value;





            if (Input.GetKey(KeyCode.LeftShift))
            {
                character.AddStamina(charMovement.sprintStaminaUsage * Time.deltaTime);
                flyForce *= FlyMod.SprintMultiplier.Value;
            }


            flyForce += Vector3.down * 100f;

            flyForce.x = Mathf.Clamp(flyForce.x, -FlyMod.MaxClamp.Value, FlyMod.MaxClamp.Value);
            flyForce.y = Mathf.Clamp(flyForce.y, -FlyMod.MaxClamp.Value, FlyMod.MaxClamp.Value);
            flyForce.z = Mathf.Clamp(flyForce.z, -FlyMod.MaxClamp.Value, FlyMod.MaxClamp.Value);
            foreach (var part in character.refs.ragdoll.partList)
            {
                //if (dirInput)
                    part.AddForce(flyForce, ForceMode.Force);

            }
            FlyMod.Log.LogInfo("Flew with " + flyForce);

        }
        else
        {
            // charMovement.maxGravity = maxGravity;
        }

    }
}
