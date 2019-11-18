using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using UnityEngine;
using Kopernicus.ConfigParser.Attributes;
using Kopernicus.ConfigParser.BuiltinTypeParsers;
using Kopernicus.ConfigParser.Enumerations;
using Kopernicus.ConfigParser.Interfaces;
using Kopernicus.Configuration.ModLoader;
using Kopernicus.Configuration.Parsing;
using Kopernicus.UI;


namespace SigmaKopernicusExpansion
{
    namespace SigmaRegionalPQSMods
    {
        public class PQSMod_Regional : PQSMod
        {
            public MapSO multiplierMap;
            public bool splitChannels;
            public Color multiplier;
            public Color preBuildColor;
            public double preBuildHeight;
            public PQSMod mod;

            public override void OnQuadPreBuild(PQ quad)
            {
                base.OnQuadPreBuild(quad);
                mod.OnQuadPreBuild(quad);
            }

            public override void OnQuadBuilt(PQ quad)
            {
                base.OnQuadBuilt(quad);
                mod.OnQuadBuilt(quad);
            }

            public override void OnSetup()
            {
                base.OnSetup();
                mod.OnSetup();
            }

            public override void OnVertexBuild(PQS.VertexBuildData data)
            {
                multiplier = multiplierMap.GetPixelColor(data.u, data.v);
                if (!splitChannels)
                {
                    multiplier.a = multiplier.r;
                }
                preBuildColor = data.vertColor;
                preBuildHeight = data.vertHeight;
                mod.OnVertexBuild(data);
                data.vertColor = Color.Lerp(preBuildColor, data.vertColor, multiplier.a);
                data.vertHeight = UtilMath.Lerp(preBuildHeight, data.vertHeight, multiplier.r);
            }

            public override void OnVertexBuildHeight(PQS.VertexBuildData data)
            {
                multiplier = multiplierMap.GetPixelColor(data.u, data.v);
                if (!splitChannels)
                {
                    multiplier.a = multiplier.r;
                }
                preBuildColor = data.vertColor;
                preBuildHeight = data.vertHeight;
                mod.OnVertexBuildHeight(data);
                data.vertColor = Color.Lerp(preBuildColor, data.vertColor, multiplier.a);
                data.vertHeight = UtilMath.Lerp(preBuildHeight, data.vertHeight, multiplier.r);
            }
        }

        [RequireConfigType(ConfigType.Node)]
        public class Regional : ModLoader<PQSMod_Regional>, IParserEventSubscriber
        {
            [ParserTargetCollection("Mods", AllowMerge = true, NameSignificance = NameSignificance.Type)]
            [KittopiaUntouchable]
            [SuppressMessage("ReSharper", "CollectionNeverQueried.Global")]
            public readonly List<IModLoader> Mods = new List<IModLoader>();

            [ParserTarget("multiplierMap", Optional = true)]
            private MapSOParserRGB<MapSO> multiplierMap
            {
                get { return Mod.multiplierMap; }
                set
                {
                    Mod.multiplierMap = value;
                }
            }

            [ParserTarget("splitChannels", Optional = true)]
            private NumericParser<Boolean> splitChannels
            {
                get { return Mod.splitChannels; }
                set { Mod.splitChannels = value; }
            }

            void IParserEventSubscriber.Apply(ConfigNode node)
            {
            }

            void IParserEventSubscriber.PostApply(ConfigNode node)
            {
                Mod.mod = Mods?.FirstOrDefault()?.Mod;
            }
        }
    }
}
