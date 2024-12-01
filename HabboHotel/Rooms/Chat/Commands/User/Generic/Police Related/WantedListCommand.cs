﻿using System;
using System.Linq;
using System.Text;
using System.Collections.Generic;

using Plus.Communication.Packets.Outgoing.Notifications;
using Plus.HabboHotel.Rooms;
using Plus.HabboHotel.Rooms.Chat.Styles;
using Plus.HabboRoleplay.RoleplayUsers;
using Plus.HabboRoleplay.Misc;

namespace Plus.HabboHotel.Rooms.Chat.Commands.Users.Generic.Police
{
    class WantedListCommand : IChatCommand
    {
        public string PermissionRequired
        {
            get { return "command_police_related_wanted_list"; }
        }

        public string Parameters
        {
            get { return ""; }
        }

        public string Description
        {
            get { return "Proporciona una lista de los más buscados de la ciudad."; }
        }

        public void Execute(GameClients.GameClient Session, Rooms.Room Room, string[] Params)
        {
            StringBuilder Message = new StringBuilder().Append("--- Los más buscados ---\n\n");

            if (RoleplayManager.WantedList.Count <= 0)
                Message.Append("Nadie está siendo buscado ahora mismo.\n");

            lock (RoleplayManager.WantedList.Values)
            {
                foreach (var Wanted in RoleplayManager.WantedList.Values)
                {
                    StringBuilder WantedStar = new StringBuilder();
                    if (Wanted.WantedLevel == 1) WantedStar.Append("¥");
                    if (Wanted.WantedLevel == 2) WantedStar.Append("¥¥");
                    if (Wanted.WantedLevel == 3) WantedStar.Append("¥¥¥");
                    if (Wanted.WantedLevel == 4) WantedStar.Append("¥¥¥¥");
                    if (Wanted.WantedLevel == 5) WantedStar.Append("¥¥¥¥¥");
                    if (Wanted.WantedLevel == 6) WantedStar.Append("¥¥¥¥¥¥");
                    Message.Append(PlusEnvironment.GetHabboById(Convert.ToInt32(Wanted.UserId)).Username + ": " + WantedStar + " Nivel de Búsqueda.\n");
                    Room RoomSeen = RoleplayManager.GenerateRoom(Convert.ToInt32(Wanted.LastSeenRoom));
                    if(RoomSeen != null)
                        Message.Append("----------\nÚltima vez vist@ en: " + RoomSeen.Name + "\n\n");
                }
            }
            Session.SendMessage(new MOTDNotificationComposer(Message.ToString()));
        }
    }
}