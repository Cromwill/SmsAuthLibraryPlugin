#import <UIKit/UIKit.h>

   extern "C" {
       void disableScreenshots() {
           UIWindow *window = [[[UIApplication sharedApplication] delegate] window];
           window.hidden = YES;
           // отобразить пустой слой, чтобы скрыть экран
           UIView *overlay = [[UIView alloc] initWithFrame:window.bounds];
           overlay.backgroundColor = [UIColor whiteColor]; // или любой другой цвет
           [window addSubview:overlay];
       }

       void enableScreenshots() {
           UIWindow *window = [[[UIApplication sharedApplication] delegate] window];
           for (UIView *subview in window.subviews) {
               if ([subview isKindOfClass:[UIView class]]) {
                   [subview removeFromSuperview];
               }
           }
           window.hidden = NO; // скрываем окно, когда снимок экрана отключен
       }
   }
