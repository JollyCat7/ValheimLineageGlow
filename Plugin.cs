using BepInEx;
using HarmonyLib;
using UnityEngine;

namespace LineageGlowMod
{
    [BepInPlugin("com.jollycat.lineageglow", "Lineage 2 Style Weapon Glow", "1.0.0")]
    public class LineageGlowPlugin : BaseUnityPlugin
    {
        private readonly Harmony harmony = new Harmony("com.jollycat.lineageglow");

        void Awake()
        {
            harmony.PatchAll();
        }

        [HarmonyPatch(typeof(VisEquipment), "AttachItem")]
        public static class VisEquipment_AttachItem_Patch
        {
            static void Postfix(GameObject __result, int itemDataQuality)
            {
                if (__result == null) return;

                Color glowColor;
                float intensity;

                if (itemDataQuality <= 15)
                {
                    float progress = Mathf.Clamp01((float)itemDataQuality / 15f);
                    glowColor = Color.Lerp(new Color(0.1f, 0.3f, 1f), new Color(0f, 0.5f, 1f), progress);
                    intensity = 1.0f + (progress * 3.5f);
                }
                else
                {
                    glowColor = new Color(1f, 0.1f, 0.1f);
                    intensity = 5.0f + ((itemDataQuality - 15) * 0.5f);
                }

                Renderer[] renderers = __result.GetComponentsInChildren<Renderer>();
                foreach (var renderer in renderers)
                {
                    foreach (var mat in renderer.materials)
                    {
                        if (mat.HasProperty("_EmissionColor"))
                        {
                            mat.EnableKeyword("_EMISSION");
                            mat.SetColor("_EmissionColor", glowColor * intensity);
                        }
                    }
                }

                Light[] lights = __result.GetComponentsInChildren<Light>();
                foreach (var light in lights)
                {
                    light.color = glowColor;
                    light.intensity = intensity;
                }
            }
        }
    }
}
