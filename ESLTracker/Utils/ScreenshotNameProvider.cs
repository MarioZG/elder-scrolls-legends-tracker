using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ESLTracker.Properties;

namespace ESLTracker.Utils
{
    public class ScreenshotNameProvider
    {
        private ITrackerFactory trackerFactory;
        ISettings settings;

        public enum ScreenShotType
        {
            Regular, //user pressed screenshot button
            Pack, //automatic when pack is added
        }

        public ScreenshotNameProvider() : this(new TrackerFactory())
        {

        }

        public ScreenshotNameProvider(ITrackerFactory trackerFactory)
        {
            this.trackerFactory = trackerFactory;
            this.settings = trackerFactory.GetService<ISettings>();
        }

        public string GetScreenShotName(ScreenShotType type)
        {
            switch (type)
            {
                case ScreenShotType.Regular:
                    return GetScreenShotName(type, settings.General_ScreenshotNameTemplate);
                case ScreenShotType.Pack:
                    return GetScreenShotName(type, settings.Packs_ScreenshotNameTemplate);
                default:
                    throw new NotImplementedException("Unknown screenshot type" + type);
            }
        }

        public string GetScreenShotName(ScreenShotType type, string nameTemplate)
        {
            switch (type)
            {
                case ScreenShotType.Regular:
                    nameTemplate = nameTemplate.Replace("{d", "{0");
                    return string.Format(nameTemplate, trackerFactory.GetDateTimeNow());
                case ScreenShotType.Pack:
                    nameTemplate = nameTemplate.Replace("{d", "{0").Replace("{n", "{1");
                    return string.Format(nameTemplate, trackerFactory.GetDateTimeNow(), trackerFactory.GetTracker().Packs.Count);
                default:
                    throw new NotImplementedException("Unknown screenshot type" + type);
            }
        }

    }
}
