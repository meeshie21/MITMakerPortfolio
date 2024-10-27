//
//  SavedNotes.swift
//  Notecandy
//
//  Created by Vikas Majithia on 8/13/24.
//
import Foundation
import SwiftData

@Model
class SavedNote
{
    let title: String
    let date: Date
    let data: String
    let transcript: String
    
    init(title: String, date: Date, data: String, transcript: String)
    {
        self.title = title
        self.date = date
        self.data = data
        self.transcript = transcript
    }
}


