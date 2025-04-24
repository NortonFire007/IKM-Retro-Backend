using AnimeWebApp.Exceptions.Base;
using IKM_Retro.DTOs.Retrospective;
using IKM_Retro.Enums;
using IKM_Retro.Models;
using IKM_Retro.Models.Retro;
using IKM_Retro.Repositories;
using IKM_Retro.Helpers.Factories;
using Microsoft.AspNetCore.Identity;

namespace IKM_Retro.Services;

public class RetrospectiveService(
    UserManager<User> userManager,
    RetrospectiveRepository retrospectiveRepository,
    RetrospectiveGroupRepository retrospectiveGroupRepository,
    InviteRepository inviteRepository,
    RetrospectiveGroupItemRepository retrospectiveGroupItemRepository)
{
    public async Task<List<RetrospectiveToUserDto>> GetByUserId(string userId)
    {
        _ = await userManager.FindByIdAsync(userId) ?? throw new NotFoundException("User not found.");

        return await retrospectiveRepository.GetByUserId(userId);
    }

    public async Task Create(string userId, PostRetrospectiveBody body)
    {
        User user = await userManager.FindByIdAsync(userId) ?? throw new NotFoundException("User not found.");

        var userRetrospectivesCount = await retrospectiveRepository.CountUserRetrospectives(userId);

        if (userRetrospectivesCount >= 3)
        {
            throw new BusinessException($"You passed your subscription limit of {3} retrospectives");
        }

        Retrospective retrospective = new()
        {
            Id = Guid.NewGuid(),
            Title = body.Title,
            Template = body.TemplateType,
            CreatorUserId = userId
        };

        await retrospectiveRepository.Add(retrospective);

        var template = RetrospectiveTemplateFactory.Create(body.TemplateType);

        foreach (var group in template.Groups)
        {
            await retrospectiveGroupRepository.Add(new Group
            {
                Name = group.Name,
                Description = group.Description,
                OrderPosition = group.SortOrder,
                Retrospective = retrospective
            });
        }

        // await retrospectiveRepository.AddRelation(new() { Retrospective = retrospective, User = user });
        await retrospectiveRepository.AddRelation(new RetrospectiveToUser
        {
            Retrospective = retrospective,
            User = user,
            Role = RoleTypeEnum.Owner
        });

        await retrospectiveRepository.SaveChangesAsync();
    }

    public async Task JoinByInvite(string userId, string code)
    {
        User user = await userManager.FindByIdAsync(userId) ?? throw new NotFoundException("User not found.");

        RetrospectiveInvite invite = await inviteRepository.GetActiveInviteByCode(code)
                                     ?? throw new NotFoundException("Invite link invalid or expired.");

        var alreadyJoined = await retrospectiveRepository.CheckIfUserJoined(invite.RetrospectiveId, userId);

        if (!alreadyJoined)
        {
            await retrospectiveRepository.AddRelation(new RetrospectiveToUser
            {
                RetrospectiveId = invite.RetrospectiveId,
                Retrospective = invite.Retrospective!, // fix
                UserId = user.Id,
                User = user,
                Role = RoleTypeEnum.Participant
            });

            await retrospectiveRepository.SaveChangesAsync();
        }
    }

    public async Task Delete(string userId, Guid retrospectiveId)
    {
        Retrospective retrospective = await retrospectiveRepository.GetById(retrospectiveId)
                                      ?? throw new NotFoundException("Retrospective not found.");

        if (retrospective.CreatorUserId != userId)
        {
            throw new BusinessException("Only the creator can delete this retrospective.");
        }

        await retrospectiveRepository.Delete(retrospectiveId);
        await retrospectiveRepository.SaveChangesAsync();
    }
}