using System;
using System.IO;
using System.Reflection;
using Microsoft.Extensions.FileProviders;

namespace YouFoos.Api.Utilities
{
    /// <summary>
    /// Contains a single method, which loads the default avatar and returns it.
    /// </summary>
    public static class DefaultAvatarLoader
    {
        /// <summary>
        /// Returns the default avatar for the YouFoos service, as a PNG file.
        /// </summary>
        public static string GetDefaultAvatar()
        {
            var fileProvider = new ManifestEmbeddedFileProvider(Assembly.GetExecutingAssembly());
            const string fileName = "Resources/default-avatar.png";
            using var fileStream = fileProvider.GetFileInfo(fileName).CreateReadStream();
            byte[] bytes = new BinaryReader(fileStream).ReadBytes((int)fileStream.Length);

            return Convert.ToBase64String(bytes);
        }
    }
}
