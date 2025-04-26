using IKM_Retro.DTOs.Base;
using IKM_Retro.DTOs.Retrospective.Group.Items;

public class BaseGroupDTO : AuditableDTO<int>
{
    public Guid RetrospectiveId { get; set; }

    public required string Name { get; set; }

    public string? Description { get; set; }

    public int OrderPosition { get; set; }
    
}

public class GroupDTO : BaseGroupDTO
{
    public List<BaseGroupItemDTO> GroupItems { get; set; }
}
