//
//  GenerationView.swift
//  Notecandy
//
//  Created by Vikas Majithia on 8/12/24.
//

import Foundation
import SwiftUI
import MarkdownUI
import GoogleGenerativeAI
import SwiftData


struct GenerationView: View {
    //@EnvironmentObject var manager : Manager
    @Query private var savedNotes : [SavedNote]
    @Environment(\.modelContext) private var context
    @Environment(\.colorScheme) var colorScheme
    @EnvironmentObject var navManager: NavigationManager //change
    
    
    let model = GenerativeModel(name: "gemini-1.5-flash", apiKey: APIKey.default)
    @State var textInput = ""
    @State var aiResponse = ""
    @State var loading = false
    @State var submitted = false
    @State var saved = false
    @State private var showingAlert = false
    @State private var title = ""
    let prompt = """
Make pretty and easy-to-read noteswith markdown. Put every single thing from this input into notes. Don't leave out any content at all.
                        All content MUST be in the notes. Use markdown notation for headers, subheaders, and bullet points, bold text, tables. Give good spacing and organization. Used numbered
                         lists often, bullet points, bold text, tables, etc. when applicable. If you have math/equations, use normal english characters. For example, a good way of writing an equation would be something like "x^2 + 3x + 5" -  do not use LaTeX notation.  I repeat, DO NOT use LaTeX notation for anything. Do NOT use HTML or CSS tags such as "<sup></sup>" for math or other purposes. I repeat DO NOT use <sup></sup> and <sub></sub> tags You can use "^" for something like "x^2" in mathematics. If you have lines of code, make sure that they do not horizontally exceed 50 characters (if they do, start a new line of code) If you have a lot of text in one spot, split up the text into smaller bullet points and prevent long blocks of text. For markdown formatting, sections should be headed by level 3 headers with body text inside them, and there should be no title at thee start of  the notes.  Here's the content:
"""

    
    
    var body: some View
    {
        NavigationView{
            
            
            
           
        VStack
        {
            if !loading && !submitted{
                Text("Type/Paste Text:").font(Font.custom("Gaegu-Bold", size : 35))
                if colorScheme == .dark {
                    TextEditor(text: $textInput)
                        .font(Font.custom("Gaegu-Regular", size: 20))
                            .foregroundColor(.white) // Set text color to white
                            .background(Color.clear) // No background fill
                            .cornerRadius(10) // Apply corner radius
                            .overlay(
                                RoundedRectangle(cornerRadius: 10)
                                    .stroke(Color.white, lineWidth: 2) // White border with specified width
                            )
                            .padding() // Add some padding outside the TextEditor
                    
                }
                else{
                    TextEditor(text: $textInput)
                    
                       
                    
                        .font(Font.custom("Gaegu-Regular", size : 20))
                        .colorMultiply(Color(red:0.9, green:0.9, blue:0.9))
                        .cornerRadius(10)
                        .padding() // Add some padding outside the TextEditor
                    //.frame(height: 200) // Optional: Set a fixed height for better appearance
                }
            }
            
            if loading{
                ProgressView()
                    .progressViewStyle(CircularProgressViewStyle())
                    .scaleEffect(2)
                    .padding()
            }
            
            
            if !loading && !submitted{
                Button(action:sendMessage,label:{Image(systemName: "paperplane.fill").tint(.powderyBlue)})
                Text("")
                
            }
            if aiResponse != "" && submitted && !saved {
                ScrollView{
                    Markdown(aiResponse).frame(maxWidth: .infinity, alignment: .leading).padding()
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

                }
                
                Button(action: {
                            showingAlert.toggle()
                        }) {
                            Text("Save Note")
                                .font(Font.custom("Gaegu-Regular", size: 25)) // Customize font size and weight here
                                .foregroundColor(.powderyBlue) // Customize the color if needed
                        }
                        .alert("Enter a title", isPresented: $showingAlert) {
                            TextField("Type a title...", text: $title)
                            Button("Submit", action: submit)
                        } message: {
                            Text("Enter a title for your note.")
                        }
            }
            
            if submitted && saved{
                Text("Note Saved!").font(Font.custom("Gaegu-Bold", size: 30))
            }
            NavigationLink(destination: ContentView().onAppear{
                navManager.popToRoot()
            }.navigationBarBackButtonHidden(true)){
                Text("Home")                    .font(Font.custom("Gaegu-Bold", size : 25)).tint(.powderyBlue)

            }
            }
        }
        .padding()
        
    }
    
    func submit()
    {
        saved = true
        context.insert(SavedNote(title: title, date: .now, data: aiResponse, transcript: ""))
    }
    
    func sendMessage(){
        loading = true
        submitted = true
        aiResponse = ""
        Task{
            do{
                let response = try await model.generateContent(prompt + textInput)
                guard let text = response.text else {
                    aiResponse = "Sorry, I could not process that. \nPlease check your connection and try again"
                    loading = false
                    return
                }
                
                textInput = ""
                aiResponse = text
                loading = false
            }catch{
                loading = false
                aiResponse = "Something went wrong! Check your input for inappropriate content. \n\(error.localizedDescription)"
            }
        }
    }
}

struct GenerationView_Preview: PreviewProvider {
    static var previews: some View {
        GenerationView()
    }
}

