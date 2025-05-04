using IKM_Retro.Enums;

namespace IKM_Retro.DTOs.Retrospective.Group.Items;

public class ConvertGroupItemToActionItemDto
{
    public ActionItemStatus? Status { get; set; }
    public ActionItemPriority? Priority { get; set; }
    public Guid? AssignedUserId { get; set; }
}