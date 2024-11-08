using System;
using BepInEx;
using HarmonyLib;
using UnityEngine;

namespace MagicTips
{


    [BepInPlugin("elin.weaselofdeath.MagicTips", "MagicTips", "0.1")]
    public class Main : BaseUnityPlugin
    {
        public void Awake()
        {
            Harmony.CreateAndPatchAll(typeof(MagicTipsPatch), null);
        }
    }

    public class MagicTipsPatch
    {

        [HarmonyPostfix]
        [HarmonyPatch(typeof(Thing), "WriteNote")]
        public static void Thing_WriteNote(ref Thing __instance, UINote n, Action<UINote> onWriteNote = null, IInspect.NoteMode mode = IInspect.NoteMode.Default, Recipe recipe = null)
        {
            if (mode != IInspect.NoteMode.Recipe && __instance.IsIdentified)
            {
                FontColor fontColor = __instance.IsCursed ? FontColor.Bad : FontColor.Great;
                fontColor = __instance.IsBlessed ? FontColor.Myth : fontColor;
                // Switched from trait type matching to string matching since Unity gives false positives on the third case for letters and non spell books
                if (__instance.trait.ToString() == "TraitSpellbook" || __instance.trait.ToString() is "TraitSpellbookRandom")
                {
                    TraitSpellbook traitSpellbook = __instance.trait as TraitSpellbook;
                    AddTextForMagic(n, traitSpellbook.Name, ref __instance, traitSpellbook.source.id, fontColor);
                }
                else if (__instance.trait.ToString() == "TraitRod" || __instance.trait.ToString() == "TraitRodRandom")
                {
                    TraitRod traitRod = __instance.trait as TraitRod;
                    AddTextForMagic(n, traitRod.Name, ref __instance, traitRod.source.id, fontColor);
                }
                else if (__instance.trait.ToString() == "TraitScroll" || __instance.trait.ToString() == "TraitScrollRandom")
                {
                    TraitScroll traitScroll = __instance.trait as TraitScroll;
                    AddTextForMagic(n, traitScroll.Name, ref __instance, traitScroll.source.id, fontColor);
                }
            }
        }

        public static void AddTextForMagic(UINote n, string spellName, ref Thing __instance, int idRef, FontColor fontColor)
        {
            n.AddText("["+ spellName+"]", fontColor);
            n.Space(1, 1);
            Element orCreateElement2 = __instance.elements.GetOrCreateElement(idRef);
            if (orCreateElement2.ValueWithoutLink == 0)
            {
                __instance.elements.ModBase(orCreateElement2.id, 1);
            }
            n.AddText(orCreateElement2.GetDetail(), FontColor.DontChange);
        }
    }
}
