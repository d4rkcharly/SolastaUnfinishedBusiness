﻿using System;
using System.Collections.Generic;
using SolastaCommunityExpansion.Builders;
using SolastaCommunityExpansion.Builders.Features;
using SolastaModApi;
using SolastaModApi.Extensions;
using static SolastaModApi.DatabaseHelper;
using static SolastaModApi.DatabaseHelper.CharacterSubclassDefinitions;

namespace SolastaCommunityExpansion.Subclasses.Wizard
{
    internal class MasterManipulator : AbstractSubclass
    {
        private static readonly Guid SubclassNamespace = new("af7255d2-8ce2-4398-8999-f1ef536001f6");
        private readonly CharacterSubclassDefinition Subclass;

        #region DcIncreaseAffinity
        private static FeatureDefinitionMagicAffinity _dcIncreaseAffinity;
        private static FeatureDefinitionMagicAffinity DcIncreaseAffinity =>
            _dcIncreaseAffinity ??= BuildMagicAffinityModifiers(0, Main.Settings.OverrideWizardMasterManipulatorArcaneManipulationSpellDc, "MagicAffinityMasterManipulatorDC", GetSpellDCPresentation().Build());
        #endregion

        internal override FeatureDefinitionSubclassChoice GetSubclassChoiceList()
        {
            return FeatureDefinitionSubclassChoices.SubclassChoiceWizardArcaneTraditions;
        }
        internal override CharacterSubclassDefinition GetSubclass()
        {
            return Subclass;
        }

        internal MasterManipulator()
        {
            // Make Control Master subclass
            CharacterSubclassDefinitionBuilder controlMaster = CharacterSubclassDefinitionBuilder
                .Create("MasterManipulator", SubclassNamespace)
                .SetGuiPresentation("TraditionMasterManipulator", Category.Subclass, RoguishShadowCaster.GuiPresentation.SpriteReference);

            GuiPresentationBuilder arcaneControlAffinityGui = new GuiPresentationBuilder(
                "Subclass/&MagicAffinityMasterManipulatorListTitle",
                "Subclass/&MagicAffinityMasterManipulatorListDescription");
            FeatureDefinitionMagicAffinity arcaneControlAffinity = BuildMagicAffinityHeightenedList(new List<string>() {
                SpellDefinitions.CharmPerson.Name, // enchantment
                SpellDefinitions.Sleep.Name, // enchantment
                SpellDefinitions.ColorSpray.Name, // illusion
                SpellDefinitions.HoldPerson.Name, // enchantment,
                SpellDefinitions.Invisibility.Name, // illusion
                SpellDefinitions.Counterspell.Name, // abjuration
                SpellDefinitions.DispelMagic.Name, // abjuration
                SpellDefinitions.Banishment.Name, // abjuration
                SpellDefinitions.Confusion.Name, // enchantment
                SpellDefinitions.PhantasmalKiller.Name, // illusion
                SpellDefinitions.DominatePerson.Name, // Enchantment
                SpellDefinitions.HoldMonster.Name // Enchantment
            }, 1,
                "MagicAffinityControlHeightened", arcaneControlAffinityGui.Build());
            controlMaster.AddFeatureAtLevel(arcaneControlAffinity, 2);
            controlMaster.AddFeatureAtLevel(DcIncreaseAffinity, 6);

            FeatureDefinitionProficiency proficiency = FeatureDefinitionProficiencyBuilder
                .Create("ManipulatorMentalSavingThrows", SubclassNamespace)
                .SetGuiPresentation(Category.Subclass)
                .SetProficiencies(RuleDefinitions.ProficiencyType.SavingThrow, AttributeDefinitions.Charisma, AttributeDefinitions.Constitution)
                .AddToDB();

            controlMaster.AddFeatureAtLevel(proficiency, 10);

            GuiPresentationBuilder DominatePower = new GuiPresentationBuilder(
                "Subclass/&PowerManipulatorDominatePersonTitle",
                "Subclass/&PowerManipulatorDominatePersonDescription");
            DominatePower.SetSpriteReference(SpellDefinitions.DominatePerson.GuiPresentation.SpriteReference);
            FeatureDefinitionPower PowerDominate = new FeatureDefinitionPowerBuilder("PowerManipulatorDominatePerson", GuidHelper.Create(SubclassNamespace, "PowerManipulatorDominatePerson").ToString(),
                0, RuleDefinitions.UsesDetermination.AbilityBonusPlusFixed, AttributeDefinitions.Intelligence, RuleDefinitions.ActivationTime.BonusAction, 1, RuleDefinitions.RechargeRate.LongRest,
                false, false, AttributeDefinitions.Intelligence,
                SpellDefinitions.DominatePerson.EffectDescription, DominatePower.Build(), false /* unique instance */).AddToDB();
            controlMaster.AddFeatureAtLevel(PowerDominate, 14);

            Subclass = controlMaster.AddToDB();
        }

        private static GuiPresentationBuilder GetSpellDCPresentation()
        {
            return new GuiPresentationBuilder("Subclass/&MagicAffinityMasterManipulatorDCTitle", "Subclass/&MagicAffinityMasterManipulatorDC" + Main.Settings.OverrideWizardMasterManipulatorArcaneManipulationSpellDc + "Description");
        }

        public static void UpdateSpellDCBoost()
        {
            if (DcIncreaseAffinity)
            {
                DcIncreaseAffinity.SetSaveDCModifier(Main.Settings.OverrideWizardMasterManipulatorArcaneManipulationSpellDc);
                DcIncreaseAffinity.SetGuiPresentation(GetSpellDCPresentation().Build());
            }
        }

        public static FeatureDefinitionMagicAffinity BuildMagicAffinityModifiers(int attackModifier, int dcModifier, string name, GuiPresentation guiPresentation)
        {
            return FeatureDefinitionMagicAffinityBuilder
                .Create(name, SubclassNamespace)
                .SetGuiPresentation(guiPresentation)
                .SetCastingModifiers(attackModifier, dcModifier, false, false, false)
                .AddToDB();
        }

        public static FeatureDefinitionMagicAffinity BuildMagicAffinityHeightenedList(List<string> spellNames, int levelBonus, string name, GuiPresentation guiPresentation)
        {
            return FeatureDefinitionMagicAffinityBuilder
                .Create(name, SubclassNamespace)
                .SetGuiPresentation(guiPresentation)
                .SetWarList(spellNames, levelBonus)
                .AddToDB();
        }
    }
}
