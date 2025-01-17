﻿using System;
using System.Linq;
using System.Text;
using System.Collections.Generic;

using Plus.HabboHotel.Rooms;

namespace Plus.HabboHotel.Rooms.Chat.Commands.Moderator.Fun
{
    class FastwalkCommand : IChatCommand
    {
        public string PermissionRequired
        {
            get { return "command_fastwalk"; }
        }

        public string Parameters
        {
            get { return ""; }
        }

        public string Description
        {
            get { return "Activa o desactiva la habilidad de caminar rápido."; }
        }

        public void Execute(GameClients.GameClient Session, Rooms.Room Room, string[] Params)
        {
            RoomUser User = Room.GetRoomUserManager().GetRoomUserByHabbo(Session.GetHabbo().Id);
            if (User == null)
                return;

            User.FastWalking = !User.FastWalking;

            if (User.SuperFastWalking)
                User.SuperFastWalking = false;

            if(User.SuperFastWalking)
                Session.SendWhisper("¡Fastwalk activado!", 1);
            else
                Session.SendWhisper("¡Fastwalk desactivado!", 1);
        }
    }
}
