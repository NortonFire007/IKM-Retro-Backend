namespace IKM_Retro.DTOs.Retrospective.Comments;

public class PostCommentRequest
{
    public int GroupItemId { get; set; }
    public string Content { get; set; } = null!;
    public bool IsAnonymous { get; set; }
}

public class BaseCommentDTO
{
    public int Id { get; set; }
    public int GroupItemId { get; set; }
    public string? UserId { get; set; }
    public string Content { get; set; } = null!;
    public int Likes { get; set; }
    public bool IsAnonymous { get; set; }
    public DateTime CreatedAt { get; set; }
}