﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Calendo.AutoSuggest
{
    /// <summary>
    /// MASTER entries appear as main suggestions when the user starts typing a command.
    /// DETAIL entries are command-specific guides for the user.
    /// </summary>
    public enum EntryType
    {
        MASTER,
        DETAIL
    }

    class AutoSuggestEntry
    {
        public string Command { get; set; }
        public string Description { get; set; }
        public EntryType Type { get; set; }

        public AutoSuggestEntry(string command, string description, EntryType type)
        {
            Command = command;
            Description = description;
            Type = type;
        }
    }
}