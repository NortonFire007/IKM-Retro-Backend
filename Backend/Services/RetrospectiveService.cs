using AnimeWebApp.Exceptions.Base;
using IKM_Retro.DTOs.Retrospective;
using IKM_Retro.Models;
using IKM_Retro.Models.Retro;
using IKM_Retro.Repositories;
using IKM_Retro.Helpers.Factories;
using Microsoft.AspNetCore.Identity;

namespace IKM_Retro.Services
{
    public class RetrospectiveService(
        UserManager<User> userManager,
        RetrospectiveRepository retrospectiveRepository,
        RetrospectiveGroupRepository retrospectiveGroupRepository,
        RetrospectiveGroupItemRepository retrospectiveGroupItemRepository)
    {
        public async Task<List<Retrospective>> GetByUserId(string userId)
        {
            _ = userManager.FindByIdAsync(userId) ?? throw new NotFoundException("User not found.");

            return await retrospectiveRepository.GetByUserId(userId);
        }

        public async Task Create(string userId, PostRetrospectiveBody body)
        {
            User user = await userManager.FindByIdAsync(userId) ?? throw new NotFoundException("User not found.");

            int userRetrospectivesCount = await retrospectiveRepository.CountUserRetrospectives(userId);

            if (userRetrospectivesCount >= 3)
            {
                throw new BusinessException($"You passed your subscribtion limit of {3} retrospectives");
            }

            Retrospective retrospective = new()
            {
                Id = Guid.NewGuid(),
                Title = body.Title,
                Template = body.TemplateType
            };

            await retrospectiveRepository.Add(retrospective);

            var template = RetrospectiveTemplateFactory.Create(body.TemplateType);

            foreach (var group in template.Groups)
            {
                await retrospectiveGroupRepository.Add(new Group
                {
                    Name = group.Name,
                    Description = group.Description,
                    Retrospective = retrospective
                });
            }

            await retrospectiveRepository.SaveChangesAsync();
        }



    }
}
