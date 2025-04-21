using IKM_Retro.DTOs.Base;
using IKM_Retro.DTOs.Retrospective.Group;
using IKM_Retro.DTOs.User;
using IKM_Retro.Enums;

namespace IKM_Retro.DTOs.Retrospective
{
    public class BaseRetrospectiveDTO: AuditableDTO<string>
    {
        public required string Title { get; set; }
        public DateTime StartDate { get; set; } = DateTime.UtcNow;
        public DateTime? EndDate { get; set; }
        public bool IsActive { get; set; } = true;
        public required TemplateTypeEnum Template { get; set; }
        public string? InviteLink { get; set; }
        public int OwnerId { get; set; }

    }

    public class RetrospectiveDTO: BaseRetrospectiveDTO
    {
        public required BaseGroupDTO Group { get; set; }

        public required BaseUserDTO AssignedUser { get; set; }
    }
}
