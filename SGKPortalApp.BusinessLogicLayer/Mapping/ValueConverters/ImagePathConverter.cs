using AutoMapper;
using SGKPortalApp.Common.Helpers;

namespace SGKPortalApp.BusinessLogicLayer.Mapping.ValueConverters
{
    /// <summary>
    /// Generic AutoMapper value converter for image paths.
    /// Converts filename stored in DB to full web path using PersonelImagePathHelper.
    /// Can be used in any mapping where image path conversion is needed.
    ///
    /// Usage in mapping profile:
    /// .ForMember(dest => dest.Resim, opt => opt.ConvertUsing(new ImagePathConverter(_imagePathHelper), src => src.Resim))
    /// </summary>
    public class ImagePathConverter : IValueConverter<string?, string?>
    {
        private readonly PersonelImagePathHelper _imagePathHelper;

        public ImagePathConverter(PersonelImagePathHelper imagePathHelper)
        {
            _imagePathHelper = imagePathHelper;
        }

        public string? Convert(string? sourceMember, ResolutionContext context)
        {
            return _imagePathHelper.GetWebPath(sourceMember);
        }
    }
}
