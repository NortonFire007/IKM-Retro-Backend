using IKM_Retro.Enums;
using IKM_Retro.Models.Base;

namespace IKM_Retro.Models.Retro;

public class ActionItem : Auditable<int>
{
    public Guid ActionId { get; set; }

    public Guid RetrospectiveId { get; set; }
    
    public string Description { get; set; } = null!;

    public Guid? AssignedUserId { get; set; }

    public DateTime? DueDate { get; set; }

    public ActionItemStatus Status { get; set; } = ActionItemStatus.Pending;

    public ActionItemPriority Priority { get; set; } = ActionItemPriority.Medium;

    public Retrospective Retrospective { get; set; } = null!;
    
    public User? User { get; set; }
}