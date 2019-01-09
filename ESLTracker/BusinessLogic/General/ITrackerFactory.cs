using TESLTracker.DataModel;

namespace ESLTracker.BusinessLogic.General
{
    public interface ITrackerFactory
    {

        ITracker CreateEmptyTracker();

        void FixUpDeserializedTracker(ITracker tracker);

    }
}
