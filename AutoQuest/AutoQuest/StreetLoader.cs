// <copyright>"☺ Raccoon corporation ©  1989"</copyright>

using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization.Json;

namespace AutoQuest
{
    public class StreetLoader
    {
        private readonly Assembly _assembly;
        private IEnumerable<Street> _loadedStreets;

        public StreetLoader()
        {
            _assembly = typeof(StreetLoader).GetTypeInfo().Assembly;
        }

        public IEnumerable<Street> LoadStreets()
        {
            if (_loadedStreets != null)
                return _loadedStreets.ToArray();

            var serializer = new DataContractJsonSerializer(typeof(Street[]));
            object loadedStreets;
            using (var stream = _assembly.GetManifestResourceStream("AutoQuest.Resources.streets.json"))
            {
                loadedStreets = serializer.ReadObject(stream);
            }
            _loadedStreets = loadedStreets as IEnumerable<Street>;
            return _loadedStreets.ToArray();
        }
    }
}