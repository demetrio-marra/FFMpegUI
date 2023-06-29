using AutoMapper;
using FFMpegUI.Models;

namespace FFMpegUI.Persistence.Mapping
{
    /// <summary>
    /// For internal models
    /// </summary>
    public class FFMpegMapperProfile : Profile
    {
        public FFMpegMapperProfile()
        {
            CreateMap<FFMpegUpdateProcessItemCommand, FFMpegProcessItemStatusNotification>()
                .ForMember(dest => dest.ProgressMessage, opt => opt.MapFrom(src => src.StatusMessage))
                .ForMember(dest => dest.ProcessId, opt => opt.Ignore());

            CreateMap<FFMpegUpdateProcessCommand, FFMpegProcessStatusNotification>()
                .ForMember(dest => dest.AllFilesTotalSize, opt => opt.Ignore())
                .ForMember(dest => dest.AllFilesCount, opt => opt.Ignore())
                .ForMember(dest => dest.ProgressMessage, opt => opt.MapFrom(src => src.StatusMessage));
        }
    }
}
