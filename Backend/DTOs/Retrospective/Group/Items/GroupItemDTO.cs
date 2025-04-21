using IKM_Retro.DTOs.Base;
using IKM_Retro.Enums;
using IKM_Retro.DTOs.User;
using System.Linq.Expressions;
using IKM_Retro.Models.Retro;

namespace IKM_Retro.DTOs.Retrospective.Group.Items
{
    public class BaseGroupItemDTO: AuditableDTO<int>
    {
        public int GroupId { get; set; }
        public string? Description { get; set; }

        public string? UserId { get; set; }

        public DateTime? DueDate { get; set; }

        public GroupItemStatusEnum Status { get; set; } = GroupItemStatusEnum.Open;

        public int OrderPosition { get; set; }

        public bool IsHidden { get; set; } = false;

        public static Expression<Func<GroupItem, BaseGroupItemDTO>> Selector => x => new BaseGroupItemDTO
        {
            Id = x.Id,
            GroupId = x.GroupId,
            UserId = x.UserId,
            DueDate = x.DueDate,
            Status = x.Status,
            OrderPosition = x.OrderPosition,
            IsHidden = x.IsHidden,
            CreatedAt = x.CreatedAt,
            UpdatedAt = x.UpdatedAt,
            
        };
    }

    public class GroupItemDTO : BaseGroupItemDTO
    {
        public required BaseGroupDTO Group { get; set; }

        public required BaseUserDTO AssignedUser { get; set; }


    }
}
