using SolastaModApi.Infrastructure;
using AK.Wwise;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.AddressableAssets;
using System;
using System.Linq;
using System.Text;
using System.CodeDom.Compiler;
using TA.AI;
using TA;
using System.Collections.Generic;
using UnityEngine.Rendering.PostProcessing;
using  static  ActionDefinitions ;
using  static  TA . AI . DecisionPackageDefinition ;
using  static  TA . AI . DecisionDefinition ;
using  static  RuleDefinitions ;
using  static  BanterDefinitions ;
using  static  Gui ;
using  static  GadgetDefinitions ;
using  static  BestiaryDefinitions ;
using  static  CursorDefinitions ;
using  static  AnimationDefinitions ;
using  static  FeatureDefinitionAutoPreparedSpells ;
using  static  FeatureDefinitionCraftingAffinity ;
using  static  CharacterClassDefinition ;
using  static  CreditsGroupDefinition ;
using  static  SoundbanksDefinition ;
using  static  CampaignDefinition ;
using  static  GraphicsCharacterDefinitions ;
using  static  GameCampaignDefinitions ;
using  static  FeatureDefinitionAbilityCheckAffinity ;
using  static  TooltipDefinitions ;
using  static  BaseBlueprint ;
using  static  MorphotypeElementDefinition ;

namespace SolastaModApi.Extensions
{
    /// <summary>
    /// This helper extensions class was automatically generated.
    /// If you find a problem please report at https://github.com/SolastaMods/SolastaModApi/issues.
    /// </summary>
    [TargetType(typeof(BaseBlueprint)), GeneratedCode("Community Expansion Extension Generator", "1.0.0")]
    public static partial class BaseBlueprintExtensions
    {
        public static T AddPrefabsByEnvironment<T>(this T entity,  params  BaseBlueprint . PrefabByEnvironmentDescription [ ]  value)
            where T : BaseBlueprint
        {
            AddPrefabsByEnvironment(entity, value.AsEnumerable());
            return entity;
        }

        public static T AddPrefabsByEnvironment<T>(this T entity, IEnumerable<BaseBlueprint.PrefabByEnvironmentDescription> value)
            where T : BaseBlueprint
        {
            entity.PrefabsByEnvironment.AddRange(value);
            return entity;
        }

        public static T ClearPrefabsByEnvironment<T>(this T entity)
            where T : BaseBlueprint
        {
            entity.PrefabsByEnvironment.Clear();
            return entity;
        }

        public static T SetCategory<T>(this T entity, System.String value)
            where T : BaseBlueprint
        {
            entity.SetField("category", value);
            return entity;
        }

        public static T SetDimensions<T>(this T entity, UnityEngine.Vector2Int value)
            where T : BaseBlueprint
        {
            entity.SetField("dimensions", value);
            return entity;
        }

        public static T SetPrefabsByEnvironment<T>(this T entity,  params  BaseBlueprint . PrefabByEnvironmentDescription [ ]  value)
            where T : BaseBlueprint
        {
            SetPrefabsByEnvironment(entity, value.AsEnumerable());
            return entity;
        }

        public static T SetPrefabsByEnvironment<T>(this T entity, IEnumerable<BaseBlueprint.PrefabByEnvironmentDescription> value)
            where T : BaseBlueprint
        {
            entity.PrefabsByEnvironment.SetRange(value);
            return entity;
        }
    }
}