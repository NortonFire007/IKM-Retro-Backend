using IKM_Retro.DTOs.Retrospective;
using IKM_Retro.DTOs.User;
using Mapster;

namespace IKM_Retro.DTOs.Mappings;

public class MapsterConfig
{
    public static void RegisterMappings()
    {
        TypeAdapterConfig<Models.Retro.Retrospective, RetrospectiveDto>
            .NewConfig()
            .Map(dest => dest.AssignedUsers,
                src => src.RetrospectiveUsers
                    .Select(ru => new BaseUserDTO { UserName = ru.User.UserName, Id = ru.User.Id }).ToList());
    }
}