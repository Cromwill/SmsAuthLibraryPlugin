#import <UnityFramework/UnityFramework-Swift.h>

extern "C"
{
    void startScreenshotDetection()
    {
        [[ScreenshotDetector shared]   startScreenshotDetection];
    }
	
	void stopScreenshotDetection()
    {
        [[ScreenshotDetector shared]   stopScreenshotDetection];
    }
}
