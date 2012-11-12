//@author A0080860H

namespace Calendo.AutoSuggest
{
    /// <summary>
    /// Master entries appear as main suggestions when the user starts typing a command.
    /// Detail entries are command-specific guides for the user.
    /// </summary>
    public enum EntryType
    {
        Master,
        Detail
    }

    public class AutoSuggestEntry
    {
        public string Command { get; set; }
        public string Description { get; set; }
        public EntryType Type { get; set; }
        public string[] Aliases { get; set; }
        public bool IsMaster { get { return Type == EntryType.Master; } }
        public bool HasAliases
        {
            get
            {
                if (Aliases != null)
                {
                    return IsMaster && Aliases.Length > 1;
                }
                else
                {
                    return false;
                }
            }
        }

        public AutoSuggestEntry(string command, string description, EntryType type, string[] aliases)
        {
            Command = command;
            Description = description;
            Type = type;
            Aliases = aliases;
        }
    }
}
