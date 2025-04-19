using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using IKM_Retro.Models.Base;

namespace IKM_Retro.Models.Retro;
[Table("RetrospectiveGroups")]
public class Group : Auditable<int>
{
    [ForeignKey("Retrospective")]
    public Guid RetrospectiveId { get; set; }
    
    [Required, MaxLength(100)]
    public required string Name { get; set; }

    [MaxLength(1000)] 
    public string? Description { get; set; }

    public int OrderPosition { get; set; } 

    public ICollection<GroupItem> GroupItems { get; set; } = new List<GroupItem>();
    
    public required Retrospective Retrospective { get; set; }
}