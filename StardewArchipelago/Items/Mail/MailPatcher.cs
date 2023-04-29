﻿using System;
using System.Collections.Generic;
using System.Linq;
using HarmonyLib;
using StardewModdingAPI;
using StardewValley;
using StardewValley.Menus;

namespace StardewArchipelago.Items.Mail
{
    public class MailPatcher
    {
        private static IMonitor _monitor;
        private static LetterActions _letterActions;
        private readonly Harmony _harmony;

        public MailPatcher(IMonitor monitor, LetterActions letterActions, Harmony harmony)
        {
            _monitor = monitor;
            _letterActions = letterActions;
            _harmony = harmony;
        }

        public void PatchMailBoxForApItems()
        {
            _harmony.Patch(
                original: AccessTools.Method(typeof(IClickableMenu), nameof(IClickableMenu.exitThisMenu)),
                postfix: new HarmonyMethod(typeof(MailPatcher), nameof(MailPatcher.ExitThisMenu_ApplyLetterAction_Postfix))
            );

            _harmony.Patch(
                original: AccessTools.Method(typeof(GameLocation), nameof(GameLocation.mailbox)),
                postfix: new HarmonyMethod(typeof(MailPatcher), nameof(MailPatcher.Mailbox_HideEmptyApLetters_Prefix))
            );
        }

        public static void ExitThisMenu_ApplyLetterAction_Postfix(IClickableMenu __instance, bool playSound)
        {
            try
            {
                if (__instance is not LetterViewerMenu letterMenuInstance || letterMenuInstance.mailTitle == null)
                {
                    return;
                }

                var title = letterMenuInstance.mailTitle;
                if (!MailKey.TryParse(title, out var apMailKey))
                {
                    return;
                }

                var apActionName = apMailKey.LetterOpenedAction;
                var apActionParameter = apMailKey.ActionParameter;

                if (string.IsNullOrWhiteSpace(apActionName))
                {
                    return;
                }

                _letterActions.ExecuteLetterAction(apActionName, apActionParameter);
            }
            catch (Exception ex)
            {
                _monitor.Log($"Failed in {nameof(ExitThisMenu_ApplyLetterAction_Postfix)}:\n{ex}", LogLevel.Error);
                return;
            }
        }

        // public void mailbox()
        public static bool Mailbox_HideEmptyApLetters_Prefix(GameLocation __instance)
        {
            try
            {
                CleanMailboxUntilNonEmptyLetter();
                return true; // run original logic
            }
            catch (Exception ex)
            {
                _monitor.Log($"Failed in {nameof(Mailbox_HideEmptyApLetters_Prefix)}:\n{ex}", LogLevel.Error);
                return true; // run original logic
            }
        }

        private static void CleanMailboxUntilNonEmptyLetter()
        {
            if (!ModEntry.Instance.State.HideEmptyArchipelagoLetters)
            {
                return;
            }

            var mailbox = Game1.mailbox;
            while (mailbox.Any())
            {
                var firstLetterInMailbox = Game1.mailbox.First();

                if (!MailKey.TryParse(firstLetterInMailbox, out var apMailKey))
                {
                    return;
                }

                if (!apMailKey.IsEmpty)
                {
                    return;
                }

                Game1.player.mailReceived.Add(firstLetterInMailbox);
                mailbox.RemoveAt(0);
            }
            
        }
    }
}
