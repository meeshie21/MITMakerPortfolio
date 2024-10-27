import SwiftUI
import AVFoundation
import Combine

struct NewRecordingView: View {
    @StateObject var whisperState = WhisperState()
    
    @State private var isTimerRunning = false
    @State private var timerSubscription: Cancellable?
    
    var body: some View {
        NavigationStack {
            VStack {
                Text(isTimerRunning ? "Timer is Running" : "Timer is Stopped")
                               .padding()
                
                HStack {
                   /* Button("Transcribe", action: {
                        Task {
                            await whisperState.transcribeSample()
                        }
                    })
                    .buttonStyle(.bordered)
                    .disabled(!whisperState.canTranscribe)*/
                    
                    if whisperState.isRecording{
                        Button("Stop recording", action: {
                            Task {
                                await whisperState.stopRecord()
                            }
                        })
                    }
                    
                    else{
                        Button("Start recording", action: {
                            Task {
                                await whisperState.startRecord()
                                startTimer()
                            }
                        })
                    }
                    
                   /* Button(whisperState.isRecording ? "Stop recording" : "Start recording", action: {
                        Task {
                            await whisperState.toggleRecord()
                        }
                    })
                    .buttonStyle(.bordered)
                    .disabled(!whisperState.canTranscribe)*/
                }
                
                ScrollView {
                    Text("Transcription: " + whisperState.transcriptionText)
                    //Text(verbatim: whisperState.messageLog)
                      //  .frame(maxWidth: .infinity, alignment: .leading)
                }
            }
            .navigationTitle("Whisper SwiftUI Demo")
            .padding()
        }
    
        
    }
    
    func startTimer() {
        // Invalidate existing timer if needed
        stopTimer() // Ensure any existing timer is stopped before starting a new one

        // Create and start a new timer
        let timerPublisher = Timer.publish(every: 10, on: .main, in: .common).autoconnect()
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
    }

}

struct NewRecView_previews: PreviewProvider {
    static var previews: some View {
        NewRecordingView()
    }
}
