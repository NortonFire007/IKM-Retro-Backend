using System.Linq.Expressions;
using IKM_Retro.DTOs.Base;
using IKM_Retro.DTOs.Retrospective;
using IKM_Retro.Models.Retro;

public class BaseGroupDTO : AuditableDTO<int>
{
    public Guid RetrospectiveId { get; set; }

    public required string Name { get; set; }

    public string? Description { get; set; }

    public int OrderPosition { get; set; }

    public static Expression<Func<Group, BaseGroupDTO>> Selector
    {
        get
        {
            return x => new BaseGroupDTO
            {
                Id = x.Id,
                RetrospectiveId = x.RetrospectiveId,
                Name = x.Name,
                Description = x.Description,
                OrderPosition = x.OrderPosition,
                CreatedAt = x.CreatedAt,
                UpdatedAt = x.UpdatedAt,
            };
        }
    }
}

public class GroupDTO : BaseGroupDTO
{
    public required BaseRetrospectiveDTO Retrospective { get; set; }
}
