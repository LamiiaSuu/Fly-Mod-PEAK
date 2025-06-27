using BepInEx;
using BepInEx.Configuration;
using BepInEx.Logging;
using HarmonyLib;
using Photon.Pun;
using UnityEngine;
using static System.Runtime.CompilerServices.RuntimeHelpers;

[BepInPlugin("com.lamia.flymod", "FlyMod", "1.2.1")]
public class FlyMod : BaseUnityPlugin
{
    internal static ConfigEntry<float> BaseForce;
    internal static ConfigEntry<float> VerticalForce;
    internal static ConfigEntry<float> SprintMultiplier;
    internal static ConfigEntry<float> GravityPull;
    internal static ConfigEntry<float> MaxClamp;
    internal static ConfigEntry<KeyCode> FlyKey;
    internal static ConfigEntry<bool> ToggleFly;
    internal static ConfigEntry<bool> CreativeFlyMode;
    internal static ManualLogSource Log;

    private void Awake()
    {
        Log = Logger;
        Log.LogInfo("FlyMod loaded.");
        Harmony harmony = new Harmony("com.lamia.flymod");
        harmony.PatchAll();
        CreativeFlyMode = Config.Bind("FlyMod", "CreativeFlyMode", true, "If true, flying feels more like noclipping in Garry's Mod or manuevering a 3d environment. Setting this to false makes flying feel more like a superhero or being boosted by a Jetpack.");
        FlyKey = Config.Bind("FlyMod", "FlyKey", KeyCode.V, "Key to activate flying (default: Mouse3/Middle mouse button).");
        ToggleFly = Config.Bind("FlyMod", "ToggleFly", true, "If true, fly is toggled on/off with the key instead of holding.");
        BaseForce = Config.Bind("FlyMod", "BaseForce", 800f, "Base force applied when flying forward/backward/sideways.");
        VerticalForce = Config.Bind("FlyMod", "VerticalForce", 800f, "Force applied when flying up or down.");
        SprintMultiplier = Config.Bind("FlyMod", "SprintMultiplier", 4f, "Multiplier when holding shift.");
        MaxClamp = Config.Bind("FlyMod", "MaxClamp", 4000f, "Clamp for maximum fly speed in any direction.");

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
    //private float maxGravity;
    //private float flyMaxGravity = -1f;
    private bool flyActive = false;

    private void Start()
    {
        character = ((Component)this).GetComponent<Character>();
        charMovement = this.GetComponent<CharacterMovement>();
        //maxGravity = charMovement.maxGravity;
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

                

            if (FlyMod.CreativeFlyMode.Value)
                character.data.isGrounded = true;
            else
            {
                character.data.sinceGrounded = 0.5f;
                //charMovement.maxGravity = flyMaxGravity;
            }


                Vector3 flyForce = character.data.lookDirection;

            if (Input.GetKey(KeyCode.W) || !FlyMod.CreativeFlyMode.Value)
            {
                flyForce *= FlyMod.VerticalForce.Value;
            }
                

            else if (Input.GetKey(KeyCode.S) && FlyMod.CreativeFlyMode.Value)
            {
                flyForce *= -FlyMod.VerticalForce.Value;
            }
                


            Vector3 right = Vector3.Cross(Vector3.up, character.data.lookDirection).normalized * FlyMod.VerticalForce.Value;

            if (Input.GetKey(KeyCode.D))
            {
                if (FlyMod.CreativeFlyMode.Value)
                    flyForce += right;
                else
                    flyForce += right / 8;
            }
                

            if (Input.GetKey(KeyCode.A))
            {
                if (FlyMod.CreativeFlyMode.Value)
                    flyForce -= right;
                else
                    flyForce -= right / 8;
            }
                


            if (Input.GetKey(KeyCode.Space))
            {
                if(FlyMod.CreativeFlyMode.Value)
                    flyForce += Vector3.up * FlyMod.VerticalForce.Value;
                else
                    flyForce += Vector3.up * (FlyMod.VerticalForce.Value/8);
            }
                


            else if (Input.GetKey(KeyCode.LeftControl) && FlyMod.CreativeFlyMode.Value)
            {
                flyForce += Vector3.down * FlyMod.VerticalForce.Value;
            }
                





            if (Input.GetKey(KeyCode.LeftShift))
            {
                character.AddStamina(charMovement.sprintStaminaUsage * Time.deltaTime);
                flyForce *= FlyMod.SprintMultiplier.Value;
            }

            if (FlyMod.CreativeFlyMode.Value)
                flyForce += Vector3.down * 100f;
            else
                flyForce += Vector3.up * 240f;


            flyForce.x = Mathf.Clamp(flyForce.x, -FlyMod.MaxClamp.Value, FlyMod.MaxClamp.Value);
            flyForce.y = Mathf.Clamp(flyForce.y, -FlyMod.MaxClamp.Value, FlyMod.MaxClamp.Value);
            flyForce.z = Mathf.Clamp(flyForce.z, -FlyMod.MaxClamp.Value, FlyMod.MaxClamp.Value);
            foreach (var part in character.refs.ragdoll.partList)
            {
                if (FlyMod.CreativeFlyMode.Value)
                {

                    //if (!part.partType.ToString().Contains("Leg") && !part.partType.ToString().Contains("Knee") && !part.partType.ToString().Contains("Foot") && !part.partType.ToString().Contains("Hip"))
                    //{
                        part.AddForce(flyForce, ForceMode.Force);
                    //}
                }
                    
                else
                {
                    if (!part.partType.ToString().Contains("Leg") && !part.partType.ToString().Contains("Knee") && !part.partType.ToString().Contains("Foot"))
                    {   
                        part.AddForce(flyForce / 7, ForceMode.Acceleration);
                    }
                }
                    

            }

        }
        else
        {

        }

    }
}
