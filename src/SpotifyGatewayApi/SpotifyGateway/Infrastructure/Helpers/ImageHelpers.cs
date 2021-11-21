using System.Collections.Generic;
using System.Linq;
using SpotifyGateway.Models.Enums;
using SpotifyAPI.Web.Models;

namespace SpotifyGateway.Infrastructure.Helpers
{
    public static class ImageHelpers
    {
        public static string GetImagePath(List<Image> images, ImageResolution imageResolution = ImageResolution.Medium)
        {
            return images is {Count: > 0}
                ? images.Count > 1 && (int) imageResolution > 0
                    ? images.Count > 2 && (int) imageResolution > 1
                        ? images.OrderBy(x => x.Width).Skip(2).FirstOrDefault()?.Url
                        : images.OrderBy(x => x.Width).Skip(1).FirstOrDefault()?.Url
                    : images.FirstOrDefault()?.Url
                : null;
        }
    }
}