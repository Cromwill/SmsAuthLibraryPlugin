import UIKit




@objc public class ScreenshotDetector: NSObject {

    @objc public static func startScreenshotDetection() {
        NotificationCenter.default.addObserver(
            forName: UIApplication.userDidTakeScreenshotNotification,
            object: nil,
            queue: OperationQueue.main) { notification in
                // Trigger Unity method when screenshot is detected
                UnitySendMessage("ScreenshotHandler", "OnScreenshotTaken", "")
        }
    }

    @objc public static func stopScreenshotDetection() {
        NotificationCenter.default.removeObserver(self, name: UIApplication.userDidTakeScreenshotNotification, object: nil)
    }
}