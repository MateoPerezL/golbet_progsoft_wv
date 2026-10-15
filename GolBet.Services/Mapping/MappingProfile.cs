using AutoMapper;
using GolBet.Entities;
using GolBet.Services.DTOs;

namespace GolBet.Services.Mapping;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        // Mapas de lectura
        CreateMap<Match, MatchDto>();
        CreateMap<Team, TeamDto>();

        // Mapas de escritura
        CreateMap<TeamFormDto, Team>()
            .ReverseMap();

        CreateMap<MatchFormDto, Match>()
            .ReverseMap();

        // Mapa del detalle de un partido
        CreateMap<Match, MatchDetailDto>()
            .ForMember(
                dto => dto.TotalBets,
                options => options.MapFrom(match => match.Bets.Count));
    }
}
