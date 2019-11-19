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
        public class KEXtoSKEX : MonoBehaviour
        {
            public static void ModuleManagerPostLoad()
            {
                UrlDir.UrlConfig kopernicus = GameDatabase.Instance?.GetConfigs("Kopernicus")?.FirstOrDefault();
                if (kopernicus?.config != null)
                {
                    var bodies = kopernicus?.config?.GetNodes();
                    var newbodies = new ConfigNode();

                    for (int body = 0; body < bodies?.Length; body++)
                    {
                        if (bodies[body]?.name == "Body")
                        {
                            var PQS = bodies[body].GetNode("PQS");

                            if (PQS != null)
                            {
                                var Mods = PQS.GetNode("Mods");

                                if (Mods != null)
                                {
                                    var mods = Mods.GetNodes();
                                    var newmods = new ConfigNode();

                                    for (int mod = 0; mod < mods?.Length; mod++)
                                    {
                                        if (mods[mod]?.name?.EndsWith("Regional") == true)
                                        {
                                            if (mods[mod].HasNode("Mod"))
                                            {
                                                ConfigNode newRegional = new ConfigNode("Regional");
                                                ConfigNode newMod = new ConfigNode("Mod");
                                                ConfigNode newPQSMod = new ConfigNode(mods[mod].name.Substring(0, mods[mod].name.Length - 8));
                                                newPQSMod.AddData(mods[mod].GetNode("Mod"));
                                                newMod.AddNode(newPQSMod);
                                                newRegional.AddData(mods[mod]);
                                                newRegional.RemoveNodes("Mod");
                                                newRegional.AddNode(newMod);
                                                mods[mod] = newRegional;
                                            }
                                        }

                                        if (mods[mod] != null)
                                        {
                                            newmods.AddNode(mods[mod]);
                                        }
                                    }

                                    Mods.ClearNodes();
                                    Mods.AddData(newmods);

                                    PQS.RemoveNodes("Mods");
                                    PQS.AddNode(Mods);
                                }

                                bodies[body].RemoveNodes("PQS");
                                bodies[body].AddNode(PQS);
                            }
                        }

                        if (bodies[body] != null)
                        {
                            newbodies.AddNode(bodies[body]);
                        }
                    }

                    kopernicus.config.ClearNodes();
                    kopernicus.config.AddData(newbodies);
                }
            }
        }

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
            [ParserTargetCollection("Mod", AllowMerge = true, NameSignificance = NameSignificance.Type)]
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
