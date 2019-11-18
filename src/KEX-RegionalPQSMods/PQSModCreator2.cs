using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using Kopernicus.ConfigParser.Attributes;
using Kopernicus.ConfigParser.BuiltinTypeParsers;
using Kopernicus.ConfigParser.Enumerations;
using Kopernicus.ConfigParser.Interfaces;
using Kopernicus.Configuration.ModLoader;
using Kopernicus.Configuration.Parsing;
using Kopernicus.UI;
using UnityEngine;


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
                //string backup = "";
                //if (mod is PQSMod_Sigma sigma1)
                //{
                //    backup = sigma1.id;
                //    sigma1.id = "Regional: ";
                //}
                mod.OnVertexBuild(data);
                //if (mod is PQSMod_Sigma sigma2)
                //{
                //    sigma2.id = backup;
                //}
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
                //string backup = "";
                //if (mod is PQSMod_Sigma sigma1)
                //{
                //    backup = sigma1.id;
                //    sigma1.id = "Regional: ";
                //}
                mod.OnVertexBuildHeight(data);
                //if (mod is PQSMod_Sigma sigma2)
                //{
                //    sigma2.id = backup;
                //}
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
                //if (Mod.mod is PQSMod_Sigma sigma)
                //{
                //    sigma.id = "PostApply: ";
                //}
            }
        }

        //    public class PQSMod_Sigma : PQSMod
        //    {
        //        internal string id = "Standard: ";

        //        public override void OnVertexBuild(PQS.VertexBuildData data)
        //        {
        //            //Debug.Log("SigmaLog: PQSMod_Sigma: " + id + "OnVertexBuild");
        //            data.vertHeight = 50;
        //        }

        //        public override void OnVertexBuildHeight(PQS.VertexBuildData data)
        //        {
        //            //Debug.Log("SigmaLog: PQSMod_Sigma: " + id + "OnVertexBuildHeight");
        //            data.vertHeight = 50;
        //        }
        //    }

        //    [RequireConfigType(ConfigType.Node)]
        //    public class Sigma : ModLoader<PQSMod_Sigma>
        //    {
        //    }
    }
}
