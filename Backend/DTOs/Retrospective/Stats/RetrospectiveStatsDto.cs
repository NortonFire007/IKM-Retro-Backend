namespace IKM_Retro.DTOs.Retrospective.Stats;

public class RetrospectiveStatsDto
{
    public int TotalCards { get; set; }
    public int TotalComments { get; set; }
    public int TotalLikes { get; set; }
    public int TotalActionItems { get; set; }
    public int CompletedActionItems { get; set; }

    public double AverageActionItemsPerBoard { get; set; }
    public double AverageCompletedActionItemsPerBoard { get; set; }
    public double AverageLikesPerCard { get; set; }
    public double AverageCommentsPerCard { get; set; }

    public double AverageCardsPerBoard { get; set; }
    public double AverageCommentsPerBoard { get; set; }
    public double AverageLikesPerBoard { get; set; }
    public double ActionItemCompletionRate { get; set; }

    public Dictionary<string, int> TemplatePopularity { get; set; } = new();
}