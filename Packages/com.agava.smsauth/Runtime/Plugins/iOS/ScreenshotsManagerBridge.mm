#import <UnityFramework/UnityFramework-Swift.h>
#import "UnityInterface.h"
#import "UnityAppController.h"

extern "C"
{
    void startScreenshotDetection()
    {
        [[ScreenshotDetector shared] startScreenshotDetection];
    }
	
	void stopScreenshotDetection()
    {
        [[ScreenshotDetector shared] stopScreenshotDetection];
    }
}
