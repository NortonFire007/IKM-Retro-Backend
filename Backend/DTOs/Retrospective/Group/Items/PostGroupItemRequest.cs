namespace IKM_Retro.DTOs.Retrospective.Group.Items;

public class PostGroupItemRequest
{
    public int GroupId { get; set; }
    public string? Content { get; set; }
    public int OrderPosition { get; set; }
    public bool IsHidden { get; set; } = false;
}