using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Resources;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using OMI.Workers;
using PckStudio.Properties;

namespace PckStudio.Internal
{
    internal sealed class AppResourceManager
    {
        public static readonly AppResourceManager Default = new AppResourceManager(Resources.ResourceManager, Resources.Culture);
        private ResourceManager _resourceManager;
        private readonly CultureInfo _culture;

        public AppResourceManager(ResourceManager resourceManager, CultureInfo culture)
        {
            _resourceManager = resourceManager ?? throw new ArgumentNullException(nameof(resourceManager));
            _culture = culture;
        }

        public T GetData<T>(byte[] rawData, IDataFormatReader<T> dataFormatReader) where T : class
        {
            _ = rawData ?? throw new ArgumentNullException(nameof(rawData));
            _ = dataFormatReader ?? throw new ArgumentNullException(nameof(dataFormatReader));

            T result = default;
            using (Stream resourceStream = new MemoryStream(rawData))
            {
                result = dataFormatReader.FromStream(resourceStream);
            }
            return result;
        }

        public T GetDataFromResource<T>(string name, IDataFormatReader<T> dataFormatReader) where T : class
        {
            return GetData((byte[])_resourceManager.GetObject(name, _culture), dataFormatReader);
        }
    }
}
