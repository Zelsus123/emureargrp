﻿using System;
using System.Data;
using System.Linq;
using System.Drawing;
using System.Collections.Generic;
using System.Collections.Concurrent;
using Plus.HabboHotel.GameClients;
using Plus.HabboHotel.Groups;
using Plus.HabboHotel.Items;
using Plus.HabboHotel.Rooms;
using Plus.Communication.Packets.Outgoing.Rooms.Engine;
using Plus.Communication.Packets.Outgoing.Rooms.Chat;
using Plus.Communication.Packets.Outgoing.Notifications;
using Plus.Communication.Packets.Outgoing.Navigator;
using Plus.HabboRoleplay.Weapons;
using Plus.Database.Interfaces;
using Plus.HabboHotel.Pathfinding;
using Plus.HabboRoleplay.Timers;

namespace Plus.HabboRoleplay.Misc
{
    public class WorkManager
    {
        /// <summary>
        /// Thread-safe dictionary containing users who are working in the specific corp
        /// </summary>
        public static ConcurrentDictionary<int, List<int>> WorkingUsersList = new ConcurrentDictionary<int, List<int>>();

        /// <summary>
        /// Add new user to the working users list
        /// </summary>
        public static void AddWorkerToList(GameClient Session)
        {
            if (Session.GetPlay().JobId <= 0)
                return;

            Group Job = PlusEnvironment.GetGame().GetGroupManager().GetJob(Session.GetPlay().JobId);

            if (Job == null)
                return;

            if (WorkingUsersList.ContainsKey(Job.Id))
            {
                List<int> Workers = WorkingUsersList[Job.Id];

                if (Workers.Contains(Session.GetHabbo().Id))
                    return;

                Workers.Add(Session.GetHabbo().Id);
                WorkingUsersList.TryUpdate(Job.Id, Workers, WorkingUsersList[Job.Id]);
            }
            else
            {
                List<int> Workers = new List<int>();

                Workers.Add(Session.GetHabbo().Id);
                WorkingUsersList.TryAdd(Job.Id, Workers);
            }

            var CurrentWorkers = WorkingUsersList[Job.Id];
            /*
            if (CurrentWorkers != null)
            {
                if (CurrentWorkers.Count > 0)
                {
                    foreach (var Bot in Bots.Manager.RoleplayBotManager.DeployedRoleplayBots.Values)
                    {
                        if (Bot.GetBotRoleplay().AIType == Bots.RoleplayBotAIType.DELIVERY)
                            continue;

                        if (Bot.GetBotRoleplay().RoamBot)
                            continue;

                        if (Bot.GetBotRoleplay().Corporation == Job.Id && Bot.GetBotRoleplayAI().OnDuty)
                            Bot.GetBotRoleplayAI().StopActivities();
                    }
                }
            }
            */
        }

        /// <summary>
        /// Removes a user from the working users list
        /// </summary>
        public static void RemoveWorkerFromList(GameClient Session)
        {
            Session.GetPlay().HospReanim = 0;

            if (Session.GetPlay().JobId <= 0)
                return;

            if (Session.GetPlay().WateringCan)
            {
                Session.GetPlay().WateringCan = false;
                if (Session.GetRoomUser() != null && Session.GetRoomUser().CurrentEffect == 192)
                    Session.GetRoomUser().ApplyEffect(0);
            }

            Group Job = PlusEnvironment.GetGame().GetGroupManager().GetJob(Session.GetPlay().JobId);

            if (Job == null)
                return;

            // Ponemos un cooldown para médicos que quieran abusar de su inmunidad para agredir
            if (Job.Name.Contains("Hospital"))
            {
                Session.GetPlay().CooldownManager.CreateCooldown("fist", 1000, 120);
                Session.GetPlay().CooldownManager.CreateCooldown("gun", 1000, 120);
            }

            if (!WorkingUsersList.ContainsKey(Job.Id))
                return;

            List<int> Workers = WorkingUsersList[Job.Id];

            if (!Workers.Contains(Session.GetHabbo().Id))
                return;

            Workers.Remove(Session.GetHabbo().Id);
            WorkingUsersList.TryUpdate(Job.Id, Workers, WorkingUsersList[Job.Id]);

            var CurrentWorkers = WorkingUsersList[Job.Id];
            /*
            if (CurrentWorkers != null)
            {
                if (CurrentWorkers.Count == 0)
                {
                    foreach (var Bot in Bots.Manager.RoleplayBotManager.DeployedRoleplayBots.Values)
                    {
                        if (Bot.GetBotRoleplay().AIType == Bots.RoleplayBotAIType.DELIVERY)
                            continue;

                        if (Bot.GetBotRoleplay().RoamBot)
                            continue;

                        if (Bot.GetBotRoleplay().Corporation == Job.Id && !Bot.GetBotRoleplayAI().OnDuty)
                            Bot.GetBotRoleplayAI().StopActivities();
                    }
                }
            }
            */
        }
    }
}