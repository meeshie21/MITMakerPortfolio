import SwiftUI
import SwiftData


struct ContentView: View {
    @EnvironmentObject var manager: Manager
    @EnvironmentObject var whisperState: WhisperState//change

    @Query(sort: \SavedNote.date, order: SortOrder.reverse) private var savedNotes: [SavedNote]
    @Environment(\.modelContext) private var context
    @Environment(\.colorScheme) var colorScheme

    @State private var showingAlert = false
    @State private var loadingNote = false
    @State private var noteToDelete: SavedNote? = nil
    @State private var isMenuVisible = false

    @StateObject private var navManager = NavigationManager()

    @State private var noteCache: [SavedNote] = [] // Cache for preloading

    var body: some View {
        NavigationStack(path: $navManager.navigationPath) {
            VStack(spacing:15) {
                HStack {
                    Text("Saved Notes")
                        .font(Font.custom("Gaegu-Bold", size: 35))
                        .bold()
                        .frame(maxWidth: .infinity, alignment: .leading)
                    
                    Spacer()
                    
                    Menu {
                                NavigationLink(destination: GenerationView().environmentObject(navManager)
                                    .modelContainer(for: SavedNote.self)
                                    .navigationBarBackButtonHidden(true)) {
                                    Label("Enter Text", systemImage: "text.bubble")
                                }

                        NavigationLink(destination: RecordingView().environmentObject(whisperState).environmentObject(navManager)
                                    .modelContainer(for: SavedNote.self)
                                    .navigationBarBackButtonHidden(true)) {
                                        Label("Record Audio", systemImage: "mic.fill")
                                }
                                    .onAppear{
                                        whisperState.isRecording = false
                                        whisperState.transcriptionText = ""
                                        UserDefaults.standard.set("", forKey: "storedTranscript")//new 1.2
                                    }//added 1.2
                            } label: {
                                Image(systemName: "plus.square.on.square.fill")
                                    .resizable()
                                    .frame(width: 30, height: 30)
                                    .foregroundColor(.powderyBlue)
                            }
                    
                   
                }
                
                
                Divider().frame(height: 2.5)
                
                    .overlay(.gray)
                
               
                
                if savedNotes.isEmpty {
                    
                    HStack{
                        Text("You have no saved notes! Click the big plus button to make your first one.").font(Font.custom("Gaegu-Regular", size: 20))

                    }
                        .frame(maxWidth: .infinity, alignment: .leading)
                }
                
            }.padding(EdgeInsets(top: 20, leading: 25, bottom: 0, trailing: 25))
            
            if !savedNotes.isEmpty{
                VStack{
                    List {
                        ForEach(savedNotes) { note in
                            NavigationLink(destination: SavedNoteView(note: note)
                                .environmentObject(manager)
                                .onAppear {
                                    manager.user_input = ""
                                    Task {
                                        await preloadNoteData(note) // Use Task to handle async work
                                    }
                                }) {
                                    HStack {
                                        Button(action: {
                                            noteToDelete = note
                                            showingAlert.toggle()
                                        }) {
                                            Image(systemName: "trash.fill")
                                                .scaledToFit()
                                                .frame(width: 24, height: 24)
                                        }
                                        .buttonStyle(PlainButtonStyle())
                                        
                                        Text(note.title).font(Font.custom("Gaegu-Regular", size: 20))
                                        
                                        Spacer()
                                         
                                        Text(formatDate(note.date)).font(Font.custom("Gaegu-Regular", size: 20))
                                    }
                                    .frame(maxWidth: .infinity, alignment: .leading)
                                }
                            
                                .listRowBackground(Color.darkerLightCandyPink2)
                        }
                    }
                    .scrollContentBackground(Visibility.hidden)
                }.alert("Delete Note?", isPresented: $showingAlert) {
                    Button("Cancel", role: .cancel) {
                        noteToDelete = nil
                    }
                    Button("Delete", role: .destructive) {
                        if let noteToDelete = noteToDelete {
                            delete(noteToDelete)
                        }
                        noteToDelete = nil
                    }
                } message: {
                    Text("This action is permanent.")
                }
            }
            
            
            Spacer()
        }
        
    }

    func formatDate(_ date: Date) -> String {
        let dateFormatter = DateFormatter()
        dateFormatter.dateFormat = "M/d/yy"
        return dateFormatter.string(from: date)
    }

    private func delete(_ deletion: SavedNote) {
        context.delete(deletion)
        do {
            try context.save()
        } catch {
            print("Failed to save context: \(error.localizedDescription)")
            // Handle error, e.g., show an alert to the user
        }
    }

    private func preloadNoteData(_ note: SavedNote) async {
        manager.user_input = note.data
        manager.transcript = note.transcript
    }
}

struct ContentPreview: PreviewProvider {
    static var previews: some View {
        ContentView()
    }
}
