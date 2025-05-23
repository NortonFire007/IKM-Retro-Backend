using IKM_Retro.DTOs.Retrospective.Group.Items;
using IKM_Retro.Exceptions.Base;
using IKM_Retro.Models.Retro;
using IKM_Retro.Repositories;
using Mapster;
using Microsoft.EntityFrameworkCore;

namespace IKM_Retro.Services;

public class GroupItemVoteService(
    RetrospectiveRepository retrospectiveRepository,
    RetrospectiveGroupRepository retrospectiveGroupRepository,
    GroupItemRepository groupItemRepository,
    GroupItemVoteRepository groupItemVoteRepository)
{
    public async Task<GroupItemVoteDTO> AddVote(string userId, int groupItemId)
    {
        var queryResult = await (
            from gi in groupItemRepository.GetAllQuery()
            join rg in retrospectiveGroupRepository.GetAllQuery() on gi.GroupId equals rg.Id
            join ru in retrospectiveRepository.GetRetrospectiveUsersQuery() on rg.RetrospectiveId equals ru
                .RetrospectiveId
            where gi.Id == groupItemId && ru.UserId == userId
            select new
            {
                GroupItem = gi,
                RetrospectiveId = rg.RetrospectiveId,
                UserInRetrospective = true
            }).FirstOrDefaultAsync();

        if (queryResult == null)
            throw new PermissionException("You can't add vote to this group item!");

        var voteInfo = await (
            from v in groupItemVoteRepository.GetAllQuery()
            where v.UserId == userId
            group v by 1
            into g
            select new
            {
                ExistingVote = g.FirstOrDefault(v => v.GroupItemId == groupItemId),
                TotalVotes = g.Sum(v => v.Count)
            }).FirstOrDefaultAsync();

        if (voteInfo?.TotalVotes >= 6)
            throw new BusinessException("You have reached limit of votes");


        GroupItemVote vote;
        if (voteInfo?.ExistingVote == null)
        {
            vote = new GroupItemVote
            {
                GroupItemId = groupItemId,
                UserId = userId,
                Count = 1
            };
            await groupItemVoteRepository.Add(vote);
        }
        else
        {
            vote = voteInfo.ExistingVote;
            vote.Count += 1;
        }

        await groupItemVoteRepository.SaveChangesAsync();
        return vote.Adapt<GroupItemVoteDTO>();
    }

    public async Task RemoveVote(string userId, int groupItemId)
    {
        GroupItemVote? voteToRemove = await (
            from v in groupItemVoteRepository.GetAllQuery()
            join gi in groupItemRepository.GetAllQuery() on v.GroupItemId equals gi.Id
            join rg in retrospectiveGroupRepository.GetAllQuery() on gi.GroupId equals rg.Id
            join ru in retrospectiveRepository.GetRetrospectiveUsersQuery() on rg.RetrospectiveId equals ru
                .RetrospectiveId
            where v.UserId == userId && v.GroupItemId == groupItemId && ru.UserId == userId
            select v).FirstOrDefaultAsync();

        if (voteToRemove == null)
            throw new NotFoundException("Group item vote not found or you don't have permission");

        if (voteToRemove.Count == 1)
        {
            groupItemVoteRepository.Remove(voteToRemove);
        }
        else
        {
            voteToRemove.Count -= 1;
        }

        Console.WriteLine($"zalupa voteToRemove {voteToRemove.Count} of {voteToRemove.Id}");

        await groupItemVoteRepository.SaveChangesAsync();
    }
}