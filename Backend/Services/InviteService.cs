using IKM_Retro.Exceptions.Base;
using IKM_Retro.Models.Retro;
using IKM_Retro.Repositories;

namespace IKM_Retro.Services;

public class InviteService(InviteRepository repository, RetrospectiveRepository retrospectiveRepository, ILogger<InviteService> logger)
{
    public async Task<RetrospectiveInvite> GenerateInviteAsync(Guid retrospectiveId,
        CancellationToken cancellationToken = default)
    {
        logger.LogInformation("Start generating invite for retrospective with ID: {RetrospectiveId}", retrospectiveId);

        await CleanupExpiredInvitesAsync(cancellationToken);

        Retrospective? retrospective = await retrospectiveRepository.GetRetrospectiveWithInviteAsync(retrospectiveId, cancellationToken);

        if (retrospective == null)
        {
            logger.LogWarning("Retrospective with ID {RetrospectiveId} not found.", retrospectiveId);
            throw new NotFoundException("Retrospective not found");
        }

        if (retrospective.InviteLink != null)
        {
            logger.LogInformation("Invite already exists for retrospective with ID: {RetrospectiveId}",
                retrospectiveId);
            return retrospective.InviteLink;
        }

        RetrospectiveInvite invite = new()
        {
            Id = Guid.NewGuid(),
            RetrospectiveId = retrospectiveId,
            Code = Guid.NewGuid().ToString(),
            ExpiresAt = DateTime.UtcNow.AddDays(7),
            IsActive = true
        };

        await repository.AddInviteAsync(invite, cancellationToken);
        await repository.SaveChangesAsyncWithCancellation(cancellationToken);

        logger.LogInformation("Invite generated successfully. Code: {InviteCode}", invite.Code);

        return invite;
    }

    public async Task<Retrospective?> GetRetrospectiveByInviteAsync(string code,
        CancellationToken cancellationToken = default)
    {
        logger.LogInformation("Searching for retrospective by invite code: {Code}", code);

        await CleanupExpiredInvitesAsync(cancellationToken);

        RetrospectiveInvite? invite = await repository.GetActiveInviteAsync(code, cancellationToken);

        if (invite == null)
        {
            logger.LogWarning("No active invite found with code: {Code}", code);
            return null;
        }

        logger.LogInformation("Found retrospective with ID: {RetrospectiveId}", invite.Retrospective.Id);
        return invite.Retrospective;
    }

    private async Task CleanupExpiredInvitesAsync(CancellationToken cancellationToken = default)
    {
        logger.LogInformation("Cleaning expired invites...");

        var expiredInvites = await repository.GetExpiredInvitesAsync(cancellationToken);

        if (expiredInvites.Any())
        {
            repository.RemoveExpiredInvites(expiredInvites);
            await repository.SaveChangesAsyncWithCancellation(cancellationToken);

            logger.LogInformation("Deleted {Count} expired invites.", expiredInvites.Count);
        }
        else
        {
            logger.LogInformation("No expired invites found.");
        }
    }
}