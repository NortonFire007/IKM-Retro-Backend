using IKM_Retro.Enums;

namespace IKM_Retro.DTOs.Retrospective;

public class PostRetrospectiveBody
{
    public required string Title { get; set; }
    public required TemplateTypeEnum TemplateType { get; set; }
}