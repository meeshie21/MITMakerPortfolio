//
//  SavedNoteView.swift
//  Noteberry
//
//  Created by Vikas Majithia on 8/17/24.
//

import SwiftUI
import MarkdownUI

struct SavedNoteView: View {
    @EnvironmentObject var manager : Manager
    @Environment(\.colorScheme) var colorScheme

    @State var note : SavedNote

    var body: some View {
        
        NavigationView{
                
            ScrollView{
                
                Markdown(manager.user_input).frame(maxWidth: .infinity, alignment: .leading).padding()
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
                
                   
                      
                
                if manager.transcript != ""{
                    Divider().frame(height: 2.5)
                    
                        .overlay(.gray)
                                        Markdown("\n# **Transcript**\n\n" + manager.transcript).frame(maxWidth: .infinity, alignment: .leading).padding()
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
                if manager.user_input == ""{
                    Text("Please wait...").font(Font.custom("Gaegu-Bold", size: 35))
                    ProgressView()
                        .progressViewStyle(CircularProgressViewStyle())
                        .scaleEffect(2)
                        .padding()
                }
            }.padding()

            
        }
    }
}

