using BackpUk;

// TODO:
// Add the event for the empty `TrackList` that is raised when no directories are tracked yet.
// Users will be able to declare their own subscribers that will execute only when they are subscribed to my event.
// E.g. when a gallery app is opened on a phone, the gallery can subscribe to my event to perform
// actions when my event is raised. This functionality is the responsiblity of the first if statement in this method for now.
// This observer interface just must be an interface that will be called something like "IEmptyTrackListManager

// Write tests.
EventHandler<int> packHandler = (sender, packedEntriesCount) => Console.WriteLine($"{packedEntriesCount} new entries successfully packed.");
EventHandler<int> unpackHandler = (sender, unpackedEntriesCount) => Console.WriteLine($"{unpackedEntriesCount} new entries successfully unpacked.");

string sourceFolderName = @"D:\test_source";
var backpuker = new BackpUker();
backpuker.PackingCompleted += packHandler;
//backpuk.StartTracking(sourceFolderName);
backpuker.FetchFromBackpUk();
backpuker.PutInBackpUk();
