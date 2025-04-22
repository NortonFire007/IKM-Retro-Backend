using IKM_Retro.Enums;
using System.Collections.ObjectModel;

namespace IKM_Retro.Helpers.Factories
{
    public static class RetrospectiveTemplateFactory
    {
        public sealed class TemplateGroup
        {
            public required string Name { get; init; }
            public required string Description { get; init; }
            public int SortOrder { get; init; }
        }
        public sealed class TemplateData
        {
            public TemplateTypeEnum Type { get; init; }
            public required string DisplayName { get; init; }
            public required string Description { get; init; }
            public ReadOnlyCollection<TemplateGroup> Groups { get; init; } = null!;
        }

        private static readonly IReadOnlyDictionary<TemplateTypeEnum, TemplateData> _templates =
            new Dictionary<TemplateTypeEnum, TemplateData>
            {
                [TemplateTypeEnum.StartStopContinue] = new()
                {
                    Type = TemplateTypeEnum.StartStopContinue,
                    DisplayName = "Start/Stop/Continue",
                    Description = "Classic retrospective format focusing on actions to start, stop, or continue",
                    Groups = new List<TemplateGroup>
                    {
                        new() { Name = "🟢 Start", Description = "Ideas to begin implementing", SortOrder = 1 },
                        new() { Name = "⛔ Stop", Description = "Things to stop doing", SortOrder = 2 },
                        new() { Name = "🔄 Continue", Description = "Things to keep doing", SortOrder = 3 }
                    }.AsReadOnly()
                },
                [TemplateTypeEnum.GladSadMad] = new()
                {
                    Type = TemplateTypeEnum.GladSadMad,
                    DisplayName = "Glad/Sad/Mad",
                    Description = "Emotion-focused retrospective examining what makes team members glad, sad, or mad",
                    Groups = new List<TemplateGroup>
                    {
                        new() { Name = "😀 Glad", Description = "Things that make us glad", SortOrder = 3 },
                        new() { Name = "😢 Sad", Description = "Things that make us sad", SortOrder = 2 },
                        new() { Name = "😡 Mad", Description = "Things that make us mad", SortOrder = 1 }
                    }.AsReadOnly()
                },
                [TemplateTypeEnum.StartStopContinueChange] = new()
                {
                    Type = TemplateTypeEnum.StartStopContinueChange,
                    DisplayName = "Start/Stop/Continue/Change",
                    Description = "Extended version of Start/Stop/Continue with an added Change category",
                    Groups = new List<TemplateGroup>
                    {
                        new() { Name = "🟢 Start", Description = "Ideas to begin implementing", SortOrder = 1 },
                        new() { Name = "⛔ Stop", Description = "Things to stop doing", SortOrder = 2 },
                        new() { Name = "🔄 Continue", Description = "Things to keep doing", SortOrder = 3 },
                        new() { Name = "🔧 Change", Description = "Things to change or adjust", SortOrder = 4 }
                    }.AsReadOnly()
                },
                [TemplateTypeEnum.KeepAddLessMore] = new()
                {
                    Type = TemplateTypeEnum.KeepAddLessMore,
                    DisplayName = "Keep/Add/Less/More",
                    Description = "Comprehensive format examining what to keep, add, reduce, or increase",
                    Groups = new List<TemplateGroup>
                    {
                        new() { Name = "✔️ Keep Doing", Description = "Things to keep doing", SortOrder = 1 },
                        new() { Name = "⛔ Stop Doing", Description = "Things to stop doing", SortOrder = 4 },
                        new() { Name = "➖ Less Of", Description = "Things to do less of", SortOrder = 3 },
                        new() { Name = "➕ More Of", Description = "Things to do more of", SortOrder = 2 },
                        new() { Name = "✅ Start Doing", Description = "Things to start doing", SortOrder = 5 }
                    }.AsReadOnly()
                }
            };
        public static TemplateData Create(TemplateTypeEnum templateType)
        {
            if (_templates.TryGetValue(templateType, out var template))
            {
                return template;
            }

            throw new ArgumentOutOfRangeException(nameof(templateType), $"Unsupported template type: {templateType}");
        }

        public static IReadOnlyCollection<TemplateData> GetAllTemplates() => _templates.Values.ToList().AsReadOnly();
    }
}