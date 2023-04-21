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
    internal DomainTwilight()
    {
        const string NAME = "DomainTwilight";

        //
        // 1
        //

        var autoPreparedSpellsDomainTwilight = FeatureDefinitionAutoPreparedSpellsBuilder
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
                .SetTargetingData(RangeType.Distance, TargetParameter=5, TargetProximityDistance=10)
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
        // 2,17
        //

        var powerTwilightSanctuary = FeatureDefinitionPowerBuilder
            .Create("PowerClericTwilightSanctuary")
            .SetGuiPresentation(ShieldOfFaith)
            .AddToDB();


        

        //
        // 6
        //

        var powerStepsOfTheNight = FeatureDefinitionPowerBuilder
            .Create($"Power{NAME}StepsOfTheNight")
            .SetGuiPresentation(Category.Feature, Fly)
            .SetUsesProficiencyBonus(ActivationTime.BonusAction)
            .SetEffectDescription(EffectDescriptionBuilder
                .Create()
                .SetTargetingData(Side.Ally, RangeType.Self, 0, TargetType.Self)
                .SetDurationData(DurationType.Minute, 1)
                .SetEffectForms(EffectFormBuilder
                    .Create()
                    .SetConditionForm(
                        DatabaseHelper.ConditionDefinitions.ConditionFlying,
                        ConditionForm.ConditionOperation.Add)
                    .Build())
                .Build())
            .AddToDB();

        //
        // 8,14
        //

        const string DIVINE_STRIKE_DESCRIPTION = "Feature/&AdditionalDamageDomainTwilightDivineStrikeDescription";

        static string PowerDivineStrikeDescription(int x)
        {
            return Gui.Format(DIVINE_STRIKE_DESCRIPTION, x.ToString());
        }

        var additionalDamageDivineStrike8 = FeatureDefinitionAdditionalDamageBuilder
            .Create($"AdditionalDamage{NAME}DivineStrike6")
            .SetGuiPresentation($"AdditionalDamage{NAME}DivineStrike", Category.Feature,
                PowerDivineStrikeDescription(1))
            .SetNotificationTag("DivineStrike")
            .SetSpecificDamageType(DamageTypeRadiant)
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
        //17
        //

        var powerTwilightShroud = FeatureDefinitionPowerBuilder
            .Create($"Power{NAME}TwilightShroud")
            .SetGuiPresentation(Category.Feature, powerTwilightSanctuary)
            .SetUsesFixed(ActivationTime.Action, RechargeRate.ChannelDivinity)
            .SetOverriddenPower(powerTwilightSanctuary)
            .SetEffectDescription(EffectDescriptionBuilder
                .Create(powerTwilightSanctuary.EffectDescription)
                .SetEffectForms(ShieldOfFaith)
                .Build())
            .SetUniqueInstance()
            .AddToDB();

        Subclass = CharacterSubclassDefinitionBuilder
            .Create(NAME)
            .SetGuiPresentation(Category.Subclass, FightingStyleDefinitions.GreatWeapon)
            .AddFeaturesAtLevel(1, autoPreparedSpellsDomainTwilight, bonusProficiencyDomainTwilight,
             powerTwilightWatcherBlessing, twilightVisionFeature, powerTwilightSharedVision)
            .AddFeaturesAtLevel(2, powerTwilightSanctuary)
            .AddFeaturesAtLevel(6, powerStepsOfTheNight)
            .AddFeaturesAtLevel(8, additionalDamageDivineStrike8)
            .AddFeaturesAtLevel(10, PowerClericDivineInterventionPaladin)
            .AddFeaturesAtLevel(14, additionalDamageDivineStrike14)
            .AddFeaturesAtLevel(17, powerTwilightShroud)
            .AddToDB();
    }

    internal override CharacterSubclassDefinition Subclass { get; }

    // ReSharper disable once UnassignedGetOnlyAutoProperty
    internal override FeatureDefinitionSubclassChoice SubclassChoice { get; }
    internal override DeityDefinition DeityDefinition => DeityDefinitions.Pakri;
}
