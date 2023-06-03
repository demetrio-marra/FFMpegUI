using AutoMapper;
using FFMpegUI.Models;
using FFMpegUI.Persistence.Entities;

namespace FFMpegUI.Persistence.Mapping
{
    public class FFMpegPersistenceMapperProfile : Profile
    {
        public FFMpegPersistenceMapperProfile()
        {
            CreateMap<FFMpegProcessItem, FFMpegPersistedProcessItem>()
                .ReverseMap();

            CreateMap<FFMpegProcess, FFMpegPersistedProcess>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.ProcessId));

            CreateMap<FFMpegProcess, FFMpegPersistedProcessFeatures>();

            CreateMap<(FFMpegPersistedProcess, FFMpegPersistedProcessFeatures), FFMpegProcess>()
               .ConstructUsing((src, ctx) => new FFMpegProcess
               {
                   ProcessId = src.Item1.Id,
                   StartDate = src.Item1.StartDate,
                   EndDate = src.Item1.EndDate,
                   Items = ctx.Mapper.Map<IEnumerable<FFMpegProcessItem>>(src.Item1.Items),
                   OverallConversionQuality = src.Item2.OverallConversionQuality,
                   AudioCodec = src.Item2.AudioCodec,
                   VideoCodec = src.Item2.VideoCodec,
                   RescaleHorizontalWidth = src.Item2.RescaleHorizontalWidth
               });
        }
    }
}
