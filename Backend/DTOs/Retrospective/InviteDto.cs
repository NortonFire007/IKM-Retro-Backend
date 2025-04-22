namespace IKM_Retro.DTOs.Retrospective;

public class InviteDto
{
    public required string Code { get; set; }
    public DateTime? ExpiresAt { get; set; }
}