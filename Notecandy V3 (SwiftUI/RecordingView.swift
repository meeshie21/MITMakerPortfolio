import SwiftUI
import Speech
import Foundation
import GoogleGenerativeAI
import MarkdownUI
import SwiftData
import AVFoundation
import Combine

struct RecordingView: View {
    //@EnvironmentObject var manager : Manager
    @Query private var savedNotes : [SavedNote]
    @Environment(\.modelContext) private var context
    @Environment(\.colorScheme) var colorScheme
    @EnvironmentObject var whisperState: WhisperState //change
    @EnvironmentObject var navManager: NavigationManager //change



    
    let model = GenerativeModel(name: "gemini-1.5-flash", apiKey: APIKey.default)
    @State var textInput = ""
    @State var aiResponse = ""
    @State var loading = false
    @State var submitted = false
    @State var saved = false
    @State private var showingAlert = false
    @State private var title = ""
    @State private var labelText = "Press to Begin Recording"
    @State private var labelFont : CGFloat = 29.5
    @State private var started = false
    
    //@StateObject var whisperState = WhisperState()


    let prompt = """
    Make pretty and easy-to-read notes with markdown. Put every single thing from this input into notes. Don't leave out any content at all.
    All content MUST be in the notes. Use markdown notation for headers, subheaders, and bullet points, bold text, tables. Give good spacing and organization. Use numbered
    lists often, bullet points, bold text, tables, etc. when applicable. If you have math/equations, use normal english characters. For example, a good way of writing an equation would be something like "x^2 + 3x + 5" - do not use LaTeX notation. I repeat, DO NOT use LaTeX notation for anything. Do NOT use HTML or CSS tags such as "<sup></sup>" for math or other purposes. I repeat DO NOT use <sup></sup> and <sub></sub> tags You can use "^" for something like "x^2" in mathematics. If you have lines of code, make sure that they do not horizontally exceed 50 characters (if they do, start a new line of code) If you have a lot of text in one spot, split up the text into smaller bullet points and prevent long blocks of text. For markdown formatting, sections should be headed by level 3  headers with body text inside them, and there should be no title at the start of the notes. Here's the content:
    """

    
    @State private var isTimerRunning = false
    @State private var timerSubscription: Cancellable?
    @State private var timerInterval: TimeInterval = 3.0
    @State private var tickCount = 0

    var body: some View {
        NavigationView {
            VStack {
                if(!submitted) {
                    if !whisperState.allowed {
                        Text("Permission Denied").font(Font.custom("Gaegu-Bold", size : 32))
                        Text("For Lecture Transcription to work, please open Settings and enable Speech Recognition and Microphone Access for Notecandy. Then, restart the app.").font(Font.custom("Gaegu-Regular", size : 20)).padding()
                    } else if !whisperState.isModelLoaded {
                        Text("Loading Model...").font(Font.custom("Gaegu-Bold", size : 32))
                        Text("Please wait for the Speech Recognition model to load before recording. ").font(Font.custom("Gaegu-Regular", size : 20)).padding()
                    } else {
                        Text(labelText).font(Font.custom("Gaegu-Bold", size: labelFont))
                    }
                }

                if loading {
                    ProgressView()
                        .progressViewStyle(CircularProgressViewStyle())
                        .scaleEffect(2)
                        .padding()
                }
                
                if !whisperState.isRecording && started && !submitted && whisperState.allowed {
                    Text("Your transcription will start to show up here in a few seconds...").padding()
                        .font(Font.custom("Gaegu-Regular", size: 20))
                }
                
                if whisperState.isRecording {
                    ScrollView {
                        if whisperState.transcriptionText == "" {
                            Text("Your transcription will start to show up here in a few seconds...").padding()
                                .font(Font.custom("Gaegu-Regular", size: 20))
                        }
                        Text(whisperState.transcriptionText)
                            .padding()
                            .font(Font.custom("Gaegu-Regular", size: 20))
                    }
                } else if aiResponse != "" && submitted && !saved {
                    ScrollView {
                        Markdown(
                           aiResponse
                        ).frame(maxWidth: .infinity, alignment: .leading).padding()
                            .markdownTextStyle(\.text) {
                                FontFamily(FontProperties.Family.custom("Gaegu-Regular"))
                            }
                            .markdownBlockStyle(\.heading2) { configuration in
                              configuration.label
                                .markdownTextStyle {
                                FontSize(26)
                                    ForegroundColor(colorScheme == .dark ?  Color.darkerLightCandyPink : Color.lightPink )
                                    BackgroundColor(nil)
                                    FontWeight(.bold)
                                
                                }
                               
                            }
                            .markdownBlockStyle(\.heading3) { configuration in
                              configuration.label
                                .markdownTextStyle {
                                FontSize(26)
                                    ForegroundColor(colorScheme == .dark ?  Color.darkerLightCandyPink : Color.lightPink )
                                    BackgroundColor(nil)
                                    FontWeight(.bold)
                                
                                }
                               
                            }
                            .markdownBlockStyle(\.heading4) { configuration in
                              configuration.label
                                .markdownTextStyle {
                                FontSize(26)
                                    ForegroundColor(colorScheme == .dark ?  Color.darkerLightCandyPink : Color.lightPink )
                                    BackgroundColor(nil)
                                    FontWeight(.bold)
                                
                                }
                               
                            }
                        
                        Divider().frame(height: 2.5)
                        
                            .overlay(.gray)
                                            Markdown("\n# **Transcript**\n\n" + whisperState.transcriptionText).frame(maxWidth: .infinity, alignment: .leading).padding()
                            .markdownTextStyle(\.text) {
                                FontFamily(FontProperties.Family.custom("Gaegu-Regular"))}
                            .markdownBlockStyle(\.heading1) { configuration in
                              configuration.label
                                .markdownTextStyle {
                                FontSize(34)
                                    ForegroundColor(colorScheme == .dark ?  Color.darkerLightCandyPink : Color.lightPink )
                                    BackgroundColor(nil)
                                    FontWeight(.bold)
                                
                                }
                               
                            }
                            .markdownBlockStyle(\.heading2) { configuration in
                              configuration.label
                                .markdownTextStyle {
                                FontSize(26)
                                    ForegroundColor(colorScheme == .dark ?  Color.darkerLightCandyPink : Color.lightPink )
                                    BackgroundColor(nil)
                                    FontWeight(.bold)
                                
                                }
                               
                            }
                            
                        
                    }
                    Button(action: {
                        showingAlert.toggle()
                    }) {
                        Text("Save Note")
                            .font(Font.custom("Gaegu-Regular", size: 25))
                            .foregroundColor(.powderyBlue)
                    }
                    .alert("Enter a title", isPresented: $showingAlert) {
                        TextField("Type a title...", text: $title)
                        Button("Submit", action: submit)
                    } message: {
                        Text("Enter a title for your note.")
                    }
                }
                
                if !submitted && whisperState.allowed {
                    Spacer()
                    HStack {
                        if started {
                            Button(action: {
                                stopRecordingPress()
                            }) {
                                Text("Stop")
                                    .padding()
                                    .background(Color.cottonCandyPink)
                                    .foregroundColor(.white)
                                    .cornerRadius(8)
                                    .font(Font.custom("Gaegu-Regular", size : 25))
                            }
                            .disabled(!whisperState.isRecording)
                        } else {
                            Button(action: {
                                switchLabel()
                                started = true
                                Task {
                                    await whisperState.startRecordInitial()
                                    startTimer()
                                }
                            }) {
                                Text("Start")
                                    .padding()
                                    .background(Color.powderyBlue)
                                    .foregroundColor(.white)
                                    .cornerRadius(8)
                                    .font(Font.custom("Gaegu-Regular", size : 25))
                            }
                            .disabled(whisperState.isRecording)
                        }
                    }
                    .padding()
                }
                
                if submitted && saved {
                    Text("Note Saved!").font(Font.custom("Gaegu-Bold", size: 30))
                }
                NavigationLink(destination: ContentView().onAppear{
                    navManager.popToRoot()
                }.navigationBarBackButtonHidden(true)) {
                    Text("Home").font(Font.custom("Gaegu-Bold", size : 25)).tint(.powderyBlue)
                }
            }
            .padding()
        }
    }

    func switchLabel() {
        labelText = "Listening..."
        labelFont = 32
    }

    func submit() {
        context.insert(SavedNote(title: title, date: .now, data:  aiResponse, transcript: whisperState.transcriptionText))
        saved = true
    }

    func stopRecordingPress() {
        stopTimer()
        Task {
            await whisperState.stopRecordFinal()
        }
        sendMessage()
    }

    func sendMessage() {
        loading = true
        submitted = true
        aiResponse = ""
        Task {
            do {
                let response = try await model.generateContent(prompt + whisperState.transcriptionText)
                guard let text = response.text else {
                    aiResponse = "Sorry, I could not process that. \nPlease check your connection and try again"
                    loading = false
                    return
                }
                
                textInput = ""
                aiResponse = text
                loading = false
            } catch {
                loading = false
                aiResponse = "Something went wrong! Check your input for inappropriate content. \n\(error.localizedDescription)"
            }
        }
    }

    func startTimer() {
        // Invalidate existing timer if needed
        stopTimer() // Ensure any existing timer is stopped before starting a new one

        // Create and start a new timer
        let timerPublisher = Timer.publish(every: timerInterval, on: .main, in: .common).autoconnect()
        timerSubscription = timerPublisher.sink { _ in
            self.timerTick()
        }

        // Update the state to indicate that the timer is running
        isTimerRunning = true
    }

    func stopTimer() {
        // Cancel the timer subscription if it exists
        timerSubscription?.cancel()
        timerSubscription = nil

        // Update the state to indicate that the timer is stopped
        isTimerRunning = false
    }

    private func timerTick() {
        // Ensure recording is active before stopping and starting a new record
        if whisperState.isRecording {
            Task {
                await whisperState.stopRecord()
            }
        }

        // Update the interval for the next timer tick
        tickCount += 1
        if tickCount == 1 {
            timerInterval = 6.0
        } else {
            timerInterval = 10.0
        }
        
        // Restart the timer with the updated interval
        startTimer()
    }
}

struct RecordingView_Previews: PreviewProvider {
    static var previews: some View {
        RecordingView()
    }
}
