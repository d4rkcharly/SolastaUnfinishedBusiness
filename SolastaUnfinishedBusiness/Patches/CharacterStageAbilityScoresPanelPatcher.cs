﻿using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Reflection.Emit;
using HarmonyLib;
using SolastaUnfinishedBusiness.Api.Helpers;
using SolastaUnfinishedBusiness.Models;

namespace SolastaUnfinishedBusiness.Patches;

public static class CharacterStageAbilityScoresPanelPatcher
{
    //PATCH: extends the cost buy table to enable `EpicPointsAndArray`
    [HarmonyPatch(typeof(CharacterStageAbilityScoresPanel), "Reset")]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    public static class Reset_Patch
    {
        public static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
        {
            if (!Main.Settings.EnableEpicPointsAndArray)
            {
                return instructions;
            }

            return instructions.ReplaceAllCode(
                instruction => instruction.opcode == OpCodes.Ldc_I4_S &&
                               instruction.operand.ToString() == CharacterContext.GameBuyPoints.ToString(),
                new CodeInstruction(OpCodes.Ldc_I4_S, CharacterContext.ModBuyPoints));
        }
    }

    //PATCH: extends the cost buy table to enable `EpicPointsAndArray`
    [HarmonyPatch(typeof(CharacterStageAbilityScoresPanel), "Refresh")]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    public static class Refresh_Patch
    {
        public static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
        {
            if (Main.Settings.EnableEpicPointsAndArray)
            {
                return instructions;
            }

            return instructions
                .ReplaceAllCode(instruction => instruction.opcode == OpCodes.Ldc_R4 && instruction.operand.ToString() ==
                        CharacterContext.GameBuyPoints.ToString(),
                    new CodeInstruction(OpCodes.Ldc_R4, 1f * CharacterContext.ModBuyPoints))
                .ReplaceAllCode(instruction => instruction.opcode == OpCodes.Ldc_I4_S &&
                                               instruction.operand.ToString() ==
                                               CharacterContext.GameBuyPoints.ToString(),
                    new CodeInstruction(OpCodes.Ldc_I4_S, CharacterContext.ModBuyPoints))
                .ReplaceAllCode(instruction => instruction.opcode == OpCodes.Ldc_I4_S &&
                                               instruction.operand.ToString() ==
                                               CharacterContext.GameMaxAttribute.ToString(),
                    new CodeInstruction(OpCodes.Ldc_I4_S, CharacterContext.ModMaxAttribute));
        }
    }
}
