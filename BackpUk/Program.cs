using BackpUk;

EventHandler<int> packHandler = (sender, packedEntriesCount) => Console.WriteLine($"{packedEntriesCount} new items successfully packed.");
EventHandler<int> unpackHandler = (sender, unpackedEntriesCount) => Console.WriteLine($"{unpackedEntriesCount} new items successfully unpacked.");

string sourceFolderName = @"D:\test_source";

var backpuker = new BackpUker();
backpuker.PackingCompleted += packHandler;
backpuker.UnpackingCompleted += unpackHandler;
backpuker.StartTracking(sourceFolderName);
//backpuker.FetchFromBackpUk();
backpuker.PutInBackpUk();
