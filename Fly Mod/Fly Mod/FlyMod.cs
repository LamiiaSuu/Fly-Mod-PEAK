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
    // private float maxGravity;
    // private float flyMaxGravity = -1f;
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
        if (Input.GetKey(KeyCode.Mouse3))
        {
            // charMovement.maxGravity = flyMaxGravity;
            character.data.isGrounded = true;


            Vector3 flyForce = character.data.lookDirection;

            if (Input.GetKey(KeyCode.W))
                flyForce *= 800f;

            else if (Input.GetKey(KeyCode.S))
                flyForce *= -800f;


            Vector3 right = Vector3.Cross(Vector3.up, character.data.lookDirection).normalized * 800f;

            if (Input.GetKey(KeyCode.D))
                flyForce += right; 

            if (Input.GetKey(KeyCode.A))
                flyForce -= right; 


            if (Input.GetKey(KeyCode.Space))
                flyForce += Vector3.up * 800f;


            else if (Input.GetKey(KeyCode.LeftControl))
                flyForce += Vector3.down * 800f;





            if (Input.GetKey(KeyCode.LeftShift))
            {
                character.AddStamina(charMovement.sprintStaminaUsage * Time.deltaTime);
                flyForce *= 4f;
            }


            flyForce += Vector3.down * 100f;

            flyForce.x = Mathf.Clamp(flyForce.x, -3600f, 3600f);
            flyForce.y = Mathf.Clamp(flyForce.y, -3600f, 3600f);
            flyForce.z = Mathf.Clamp(flyForce.z, -3600f, 3600f);
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
