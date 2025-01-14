﻿using ApacheTech.VintageMods.CampaignCartographer.Features.ManualWaypoints.Model;
using ApacheTech.VintageMods.CampaignCartographer.Services.Waypoints.Extensions;
using ApacheTech.VintageMods.Core.Abstractions.ModSystems;
using ApacheTech.VintageMods.Core.Common.StaticHelpers;
using ApacheTech.VintageMods.Core.Hosting.Configuration;
using ApacheTech.VintageMods.FluentChatCommands;
using Vintagestory.API.Client;
using Vintagestory.API.Common;

// ReSharper disable UnusedType.Global

namespace ApacheTech.VintageMods.CampaignCartographer.Features.ManualWaypoints
{
    /// <summary>
    ///     Feature: Manual Waypoint Addition
    ///      • Add a waypoint for the block the player is currently targetting. `(.wps)`
    /// </summary>
    /// <seealso cref="ClientModSystem" />
    public sealed class BlockSelectionWaypoints : ClientModSystem
    {
        private ICoreClientAPI _capi;

        /// <summary>
        ///     Minor convenience method to save yourself the check for/cast to ICoreClientAPI in Start()
        /// </summary>
        /// <param name="capi">
        ///     The core API implemented by the client.
        ///     The main interface for accessing the client.
        ///     Contains all sub-components, and some miscellaneous methods.
        /// </param>
        public override void StartClientSide(ICoreClientAPI capi)
        {
            FluentChat.ClientCommand("wps")
                .RegisterWith(_capi = capi)
                .HasDescription(LangEx.FeatureString("ManualWaypoints.BlockSelectionWaypoints", "Description"))
                .HasDefaultHandler(DefaultHandler);
        }

        private void DefaultHandler(int groupId, CmdArgs args)
        {
            var blockSelection = _capi.World.Player.CurrentBlockSelection;
            if (blockSelection is null) return;
            var position = blockSelection.Position;
            var block = _capi.World.BlockAccessor.GetBlock(position);
            var title = block.GetPlacedBlockName(_capi.World, position);
            
            var template = ModSettings.World.
                Feature<ManualWaypointsSettings>("ManualWaypoints")
                .BlockSelectionWaypointTemplate;

            var waypoint = new ManualWaypointTemplateModel
            {
                Colour = template.Colour,
                DisplayedIcon = template.DisplayedIcon,
                ServerIcon = template.ServerIcon,
                Title = title,
                HorizontalCoverageRadius = template.HorizontalCoverageRadius,
                VerticalCoverageRadius = template.VerticalCoverageRadius
            };
            waypoint.AddToMap(position);
        }
    }
}