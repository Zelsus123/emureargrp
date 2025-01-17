﻿using System;
using System.Linq;
using System.Text;
using System.Collections.Generic;

using Plus.HabboHotel.Rooms;
using Plus.HabboHotel.GameClients;

namespace Plus.HabboHotel.Rooms.Chat.Commands.Moderator
{
    class KickCommand : IChatCommand
    {
        public string PermissionRequired
        {
            get { return "command_kick"; }
        }

        public string Parameters
        {
            get { return "%username% %reason%"; }
        }

        public string Description
        {
            get { return "Kickea a un usuario con una razón."; }
        }

        public void Execute(GameClients.GameClient Session, Rooms.Room Room, string[] Params)
        {
            if (Params.Length == 1)
            {
                Session.SendWhisper("Ingresa el nombre de usuario.", 1);
                return;
            }

            GameClient TargetClient = PlusEnvironment.GetGame().GetClientManager().GetClientByUsername(Params[1]);
            if (TargetClient == null)
            {
                Session.SendWhisper("No se encontró a ese usuario.", 1);
                return;
            }

            if (TargetClient.GetHabbo() == null)
            {
                Session.SendWhisper("No se encontró a ese usuario.", 1);
                return;
            }

            if (TargetClient.GetHabbo().Username == Session.GetHabbo().Username)
            {
                Session.SendWhisper("¿Es enserio?...", 1);
                return;
            }

            if (!TargetClient.GetHabbo().InRoom)
            {
                Session.SendWhisper("Esa persona no se encuentra en ninguna zona.", 1);
                return;
            }

            Room TargetRoom;
            if (!PlusEnvironment.GetGame().GetRoomManager().TryGetRoom(TargetClient.GetHabbo().CurrentRoomId, out TargetRoom))
                return;

            if (Params.Length > 2)
                TargetClient.SendNotification("Un Moderador te ha kickeado de la zona. Razón: " + CommandManager.MergeParams(Params, 2));
            else
                TargetClient.SendNotification("Un Moderador te ha kickeado de la zona.");

            TargetRoom.GetRoomUserManager().RemoveUserFromRoom(TargetClient, true, false);
        }
    }
}
