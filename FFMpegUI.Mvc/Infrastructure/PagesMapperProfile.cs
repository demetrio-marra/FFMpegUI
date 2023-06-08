using AutoMapper;
using FFMpegUI.Models;
using FFMpegUI.Mvc.Pages;
using FFMpegUI.Persistence.Entities;

namespace FFMpegUI.Mvc.Infrastructure
{
    public class PagesMapperProfile : Profile
    {
        public PagesMapperProfile()
        {
            CreateMap<FFMpegProcess, ProcessiModel>()
                .ReverseMap();

            CreateMap<FFMpegPersistedProcess, FFMpegProcessSummary>()
                .ForMember(dest => dest.ProcessId, opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.FilesCount, opt => opt.MapFrom(src => src.Items.Count()))
                .ForMember(dest => dest.TotalFilesSize, opt => opt.MapFrom(src => (src.SourceFilesTotalSize ?? 0) + (src.ConvertedFilesTotalSize ?? 0)));
        }
    }
}
