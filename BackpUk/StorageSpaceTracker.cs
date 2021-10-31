namespace BackpUk
{
    public class StorageSpaceTracker
    {
        readonly DirectoryInfo _trackedStorage;
        DirectoryInfo TrackedStorage
        {
            get
            {
                _trackedStorage.Refresh();
                return _trackedStorage;
            }
        }
        // Probably should get rid of `Space` suffix.
        double LowSpaceThreshold { get; }
        double SpaceLimit { get; }
        double OccupiedSpace => TrackedStorage.EnumerateFiles("*", SearchOption.AllDirectories).Sum(spaceConsumer => spaceConsumer.Length);
        double AvailableSpace { get; }

        public StorageSpaceTracker(string directoryPath)
        {
            _trackedStorage = new(directoryPath);
        }
    }
}
