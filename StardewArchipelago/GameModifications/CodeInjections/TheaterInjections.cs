﻿using System;
using Microsoft.Xna.Framework;
using StardewArchipelago.Archipelago;
using StardewModdingAPI;
using StardewValley;
using StardewValley.Events;
using StardewValley.Locations;
using xTile.Tiles;

namespace StardewArchipelago.GameModifications.CodeInjections
{
    public class TheaterInjections
    {
        public const string MOVIE_THEATER_ITEM = "Progressive Movie Theater";
        private const string MOVIE_THEATER_MAIL = "ccMovieTheater";
        private const string ABANDONED_JOJA_MART = "AbandonedJojaMart";
        private const string MOVIE_THEATER = "MovieTheater";
        private const int CC_EVENT_ID = 191393;
        private static IMonitor _monitor;
        private static IModHelper _modHelper;
        private static ArchipelagoClient _archipelago;

        private static Point GetMissingBundleTile(GameLocation location) => location is MovieTheater ? new Point(17, 8) : new Point(8, 8);

        public static void Initialize(IMonitor monitor, IModHelper modHelper, ArchipelagoClient archipelago)
        {
            _monitor = monitor;
            _modHelper = modHelper;
            _archipelago = archipelago;
        }

        // public static FarmEvent pickFarmEvent()
        public static void PickFarmEvent_BreakJojaDoor_Postfix(ref FarmEvent __result)
        {
            try
            {
                if (Game1.weddingToday || __result != null || !_archipelago.HasReceivedItem(MOVIE_THEATER_ITEM))
                {
                    return;
                }

                if ((Game1.isRaining || Game1.isLightning || Game1.IsWinter) && !Game1.MasterPlayer.mailReceived.Contains("abandonedJojaMartAccessible"))
                {
                    __result = new WorldChangeEvent(12);
                    return;
                }

                if (_archipelago.GetReceivedItemCount(MOVIE_THEATER_ITEM) < 2)
                {
                    return;
                }

                if (!Game1.player.mailReceived.Contains("ccMovieTheater%&NL&%") && !Game1.player.mailReceived.Contains("ccMovieTheater"))
                {
                    __result = new WorldChangeEvent(11);
                    Game1.player.mailReceived.Add("ccMovieTheater");
                    return;
                }

                return;
            }
            catch (Exception ex)
            {
                _monitor.Log($"Failed in {nameof(PickFarmEvent_BreakJojaDoor_Postfix)}:\n{ex}", LogLevel.Error);
                return;
            }
        }

        // public virtual void MakeMapModifications(bool force = false)
        public static bool MakeMapModifications_PlaceMissingBundleNote_Prefix(GameLocation __instance, bool force)
        {
            try
            {
                if (__instance.Name != ABANDONED_JOJA_MART && __instance.Name != MOVIE_THEATER)
                {
                    return true; // run original logic
                }

                var abandonedJojaMart = Game1.getLocationFromName(ABANDONED_JOJA_MART);
                var junimoNotePoint = GetMissingBundleTile(__instance);

                if (Game1.MasterPlayer.hasOrWillReceiveMail("apccMovieTheater"))
                {
                    __instance.removeTile(junimoNotePoint.X, junimoNotePoint.Y, "Buildings");
                    return false; // don't run original logic
                }

                if (__instance.map.TileSheets.Count < 3)
                {
                    var abandonedJojaIndoorTileSheet = abandonedJojaMart.map.GetTileSheet("indoor");

                    // aaa is to make it get sorted to index 0, because the dumbass CC assumes the first tilesheet is the correct one
                    var indoorTileSheet = new TileSheet("aaa" + abandonedJojaIndoorTileSheet.Id, __instance.map, abandonedJojaIndoorTileSheet.ImageSource, abandonedJojaIndoorTileSheet.SheetSize, abandonedJojaIndoorTileSheet.TileSize);
                    __instance.map.AddTileSheet(indoorTileSheet);
                }

                var junimoNoteTileFrames = CommunityCenter.getJunimoNoteTileFrames(0, __instance.map);
                var layerId = "Buildings";
                __instance.map.GetLayer(layerId).Tiles[junimoNotePoint.X, junimoNotePoint.Y] = new AnimatedTile(__instance.map.GetLayer(layerId), junimoNoteTileFrames, 70L);

                return false; // don't run original logic
            }
            catch (Exception ex)
            {
                _monitor.Log($"Failed in {nameof(MakeMapModifications_PlaceMissingBundleNote_Prefix)}:\n{ex}", LogLevel.Error);
                return true; // run original logic
            }
        }

        // public bool checkTileIndexAction(int tileIndex)
        public static bool CheckTileIndexAction_InteractWithMissingBundleNote_Prefix(GameLocation __instance, int tileIndex, ref bool __result)
        {
            try
            {
                if (__instance.Name != ABANDONED_JOJA_MART && __instance.Name != MOVIE_THEATER)
                {
                    return true; // run original logic
                }

                switch (tileIndex)
                {
                    // I think these are... bundle animation sprites... yeah wtf
                    case 1799:
                    case 1824:
                    case 1825:
                    case 1826:
                    case 1827:
                    case 1828:
                    case 1829:
                    case 1830:
                    case 1831:
                    case 1832:
                    case 1833:
                        // Game1.activeClickableMenu = (IClickableMenu) new JunimoNoteMenu(6, (Game1.getLocationFromName("CommunityCenter") as CommunityCenter).bundlesDict())
                        ((AbandonedJojaMart)(Game1.getLocationFromName("AbandonedJojaMart"))).checkBundle();
                        __result = true;
                        return false; // don't run original logic
                }

                return false; // don't run original logic
            }
            catch (Exception ex)
            {
                _monitor.Log($"Failed in {nameof(CheckTileIndexAction_InteractWithMissingBundleNote_Prefix)}:\n{ex}", LogLevel.Error);
                return true; // run original logic
            }
        }

        // private void doRestoreAreaCutscene()
        public static bool DoRestoreAreaCutscene_InteractWithMissingBundleNote_Prefix(AbandonedJojaMart __instance)
        {
            try
            {
                // Game1.player.freezePause = 1000;
                var junimoNotePoint = GetMissingBundleTile(__instance);
                DelayedAction.removeTileAfterDelay(junimoNotePoint.X, junimoNotePoint.Y, 100, Game1.currentLocation, "Buildings");
                // Game1.getLocationFromName(nameof(AbandonedJojaMart)).startEvent(new Event(Game1.content.Load<Dictionary<string, string>>("Data\\Events\\AbandonedJojaMart")["missingBundleComplete"], 192393));
                
                Game1.addMailForTomorrow("apccMovieTheater", true, true);
                return false; // don't run original logic
            }
            catch (Exception ex)
            {
                _monitor.Log($"Failed in {nameof(DoRestoreAreaCutscene_InteractWithMissingBundleNote_Prefix)}:\n{ex}", LogLevel.Error);
                return true; // run original logic
            }
        }

        private static bool hasSeenCCCeremonyCutscene;
        private static bool hasPamHouseUpgrade;
        private static bool hasShortcuts;

        // public override void MakeMapModifications(bool force = false)
        public static bool MakeMapModifications_JojamartAndTheater_Prefix(Town __instance, bool force)
        {
            try
            {
                if (_archipelago.GetReceivedItemCount(MOVIE_THEATER_ITEM) >= 2)
                {
                    var rectangle = new Microsoft.Xna.Framework.Rectangle(84, 41, 27, 15);
                    __instance.ApplyMapOverride("Town-Theater", rectangle, rectangle);
                    return true; // run original logic
                }

                if (_archipelago.GetReceivedItemCount(MOVIE_THEATER_ITEM) >= 1)
                {
                    // private void showDestroyedJoja()
                    var showDestroyedJojaMethod = _modHelper.Reflection.GetMethod(__instance, "showDestroyedJoja");
                    showDestroyedJojaMethod.Invoke();
                    if (Utility.doesMasterPlayerHaveMailReceivedButNotMailForTomorrow("abandonedJojaMartAccessible"))
                    {
                        __instance.crackOpenAbandonedJojaMartDoor();
                    }
                }
                else
                {
                    hasSeenCCCeremonyCutscene = Utility.HasAnyPlayerSeenEvent(CC_EVENT_ID);
                    if (hasSeenCCCeremonyCutscene)
                    {
                        Game1.player.eventsSeen.Remove(CC_EVENT_ID);
                    }
                }

                return true; // run original logic
            }
            catch (Exception ex)
            {
                _monitor.Log($"Failed in {nameof(MakeMapModifications_JojamartAndTheater_Prefix)}:\n{ex}", LogLevel.Error);
                return true; // run original logic
            }
        }

        // public override void MakeMapModifications(bool force = false)
        public static void MakeMapModifications_JojamartAndTheater_Postfix(Town __instance, bool force)
        {
            try
            {
                if (hasSeenCCCeremonyCutscene && !Game1.player­.eventsSeen.Contains(CC_EVENT_ID))
                {
                    Game1.player.eventsSeen.Add(CC_EVENT_ID);
                }

                return;
            }
            catch (Exception ex)
            {
                _monitor.Log($"Failed in {nameof(MakeMapModifications_JojamartAndTheater_Postfix)}:\n{ex}", LogLevel.Error);
                return;
            }
        }
    }
}