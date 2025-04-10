import Foundation
import UIKit

@objc public class ScreenshotDetector: NSObject {

	@objc public static let shared = ScreenshotDetector()

    @objc public func startScreenshotDetection() {
        NotificationCenter.default.addObserver(
            forName: UIApplication.userDidTakeScreenshotNotification,
            object: nil,
            queue: OperationQueue.main) { notification in
                // Trigger Unity method when screenshot is detected
                UnitySendMessage("ScreenshotProtector", "OnScreenshotTaken", "")
        }
    }

    @objc public func stopScreenshotDetection() {
        NotificationCenter.default.removeObserver(self, name: UIApplication.userDidTakeScreenshotNotification, object: nil)
    }
}