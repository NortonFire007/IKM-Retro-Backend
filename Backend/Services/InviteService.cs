using AnimeWebApp.Exceptions.Base;
using IKM_Retro.Models.Retro;
using IKM_Retro.Repositories;
using Microsoft.Extensions.Logging;

namespace IKM_Retro.Services;

public class InviteService
{
    private readonly InviteRepository _repository;
    private readonly ILogger<InviteService> _logger;

    public InviteService(InviteRepository repository, ILogger<InviteService> logger)
    {
        _repository = repository;
        _logger = logger;
    }

    public async Task<RetrospectiveInvite> GenerateInviteAsync(Guid retrospectiveId,
        CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Start generating invite for retrospective with ID: {RetrospectiveId}", retrospectiveId);

        await CleanupExpiredInvitesAsync(cancellationToken);

        var retrospective = await _repository.GetRetrospectiveWithInviteAsync(retrospectiveId, cancellationToken);

        if (retrospective == null)
        {
            _logger.LogWarning("Retrospective with ID {RetrospectiveId} not found.", retrospectiveId);
            throw new NotFoundException("Retrospective not found");
        }

        if (retrospective.InviteLink != null)
        {
            _logger.LogInformation("Invite already exists for retrospective with ID: {RetrospectiveId}",
                retrospectiveId);
            return retrospective.InviteLink;
        }

        var invite = new RetrospectiveInvite
        {
            Id = Guid.NewGuid(),
            RetrospectiveId = retrospectiveId,
            Code = Guid.NewGuid().ToString(),
            ExpiresAt = DateTime.UtcNow.AddDays(7),
            IsActive = true
        };

        retrospective.InviteLink = invite;

        _repository.UpdateRetrospective(retrospective);
        await _repository.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Invite generated successfully. Code: {InviteCode}", invite.Code);

        return invite;
    }

    public async Task<Retrospective?> GetRetrospectiveByInviteAsync(string code,
        CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Searching for retrospective by invite code: {Code}", code);

        await CleanupExpiredInvitesAsync(cancellationToken);

        var invite = await _repository.GetActiveInviteAsync(code, cancellationToken);

        if (invite == null)
        {
            _logger.LogWarning("No active invite found with code: {Code}", code);
            return null;
        }

        _logger.LogInformation("Found retrospective with ID: {RetrospectiveId}", invite.Retrospective.Id);
        return invite.Retrospective;
    }

    private async Task CleanupExpiredInvitesAsync(CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Cleaning expired invites...");

        var expiredInvites = await _repository.GetExpiredInvitesAsync(cancellationToken);

        if (expiredInvites.Any())
        {
            _repository.RemoveExpiredInvites(expiredInvites);
            await _repository.SaveChangesAsync(cancellationToken);

            _logger.LogInformation("Deleted {Count} expired invites.", expiredInvites.Count);
        }
        else
        {
            _logger.LogInformation("No expired invites found.");
        }
    }
}