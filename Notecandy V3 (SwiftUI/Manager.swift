//
//  Manager.swift
//  Notecandy
//
//  Created by Vikas Majithia on 8/12/24.
//

import Foundation

class Manager : ObservableObject{
    @Published var user_input : String = ""
    @Published var transcript : String = ""
}
