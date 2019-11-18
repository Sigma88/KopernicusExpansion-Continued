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
            public PQSMod mod;
            public MapSO multiplierMap;
            public bool splitChannels = false;
            public float offset = 0;
            public float deformity = 1;
            Color multiplier;
            Color preBuildColor;
            double preBuildHeight;

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
                data.vertColor = Color.Lerp(preBuildColor, data.vertColor, offset + multiplier.a * deformity);
                data.vertHeight = UtilMath.Lerp(preBuildHeight, data.vertHeight, offset + multiplier.r * deformity);
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
                data.vertColor = Color.Lerp(preBuildColor, data.vertColor, offset + multiplier.a * deformity);
                data.vertHeight = UtilMath.Lerp(preBuildHeight, data.vertHeight, offset + multiplier.r * deformity);
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
                set { Mod.multiplierMap = value; }
            }

            [ParserTarget("splitChannels", Optional = true)]
            private NumericParser<bool> splitChannels
            {
                get { return Mod.splitChannels; }
                set { Mod.splitChannels = value; }
            }

            [ParserTarget("offset", Optional = true)]
            private NumericParser<float> offset
            {
                get { return Mod.offset; }
                set { Mod.offset = value; }
            }

            [ParserTarget("deformity", Optional = true)]
            private NumericParser<float> deformity
            {
                get { return Mod.deformity; }
                set { Mod.deformity = value; }
            }

            void IParserEventSubscriber.Apply(ConfigNode node)
            {
            }

            void IParserEventSubscriber.PostApply(ConfigNode node)
            {
                Mod.mod = Mods?.FirstOrDefault()?.Mod;
                Mod.mod.modEnabled = false;
            }
        }
    }
}
