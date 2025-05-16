namespace IKM_Retro.DTOs.Retrospective.Comments;

public class PostActionItemCommentRequest
{
    public int ActionItemId { get; set; }
    public string Content { get; set; } = null!;
}

public class BaseActionItemCommentDto
{
    public int Id { get; set; }
    public int ActionItemId { get; set; }
    public string? UserId { get; set; }
    public string Content { get; set; } = null!;
    public DateTime CreatedAt { get; set; }
}