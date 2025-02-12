using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Pathoschild.Stardew.Common;
using StardewValley;
using StardewValley.Pathfinding;

namespace Pathoschild.Stardew.LookupAnything.Framework.Fields;

/// <summary>A metadata field which shows an NPC's schedule.</summary>
internal class ScheduleField : GenericField
{
    /*********
    ** Public methods
    *********/
    /// <summary>Construct an instance.</summary>
    /// <param name="npc">The NPC whose schedule to display.</param>
    /// <param name="gameHelper">Provides utility methods for interacting with the game code.</param>
    public ScheduleField(NPC npc, GameHelper gameHelper)
        : base(I18n.Npc_Schedule(), ScheduleField.GetText(npc, gameHelper)) { }


    /*********
    ** Private methods
    *********/
    /// <summary>Get the text to display.</summary>
    /// <param name="npc">The NPC whose schedule to display.</param>
    /// <param name="gameHelper">Provides utility methods for interacting with the game code.</param>
    private static IEnumerable<IFormattedText> GetText(NPC npc, GameHelper gameHelper)
    {
        ScheduleEntry[] schedule = ScheduleField.FormatSchedule(npc.Schedule).ToArray();

        // current location
        {
            string locationName = npc.currentLocation is not null
                ? gameHelper.GetLocationDisplayName(npc.currentLocation.Name, npc.currentLocation.GetData())
                : "???";

            yield return new FormattedText(I18n.Npc_Schedule_CurrentPosition(locationName: locationName, x: npc.TilePoint.X, y: npc.TilePoint.Y));
            yield return new FormattedText(Environment.NewLine + Environment.NewLine);
        }

        // schedule entries
        if (schedule.Length > 0)
        {
            for (int i = 0; i < schedule.Length; i++)
            {
                (int time, SchedulePathDescription entry) = schedule[i];

                string locationName = gameHelper.GetLocationDisplayName(entry.targetLocationName, Game1.getLocationFromName(entry.targetLocationName)?.GetData());
                bool isStarted = Game1.timeOfDay >= time;
                bool isFinished = i < schedule.Length - 1 && Game1.timeOfDay >= schedule[i + 1].Time;

                Color textColor = isStarted
                    ? (isFinished ? Color.Gray : Color.Green)
                    : Color.Black;

                if (i > 0)
                    yield return new FormattedText(Environment.NewLine);
                yield return new FormattedText(I18n.Npc_Schedule_Entry(time: CommonHelper.FormatTime(time), locationName: locationName, x: entry.targetTile.X, y: entry.targetTile.Y), textColor);
            }
        }
        else
            yield return new FormattedText(I18n.Npc_Schedule_NoEntries());
    }

    /// <summary>Returns a collection of schedule entries sorted by time. Consecutive entries with the same target location are omitted.</summary>
    /// <param name="schedule">The schedule to format.</param>
    private static IEnumerable<ScheduleEntry> FormatSchedule(Dictionary<int, SchedulePathDescription> schedule)
    {
        List<int> sortedKeys = [.. schedule.Keys.OrderBy(key => key)];
        string prevTargetLocationName = string.Empty;

        foreach (int time in sortedKeys)
        {
            // skip if the entry does not exist or the previous entry was for the same location
            if (!schedule.TryGetValue(time, out SchedulePathDescription? entry) || entry.targetLocationName == prevTargetLocationName)
                continue;

            prevTargetLocationName = entry.targetLocationName;
            yield return new ScheduleEntry(time, entry);
        }
    }

    /// <summary>An entry in an NPC's schedule.</summary>
    /// <param name="Time">The time that the event starts.</param>
    /// <param name="Description">A description of the event.</param>
    private record ScheduleEntry(int Time, SchedulePathDescription Description);
}
