using IKM_Retro.Enums;

namespace IKM_Retro.DTOs.Retrospective.ActionItem;

public class UpdateActionItemDto
{
    public string? Description { get; set; }
    public ActionItemPriority? Priority { get; set; }
    public ActionItemStatus? Status { get; set; }
    public DateTime? DueDate { get; set; }
    public Guid? AssignedUserId { get; set; }
}