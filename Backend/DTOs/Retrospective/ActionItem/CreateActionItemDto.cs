using IKM_Retro.Enums;

namespace IKM_Retro.DTOs.Retrospective.ActionItem;

public class CreateActionItemDto
{
    public Guid RetrospectiveId { get; set; }
    public string Description { get; set; } = null!;
    public Guid? AssignedUserId { get; set; }
    public DateTime? DueDate { get; set; }
    public ActionItemStatus Status { get; set; } = ActionItemStatus.Pending;
    public ActionItemPriority Priority { get; set; } = ActionItemPriority.Medium;
}