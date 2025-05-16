using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using IKM_Retro.Models.Base;

namespace IKM_Retro.Models.Retro;

[Table("RetrospectiveGroupItems")]
public class GroupItem : Auditable<int>
{
    [ForeignKey("Group")] public int GroupId { get; set; }

    [Required, MaxLength(250)] public string? Content { get; set; }

    [ForeignKey("User"), MaxLength(128)] public string? UserId { get; set; }

    public DateTime? DueDate { get; set; }

    public int OrderPosition { get; set; }

    public bool IsHidden { get; set; }

    public ICollection<GroupItemComment?>? Comments { get; set; }
    public Group? Group { get; set; }

    public User? AssignedUser { get; set; }
}