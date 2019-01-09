using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TESLTracker.Utils;

namespace ESLTracker.Utils.Converters
{
    public class VersionToVisibilty : ToVisibilityConverter<SerializableVersion>
    {
        SerializableVersion version;

        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            if (converter == null)
            {
                converter = new VersionToVisibilty();
            }
            return converter;
        }

        protected override void CheckForOtherParameters(object parameter)
        {
            base.CheckForOtherParameters(parameter);
            if (parameter != null) {
                Version v = null;
                bool versionParsed = parameter.ToString().ToLowerInvariant().Split(ParameterSeparator)
                        .Any(s => Version.TryParse(s, out v)
                    );
                if (versionParsed)
                {
                    version = new SerializableVersion(v);
                }
            }
        }

        protected override bool Condition(object value)
        {
            return (version == null) || version == (value as SerializableVersion);
        }
    }
}
