﻿using System.Collections.Generic;

// ReSharper disable StringLiteralTypo

namespace ApacheTech.VintageMods.CampaignCartographer.Services.Waypoints
{
    /// <summary>
    ///     Dictionaries that map between block codes, and waypoint types.
    /// </summary>
    public static class CrossMaps
    {
        public static Dictionary<string, string> Ores { get; } = new()
        {
            { "looseores-anthracite-", "anthracite" },
            { "looseores-bismuthinite-", "bismuthinite" },
            { "looseores-cassiterite-", "cassiterite" },
            { "looseores-sphalerite-", "sphalerite" },
            { "looseores-nativecopper-", "copper" },
            { "looseores-malachite-", "copper" },
            { "looseores-galena-", "galena" },
            { "looseores-quartz-", "quartz" },
            { "looseores-galena_nativesilver-", "silver" },
            { "looseores-quartz_nativesilver-", "silver" },
            { "looseores-quartz_nativegold-", "gold" },
            { "looseores-limonite-", "limonite" },
            { "looseores-hematite-", "hematite" },
            { "looseores-magnetite-", "magnetite" },
            { "looseores-chromite-", "chromite" },
            { "looseores-rhodochrosite-", "rhodochrosite" },
            { "looseores-ilmenite-", "ilmenite" },
            { "looseores-quartz_wolframite-", "wolframite" },
            { "looseores-stibnite-", "stibnite" },
            { "looseores-lignite-", "brcoal" },
            { "looseores-bituminouscoal-", "blcoal" },
            { "looseores-sulfur-", "sulphur" },
            { "looseores-sylvite-halite", "halite" },
            { "looseores-borax-", "borax" },
            { "looseores-kernite-", "kernite" },
            { "looseores-graphite-", "graphite" },
            { "looseores-cinnabar-", "cinnabar" },
            { "looseores-corundum-", "corundum" },
            { "looseores-lapislazuli-", "lapis" },
            { "looseores-emerald-", "emerald" },
            { "looseores-diamond-", "diamond" },
            { "looseores-olivine-", "olivine" },
            { "looseores-pentlandite-", "pentlandite" },
            { "looseores-fluorite-", "fluorite" },
            { "looseores-phosphorite-", "phosphorite" },
            { "looseores-uranium-", "uranium" },
            { "loosestones-meteorite-", "meteor" },
        };

        public static Dictionary<string, string> Stones { get; } = new()
        {
            { "loosestones-suevite-", "suevite" },
            { "loosestones-andesite-", "andesite" },
            { "loosestones-chalk-", "chalk" },
            { "loosestones-chert-", "chert" },
            { "loosestones-conglomerate-", "conglomerate" },
            { "loosestones-limestone-", "limestone" },
            { "loosestones-claystone-", "claystone" },
            { "loosestones-granite-", "granite" },
            { "loosestones-sandstone-", "sandstone" },
            { "loosestones-shale-", "shale" },
            { "loosestones-basalt-", "basalt" },
            { "loosestones-peridotite-", "peridotite" },
            { "loosestones-phyllite-", "phyllite" },
            { "loosestones-slate-", "slate" },
            { "loosestones-obsidian-", "obsidian" },
            { "loosestones-kimberlite-", "kimberlite" },
            { "loosestones-scoria-", "scoria" },
            { "loosestones-tuff-", "tuff" },
            { "loosestones-bauxite-", "bauxite" },
            { "loosestones-whitemarble-", "whitemarble" },
            { "loosestones-redmarble-", "redmarble" },
            { "loosestones-greenmarble-", "greenmarble" }
        };

        public static Dictionary<string, string> Organics { get; } = new()
        {
            { "-resin-", "resin" },
            { "-resinharvested-", "resin" },
            { "-blueberry-ripe", "blueberry" },
            { "-cranberry-ripe", "cranberry" },
            { "-blackcurrant-ripe", "blackcurrant" },
            { "-redcurrant-ripe", "redcurrant" },
            { "-whitecurrant-ripe", "whitecurrant" },
            { "-flyagaric-", "flyagaric" },
            { "-bolete-", "bolete" },
            { "-fieldmushroom-", "fieldmushroom" },
            { "-almondmushroom-", "almondmushroom" },
            { "-bitterbolete-", "bitterbolete" },
            { "-blacktrumpet-", "blacktrumpet" },
            { "-chanterelle-", "chanterelle" },
            { "-commonmorel-", "commonmorel" },
            { "-deathcap-", "deathcap" },
            { "-devilbolete-", "devilbolete" },
            { "-earthball-", "earthball" },
            { "-elfinsaddle-", "elfinsaddle" },
            { "-golddropmilkcap-", "golddropmilkcap" },
            { "-greencrackedrussula-", "greencrackedrussula" },
            { "-indigomilkcap-", "indigomilkcap" },
            { "-jackolantern-", "jackolantern" },
            { "-kingbolete-", "kingbolete" },
            { "-lobster-", "lobster" },
            { "-orangeoakbolete-", "orangeoakbolete" },
            { "-paddystraw-", "paddystraw" },
            { "-puffball-", "puffball" },
            { "-redwinecap-", "redwinecap" },
            { "-saffronmilkcap-", "saffronmilkcap" },
            { "-violetwebcap-", "violetwebcap" },
            { "-beardedtooth-", "beardedtooth" },
            { "-chickenofthewoods-", "chickenofthewoods" },
            { "-dryadsaddle-", "dryadsaddle" },
            { "-pinkoyster-", "pinkoyster" },
            { "-tinderhoof-", "tinderhoof" },
            { "-whiteoyster-", "whiteoyster" },
            { "-reishi-", "reishi" },
            { "-funeralbell-", "funeralbell" },
            { "-deerear-", "deerear" },
            { "-livermushroom-", "livermushroom" },
            { "-pinkbonnet-", "pinkbonnet" },
            { "-shiitake-", "shiitake" },
            { "-witchhat-", "witchhat" },
            { "-devilstooth-", "devilstooth" }
        };
    }
}