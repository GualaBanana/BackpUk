using BackpUk;

// Write tests.
EventHandler<int> packHandler = (sender, packedEntriesCount) => Console.WriteLine($"{packedEntriesCount} new items successfully packed.");
EventHandler<int> unpackHandler = (sender, unpackedEntriesCount) => Console.WriteLine($"{unpackedEntriesCount} new items successfully unpacked.");

string sourceFolderName = @"D:\test_source";

var backpuker = new BackpUker();
backpuker.PackingCompleted += packHandler;
backpuker.UnpackingCompleted += unpackHandler;
//backpuker.StartTracking(sourceFolderName);
//backpuker.FetchFromBackpUk();
backpuker.PutInBackpUk();

// For all of the following must be responsible the installer. Config is unaware of all of this shit. It just accesses the data (specifically config.json
// in the directory where it was installed, i.e. it always accesses the relative path to config.json located in the directory provided as InstallationPath
// on the installation step to the installer and added to the config.json as a result). So IsInstalled property must be in the installer.
// check if the app is installed
// if (appIsNotInstalled)
// start installing
// 1. Ask for the directory where the user wants to install the app
// 2. All the installation options are chosen by the user, start installation

