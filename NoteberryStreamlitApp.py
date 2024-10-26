import streamlit as st
import time as t
import html
import pickle

from fpdf import FPDF
import base64

import os
from gtts import gTTS
from pydub import AudioSegment
from io import BytesIO

import fitz  # PyMuPDF

import speech_recognition
import pyttsx3

# Set light purple color directly
light_purple_color = "#d1c4e9"  # Light purple color

st.markdown(
    f"""
    <style>
    @import url('https://fonts.googleapis.com/css2?family=Poppins:wght@300;400;600&display=swap');

    body {{
        font-family: 'Poppins', sans-serif;
        background-color: #eaeaea; /* Bright background */
        color: #333; /* Darker text for contrast */
    }}
    h1, h2, h3, h4, h5, h6 {{
        font-family: 'Poppins', sans-serif;
        color: {light_purple_color}; /* Light purple color for headings */
        font-weight: 600; /* Bold headings */
    }}
    .stapp {{
        max-width: 900px;
        margin: auto;
        padding: 20px;
        background-color: #ffffff; /* White background for the app */
        border-radius: 10px;
        box-shadow: 0px 0px 20px rgba(0, 0, 0, 0.15);
    }}
    .stbutton>button {{
        background-color: {light_purple_color}; /* Custom light purple buttons */
        color: white;
        border-radius: 8px;
        padding: 10px 20px;
        font-size: 16px;
        font-family: 'Poppins', sans-serif; /* Font for button text */
    }}
    .stbutton>button:hover {{
        background-color: #b39ddb; /* Slightly darker purple on hover */
    }}
    .stcheckbox>div:first-child {{
        transform: scale(1.3);
        margin-right: 10px;
    }}
    .stcheckbox>div>div {{
        font-family: 'Poppins', sans-serif; /* Font for checkbox labels */
        color: {light_purple_color}; /* Color for checkbox labels */
    }}

    /* Styling for select box labels */
    .stSelectbox>div>label,
    .stMultiSelectbox>div>label {{
        font-family: 'Poppins', sans-serif; /* Font for select box labels */
        color: {light_purple_color}; /* Color for select box labels */
        font-weight: 600; /* Bold select box labels */
    }}
    .stselectbox>div>div:first-child,
    .stmultiselect>div>div:first-child {{
        font-size: 18px;
        font-weight: 500;
        color: {light_purple_color}; /* Color for dropdowns */
    }}
    .stselectbox>div>div>div>div,
    .stmultiselect>div>div>div>div {{
        background-color: #f7f7f7; /* Background for dropdown items */
        color: #333;
        border-radius: 8px;
    }}
    .stselectbox>div>div>div>div:hover,
    .stmultiselect>div>div>div>div:hover {{
        background-color: #e0e0e0; /* Hover color for dropdown items */
    }}
    .sttextinput>div>div>input,
    .sttextarea>div>textarea {{
        border-radius: 8px;
        padding: 10px;
        border: 1px solid #ccc; /* Light border */
        font-size: 16px;
        background-color: #ffffff; /* White background for input fields */
        color: #333; /* Dark text */
        font-family: 'Poppins', sans-serif; /* Font for inputs */
    }}
    .sttextinput>div>div>input:focus,
    .sttextarea>div>textarea:focus {{
        outline: none;
        border: 1px solid {light_purple_color}; /* Custom light purple border on focus */
    }}

    .stfile_uploader>div>div {{
        border: 2px dashed {light_purple_color}; /* Dashed border for file uploader */
        border-radius: 8px;
        padding: 20px;
        background-color: #f9f9f9; /* Light gray background for file uploader */
        color: {light_purple_color}; /* Color for file uploader text */
        font-family: 'Poppins', sans-serif; /* Font for file uploader text */
    }}
    .stfile_uploader:hover>div {{
        background-color: #f0f0f0; /* Hover effect for file uploader */
    }}
    .stslider>div>div {{
        color: {light_purple_color}; /* Slider text color */
        font-family: 'Poppins', sans-serif; /* Font for slider labels */
    }}
    .stradio>div>div {{
        color: {light_purple_color}; /* Radio button text color */
    }}
    .stexpander>div>div:first-child {{
        background-color: {light_purple_color}; /* Expander title background */
        color: white;
        border-radius: 8px;
        padding: 10px;
    }}
    .ststatus {{
        background-color: {light_purple_color}; /* Status background */
        color: white;
        font-size: 16px;
        font-family: 'Poppins', sans-serif; /* Font for status messages */
    }}
    .stmarkdown h2 {{
        color: {light_purple_color}; /* Markdown heading color */
    }}
    .stprogress>div {{
        background-color: {light_purple_color}; /* Progress bar color */
        border-radius: 8px;
    }}
    .st.spinner {{
        color: {light_purple_color}; /* Spinner color */
        font-family: 'Poppins', sans-serif; /* Font for spinner */
    }}
    /* Additional styles for Toast */
    .Toastify__toast {{
        background-color: {light_purple_color}; /* Toast background */
        color: white; /* Toast text color */
        font-family: 'Poppins', sans-serif; /* Font for toast text */
    }}
    </style>
    """,
    unsafe_allow_html=True,
)

def read_pdf(file):
    pdf_document = fitz.open(stream=file.read(), filetype="pdf")
    text = ""
    for page_num in range(pdf_document.page_count):
        page = pdf_document.load_page(page_num)
        text += page.get_text("text")  # Using "text" option for better text extraction
    return text


def transcribe_audio_chunks(audio_file, chunk_length=30):
    recognizer = speech_recognition.Recognizer()

    # Load the .wav file
    with speech_recognition.AudioFile(audio_file) as source:
        audio_duration = source.DURATION  # Get the total duration of the audio file

        # Initialize an empty string to store the complete transcription
        full_transcription = ""

        # Process the audio in chunks
        for start_time in range(0, int(audio_duration), chunk_length):
            # Calculate the end time for the current chunk
            end_time = min(start_time + chunk_length, int(audio_duration))

            # Record the chunk from start_time for the duration of chunk_length
            audio_chunk = recognizer.record(source, offset=start_time, duration=chunk_length)

            # Try to recognize the chunk
            try:
                chunk_transcription = recognizer.recognize_google(audio_chunk)
                full_transcription += chunk_transcription + " "
            except speech_recognition.UnknownValueError:
                print(f"Google Web Speech API could not understand the audio from {start_time}s to {end_time}s.")
            except speech_recognition.RequestError as e:
                print(f"Could not request results from Google Web Speech API; {e}")

    return full_transcription.strip()


def wav_to_audiodata(wav_file_path):
    # Initialize the recognizer
    recognizer = speech_recognition.Recognizer()

    # Load the WAV file using AudioFile
    audio_data = speech_recognition.AudioFile(wav_file_path)

    # Convert the WAV file to AudioData
    with audio_data as source:
        audio = recognizer.record(source)

    return audio


# Example usage:
# audiodata = wav_to_audiodata("path_to_your_file.wav")


def convert_to_wav_mono_pcm(input_file):
    # Load the audio file
    audio = AudioSegment.from_file(input_file)
    # Convert to mono
    audio = audio.set_channels(1)
    # Ensure it is PCM format
    audio = audio.set_sample_width(2)
    # Create a temporary file for the converted audio
    output_file = "converted.wav"
    # Export as WAV
    audio.export(output_file, format="wav")
    return output_file

def create_download_link(filename):
    with open(filename, "rb") as f:
        pdf_bytes = f.read()
        b64 = base64.b64encode(pdf_bytes).decode()  # encode in base64 (binary-to-text)
        return f'<a href="data:application/pdf;base64,{b64}" download="{filename}">Download file</a>'

def show_pdf_file():
    from markdown_pdf import MarkdownPdf
    from markdown_pdf import Section
    import html as h

    # Initialize MarkdownPdf and add sections
    pdf = MarkdownPdf(toc_level=1)
    pdf.add_section(
        Section(
            '''<style>
            @import url('https://fonts.googleapis.com/css2?family=Roboto:wght@400;700&display=swap');
            body {
            font-family: 'Roboto', sans-serif;
            font-size: 12pt;
            color: #333333;
            }
            h1 {
            font-size: 36pt;
            color: #0066cc;
            font-weight: bold;
            }
            h2 {
            font-size: 20pt;
            color: #333333;
            }
            h3 {
            font-size: 16pt;
            color: #333333;
            }
            p {
            margin-bottom: 10pt;
            }
            </style>'''

            +
              "\n" + myContainer.notestext))  # Replace with your actual content

    # Save the PDF to a file
    pdf_file = "AI-Generator-" + str(myContainer.notestext[myContainer.notestext.find(' ') + 1:myContainer.notestext.find('\n')]) + "_notes.pdf"
    pdf.save(pdf_file)

    # Create a download link for the PDF
    html = create_download_link(pdf_file)

    # Display the download link in Streamlit
    st.markdown(html, unsafe_allow_html=True)


import pathlib
import textwrap
from datetime import time

import google.generativeai as genai

from IPython.display import display
from IPython.display import Markdown

st.title("AI Content Generator")
option = st.selectbox('Select an option',['Upload Audio/Record a Lecture', 'Upload PDF/Enter Text Content'])
contentformat = st.selectbox('Select an option',['Generate Notes', 'Generate Audio Notes', 'Generate Video Notes (Experimental)'])
summarize = st.checkbox("Summarize (This will condense the content into short, precise information)")

recordingmode = option == 'Upload Audio/Record a Lecture'
uploadedaudio = st.file_uploader(label="Upload WAV or MP3 Audio", type=['wav', 'mp3']) if recordingmode else st.empty()
uploadedtext = st.file_uploader(label="Upload PDF Document (May not work well with Math/Equations)", type='pdf') if not recordingmode else st.empty()

subheader = st.empty()

if uploadedtext is None or uploadedaudio is None:
    subheader = st.subheader("OR")
else:
    subheader = st.empty()

user_input = None if recordingmode else (st.text_area("Enter your content:", height=200) if uploadedtext is None else read_pdf(uploadedtext))


class datacontainerobj:
    def __init__(self):
        try:
            with open("audiotranscription.p", "rb") as audiofile:
                self.audiotranscription = audiofile
        except FileNotFoundError:
            self.audiotranscription = None


        self.pressed = False
        self.notestext = None
        self.audiogeneratefromlecture = None
        self.hasStoppedRecording = False
        self.hasStartedRecording = False
        self.transcription = None
        self.transcript = ""


myContainer = datacontainerobj()





def generatecontent(recordingmode = False):
    myContainer.pressed = False
    # Use the input from the text area to run code
    if contentformat == 'Generate Notes' and not myContainer.pressed:
        myContainer.pressed = True

        # The 'with' statement is used here to manage the spinner context
        with st.spinner("Initializing AI Model..."):
            genai.configure(api_key='AIzaSyCZObffhLbps5I-NhjgDF5seb-eeZQ8bP8')

            model = genai.GenerativeModel('gemini-1.5-flash')

        def to_markdown(text):
            text = text.replace('•', '  *')
            return Markdown(textwrap.indent(text, '> ', predicate=lambda _: True))

        input_text = user_input if not recordingmode else myContainer.audiotranscription

        response = ""

        with st.spinner("Processing your content with AI..."):
            response = model.generate_content(str(r'''Put every single thing from this input into notes. Don't leave out any content at all. 
            All content MUST be in the notes. Use markdown notation for headers, subheaders, and bullet points. Give good spacing and organization. Used numbered
             lists often, bullet points, etc. when applicable. If you have math equations, use Latex notation. For example, "f&#x27;(x) = lim_{\Delta x \to 0} \frac{f(x + \Delta x) - f(x)}{\Delta x}" would be
             converted to LaTeX notation. Here's the content:''') + str(
                input_text)) if not summarize else model.generate_content(str(r'''Summarize and condense this content into notes with just the most important
                information. Only get the important info and overview, no need to include all of the content. Use markdown notation for headers, subheaders, and bullet points. Used numbered
             lists often, bullet points, etc. when applicable.  If you have math equations, use Latex notation. For example, "f&#x27;(x) = lim_{\Delta x \to 0} \frac{f(x + \Delta x) - f(x)}{\Delta x}" would be
             converted to LaTeX notation. Give good spacing and organization.  Here's the content''') + str(
                input_text))
            myContainer.notestext = response.text

        st.write("Your AI Notes are Ready!")

        show_pdf_file()

        # Render the styled box using st.markdown
        st.markdown(myContainer.notestext, unsafe_allow_html=True)
        myContainer.pressed = False






















































    elif contentformat == 'Generate Audio Notes' and not myContainer.pressed:
        myContainer.notestext = None
        myContainer.pressed = True

        # The 'with' statement is used here to manage the spinner context
        with st.spinner("Initializing AI Audio Model..."):
            genai.configure(api_key='AIzaSyCZObffhLbps5I-NhjgDF5seb-eeZQ8bP8')

            model = genai.GenerativeModel('gemini-1.5-flash')

        def to_markdown(text):
            text = text.replace('•', '  *')
            return Markdown(textwrap.indent(text, '> ', predicate=lambda _: True))

        input_text = user_input if not recordingmode else myContainer.audiotranscription

        with st.spinner("Processing your content with AI..."):
            myContainer.audiogeneratefromlecture = model.generate_content(str('''Summarize and put this content into notes with ALL of the
                information. Include all information, don't leave out ANY content. Then, take those notes and make it into a long paragraph to be read out loud as a 
                lecture, proportional to the length of the notes. No characters other than what you would see in an english essay, because
               a text to speech agent will be reading this out loud. For example, no greater than symbols because the
               text to speech agent will read out "greater than symbol." Have good spacing in the script because
               a transcript will be provided. Don't provide me with the notes, just the lecture script paragraph. If certain sentences don't make sense,
                rephrase the sentence to make sense in English language (as some of the sentences are from a faulty audio transript). Heres the content:''') + str(
                input_text)).text if not summarize else model.generate_content(str('''Summarize and condense this content into notes with just the most important
                information. Only get the important info and overview, no need to include all of the content. Just a brief summary of the most important stuff. 
                Then, take those notes and make it into a paragraph to be read out loud as a lecture, proportional to the length of the notes. 
                No characters other than what you would see in an english essay, because
               a text to speech agent will be reading this out loud. For example, no greater than symbols because the
               text to speech agent will read out "greater than symbol." Have good spacing in the script because
               a transcript will be provided. Don't provide me with the notes, just the lecture script paragraph. If certain sentences don't make sense,
                rephrase the sentence to make sense in English language (as some of the sentences are from a faulty audio transript). Heres the content:''') + str(
                input_text)).text

        # Function to split text into segments of maximum 1500 characters ending at sentence boundaries
        def split_text_into_segments(text, max_length=1500):
            import re
            sentence_endings = re.compile(r'(?<=[.!?]) +')
            sentences = sentence_endings.split(text)

            segments = []
            current_segment = ""

            for sentence in sentences:
                if len(current_segment) + len(sentence) + 1 <= max_length:
                    if current_segment:
                        current_segment += " "
                    current_segment += sentence
                else:
                    segments.append(current_segment)
                    current_segment = sentence

            if current_segment:
                segments.append(current_segment)

            return segments

        # Function to synthesize speech and return audio bytes using gTTS
        def synthesize_speech(text, lang='en'):
            tts = gTTS(text=text, lang=lang)
            audio_bytes = BytesIO()
            tts.write_to_fp(audio_bytes)
            audio_bytes.seek(0)
            return audio_bytes.read()

        # Function to join audio streams
        def join_audio_streams(audio_streams):
            combined = AudioSegment.empty()
            for audio_stream in audio_streams:
                audio_segment = AudioSegment.from_file(BytesIO(audio_stream))
                combined += audio_segment

            output_bytes = BytesIO()
            combined.export(output_bytes, format="mp3")
            output_bytes.seek(0)
            return output_bytes

        # Main text to be converted to speech
        text_to_convert = myContainer.audiogeneratefromlecture

        with st.spinner("Generating Audio File... "):
            # Split the text into segments
            segments = split_text_into_segments(text_to_convert)

            # Synthesize speech for each segment and collect audio streams
            audio_streams = []
            for segment in segments:
                audio_stream = synthesize_speech(segment)
                audio_streams.append(audio_stream)

            # Join the audio streams into one
            final_output_bytes = join_audio_streams(audio_streams)

        # Display the combined audio to the user
        st.write("Your audio file is ready!")
        st.audio(final_output_bytes.getvalue(), format='audio/mp3')
        st.write("")
        st.subheader("Transcript:")
        with st.spinner("Generating user-friendly transcript..."):
            st.markdown((model.generate_content('''If there are any math equations present or anything that looks like it
            should be a math equation (such as the limit of f of x as x approaches a), convert them
                                                to latex notation. Add in markdown headers and subheaders. Keep the text unchanged.
                                                Keeping the text unchanged, use markdown format to apply bullet points and numbered lists if applicable.
                                                Keep everything else unchanged. If there is math that should be converted to equations or symbols
                                                use Latex notation. Keep the text unchanged.
                                                Here's the content: ''' + myContainer.audiogeneratefromlecture).text),
                        unsafe_allow_html=True)
        myContainer.pressed = False



    # Use the input from the text area to run code
    elif contentformat == 'Generate Video Notes (Experimental)' and not myContainer.pressed:
        myContainer.notestext = None
        myContainer.pressed = True

        # The 'with' statement is used here to manage the spinner context
        with st.spinner("Initializing AI Video Model..."):
            genai.configure(api_key='AIzaSyCZObffhLbps5I-NhjgDF5seb-eeZQ8bP8')

            model = genai.GenerativeModel('gemini-1.5-flash')

        def to_markdown(text):
            text = text.replace('•', '  *')
            return Markdown(textwrap.indent(text, '> ', predicate=lambda _: True))

        input_text = user_input if not recordingmode else myContainer.audiotranscription

        with st.spinner("Processing your content with AI..."):
            response = model.generate_content(str('''Summarize and put this content into notes with ALL of the
                information. Include all information, don't leave out ANY content. Then, take those notes and make it into a long paragraph to be read out loud as a 
                lecture, proportional to the length of the notes. No characters other than what you would see in an english essay, because
               a text to speech agent will be reading this out loud. For example, no greater than symbols because the
               text to speech agent will read out "greater than symbol." Have good spacing in the script because
               a transcript will be provided. Don't provide me with the notes, just the lecture script paragraph. This paragraph will be the video script.

               Demarcate this video script with quotation marks, and before the video script, generate a prompt for an AI video generator to make a video about 
               that topic demarcated by curly brackets, give the tone/mood, background images that correspond to the script, text to speech : male voice, 
               and primary audience and goal. Heres the content:''') + str(
                input_text)) if not summarize else model.generate_content(str('''Summarize and condense this content into notes with just the most important
                information. Only get the important info and overview, no need to include all of the content. Just a brief summary of the most important stuff. 
                Then, take those notes and make it into a paragraph to be read out loud as a lecture, proportional to the length of the notes. 
                No characters other than what you would see in an english essay, because
               a text to speech agent will be reading this out loud. For example, no greater than symbols because the
               text to speech agent will read out "greater than symbol." Have good spacing in the script because
               a transcript will be provided. Don't provide me with the notes, just the lecture script paragraph. This paragraph will be the video script.


                Demarcate this video script with quotation marks, and before the video script, generate a prompt for an AI video generator to make a video about 
               that topic demarcated by curly brackets, give the tone/mood, background images that correspond to the script, text to speech : male voice, 
               and primary audience and goal. Heres the content:''') + str(input_text))

        def extract_inside_curly_braces(s):
            start = s.find('{') + 1
            end = s.find('}', start)
            if start > 0 and end > start:
                return s[start:end]
            return None  # or return an empty string or raise an error, depending on your needs

        def after_curly_braces(s):
            start = s.find('{') + 1
            end = s.find('}', start)
            if start > 0 and end > start:
                return s[end + 1:]
            return None  # or return an empty string or raise an error, depending on your needs


        import requests

        url = 'https://www.veed.io/text-to-video-ap/api/generate/async'
        headers = {
            'Content-Type': 'application/json',
        }

        data = {
            "prompt": extract_inside_curly_braces(response.text),
            "script": after_curly_braces(response.text),
            "voice": "male"
        }

        with st.spinner("Generating Video Link..."):
            response = requests.post(url, json=data, headers=headers)

        if response.status_code == 200:
            result = response.json()

            progress_bar = st.progress(0)
            percent_complete = 0
            loading_message = st.empty()

            for i in range(1000):
                percent_complete += 0.001
                percentstring = str(percent_complete * 100)
                loading_message.write("Loading Final Video (" + percentstring[0: percentstring.find('.') + 2] + "%)...")
                t.sleep(.18 * (len(after_curly_braces(response.text)) / 2500.0))  # Simulate work being done
                progress_bar.progress(percent_complete)

            loading_message = st.empty()
            st.write("Video Link:", result['project']['link'])
            myContainer.pressed = False

            # print("Project ID:", result['project']['id'])
            # print("Project Title:", result['project']['title'])
            # print("Project Link:", result['project']['link'])
            # print("Project Thumbnail:", result['project']['thumbnail'])
        else:
            st.write("There was an error. Please try again.")
            myContainer.pressed = False


if recordingmode:
    import shutil

    import streamlit as st
    import pyaudio
    import wave
    import os


    def specialgeneratefortranscribe():
        with st.expander("Final Transcript"):
            st.write(str(pickle.load(open("audiotranscription.p", "rb"))))
        st.write("Recording Stopped.")
        myContainer.hasStoppedRecording = True
        st.toast("Generation Started...")
        generatecontent(recordingmode=True)
        st.toast("Generation Complete!")
        st.balloons()
        st.session_state.running = False

    def record_and_transcribe_audio(stopbutton : st.button):
        if stopbutton:
            specialgeneratefortranscribe()

        if st.session_state.running == True:
            recognizer = speech_recognition.Recognizer()

            try:
                with speech_recognition.Microphone() as mic:
                    recognizer.adjust_for_ambient_noise(mic)
                    audio = recognizer.listen(mic)

                    text = recognizer.recognize_google(audio)
                    text = text.lower()
                    myContainer.transcript = myContainer.transcript + text
                    myContainer.transcription.write(str(myContainer.transcript))
                    myContainer.audiotranscription = str(myContainer.transcript)
                    pickle.dump(myContainer.audiotranscription, open("audiotranscription.p", "wb"))


            except speech_recognition.UnknownValueError:
                recognizer = speech_recognition.Recognizer()
                pass


    if 'running' not in st.session_state:
        st.session_state.running = False

    if uploadedaudio is None:
        startbutton = st.button("Start Recording")
        subheader = st.empty()

        if startbutton:
            subheader = st.subheader("Transcript: ")
            myContainer.transcription = st.empty()
            st.session_state.running = True

    if st.session_state.running == True:
        stopbutton = st.button("Stop Recording")

        for i in range(10000):
            if st.session_state.running == False:
                break
            record_and_transcribe_audio(stopbutton=stopbutton)

    elif st.button("Generate from Upload"):
        with st.status(label = "Transcribing Upload", expanded = True):
            text = transcribe_audio_chunks(convert_to_wav_mono_pcm(uploadedaudio))
            st.write(str(text))
            myContainer.audiotranscription = str(text)

        st.toast("Generation Started...")
        generatecontent(recordingmode=True)
        st.toast("Generation Complete!")
        st.balloons()




elif st.button("Generate"):
    st.toast("Generation Started...")
    generatecontent(recordingmode=False)
    st.toast("Generation Complete!")
    st.balloons()






































