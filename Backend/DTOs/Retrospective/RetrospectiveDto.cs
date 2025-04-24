using IKM_Retro.DTOs.Base;
using IKM_Retro.DTOs.User;
using IKM_Retro.Enums;

namespace IKM_Retro.DTOs.Retrospective;

public class BaseRetrospectiveDTO : AuditableDTO<Guid>
{
    public required string Title { get; set; }
    public DateTime StartDate { get; set; } = DateTime.UtcNow;
    public DateTime? EndDate { get; set; }
    public bool IsActive { get; set; } = true;
    public required TemplateTypeEnum Template { get; set; }
    public string CreatorUserId { get; set; }
}

public class RetrospectiveDto : BaseRetrospectiveDTO
{
    public required List<BaseGroupDTO> Groups { get; set; }

    public required BaseUserDTO AssignedUser { get; set; }
        
    public required List<BaseUserDTO> AssignedUsers { get; set; }
}