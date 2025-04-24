using IKM_Retro.DTOs.Base;
using IKM_Retro.DTOs.User;

namespace IKM_Retro.DTOs.Retrospective.Group.Items;

public class BaseGroupItemDTO: AuditableDTO<int>
{
    public int GroupId { get; set; }
    public string? Content { get; set; }
    public string? UserId { get; set; }
    public DateTime? DueDate { get; set; }
    public int OrderPosition { get; set; }
    public bool IsHidden { get; set; } = false;
}

public class GroupItemDTO : BaseGroupItemDTO
{
    public required BaseGroupDTO Group { get; set; }
    public required BaseUserDTO AssignedUser { get; set; }
}

