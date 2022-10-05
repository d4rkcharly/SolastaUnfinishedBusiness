﻿using System.Diagnostics.CodeAnalysis;
using HarmonyLib;
using SolastaUnfinishedBusiness.Api.Extensions;
using SolastaUnfinishedBusiness.Api.Helpers;
using SolastaUnfinishedBusiness.CustomBehaviors;
using SolastaUnfinishedBusiness.CustomInterfaces;
using SolastaUnfinishedBusiness.Models;

namespace SolastaUnfinishedBusiness.Patches;

public static class RulesetEffectPowerPatcher
{
    [HarmonyPatch(typeof(RulesetEffectPower), "SaveDC", MethodType.Getter)]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    public static class SaveDC_Getter_Patch
    {
        public static void Postfix(RulesetEffectPower __instance, ref int __result)
        {
            //PATCH: allow devices have DC based on user or item summoner stats, instead of static value
            var originItem = __instance.OriginItem;

            if (originItem == null || originItem.UsableDeviceDescription.SaveDC >= 0)
            {
                return;
            }

            var user = __instance.User;
            CharacterClassDefinition classDefinition = null;
            if (originItem.UsableDeviceDescription.SaveDC == -2)
            {
                user = EffectHelpers.GetCharacterByEffectGuid(originItem.SourceSummoningEffectGuid) ?? user;
            }

            var classHolder = originItem.ItemDefinition.GetFirstSubFeatureOfType<IClassHoldingFeature>();
            if (classHolder != null)
            {
                classDefinition = classHolder.Class;
            }

            var usablePower = __instance.UsablePower;

            UsablePowersProvider.UpdateSaveDc(user, usablePower, classDefinition);
            __result = usablePower.SaveDC;
        }
    }

    [HarmonyPatch(typeof(RulesetEffectPower), "EffectDescription", MethodType.Getter)]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    public static class EffectDescription_Getter_Patch
    {
        public static void Postfix(RulesetEffectPower __instance, ref EffectDescription __result)
        {
            //PATCH: support for `ICustomMagicEffectBasedOnCaster` and `IModifySpellEffect` 
            // allowing to pick and/or tweak power effect depending on some properties of the user
            __result = CustomFeaturesContext.ModifyPowerEffect(__result, __instance);
        }
    }
}
