﻿using System;
using System.Linq;
using ApacheTech.VintageMods.CampaignCartographer.Features.GPS.Packets;
using ApacheTech.VintageMods.Core.Abstractions.ModSystems;
using ApacheTech.VintageMods.Core.Common.StaticHelpers;
using ApacheTech.VintageMods.Core.Extensions.Game;
using ApacheTech.VintageMods.Core.Services;
using ApacheTech.VintageMods.FluentChatCommands;
using Vintagestory.API.Client;
using Vintagestory.API.Common;
using Vintagestory.API.Config;
using Vintagestory.API.Server;

// ReSharper disable UnusedMember.Local
// ReSharper disable UnusedType.Global
// ReSharper disable StringLiteralTypo

namespace ApacheTech.VintageMods.CampaignCartographer.Features.GPS
{
    /// <summary>
    ///     Feature: Global Positioning System.
    /// 
    ///      - Display your current XYZ coordinates.
    ///      - Copy your current XYZ coordinates to clipboard.
    ///      - Send your current XYZ coordinates as a chat message to the current chat group.
    ///      - Whisper your current XYZ coordinates to a single player (server settings permitting).
    ///      - Server: Enable/Disable permissions to whisper to other members of the server.
    /// </summary>
    /// <seealso cref="UniversalModSystem" />
    public sealed class Gps : UniversalModSystem
    {
        private GpsSettings _settings;

        private IClientNetworkChannel _clientChannel;

        private ICoreClientAPI _capi;
        private ICoreServerAPI _sapi;
        
        /// <summary>
        ///     Minor convenience method to save yourself the check for/cast to ICoreClientAPI in Start()
        /// </summary>
        /// <param name="capi">The core API implemented by the client. The main interface for accessing the client. Contains all sub-components, and some miscellaneous methods.</param>
        public override void StartClientSide(ICoreClientAPI capi)
        {
            _clientChannel = ModServices.Network.DefaultClientChannel?
                .RegisterMessageType<WhisperPacket>()
                .RegisterMessageType<GpsSettings>()
                .SetMessageHandler<GpsSettings>(OnClientSetPropertyPacketReceived);

            FluentChat.ClientCommand("gps")
                .RegisterWith(_capi = capi)
                .HasDescription(LangEx.FeatureString("GPS.Client", "SettingsCommandDescription"))
                .HasDefaultHandler(OnClientDefaultHandler)
                .HasSubCommand("chat").WithHandler(OnClientSubCommandBroadcast)
                .HasSubCommand("copy").WithHandler(OnClientSubCommandClipboard)
                .HasSubCommand("to").WithHandler(OnClientSubCommandWhisper);
        }

        /// <summary>
        ///     Minor convenience method to save yourself the check for/cast to ICoreServerAPI in Start()
        /// </summary>
        /// <param name="sapi">The core API implemented by the server. The main interface for accessing the server. Contains all sub-components, and some miscellaneous methods.</param>
        public override void StartServerSide(ICoreServerAPI sapi)
        {
            _sapi = sapi;
            _settings = ModServices.IOC.Resolve<GpsSettings>();

            ModServices.Network.DefaultServerChannel
                .RegisterMessageType<WhisperPacket>()
                .RegisterMessageType<GpsSettings>()
                .SetMessageHandler<WhisperPacket>(OnServerWhisperPacketReceived);
            
            // GUIs don't work, server side.
            FluentChat
                .ServerCommand("gpsadmin")
                .RequiresPrivilege(Privilege.controlserver)
                .HasDescription(LangEx.FeatureString("GPS.Server", "SettingsCommandDescription"))
                .RegisterWith(sapi)
                .HasSubCommand("enable-whispers").WithHandler(OnEnableWhispers)
                .HasSubCommand("disable-whispers").WithHandler(OnDisableWhispers);

            sapi.Event.PlayerNowPlaying += OnServerPlayerJoin;
        }

        private void OnDisableWhispers(string subCommandName, IServerPlayer player, int groupId, CmdArgs args)
        {
            _settings.WhispersAllowed = false;
            _sapi.Logger.Audit("[Campaign Cartographer] GPS Whispers Disabled.");
            ModServices.Network.DefaultServerChannel.BroadcastPacket(_settings);
        }

        private void OnEnableWhispers(string subCommandName, IServerPlayer player, int groupId, CmdArgs args)
        {
            _settings.WhispersAllowed = true;
            _sapi.Logger.Audit("[Campaign Cartographer] GPS Whispers Enabled.");
            ModServices.Network.DefaultServerChannel.BroadcastPacket(_settings);
        }   

        #region Client Chat Commands

        /// <summary>
        ///     Client Default Command Handler: .gps
        /// </summary>
        /// <param name="groupId">The ID of the chat group to send messages to.</param>
        /// <param name="args">The arguments sent along with the command.</param>
        private void OnClientDefaultHandler(int groupId, CmdArgs args)
        {
            var player = _capi.World.Player;
            var pos = PlayerLocationMessage(player);
            _capi.ShowChatMessage(pos);
        }

        /// <summary>
        ///     Client Sub-Command Handler: .gps chat
        /// </summary>
        /// <param name="subCommandName">The name of the sub command.</param>
        /// <param name="groupId">The ID of the chat group to send messages to.</param>
        /// <param name="args">The arguments sent along with the command.</param>
        private void OnClientSubCommandBroadcast(string subCommandName, int groupId, CmdArgs args)
        {
            var player = _capi.World.Player;
            var pos = PlayerLocationMessage(player);
            _capi.SendChatMessage(pos);
        }

        /// <summary>
        ///     Client Sub-Command Handler: .gps copy
        /// </summary>
        /// <param name="subCommandName">The name of the sub command.</param>
        /// <param name="groupId">The ID of the chat group to send messages to.</param>
        /// <param name="args">The arguments sent along with the command.</param>
        private void OnClientSubCommandClipboard(string subCommandName, int groupId, CmdArgs args)
        {
            var player = _capi.World.Player;
            var pos = PlayerLocationMessage(player);
            _capi.Forms.SetClipboardText($"{player.PlayerName}: {pos}");
            _capi.ShowChatMessage(LangEx.FeatureString("GPS.Client", "location-copied-to-clipboard"));
        }

        /// <summary>
        ///     Client Sub-Command Handler: .gps to [playerName]
        /// </summary>
        /// <param name="subCommandName">The name of the sub command.</param>
        /// <param name="groupId">The ID of the chat group to send messages to.</param>
        /// <param name="args">The arguments sent along with the command.</param>
        private void OnClientSubCommandWhisper(string subCommandName, int groupId, CmdArgs args)
        {
            if (_clientChannel is not null && !_clientChannel.Connected)
            {
                _capi.ShowChatMessage(LangEx.Get("error-messages.mod-not-installed-on-server"));
                return;
            }
            if (!_settings.WhispersAllowed)
            {
                _capi.ShowChatMessage(LangEx.Get("error-messages.feature-disabled"));
                return;
            }
            var message = PlayerLocationMessage(_capi.World.Player);
            var recipient = args.PopWord();

            _clientChannel?.SendPacket(new WhisperPacket
            {
                RecipientName = recipient,
                GroupId = GlobalConstants.AllChatGroups,
                Message = message
            });
        }

        #endregion

        /// <summary>
        ///     Sets a property within the class, from the other app-side.
        /// </summary>
        /// <param name="packet">The packet, containing the property name, and value to set.</param>
        private void OnClientSetPropertyPacketReceived(GpsSettings packet)
        {
            _settings = packet;
        }

        /// <summary>
        ///     Called when any player joins the Server.
        /// </summary>
        /// <param name="joiningPlayer">The player that is joining the server.</param>
        private void OnServerPlayerJoin(IServerPlayer joiningPlayer)
        {
            ModServices.Network.DefaultServerChannel.SendPacket(_settings, joiningPlayer);
        }

        /// <summary>
        ///     Called when a <see cref="WhisperPacket"/> packet is received on the Server.
        /// </summary>
        /// <param name="fromPlayer">The player who's Client sent the packet..</param>
        /// <param name="packet">The packet set from the Client.</param>
        private void OnServerWhisperPacketReceived(IServerPlayer fromPlayer, WhisperPacket packet)
        {
            var toPlayer = _sapi.World.AllOnlinePlayers
                .FirstOrDefault(p =>
                    p.PlayerName.StartsWith(packet.RecipientName, StringComparison.InvariantCultureIgnoreCase));

            if (toPlayer is null)
            {
                _sapi.SendMessage(fromPlayer, packet.GroupId, LangEx.FeatureString("GPS.Client", "player-not-found", packet.RecipientName), EnumChatType.OwnMessage);
                return;
            }

            var receivedMessage = LangEx.FeatureString("GPS.Server", "whisper-received", fromPlayer.PlayerName, packet.Message);
            _sapi.SendMessage(toPlayer as IServerPlayer, packet.GroupId, receivedMessage, EnumChatType.OwnMessage);

            var sentMessage = LangEx.FeatureString("GPS.Server", "whisper-sent", toPlayer.PlayerName, packet.Message);
            _sapi.SendMessage(fromPlayer, packet.GroupId, sentMessage, EnumChatType.OwnMessage);
        }

        /// <summary>
        ///     Retrieves the player's current location. 
        /// </summary>
        /// <param name="player">The player to find the location of.</param>
        /// <returns>A string representation of the XYZ coordinates of the player.</returns>
        private static string PlayerLocationMessage(IPlayer player)
        {
            var pos = player.Entity.Pos.AsBlockPos.RelativeToSpawn();
            return $"X = {pos.X}, Y = {pos.Y}, Z = {pos.Z}.";
        }
    }
}
