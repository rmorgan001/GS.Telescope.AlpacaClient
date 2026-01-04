using System;

namespace GS.Telescope.AlpacaClient.Models
{
    /// <summary>
    /// Main menu list items
    /// </summary>
    /// <param name="type">PageViewModel</param>
    /// <param name="kind">icon name</param>
    /// <param name="name">never changes</param>
    /// <param name="label">display text</param>
    public class MainMenuItemTemplate(Type type, string kind, string name, string label)
    {
        public Type ModelType { get; } = type;
        public string Kind { get; } = kind;
        public string Name { get; } = name;
        public string Label { get; } = label;
    }
}
