namespace Utils
{
    using System;
    using System.Collections.Generic;
    using UnityEngine;

    public enum FailReason
    {
        PlayerDied,
        CatDied
    }
    public static class Phrases
    {
        public static Dictionary<FailReason, string> FailReasonDict = new Dictionary<FailReason, string>()
        {
            { FailReason.PlayerDied, "You are dead" },
            { FailReason.CatDied, "Kitten is dead" },
        };
    }
}
