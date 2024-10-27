//
//  NotecandyApp.swift
//  Notecandy
//
//  Created by Vikas Majithia on 8/12/24.
//

import SwiftUI
import SwiftData

@main
struct NotecandyApp: App {
    @StateObject private var manager = Manager()
    @StateObject var whisperState = WhisperState()
    @StateObject var navManager = NavigationManager()
    
    var body: some Scene {
        WindowGroup {
            ContentView()
                .modelContainer(for : SavedNote.self)
                .environmentObject(manager).environmentObject(whisperState) .onAppear {
                }
        }
    }
}
