﻿using System.Collections;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using SolastaUnfinishedBusiness.Api.Extensions;
using SolastaUnfinishedBusiness.Builders;
using SolastaUnfinishedBusiness.Builders.Features;
using SolastaUnfinishedBusiness.CustomBehaviors;
using SolastaUnfinishedBusiness.CustomDefinitions;
using SolastaUnfinishedBusiness.CustomUI;
using SolastaUnfinishedBusiness.Models;
using UnityEngine;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.FeatureDefinitionConditionAffinitys;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.SpellDefinitions;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.MonsterDefinitions;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.CharacterSubclassDefinitions;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.FeatureDefinitionSenses;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.FeatureDefinitionMoveModes;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.FeatureDefinitionDamageAffinitys;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.FeatureDefinitionSummoningAffinitys;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.DecisionPackageDefinitions;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.ConditionDefinitions;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.FeatureDefinitionActionAffinitys;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.FeatureDefinitionAdditionalDamages;
using static RuleDefinitions;

namespace SolastaUnfinishedBusiness.Subclasses;

internal sealed class MartialMarshal : AbstractSubclass
{
    private const string FeatureSetMarshalKnowYourEnemyName = "FeatureSetMarshalKnowYourEnemy";

    private const string MarshalCoordinatedAttackName = "MarshalCoordinatedAttack";

    private const string EternalComradeName = "MarshalEternalComrade";

    private static readonly MonsterDefinition EternalComrade = BuildEternalComradeMonster();

    internal MartialMarshal()
    {
        Subclass = CharacterSubclassDefinitionBuilder
            .Create("MartialMarshal")
            .SetGuiPresentation(Category.Subclass, OathOfJugement.GuiPresentation.SpriteReference)
            .AddFeaturesAtLevel(3,
                BuildMarshalCoordinatedAttack(),
                BuildFeatureSetMarshalKnowYourEnemyFeatureSet(),
                BuildStudyEnemyPower())
            .AddFeaturesAtLevel(7,
                BuildFeatureSetMarshalEternalComrade())
            .AddFeaturesAtLevel(10,
                BuildFeatureSetMarshalFearlessCommander(),
                BuildEncourage())
            .AddToDB();
    }

    internal override CharacterSubclassDefinition Subclass { get; }

    internal override FeatureDefinitionSubclassChoice SubclassChoice =>
        FeatureDefinitionSubclassChoices.SubclassChoiceFighterMartialArchetypes;

    private static int GetKnowledgeLevelOfEnemy(RulesetCharacter enemy)
    {
        return ServiceRepository.GetService<IGameLoreService>().Bestiary.TryGetBestiaryEntry(enemy, out var entry)
            ? entry.KnowledgeLevelDefinition.Level
            : 0;
    }

    private static void FeatureSetMarshalKnowYourEnemyComputeAttackModifier(
        RulesetCharacter myself,
        RulesetCharacter defender,
        RulesetAttackMode attackMode,
        ref ActionModifier attackModifier)
    {
        // no spell attack
        if (attackMode == null || defender == null)
        {
            return;
        }

        var knowledgeLevelOfEnemy = GetKnowledgeLevelOfEnemy(defender);

        attackModifier.attackRollModifier += knowledgeLevelOfEnemy;
        attackModifier.attackToHitTrends.Add(new TrendInfo(knowledgeLevelOfEnemy,
            FeatureSourceType.CharacterFeature, FeatureSetMarshalKnowYourEnemyName, null));
    }

    private static FeatureDefinitionFeatureSet BuildFeatureSetMarshalKnowYourEnemyFeatureSet()
    {
        var onComputeAttackModifierMarshalKnowYourEnemy = FeatureDefinitionOnComputeAttackModifierBuilder
            .Create("OnComputeAttackModifierMarshalKnowYourEnemy")
            .SetGuiPresentation(FeatureSetMarshalKnowYourEnemyName, Category.Feature)
            .SetOnComputeAttackModifierDelegate(FeatureSetMarshalKnowYourEnemyComputeAttackModifier)
            .AddToDB();

        var additionalDamageMarshalFavoredEnemyHumanoid = FeatureDefinitionAdditionalDamageBuilder
            .Create("AdditionalDamageMarshalFavoredEnemyHumanoid")
            .SetGuiPresentationNoContent()
            .SetNotificationTag("FavoredEnemy")
            .SetTriggerCondition(AdditionalDamageTriggerCondition.SpecificCharacterFamily)
            .SetDamageValueDetermination(AdditionalDamageValueDetermination.TargetKnowledgeLevel)
            .SetAdditionalDamageType(AdditionalDamageType.SameAsBaseDamage)
            .AddToDB();

        additionalDamageMarshalFavoredEnemyHumanoid.requiredCharacterFamily = CharacterFamilyDefinitions.Humanoid;

        return FeatureDefinitionFeatureSetBuilder
            .Create(FeatureSetMarshalKnowYourEnemyName)
            .SetGuiPresentation(Category.Feature)
            .AddFeatureSet(
                onComputeAttackModifierMarshalKnowYourEnemy,
                AdditionalDamageRangerFavoredEnemyAberration,
                AdditionalDamageRangerFavoredEnemyBeast,
                AdditionalDamageRangerFavoredEnemyCelestial,
                AdditionalDamageRangerFavoredEnemyConstruct,
                AdditionalDamageRangerFavoredEnemyDragon,
                AdditionalDamageRangerFavoredEnemyElemental,
                AdditionalDamageRangerFavoredEnemyFey,
                AdditionalDamageRangerFavoredEnemyFiend,
                AdditionalDamageRangerFavoredEnemyGiant,
                AdditionalDamageRangerFavoredEnemyMonstrosity,
                AdditionalDamageRangerFavoredEnemyOoze,
                AdditionalDamageRangerFavoredEnemyPlant,
                AdditionalDamageRangerFavoredEnemyUndead,
                additionalDamageMarshalFavoredEnemyHumanoid
            )
            .SetMode(FeatureDefinitionFeatureSet.FeatureSetMode.Union)
            .AddToDB();
    }

    private static FeatureDefinitionPower BuildStudyEnemyPower()
    {
        return FeatureDefinitionPowerBuilder
            .Create("PowerMarshalStudyYourEnemy")
            .SetGuiPresentation(Category.Feature, IdentifyCreatures.GuiPresentation.SpriteReference)
            .SetFixedUsesPerRecharge(2)
            .SetCostPerUse(1)
            .SetRechargeRate(RechargeRate.ShortRest)
            .SetActivationTime(ActivationTime.BonusAction)
            .SetEffectDescription(EffectDescriptionBuilder
                .Create(IdentifyCreatures.EffectDescription)
                .SetDurationData(DurationType.Instantaneous)
                .ClearRestrictedCreatureFamilies()
                .SetEffectForms(new StudyEnemyEffectDescription())
                .SetTargetingData(
                    Side.Enemy,
                    RangeType.Distance,
                    12,
                    TargetType.Individuals,
                    1,
                    2,
                    ActionDefinitions.ItemSelectionType.Equiped)
                .Build())
            .AddToDB();
    }

    private static IEnumerator MarshalCoordinatedAttackOnAttackHitDelegate(
        GameLocationCharacter attacker,
        GameLocationCharacter defender,
        RollOutcome outcome,
        CharacterActionParams actionParams,
        [NotNull] RulesetAttackMode attackMode,
        ActionModifier attackModifier)
    {
        // melee only
        if (attackMode.ranged || outcome is RollOutcome.CriticalFailure or RollOutcome.Failure ||
            actionParams.actionDefinition.Id == ActionDefinitions.Id.AttackOpportunity)
        {
            yield break;
        }

        var characterService = ServiceRepository.GetService<IGameLocationCharacterService>();
        var gameLocationBattleService = ServiceRepository.GetService<IGameLocationBattleService>();
        var battleManager = gameLocationBattleService as GameLocationBattleManager;
        var allies = new List<GameLocationCharacter>();

        foreach (var guestCharacter in characterService.GuestCharacters)
        {
            if (guestCharacter.RulesetCharacter is not RulesetCharacterMonster rulesetCharacterMonster
                || !rulesetCharacterMonster.MonsterDefinition.CreatureTags.Contains(EternalComradeName))
            {
                continue;
            }

            if (!rulesetCharacterMonster.TryGetConditionOfCategoryAndType(AttributeDefinitions.TagConjure,
                    RuleDefinitions.ConditionConjuredCreature,
                    out var activeCondition)
                || activeCondition.SourceGuid != attacker.Guid)
            {
                continue;
            }

            var gameLocationMonster = GameLocationCharacter.GetFromActor(rulesetCharacterMonster);

            if (gameLocationMonster.GetActionTypeStatus(ActionDefinitions.ActionType.Reaction) ==
                ActionDefinitions.ActionStatus.Available
                && !gameLocationMonster.Prone)
            {
                allies.Add(gameLocationMonster);
            }
        }

        allies.AddRange(characterService.PartyCharacters
            .Where(partyCharacter => partyCharacter.GetActionTypeStatus(ActionDefinitions.ActionType.Reaction) == 0 &&
                                     !partyCharacter.Prone)
            .Where(partyCharacter => partyCharacter != attacker));

        var reactions = new List<CharacterActionParams>();

        foreach (var partyCharacter in allies)
        {
            var allAttackMode = partyCharacter.FindActionAttackMode(ActionDefinitions.Id.AttackMain);
            var attackParams = default(BattleDefinitions.AttackEvaluationParams);
            var actionModifierBefore = new ActionModifier();
            var canAttack = true;

            if (allAttackMode == null)
            {
                continue;
            }

            if (allAttackMode.Ranged)
            {
                attackParams.FillForPhysicalRangeAttack(partyCharacter, partyCharacter.LocationPosition, allAttackMode,
                    defender, defender.LocationPosition, actionModifierBefore);
            }
            else
            {
                attackParams.FillForPhysicalReachAttack(partyCharacter, partyCharacter.LocationPosition, allAttackMode,
                    defender, defender.LocationPosition, actionModifierBefore);
            }

            if (!gameLocationBattleService.CanAttack(attackParams))
            {
                canAttack = false;

                var cantrips = ReactionRequestWarcaster.GetValidCantrips(battleManager, partyCharacter, defender);

                if (cantrips == null || cantrips.Empty())
                {
                    continue;
                }
            }

            var reactionParams = new CharacterActionParams(partyCharacter, ActionDefinitions.Id.AttackOpportunity,
                allAttackMode, defender, actionModifierBefore)
            {
                StringParameter2 = MarshalCoordinatedAttackName, BoolParameter4 = !canAttack
            };

            reactions.Add(reactionParams);
        }

        if (reactions.Empty() || battleManager == null)
        {
            yield break;
        }

        var actionService = ServiceRepository.GetService<IGameLocationActionService>();
        var count = actionService.PendingReactionRequestGroups.Count;

        foreach (var reaction in reactions)
        {
            actionService.ReactForOpportunityAttack(reaction);
        }

        yield return battleManager.WaitForReactions(attacker, actionService, count);
    }

    private static FeatureDefinition BuildMarshalCoordinatedAttack()
    {
        return FeatureDefinitionBuilder
            .Create(MarshalCoordinatedAttackName)
            .SetGuiPresentation(Category.Feature)
            .SetCustomSubFeatures(new ReactToAttackFinished(MarshalCoordinatedAttackOnAttackHitDelegate))
            .AddToDB();
    }

    private static MonsterDefinition BuildEternalComradeMonster()
    {
        var effectDescription = EffectDescriptionBuilder
            .Create()
            .SetAnimationMagicEffect(AnimationDefinitions.AnimationMagicEffect.Count)
            .SetEffectForms(
                EffectFormBuilder
                    .Create()
                    .SetDamageForm(
                        false,
                        DieType.D1,
                        DamageTypeSlashing,
                        2,
                        DieType.D6,
                        2)
                    .Build())
            .Build();

        var attackMarshalEternalComrade = MonsterAttackDefinitionBuilder
            .Create(MonsterAttackDefinitions.Attack_Generic_Guard_Longsword, "AttackMarshalEternalComrade")
            .SetEffectDescription(effectDescription)
            .AddToDB();

        //TODO: create a builder for this
        attackMarshalEternalComrade.magical = true;

        return MonsterDefinitionBuilder
            .Create(SuperEgo_Servant_Hostile, EternalComradeName)
            .SetOrUpdateGuiPresentation(Category.Monster)
            .SetFeatures(
                SenseNormalVision,
                SenseDarkvision24,
                MoveModeMove10,
                MoveModeFly10,
                ActionAffinityFightingStyleProtection,
                ConditionAffinityCharmImmunity,
                ConditionAffinityExhaustionImmunity,
                ConditionAffinityFrightenedImmunity,
                ConditionAffinityGrappledImmunity,
                ConditionAffinityParalyzedmmunity,
                ConditionAffinityPetrifiedImmunity,
                ConditionAffinityPoisonImmunity,
                ConditionAffinityProneImmunity,
                ConditionAffinityRestrainedmmunity,
                DamageAffinityColdImmunity,
                DamageAffinityNecroticImmunity,
                DamageAffinityFireResistance,
                DamageAffinityBludgeoningResistance,
                DamageAffinityPiercingResistance,
                DamageAffinitySlashingResistance,
                DamageAffinityFireResistance,
                DamageAffinityAcidResistance,
                DamageAffinityLightningResistance,
                DamageAffinityThunderResistance,
                ConditionAffinityHinderedByFrostImmunity
            )
            .SetAttackIterations(
                new MonsterAttackIteration(attackMarshalEternalComrade, 1))
            .SetArmorClass(16)
            .SetAlignment("Neutral")
            .SetCharacterFamily(CharacterFamilyDefinitions.Undead.name)
            .SetCreatureTags(EternalComradeName)
            .SetDefaultBattleDecisionPackage(DefaultMeleeWithBackupRangeDecisions)
            .SetFullyControlledWhenAllied(true)
            .SetDefaultFaction("Party")
            .SetBestiaryEntry(BestiaryDefinitions.BestiaryEntry.None)
            .AddToDB();
    }

    private static FeatureDefinitionFeatureSet BuildFeatureSetMarshalEternalComrade()
    {
        //TODO: make this use concentration and reduce the duration to may be 3 rounds
        //TODO: increase the number of use to 2 and recharge per long rest
        var powerMarshalSummonEternalComrade = FeatureDefinitionPowerBuilder
            .Create("PowerMarshalSummonEternalComrade")
            .SetGuiPresentation(Category.Feature, Bane.GuiPresentation.SpriteReference)
            .SetCostPerUse(1)
            .SetUsesFixed(1)
            .SetFixedUsesPerRecharge(1)
            .SetRechargeRate(RechargeRate.ShortRest)
            .SetActivationTime(ActivationTime.BonusAction)
            .SetEffectDescription(
                EffectDescriptionBuilder
                    .Create(ConjureAnimalsOneBeast.EffectDescription)
                    .SetDurationData(DurationType.Round, 10)
                    .SetEffectForms(
                        EffectFormBuilder
                            .Create()
                            .SetSummonCreatureForm(1, EternalComradeName)
                            .CreatedByCharacter()
                            .Build())
                    .Build()
            )
            .AddToDB();

        GlobalUniqueEffects.AddToGroup(GlobalUniqueEffects.Group.Familiar, powerMarshalSummonEternalComrade);

        var conditionMarshalEternalComradeAc = ConditionDefinitionBuilder
            .Create(ConditionKindredSpiritBondAC, "ConditionMarshalEternalComradeAC")
            .SetGuiPresentationNoContent()
            .SetAmountOrigin((ConditionDefinition.OriginOfAmount)ExtraOriginOfAmount.SourceProficiencyBonus)
            .AddToDB();

        var conditionMarshalEternalComradeSavingThrow = ConditionDefinitionBuilder
            .Create(ConditionKindredSpiritBondSavingThrows, "ConditionMarshalEternalComradeSavingThrow")
            .SetGuiPresentationNoContent()
            .SetAmountOrigin((ConditionDefinition.OriginOfAmount)ExtraOriginOfAmount.SourceProficiencyBonus)
            .AddToDB();

        var conditionMarshalEternalComradeDamage = ConditionDefinitionBuilder
            .Create(ConditionKindredSpiritBondMeleeDamage, "ConditionMarshalEternalComradeDamage")
            .SetGuiPresentationNoContent()
            .SetAmountOrigin((ConditionDefinition.OriginOfAmount)ExtraOriginOfAmount.SourceProficiencyBonus)
            .AddToDB();

        var conditionMarshalEternalComradeHit = ConditionDefinitionBuilder
            .Create(ConditionKindredSpiritBondMeleeAttack, "ConditionMarshalEternalComradeHit")
            .SetGuiPresentationNoContent()
            .SetAmountOrigin((ConditionDefinition.OriginOfAmount)ExtraOriginOfAmount.SourceProficiencyBonus)
            .AddToDB();

        var conditionMarshalEternalComradeHp = ConditionDefinitionBuilder
            .Create(ConditionKindredSpiritBondHP, "ConditionMarshalEternalComradeHP")
            .SetGuiPresentationNoContent()
            .SetAmountOrigin((ConditionDefinition.OriginOfAmount)ExtraOriginOfAmount.SourceClassLevel)
            .SetAllowMultipleInstances(true)
            .AddToDB();

        //TODO: create a builder for this
        conditionMarshalEternalComradeHp.additionalDamageType = FighterClass;

        var summoningAffinityMarshalEternalComrade = FeatureDefinitionSummoningAffinityBuilder
            .Create(SummoningAffinityKindredSpiritBond, "SummoningAffinityMarshalEternalComrade")
            .ClearEffectForms()
            .SetRequiredMonsterTag(EternalComradeName)
            .SetAddedConditions(
                conditionMarshalEternalComradeAc,
                conditionMarshalEternalComradeSavingThrow,
                conditionMarshalEternalComradeDamage,
                conditionMarshalEternalComradeHit,
                conditionMarshalEternalComradeHp,
                conditionMarshalEternalComradeHp)
            .AddToDB();

        return FeatureDefinitionFeatureSetBuilder
            .Create("FeatureSetMarshalEternalComrade")
            .SetGuiPresentation(Category.Feature)
            .SetFeatureSet(summoningAffinityMarshalEternalComrade, powerMarshalSummonEternalComrade)
            .AddToDB();
    }

    private static FeatureDefinitionFeatureSet BuildFeatureSetMarshalFearlessCommander()
    {
        return FeatureDefinitionFeatureSetBuilder
            .Create("FeatureSetMarshalFearlessCommander")
            .SetGuiPresentation(Category.Feature)
            .SetFeatureSet(ConditionAffinityFrightenedImmunity)
            .SetMode(FeatureDefinitionFeatureSet.FeatureSetMode.Union)
            .AddToDB();
    }

    private static FeatureDefinitionPower BuildEncourage()
    {
        var conditionMarshalEncouraged = ConditionDefinitionBuilder
            .Create("ConditionMarshalEncouraged")
            .SetGuiPresentation("PowerMarshalEncouragement", Category.Feature,
                ConditionBlessed.GuiPresentation.SpriteReference)
            .SetSilent(Silent.WhenAddedOrRemoved)
            .SetFeatures(
                FeatureDefinitionCombatAffinitys.CombatAffinityBlessed,
                FeatureDefinitionSavingThrowAffinitys.SavingThrowAffinityConditionBlessed)
            .SetTurnOccurence(TurnOccurenceType.StartOfTurn)
            .AddToDB();

        conditionMarshalEncouraged.ConditionTags.Add("Buff");

        // this allows the condition to still display as a label on character panel
        Global.CharacterLabelEnabledConditions.Add(conditionMarshalEncouraged);

        return FeatureDefinitionPowerBuilder
            .Create("PowerMarshalEncouragement")
            .SetGuiPresentation(Category.Feature, Bless.GuiPresentation.SpriteReference)
            .Configure(
                UsesDetermination.Fixed,
                ActivationTime.PermanentUnlessIncapacitated,
                RechargeRate.AtWill,
                EffectDescriptionBuilder
                    .Create()
                    .SetCreatedByCharacter()
                    .SetCanBePlacedOnCharacter()
                    .SetTargetingData(
                        Side.Ally,
                        RangeType.Self,
                        0,
                        TargetType.Cube,
                        5,
                        2)
                    .SetDurationData(DurationType.Permanent)
                    .SetRecurrentEffect(
                        RecurrentEffect.OnActivation | RecurrentEffect.OnEnter | RecurrentEffect.OnTurnStart)
                    .SetEffectForms(
                        EffectFormBuilder
                            .Create()
                            .CreatedByCharacter()
                            .SetConditionForm(conditionMarshalEncouraged, ConditionForm.ConditionOperation.Add, false,
                                false)
                            .Build())
                    .Build())
            .SetShowCasting(false)
            .AddToDB();
    }

    private sealed class StudyEnemyEffectDescription : CustomEffectForm
    {
        internal override void ApplyForm(
            RulesetImplementationDefinitions.ApplyFormsParams formsParams,
            List<string> effectiveDamageTypes,
            bool retargeting,
            bool proxyOnly,
            bool forceSelfConditionOnly,
            EffectApplication effectApplication = EffectApplication.All,
            [CanBeNull] List<EffectFormFilter> filters = null)
        {
            var manager = ServiceRepository.GetService<IGameLoreService>();
            var gameLocationCharacter = GameLocationCharacter.GetFromActor(formsParams.targetCharacter);

            if (gameLocationCharacter.RulesetCharacter is not RulesetCharacterMonster creature)
            {
                return;
            }

            if (!manager.Bestiary.TryGetBestiaryEntry(creature, out var entry)
                && (creature.MonsterDefinition.BestiaryEntry == BestiaryDefinitions.BestiaryEntry.Full
                    || (creature.MonsterDefinition.BestiaryEntry == BestiaryDefinitions.BestiaryEntry.Reference &&
                        creature.MonsterDefinition.BestiaryReference.BestiaryEntry ==
                        BestiaryDefinitions.BestiaryEntry.Full)))
            {
                entry = manager.Bestiary.AddNewMonsterEntry(creature.MonsterDefinition);
                manager.MonsterKnowledgeChanged?.Invoke(creature.MonsterDefinition, entry.KnowledgeLevelDefinition);
            }

            if (entry == null)
            {
                return;
            }

            var checkModifier = new ActionModifier();
            var roller = GameLocationCharacter.GetFromActor(formsParams.sourceCharacter);

            roller.RollAbilityCheck(AttributeDefinitions.Wisdom, SkillDefinitions.Survival,
                10 + Mathf.FloorToInt(entry.MonsterDefinition.ChallengeRating), AdvantageType.None, checkModifier,
                false, -1, out var outcome, out _, true);

            var level = entry.KnowledgeLevelDefinition.Level;
            var num = level;

            if (outcome is RollOutcome.Success or RollOutcome.CriticalSuccess)
            {
                var num2 = outcome == RollOutcome.Success ? 1 : 2;

                num = Mathf.Min(entry.KnowledgeLevelDefinition.Level + num2, 4);
                manager.LearnMonsterKnowledge(entry.MonsterDefinition, manager.Bestiary.SortedKnowledgeLevels[num]);
            }

            gameLocationCharacter.RulesetCharacter.MonsterIdentificationRolled?.Invoke(
                gameLocationCharacter.RulesetCharacter, entry.MonsterDefinition, outcome, level, num);
        }

        internal override void FillTags(Dictionary<string, TagsDefinitions.Criticity> tagsMap)
        {
        }
    }
}
