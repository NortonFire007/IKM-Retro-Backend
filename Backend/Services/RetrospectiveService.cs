using IKM_Retro.DTOs.Retrospective;
using IKM_Retro.Enums;
using IKM_Retro.Exceptions.Base;
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
    InviteRepository inviteRepository)
{
    public async Task<RetrospectiveToUserDto> GetById(Guid id)
    {
        RetrospectiveToUserDto retrospectiveDto = await retrospectiveRepository.GetByIdDto(id) ??
                                                  throw new NotFoundException("Retrospective not found");
        return retrospectiveDto;
    }

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

        if (!await retrospectiveRepository.IsUserInRetrospective(invite.RetrospectiveId, userId))
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

    public async Task Update(string userId, Guid retrospectiveId, UpdateRetrospectiveDto dto)
    {
        Retrospective retrospective = await retrospectiveRepository.GetById(retrospectiveId)
                                      ?? throw new NotFoundException("Retrospective not found.");

        if (retrospective.CreatorUserId != userId)
            throw new BusinessException("Only the creator can update this retrospective.");

        if (!string.IsNullOrWhiteSpace(dto.Title))
            retrospective.Title = dto.Title;

        if (dto.IsActive.HasValue)
            retrospective.IsActive = dto.IsActive.Value;

        await retrospectiveRepository.SaveChangesAsync();
    }

    public async Task Delete(string userId, Guid retrospectiveId)
    {
        Retrospective retrospective = await retrospectiveRepository.GetById(retrospectiveId)
                                      ?? throw new NotFoundException("Retrospective not found.");

        if (retrospective.CreatorUserId != userId)
        {
            throw new BusinessException("Only the creator can delete this retrospective.");
        }

        retrospectiveRepository.Delete(retrospective);
        await retrospectiveRepository.SaveChangesAsync();
    }

    public async Task<List<Retrospective>> GetUserRetrospectivesAsync(string userId)
    {
        return await retrospectiveRepository.GetByUserIdAsync(userId);
    }

    public async Task<List<RetrospectiveDto>> GetCreatedRetrospectivesAsync(string userId)
    {
        return await retrospectiveRepository.GetCreatedByUserAsync(userId);
    }

    public async Task<List<RetrospectiveDto>> GetJoinedRetrospectivesAsync(string userId)
    {
        return await retrospectiveRepository.GetJoinedByUserAsync(userId);
    }
}