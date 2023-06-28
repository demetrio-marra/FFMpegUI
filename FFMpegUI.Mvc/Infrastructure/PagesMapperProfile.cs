using AutoMapper;
using FFMpegUI.Messages;
using FFMpegUI.Models;
using FFMpegUI.Mvc.Data;
using FFMpegUI.Mvc.Helpers;
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

            CreateMap<FFMpegProcessItemMessage, ProcessItemProgressViewModel>()
                .ForMember(dest => dest.ProgressMessage, opt => opt.MapFrom(src => src.ProgressMessage))
                .ForMember(dest => dest.ProcessItemId, opt => opt.MapFrom(src => src.ProcessItemId.ToString()))
                .ForMember(dest => dest.ConvertedFileLink, opt => opt.MapFrom(src => src.ConvertedFileId.HasValue ? $"<a href=\"/DettaglioProcesso?id={src.ConvertedFileId}&amp;handler=Download\">{src.ConvertedFileName}</a>" : null))
                .ForMember(dest => dest.EndDate, opt => opt.MapFrom(src => src.EndDate.HasValue ? src.EndDate.Value.ToString("g") : null))
                .ForMember(dest => dest.StartDate, opt => opt.MapFrom(src => src.StartDate.HasValue ? src.StartDate.Value.ToString("g") : null))
                .ForMember(dest => dest.ConvertedFileSize, opt => opt.MapFrom(src => src.ConvertedFileSize.HasValue ? FileSystemHelper.FormatFileSize(src.ConvertedFileSize.Value) : null));
        }
    }
}
