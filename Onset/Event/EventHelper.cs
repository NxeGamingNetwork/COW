﻿using System;
using System.Collections.Generic;
using System.Text;

namespace Onset.Event
{
    /// <summary>
    /// The EventHelper has some utility functions for the event section of the onset server.
    /// </summary>
    public static class EventHelper
    {
        /// <summary>
        /// Returns a boolean whether the given event is a player event - so needs a player as the first argument - or not.
        /// </summary>
        /// <param name="type">The event type</param>
        /// <returns>True if it is a player event</returns>
        public static bool IsPlayerEvent(this EventType type)
        {
            return type == EventType.PlayerQuit;
        }
    }
}
