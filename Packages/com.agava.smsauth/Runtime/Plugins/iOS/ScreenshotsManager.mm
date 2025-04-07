#import <UIKit/UIKit.h>

extern "C"
{
	void enableScreenshots() 
	{
	   UIWindow *window = [[[UIApplication sharedApplication] delegate] window];
	   for (UIView *subview in window.subviews) {
		   if ([subview isKindOfClass:[UIView class]]) {
			   [subview removeFromSuperview];
		   }
	   }
	   window.hidden = NO;
	}
	
	void disableScreenshots() 
	{
	   UIWindow *window = [[[UIApplication sharedApplication] delegate] window];
	   window.hidden = YES;
	   UIView *overlay = [[UIView alloc] initWithFrame:window.bounds];
	   overlay.backgroundColor = [UIColor whiteColor];
	   [window addSubview:overlay];
    }
}