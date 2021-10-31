using SyncTool;

// TODO:
// Add the event for the empty `TrackList` that is raised when no directories are tracked yet.
// Users will be able to declare their own subscribers that will execute only when they are subscribed to my event.
// E.g. when a gallery app is opened on a phone, the gallery can subscribe to my event to perform
// actions when my event is raised. This functionality is the responsiblity of the first if statement in this method for now.
// This observer interface just must be an interface that will be called something like "IEmptyTrackListManager

// Write tests.
EventHandler<int> syncHandler = (sender, newSyncEntries) => Console.WriteLine($"{newSyncEntries} new entries successfully synchronized.");

string sourceFolderName = @"D:\test_source";
Sync syncTool = new();
syncTool.SyncCompleted += syncHandler;
//syncTool.StartTracking(sourceFolderName);
syncTool.SynchronizeWithCloud();
//syncTool.Synchronize();
