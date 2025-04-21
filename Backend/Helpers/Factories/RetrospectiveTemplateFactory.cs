using IKM_Retro.Enums;

namespace IKM_Retro.Helpers.Factories
{
    public static class RetrospectiveTemplateFactory
    {
        public class TemplateGroup
        {
            public required string Name { get; set; }
            public required string Description { get; set; }
        }

        public class TemplateData
        {
            public TemplateTypeEnum Type { get; set; }
            public List<TemplateGroup> Groups { get; set; } = [];
        }

        public static TemplateData Create(TemplateTypeEnum templateType)
        {
            return templateType switch
            {
                TemplateTypeEnum.StartStopContinue => new TemplateData
                {
                    Type = TemplateTypeEnum.StartStopContinue,
                    Groups =
                    [
                        new TemplateGroup { Name = "🟢 Start", Description = "Ideas to begin implementing" },
                        new TemplateGroup { Name = "⛔ Stop", Description = "Things to stop doing" },
                        new TemplateGroup { Name = "🔄 Continue", Description = "Things to keep doing" }
                    ]
                },

                TemplateTypeEnum.GladSadMad => new TemplateData
                {
                    Type = TemplateTypeEnum.GladSadMad,
                    Groups =
                    [
                        new TemplateGroup { Name = "😡 Mad", Description = "Things that make us mad" },
                        new TemplateGroup { Name = "😢 Sad", Description = "Things that make us sad" },
                        new TemplateGroup { Name = "😀 Glad", Description = "Things that make us glad" }
                    ]
                },

                TemplateTypeEnum.StartStopContinueChange => new TemplateData
                {
                    Type = TemplateTypeEnum.StartStopContinueChange,
                    Groups =
                    [
                        new TemplateGroup { Name = "🟢 Start", Description = "Ideas to begin implementing" },
                        new TemplateGroup { Name = "⛔ Stop", Description = "Things to stop doing" },
                        new TemplateGroup { Name = "🔄 Continue", Description = "Things to keep doing" },
                        new TemplateGroup { Name = "🔧 Change", Description = "Things to change or adjust" }
                    ]
                },

                TemplateTypeEnum.KeepAddLessMore => new TemplateData
                {
                    Type = TemplateTypeEnum.KeepAddLessMore,
                    Groups =
                    [
                        new TemplateGroup { Name = "✔️ Keep Doing", Description = "Things to keep doing" },
                        new TemplateGroup { Name = "➖ Less Of", Description = "Things to do less of" },
                        new TemplateGroup { Name = "➕ More Of", Description = "Things to do more of" },
                        new TemplateGroup { Name = "⛔ Stop Doing", Description = "Things to stop doing" },
                        new TemplateGroup { Name = "✅ Start Doing", Description = "Things to start doing" }
                    ]
                },

                _ => throw new ArgumentOutOfRangeException(nameof(templateType), $"Unsupported template type: {templateType}")
            };
        }
    }
}