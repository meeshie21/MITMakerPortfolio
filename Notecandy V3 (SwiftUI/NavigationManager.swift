import SwiftUI

class NavigationManager: ObservableObject {
    @Published var navigationPath = NavigationPath()

    func popToRoot() {
            navigationPath.removeLast(navigationPath.count)
        }
}
