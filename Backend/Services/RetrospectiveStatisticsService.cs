using IKM_Retro.Data;
using IKM_Retro.DTOs.Retrospective.Stats;
using IKM_Retro.Enums;
using IKM_Retro.Models.Retro;
using Microsoft.EntityFrameworkCore;

namespace IKM_Retro.Services;

public class RetrospectiveStatisticsService
{
    private readonly RetroDbContext _ctx;

    private static double CalculateAverage(int numerator, int denominator, int decimalPlaces = 2)
    {
        return denominator > 0 ? Math.Round((double)numerator / denominator, decimalPlaces) : 0;
    }

    public RetrospectiveStatisticsService(RetroDbContext ctx)
    {
        _ctx = ctx;
    }

    public async Task<RetrospectiveStatsDto> GetStatisticsAsync()
    {
        var totalBoards = await _ctx.Retrospectives.CountAsync();

        var groupItems = await _ctx.Set<GroupItem>()
            .Include(i => i.Comments)
            .Include(i => i.Group)
            .ThenInclude(g => g.Retrospective)
            .ToListAsync();

        var totalCards = groupItems.Count;
        var totalComments = groupItems.Sum(i => i.Comments?.Count ?? 0);
        var totalLikes = await _ctx.GroupItemVotes.SumAsync(v => v.Count);

        var actionItems = await _ctx.ActionItems.ToListAsync();
        var totalActionItems = actionItems.Count;
        var completedActionItems = actionItems.Count(ai => ai.Status == ActionItemStatus.Completed);

        var templatePopularity = await _ctx.Retrospectives
            .GroupBy(r => r.Template)
            .Select(g => new { Template = g.Key.ToString(), Count = g.Count() })
            .OrderByDescending(x => x.Count)
            .ToDictionaryAsync(x => x.Template, x => x.Count);

        return new RetrospectiveStatsDto
        {
            TotalCards = totalCards,
            TotalComments = totalComments,
            TotalLikes = totalLikes,
            TotalActionItems = totalActionItems,
            CompletedActionItems = completedActionItems,
            
            AverageActionItemsPerBoard = CalculateAverage(totalActionItems, totalBoards),
            AverageCompletedActionItemsPerBoard = CalculateAverage(completedActionItems, totalBoards),
            AverageLikesPerCard = CalculateAverage(totalLikes, totalCards),
            AverageCommentsPerCard = CalculateAverage(totalComments, totalCards),

            AverageCardsPerBoard = CalculateAverage(totalCards, totalBoards),
            AverageCommentsPerBoard = CalculateAverage(totalComments, totalBoards),
            AverageLikesPerBoard = CalculateAverage(totalLikes, totalBoards),
            
            ActionItemCompletionRate = totalActionItems > 0
                ? Math.Round((double)completedActionItems / totalActionItems * 100, 2)
                : 0,
            
            TemplatePopularity = templatePopularity
        };
    }
}