using SolastaUnfinishedBusiness.Builders;
using SolastaUnfinishedBusiness.Builders.Features;
using SolastaUnfinishedBusiness.CustomBehaviors;
using SolastaUnfinishedBusiness.Models;
using static RuleDefinitions;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.FeatureDefinitionPowers;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.SpellDefinitions;
using static SolastaUnfinishedBusiness.Builders.Features.AutoPreparedSpellsGroupBuilder;

namespace SolastaUnfinishedBusiness.Subclasses;

internal sealed class DomainSmith : AbstractSubclass
{
    internal DomainSmith()
    {
        const string NAME = "DomainTwilight";

        //
        // 1 (6, 11, 16)
        //

        var autoPreparedSpellsDomainSmith = FeatureDefinitionAutoPreparedSpellsBuilder
            .Create($"AutoPreparedSpells{NAME}")
            .SetGuiPresentation("ExpandedSpells", Category.Feature)
            .SetAutoTag("Domain")
            .SetPreparedSpellGroups(
                BuildSpellGroup(1, FaerieFire, Sleep),
                BuildSpellGroup(3, MoonBeam, SeeInvisibility),
                BuildSpellGroup(5, ProtectionFromEnergy, BeaconOfHope),
                BuildSpellGroup(7, GreaterInvisibility, DeathWard),
                BuildSpellGroup(9, DispelEvilAndGood, WallOfForce))
            .SetSpellcastingClass(CharacterClassDefinitions.Cleric)
            .AddToDB();

        var bonusProficiencyDomainTwilight = FeatureDefinitionProficiencyBuilder
            .SetProficiencies(ProficiencyType.Armor, EquipmentDefinitions.HeavyArmorCategory)
            .SetProficiencies(ProficiencyType.Weapon, EquipmentDefinitions.MartialWeaponCategory)
            .AddToDB();

        var twilightVisionFeature = FeatureDefinitionFeatureSetBuilder
            .Create(SenseDarkvision24, $"FeatureSet{NAME}SuperiorSight")
            .SetGuiPresentation(Category.Feature)
            .AddFeatureSet(SenseDarkvision24)
            .AddToDB();
        
        var powerTwilightSharedVision = FeatureDefinitionPowerBuilder
            .Create($"Power{NAME}SharedVision")
            .SetGuiPresentation(Category.Feature, Darkvision)
            .SetUsesFixed(ActivationTime.Action, RechargeRate.LongRest)
            .SetEffectDescription(EffectDescriptionBuilder
                .Create(Darkvision.EffectDescription)
                .SetTargetingData(RangeType.Distance, TargetParameter=6, TargetProximityDistance=10)
                .SetDurationData(DurationType.Hour, 1)
                .Build())
            .SetUniqueInstance()
            .AddToDB();

         var powerTwilightWatcherBlessing = FeatureDefinitionPowerBuilder
            .Create($"Power{NAME}WatcherBlessing")
            .SetGuiPresentation(Category.Feature, Guidance)
            .AddFeatureSet(
                FeatureDefinitionCombatAffinitys.CombatAffinityEagerForBattle)
            .SetCustomSubFeatures(
                DoNotTerminateWhileUnconscious.Marker,
                ExtraCarefulTrackedItem.Marker,
                SkipEffectRemovalOnLocationChange.Always)
            .SetUsesFixed(ActivationTime.Action)
            .SetEffectDescription(EffectDescriptionBuilder
                .Create(Guidance.EffectDescription)
                .SetDurationData(DurationType.Permanent)
                .Build())
            .SetUniqueInstance()
            .AddToDB();

        //
        // 2
        //

        

        //
        // 6
        //

        const string DIVINE_STRIKE_DESCRIPTION = "Feature/&AdditionalDamageDomainSmithDivineStrikeDescription";

        static string PowerDivineStrikeDescription(int x)
        {
            return Gui.Format(DIVINE_STRIKE_DESCRIPTION, x.ToString());
        }

        var additionalDamageDivineStrike6 = FeatureDefinitionAdditionalDamageBuilder
            .Create($"AdditionalDamage{NAME}DivineStrike6")
            .SetGuiPresentation($"AdditionalDamage{NAME}DivineStrike", Category.Feature,
                PowerDivineStrikeDescription(1))
            .SetNotificationTag("DivineStrike")
            .SetSpecificDamageType(DamageTypeFire)
            .SetDamageDice(DieType.D8, 1)
            .SetAdvancement(AdditionalDamageAdvancement.ClassLevel, 1, 1, 8, 6)
            .SetFrequencyLimit(FeatureLimitedUsage.OnceInMyTurn)
            .SetAttackModeOnly()
            .AddToDB();

        var additionalDamageDivineStrike14 = FeatureDefinitionBuilder
            .Create($"AdditionalDamage{NAME}DivineStrike14")
            .SetGuiPresentation($"AdditionalDamage{NAME}DivineStrike", Category.Feature,
                PowerDivineStrikeDescription(2))
            .AddToDB();

        //
        // 8
        //

        var attributeModifierForgeMastery = FeatureDefinitionAttributeModifierBuilder
            .Create($"AttributeModifier{NAME}ForgeMastery")
            .SetGuiPresentation(Category.Feature)
            .SetSituationalContext(SituationalContext.WearingArmor)
            .SetModifier(FeatureDefinitionAttributeModifier.AttributeModifierOperation.Additive,
                AttributeDefinitions.ArmorClass,
                1)
            .AddToDB();

        var damageAffinityForgeMastery = FeatureDefinitionDamageAffinityBuilder
            .Create(FeatureDefinitionDamageAffinitys.DamageAffinityFireResistance, "DamageAffinityForgeMastery")
            .SetDamageAffinityType(DamageAffinityType.Resistance)
            .SetDamageType(DamageTypeFire)
            .AddToDB();

        Subclass = CharacterSubclassDefinitionBuilder
            .Create(NAME)
            .SetGuiPresentation(Category.Subclass, FightingStyleDefinitions.GreatWeapon)
            .AddFeaturesAtLevel(1, autoPreparedSpellsDomainSmith, bonusProficiencyDomainForge, powerReinforceArmor1)
            .AddFeaturesAtLevel(2, powerAdamantBenediction)
            .AddFeaturesAtLevel(6, additionalDamageDivineStrike6, powerReinforceArmor6)
            .AddFeaturesAtLevel(8, attributeModifierForgeMastery, damageAffinityForgeMastery)
            .AddFeaturesAtLevel(10, PowerClericDivineInterventionPaladin)
            .AddFeaturesAtLevel(11, powerReinforceArmor11)
            .AddFeaturesAtLevel(14, additionalDamageDivineStrike14)
            .AddFeaturesAtLevel(16, powerReinforceArmor16)
            .AddToDB();
    }

    internal override CharacterSubclassDefinition Subclass { get; }

    // ReSharper disable once UnassignedGetOnlyAutoProperty
    internal override FeatureDefinitionSubclassChoice SubclassChoice { get; }
    internal override DeityDefinition DeityDefinition => DeityDefinitions.Pakri;

    private static bool CanArmorBeReinforced(RulesetCharacter character, RulesetItem item)
    {
        var definition = item.ItemDefinition;

        if (!definition.IsArmor || !character.IsProficientWithItem(definition))
        {
            return false;
        }

        return !definition.Magical;
    }
}
