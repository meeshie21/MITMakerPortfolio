import datetime
import io
import traceback

import flet as ft
import time as t

import tempfile

import base64

import textwrap

from google.generativeai.types import HarmCategory, HarmBlockThreshold
from gtts import gTTS
from pydub import AudioSegment
from io import BytesIO

import fitz  # PyMuPDF

import speech_recognition

import requests
from IPython.display import Markdown
from flet.security import encrypt, decrypt

import google.generativeai as genai


def clearandaddtitle(page):
    page.controls.clear()

    '''title = ft.Text(spans=[
            ft.TextSpan(
                "Notecandy",
                ft.TextStyle(
                    size=42,
                    weight=ft.FontWeight.BOLD,
                    foreground=ft.Paint(
                        gradient=ft.PaintLinearGradient(
                            (0, 20),
                            (150, 20),
                            ["#E56399", "#6CBEED"]
                        )
                    ),
                ),
            ),
        ft.TextSpan(
            "\nFree AI Note-Taker",
            ft.TextStyle(
                size=20,
                weight=ft.FontWeight.BOLD,
                foreground=ft.Paint(
                    gradient=ft.PaintLinearGradient(
                        (0, 20),
                        (150, 20),
                        ["#E56399", "#6CBEED"]
                    )
                ),
            ),
        )
        ],)

    page.add(title)
    '''
    page.update()


def read_pdf(file_path):
    pdf_document = fitz.open(file_path)  # Open the PDF using the file path
    text = ""
    for page_num in range(pdf_document.page_count):
        page = pdf_document.load_page(page_num)
        text += page.get_text("text")  # Using "text" option for better text extraction
    return text
def convert_to_wav_mono_pcm(input_file):
    # Load the audio file
    audio = AudioSegment.from_file(input_file)
    # Convert to mono
    audio = audio.set_channels(1)
    # Ensure it is PCM format
    audio = audio.set_sample_width(2)
    # Create a temporary file for the converted audio
    with tempfile.NamedTemporaryFile(suffix=".wav", delete=False) as temp_file:
        output_file = temp_file.name
        # Export as WAV
        audio.export(output_file, format="wav")

    return output_file
def generateafteroption(myContainer, inputstr, page: ft.Page):
    recordingmode = myContainer.recordingmode
    summarize = myContainer.summarize

    clearandaddtitle(page=page)

    def uploadbeforegenerate():
        page.appbar.leading.on_click = None
        clearandaddtitle(page=page)
        selected_files = ft.Text("")
        page.add(selected_files)
        page.update()

        def pick_files_result(e: ft.FilePickerResultEvent):
            selected_files.value = (
                ", ".join(map(lambda f: f.name, e.files)) if e.files else "Cancelled!"
            )
            selected_files.update()

            if e.files:
                for f in e.files:
                    path = f.path

            myContainer.uploadedaudio = path if recordingmode else None
            myContainer.uploadedtext = None if recordingmode else path

        uploadedaudio = ft.FilePicker(on_result=pick_files_result) if recordingmode else None
        uploadedtext = ft.FilePicker(on_result=pick_files_result) if not recordingmode else None

        if recordingmode:
            page.add(uploadedaudio)
            page.add(
                ft.ElevatedButton(height=80, content=
                ft.Row(
                    # width=230,
                    controls=[
                        ft.Icon(ft.icons.AUDIO_FILE, size=45),
                        ft.Column(spacing=-3, controls=[
                            ft.Text("", size=8),
                            ft.Text("Upload Audio", size=22,
                                    color=ft.colors.INVERSE_SURFACE,
                                    font_family="Gaegu"),
                            ft.Text("Click to select a file", size=15,
                                    color="grey",
                                    font_family="Gaegu")
                        ])]
                ), on_click=lambda _: uploadedaudio.pick_files(
                                allow_multiple=False, file_type=ft.FilePickerFileType.CUSTOM,
                                allowed_extensions=['mp3', 'wav', 'm4a']))
            )

        else:
            page.add(uploadedtext)
            page.add(
                ft.ElevatedButton(height=80, content=
                ft.Row(
                    # width=230,
                    controls=[
                        ft.Icon(ft.icons.UPLOAD_FILE_OUTLINED, size=45),
                        ft.Column(spacing=-3, controls=[
                            ft.Text("", size=8),
                            ft.Text("Upload PDF", size=22,
                                    color=ft.colors.INVERSE_SURFACE,
                                    font_family="Gaegu"),
                            ft.Text("Click to select a file", size=15,
                                    color="grey",
                                    font_family="Gaegu")
                        ])]
                ), on_click=lambda _: uploadedtext.pick_files(
                                allow_multiple=False, file_type=ft.FilePickerFileType.CUSTOM,
                                allowed_extensions=['pdf']))
            )

        subheader = ft.Text("", theme_style=ft.TextThemeStyle.BODY_LARGE)
        page.add(subheader)
        page.update()

        if recordingmode:
            while uploadedaudio.result is None:
                page.update()
        else:
            while uploadedtext.result is None:
                page.update()

        page.add(ft.ElevatedButton(icon=ft.icons.NAVIGATE_NEXT_OUTLINED, text="Generate", on_click=generation))

    def silence_based_conversion(audio_file):
        # open the audio file stored in
        # the local system as a wav file.
        song = AudioSegment.from_wav(audio_file)

        with tempfile.NamedTemporaryFile(suffix=".wav", delete=False) as temp_file:
            temp_song_name = temp_file.name
            song.export(temp_song_name, format="wav")

        # open a file where we will concatenate
        # and store the recognized text

        full_transcription = ""
        myContainer.silencebasedtranscription = full_transcription

        # split track where silence is 0.5 seconds
        # or more and get chunks
        # Set the interval in milliseconds (e.g., 30 seconds)
        interval = 30 * 1000  # 30 seconds

        # Split the audio into chunks and store them in a list
        chunks = [song[i:i + interval] for i in range(0, len(song), interval)]

        # chunks = split_on_silence(song,
        # must be silent for at least 0.5 seconds
        # or 500 ms. adjust this value based on user
        # requirement. if the speaker stays silent for
        # longer, increase this value. else, decrease it.
        # min_silence_len=600,

        # consider it silent if quieter than -16 dBFS
        # adjust this per requirement
        # silence_thresh=-16
        # )

        i = 0
        # process each chunk
        for chunk in chunks:
            with tempfile.NamedTemporaryFile(suffix=".wav", delete=False) as temp_file:
                temp_file_name = temp_file.name
                chunk.export(temp_file_name, format="wav")

            # create a speech recognition object
            r = speech_recognition.Recognizer()
            r.pause_threshold = 0.05
            r.phrase_threshold = 0.1
            r.energy_threshold = 150
            r.non_speaking_duration = 0.001

            # recognize the chunk
            with speech_recognition.AudioFile(temp_file_name) as source:
                audio_listened = r.listen(source)

            try:
                # try converting it to text
                rec = r.recognize_google(audio_listened)
                rec = rec[14: len(rec) - 3]
                # write the output to the file.
                full_transcription = myContainer.silencebasedtranscription
                myContainer.silencebasedtranscription = full_transcription + rec + " "
                # catch any errors.
            except speech_recognition.UnknownValueError:
                r = speech_recognition.Recognizer()
            except speech_recognition.RequestError as e:
                page.add(ft.Text(("Could not request results. check your internet connection")))

            i += 1

    def change_user_input(e):
        myContainer.user_input = e.control.value
        page.update()

    def generatecontent(recordingmode: bool):
        clearandaddtitle(page=page)
        myContainer.pressed = False
        # Use the input from the text area to run code
        if myContainer.format == 'Generate Notes' and not myContainer.pressed:
            myContainer.pressed = True

            aiprogressbar = ft.Row([ft.ProgressRing(), ft.Text("Initializing AI Model...")])
            # The 'with' statement is used here to manage the spinner context
            page.add(aiprogressbar)
            page.update()

            genai.configure(api_key=decrypt(page.client_storage.get("secret"), "1234"))

            model = genai.GenerativeModel('gemini-1.5-flash')
            page.remove(aiprogressbar)
            page.update()

            def to_markdown(text):
                text = text.replace('•', '  *')
                return Markdown(textwrap.indent(text, '> ', predicate=lambda _: True))

            input_text = myContainer.audiotranscription if recordingmode else myContainer.user_input

            response = ""

            aiprogressbar = ft.Row([ft.ProgressRing(), ft.Text("Processing your content with AI...")])

            page.add(aiprogressbar)
            page.update()

            # Render the styled box using st.markdown

            try:
                if myContainer.lecturetranscription != "":
                    myContainer.lecturetranscription = model.generate_content(
                        "Post process this lecture transcript by adding capitalization and punctuation as needed and return only the transcript in your response. If there are any apparent mis-transcriptions, fix them. Here it is: " + myContainer.lecturetranscription).text
                    input_text = myContainer.lecturetranscription

                response = model.generate_content(str(r'''Make pretty and easy-to-read noteswith markdown. Put every single thing from this input into notes. Don't leave out any content at all. 
                        All content MUST be in the notes. Use markdown notation for headers, subheaders, and bullet points, bold text, tables. Sections should be headed by level 3 headers (3 hashtags - ### for markdown) at all times and should contain body text. Give good spacing and organization. Used numbered
                         lists often, bullet points, bold text, tables, etc. when applicable. If you have math/equations, use normal english characters and do not use LaTeX notation. Do not use HTML or CSS tags such as "<sup></sup>" for math or other purposes. If you have a lot of text in one spot, split up the text into smaller bullet points and prevent long blocks of text. Sections should be headed by level 3 headers (3 hashtags - ### for markdown) at all times and should contain body text. Here's the content:''') + str(
                    input_text)) if not summarize else model.generate_content(str(r'''Make pretty and easy-to-read notes with markdown. Summarize and condense this content into notes with just the most important
                            information. Only get the important info and overview, no need to include all of the content. Use markdown notation for headers, subheaders, and bullet points, and bold text, tables. If you have a lot of text in one spot, split up the text into smaller bullet points and prevent long blocks of text. Sections should be headed by level 3 headers (3 hashtags - ### for markdown) at all times and should contain body text. Used numbered
                         lists often, bullet points,bold text, tables,  etc. when applicable. If you have math/equations, use normal english characters and do not use LaTeX notation. Do not use HTML or CSS tags such as "<sup></sup>" for math or other purposes. Sections should be headed by level 3 headers (3 hashtags - ### for markdown) at all times and should contain body text. Give good spacing and organization.  Here's the content''') + str(
                    input_text))
            except ValueError:
                page.add(ft.Text(
                    "The response was blocked. Check your input for any inappropriate content and try again."))
                page.remove(aiprogressbar)
                page.update()
            except Exception as e:
                page.add(ft.Text("An unknown error occurred. Please check your internet connection and try again."))
                page.remove(aiprogressbar)
                page.update()
            myContainer.notestext = response.text
            print(myContainer.notestext)
            markdownrender = ft.Markdown(myContainer.notestext.replace("\n###", "\n#\n#\n###"),
                                         extension_set=ft.MarkdownExtensionSet.GITHUB_WEB)

            page.remove(aiprogressbar)
            page.update()

            def savenote(e):
                clearandaddtitle(page=page)

                notetitle = ft.TextField(label="Click to Enter Title", value="")
                page.add(notetitle)
                page.update()

                def savenotewithtitle(e):
                    page.remove(e.control)
                    if myContainer.lecturetranscription != "":
                        transcripts = page.client_storage.get("savedlecturetranscripts")
                        transcripts.append(myContainer.lecturetranscription)
                        page.client_storage.set("savedlecturetranscripts", transcripts)
                    else:
                        transcripts = page.client_storage.get("savedlecturetranscripts")
                        transcripts.append("")
                        page.client_storage.set("savedlecturetranscripts", transcripts)

                    notes = page.client_storage.get("savednotes")
                    notes.append(myContainer.notestext.replace("\n###", "\n#\n#\n###"))
                    page.client_storage.set("savednotes", notes)

                    titles = page.client_storage.get("savednotetitles")
                    titles.append(notetitle.value + "NoteDate" + str(datetime.datetime.now().strftime("%m/%d/%Y")))
                    page.client_storage.set("savednotetitles", titles)

                    clearandaddtitle(page=page)
                    page.add(ft.Text("Note Saved!"))
                    page.update()

                page.add(ft.ElevatedButton("Submit", on_click=savenotewithtitle))
                page.update()

            savenotebutton = ft.IconButton(icon=ft.icons.SAVE, on_click=savenote,
                                           icon_color=ft.colors.ON_SURFACE_VARIANT)

            if myContainer.lecturetranscription != "":
                page.add(ft.Text("Lecture Transcript", theme_style=ft.TextThemeStyle.HEADLINE_MEDIUM,
                                 weight=ft.FontWeight.BOLD))
                page.add(ft.Divider(thickness=2))
                page.add(ft.Text(myContainer.lecturetranscription))
                page.add(ft.Text(" "))
                page.update()

            page.add(ft.Row(spacing=5, controls=[ft.Text("Notes", theme_style=ft.TextThemeStyle.HEADLINE_MEDIUM,
                                                         weight=ft.FontWeight.BOLD), savenotebutton]))
            page.add(ft.Divider(thickness=2))

            page.add(markdownrender)
            myContainer.pressed = False
            page.update()






















































        elif myContainer.format == 'Generate Audio Notes' and not myContainer.pressed:
            myContainer.notestext = None
            myContainer.pressed = True

            aiprogressbar = ft.Row([ft.ProgressRing(), ft.Text("Initializing AI Model...")])

            # The 'with' statement is used here to manage the spinner context
            page.add(aiprogressbar)
            page.update()

            genai.configure(api_key=decrypt(page.client_storage.get("secret"), "1234"))

            model = genai.GenerativeModel('gemini-1.5-flash')

            page.remove(aiprogressbar)
            page.update()

            def to_markdown(text):
                text = text.replace('•', '  *')
                return Markdown(textwrap.indent(text, '> ', predicate=lambda _: True))

            input_text = myContainer.audiotranscription if recordingmode else myContainer.user_input

            aiprogressbar = ft.Row([ft.ProgressRing(), ft.Text("Processing your content with AI...")])

            page.add(aiprogressbar)
            page.update()

            try:
                if myContainer.lecturetranscription != "":
                    myContainer.lecturetranscription = model.generate_content(
                        "Post process this lecture transcript by adding capitalization and punctuation as needed and return only the transcript in your response. If there are any apparent mis-transcriptions, fix them. Here it is: " + myContainer.lecturetranscription).text
                    input_text = myContainer.lecturetranscription

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

            except ValueError:
                page.add(ft.Text(
                    "The response was blocked. Check your input for any inappropriate content and try again."))
                page.remove(aiprogressbar)
                page.update()
            except Exception as e:
                page.add(ft.Text("An unknown error occurred. Please check your internet connection and try again."))
                page.remove(aiprogressbar)
                page.update()

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

            audioprogressbar = ft.Row([ft.ProgressRing(), ft.Text("Generating Audio File...")])

            page.remove(aiprogressbar)
            page.add(audioprogressbar)
            page.update()

            # Split the text into segments
            segments = split_text_into_segments(text_to_convert)

            # Synthesize speech for each segment and collect audio streams
            audio_streams = []
            for segment in segments:
                try:
                    audio_stream = synthesize_speech(segment)
                    audio_streams.append(audio_stream)
                except Exception as e:
                    text = ft.Text(
                        "An unknown error occurred. Please check your input and internet connection and try again.")
                    page.remove(audioprogressbar)
                    page.update()
            # Join the audio streams into one
            final_output_bytes = join_audio_streams(audio_streams)

            transcriptprogressbar = ft.Row(
                [ft.ProgressRing(), ft.Text("Generating user-friendly transcript...")])

            page.update()

            page.remove(audioprogressbar)
            page.add(transcriptprogressbar)
            try:
                texttorender = model.generate_content('''Add in markdown headers and subheaders. Keep the text unchanged.
                                                                                                    Keeping the text unchanged, use markdown format to apply bullet points, bold text, tables,  and numbered lists if applicable.
                                                                                                    If you have a lot of text in one spot, split up the text into smaller bullet points and prevent long blocks of text                                                                                                            Sections should be headed by level 3 headers (3 hashtags - ### for markdown) at all times and should contain body text. 
                                                                                                    Keep everything else unchanged.  
                                                                                                    If you have math/equations, use normal english characters and do not use LaTeX notation. Do not use HTML or CSS tags such as "<sup></sup>" for math or other purposes.                                                                                                                                                                       Sections should be headed by level 3 headers (3 hashtags - ### for markdown) at all times and should contain body text. 
                                                                             Keep the text unchanged.
                                                                                                    Here's the content: ''' + myContainer.audiogeneratefromlecture).text
                if myContainer.lecturetranscription != "":
                    page.add(ft.Text("Lecture Transcript", theme_style=ft.TextThemeStyle.HEADLINE_MEDIUM,
                                     weight=ft.FontWeight.BOLD))
                    page.add(ft.Divider(thickness=2))
                    page.add(ft.Text(myContainer.lecturetranscription))
                    page.add(ft.Text(" "))
                    page.update()

                myContainer.audionotetosave = texttorender.replace("\n###", "\n#\n#\n###")
                text = ft.Markdown(str(texttorender).replace("\n###", "\n#\n#\n###"),
                                   extension_set=ft.MarkdownExtensionSet.GITHUB_WEB)
            except ValueError:
                text = ft.Text(
                    "We were unable to generate a transcript for your audio file. Check your input for any inappropriate content and try again.")
                page.remove(transcriptprogressbar)
                page.update()
            except Exception as e:
                text = ft.Text(
                    "An unknown error occurred during transcript generation. Please check your internet connection and try again.")
                page.remove(transcriptprogressbar)
                page.update()

            audiofile = convert_to_wav_mono_pcm(final_output_bytes)
            audio = ft.Audio(src=audiofile, autoplay=False)

            def playaudio(e):
                audio.resume()
                play_button.visible = False
                pause_button.visible = True
                page.update()

                while True:
                    slider.value = (float(audio.get_current_position()) / float(audio.get_duration()))
                    page.update()

            def pauseaudio(e):
                audio.pause()
                pause_button.visible = False
                play_button.visible = True

            # Play button
            play_button = ft.IconButton(on_click=playaudio, icon=ft.icons.PLAY_ARROW)
            pause_button = ft.IconButton(on_click=pauseaudio, icon=ft.icons.PAUSE)
            pause_button.visible = False

            slider = ft.ProgressBar(width=int(1300.0 * (page.width / 1920.0)), color=page.theme.color_scheme.primary,
                                    bgcolor="#eeeeee")
            slider.value = 0

            row = ft.Row(spacing=14, controls=[play_button, pause_button, audio, slider])

            def savenote(e):
                audio.pause()

                clearandaddtitle(page=page)

                notetitle = ft.TextField(label="Click to Enter Title", value="")
                page.add(notetitle)
                page.update()

                def savenotewithtitle(e):
                    page.remove(e.control)

                    if myContainer.lecturetranscription != "":
                        transcripts = page.client_storage.get("savedlecturetranscriptsaudio")
                        transcripts.append(myContainer.lecturetranscription)
                        page.client_storage.set("savedlecturetranscriptsaudio", transcripts)
                    else:
                        transcripts = page.client_storage.get("savedlecturetranscriptsaudio")
                        transcripts.append("")
                        page.client_storage.set("savedlecturetranscriptsaudio", transcripts)

                    notes = page.client_storage.get("savedaudionotes")
                    notes.append(myContainer.audionotetosave)
                    page.client_storage.set("savedaudionotes", notes)

                    titles = page.client_storage.get("savedaudionotetitles")
                    titles.append(notetitle.value + "NoteDate" + str(datetime.datetime.now().strftime("%m/%d/%Y")))
                    page.client_storage.set("savedaudionotetitles", titles)

                    audios = page.client_storage.get("savedaudios")
                    print(audiofile)

                    audios.append(str(base64.b64encode(open(audiofile, "rb").read()).decode("ascii")))
                    page.client_storage.set("savedaudios", audios)

                    clearandaddtitle(page=page)
                    page.add(ft.Text("Note Saved!"))

                page.add(ft.ElevatedButton("Submit", on_click=savenotewithtitle))
                page.update()

            savenotebutton = ft.IconButton(icon=ft.icons.SAVE_SHARP, on_click=savenote,
                                           icon_color=ft.colors.ON_SURFACE_VARIANT)

            page.remove(transcriptprogressbar)
            # Adding controls to the page
            page.add(row)

            page.update()

            page.add(ft.Row(spacing=5, controls=[ft.Text("Notes", theme_style=ft.TextThemeStyle.HEADLINE_MEDIUM,
                                                         weight=ft.FontWeight.BOLD), savenotebutton]))
            page.add(ft.Divider(thickness=2))

            page.add(text)
            page.update()

            myContainer.pressed = False







        # Use the input from the text area to run code
        elif myContainer.format == 'Generate Video Notes (Experimental)' and not myContainer.pressed:
            myContainer.notestext = None
            myContainer.pressed = True

            aiprogressbar = ft.Row([ft.ProgressRing(), ft.Text("Initializing AI Model...")])

            # The 'with' statement is used here to manage the spinner context
            page.add(aiprogressbar)
            page.update()

            genai.configure(api_key=decrypt(page.client_storage.get("secret"), "1234"))

            model = genai.GenerativeModel('gemini-1.5-flash')

            page.remove(aiprogressbar)
            page.update()

            def to_markdown(text):
                text = text.replace('•', '  *')
                return Markdown(textwrap.indent(text, '> ', predicate=lambda _: True))

            input_text = myContainer.audiotranscription if recordingmode else myContainer.user_input

            aiprogressbar = ft.Row([ft.ProgressRing(), ft.Text("Processing your content with AI...")])

            page.add(aiprogressbar)
            page.update()

            try:
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
            except ValueError:
                page.add(ft.Text(
                    "The response was blocked. Check your input for any inappropriate content and try again."))
                page.remove(aiprogressbar)
                page.update()
            except Exception as e:
                page.add(ft.Text("An unknown error occurred. Please check your internet connection and try again."))
                page.remove(aiprogressbar)
                page.update()

            page.remove(aiprogressbar)
            page.update()

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

            videoprogressbar = ft.Row([ft.ProgressRing(), ft.Text("Generating Video Link...")])

            page.add(videoprogressbar)
            page.update()

            response = requests.post(url, json=data, headers=headers)

            page.remove(videoprogressbar)
            page.update()

            if response.status_code == 200:
                result = response.json()

                progress_text = ft.Text("Loading Final Video...", style="headlineSmall")
                progress_bar = ft.ProgressBar(width=400, color=ft.colors.BLUE, bgcolor="#eeeeee")
                page.add(progress_text, progress_bar)
                page.update()

                percent_complete = 0

                for i in range(1000):
                    percent_complete += 0.001
                    percentstring = str(percent_complete * 100)
                    progress_text.value = str(
                        "Loading Final Video (" + percentstring[0: percentstring.find('.') + 2] + "%)...")
                    t.sleep(.18 * (len(after_curly_braces(response.text)) / 2500.0))  # Simulate work being done
                    page.update()

                page.remove(progress_text)
                page.remove(progress_bar)

                page.add(ft.Text("Video Link: ", spans=[ft.TextSpan(
                    "Click Here",
                    ft.TextStyle(decoration=ft.TextDecoration.UNDERLINE),
                    url=result['project']['link'])]))
                page.update()

                myContainer.pressed = False

                # print("Project ID:", result['project']['id'])
                # print("Project Title:", result['project']['title'])
                # print("Project Link:", result['project']['link'])
                # print("Project Thumbnail:", result['project']['thumbnail'])
            else:
                page.add(ft.Text("There was an error. Please try again."))
                page.update()

                myContainer.pressed = False

    def generation(e):
        def backtomain(e):
            main(page)
        page.appbar.leading.on_click = backtomain
        myContainer.notetosave = ""
        myContainer.audionotetosave = ""

        clearandaddtitle(page=page)
        if recordingmode:
            def record_and_transcribe_audio():
                recognizer = speech_recognition.Recognizer()
                recognizer.pause_threshold = 0.05
                recognizer.phrase_threshold = 0.1
                recognizer.energy_threshold = 150
                recognizer.non_speaking_duration = 0.001

                audioprogressbar = ft.Row([ft.ProgressRing(), ft.Text("Listening and Transcribing")])
                page.add(audioprogressbar)
                page.update()

                def specialgeneratefortranscribe(e):
                    page.remove(e.control)
                    myContainer.audiotranscription = myContainer.lecturetranscription
                    page.remove(audioprogressbar)
                    finishing = ft.Row([ft.ProgressRing(), ft.Text("Finishing Up...")])
                    page.add(finishing)
                    page.update()
                    t.sleep(30)
                    page.update()
                    myContainer.hasStoppedRecording = True
                    page.update()
                    try:
                        generatecontent(recordingmode=True)
                    except Exception as e:
                        traceback.print_exc()
                        page.add(ft.Text(
                            "An Unknown Error occurred. Please check your internet connection and input and try again."))
                    page.update()

                page.add(ft.ElevatedButton(height=80, content=
                ft.Row(
                    # width=230,
                    controls=[
                        ft.Icon(ft.icons.START_ROUNDED, size=45),
                        ft.Column(spacing=-3, controls=[
                            ft.Text("", size=8),
                            ft.Text("Stop Recording", size=22,
                                    color=ft.colors.INVERSE_SURFACE,
                                    font_family="Gaegu"),
                            ft.Text("Click to finish recording", size=15,
                                    color="grey",
                                    font_family="Gaegu")
                        ])]
                ), on_click=specialgeneratefortranscribe))
                page.update()

                exp = ft.ExpansionPanel(
                    header=ft.ListTile(title=ft.Text("Raw Transcription")),
                )

                exp.content = ft.ListTile(
                    title=ft.Text(
                        "This is a rough transcript. It'll be post-processed during generation."),
                    subtitle=ft.Text("")
                )

                panel = ft.ExpansionPanelList(
                    controls=[exp
                              ],
                    elevation=8
                )

                page.add(panel)
                page.update()

                while myContainer.hasStoppedRecording is False:
                    # transcription = ft.Text("")
                    # page.add(transcription)
                    # page.update()

                    try:
                        with speech_recognition.Microphone() as mic:

                            audio = recognizer.listen(mic)

                            text = recognizer.recognize_google(audio, language="en-US")
                            # text = text.lower()
                            # text = text[14:len(text) - 3]
                            # transcription.value = text
                            page.update()

                            myContainer.audiotranscription = str(
                                text) if myContainer.audiotranscription is None else myContainer.audiotranscription.replace(
                                "\n", "") + " " + str(text)
                            myContainer.lecturetranscription = str(
                                text) if myContainer.lecturetranscription == "" else myContainer.lecturetranscription.replace(
                                "\n", "") + " " + str(text)

                            exp.content = ft.ListTile(
                                title=ft.Text(
                                    "This is a rough transcript. This will be post-processed during generation."),
                                subtitle=ft.Text(myContainer.lecturetranscription)
                            )

                            page.update()

                    except speech_recognition.UnknownValueError:
                        recognizer = speech_recognition.Recognizer()
                        pass

            if myContainer.uploadedaudio is None:
                def startRecordingButton(e):
                    page.remove(e.control)
                    page.update()

                    myContainer.hasStartedRecording = True
                    record_and_transcribe_audio()

                page.add(ft.ElevatedButton(height=80, content=
                ft.Row(
                    # width=230,
                    controls=[
                        ft.Icon(ft.icons.START_ROUNDED, size=45),
                        ft.Column(spacing=-3, controls=[
                            ft.Text("", size=8),
                            ft.Text("Start Recording", size=22,
                                    color=ft.colors.INVERSE_SURFACE,
                                    font_family="Gaegu"),
                            ft.Text("Click to begin recording lecture", size=15,
                                    color="grey",
                                    font_family="Gaegu")
                        ])]
                ), on_click=startRecordingButton))
                page.update()

            else:
                transcriptionprogressbar = ft.Row([ft.ProgressRing(), ft.Text("Transcribing Upload")])
                page.add(transcriptionprogressbar)
                page.update()

                silence_based_conversion(convert_to_wav_mono_pcm(myContainer.uploadedaudio))
                text = myContainer.silencebasedtranscription
                page.add(ft.Text((str(text))))
                page.update()

                myContainer.audiotranscription = str(text)

                page.remove(transcriptionprogressbar)
                page.update()

                myContainer.hasStoppedRecording = True
                page.update()
                try:
                    generatecontent(recordingmode=True)
                except Exception as e:
                    traceback.print_exc()
                    page.add(ft.Text(
                        "An Unknown Error occurred. Please check your internet connection and input and try again."))

                page.update()



        else:
            if myContainer.uploadedtext is not None:
                myContainer.user_input = read_pdf(myContainer.uploadedtext)

            myContainer.hasStoppedRecording = True
            page.update()
            try:
                generatecontent(recordingmode=False)
            except Exception as e:
                traceback.print_exc()
                page.add(ft.Text(
                    "An Unknown Error occurred. Please check your internet connection and input and try again."))
            page.update()

    def recgeneration():
        myContainer.notetosave = ""
        myContainer.audionotetosave = ""

        clearandaddtitle(page=page)
        if recordingmode:
            def record_and_transcribe_audio():
                recognizer = speech_recognition.Recognizer()
                recognizer.pause_threshold = 0.05
                recognizer.phrase_threshold = 0.1
                recognizer.energy_threshold = 150
                recognizer.non_speaking_duration = 0.001

                audioprogressbar = ft.Row([ft.ProgressRing(), ft.Text("Listening and Transcribing")])
                page.add(audioprogressbar)
                page.update()

                def specialgeneratefortranscribe(e):
                    page.remove(e.control)
                    page.remove(audioprogressbar)
                    finishing = ft.Row([ft.ProgressRing(), ft.Text("Finishing Up...")])
                    page.add(finishing)
                    page.update()
                    t.sleep(30)
                    page.update()
                    myContainer.hasStoppedRecording = True
                    page.update()
                    try:
                        generatecontent(recordingmode=True)
                    except Exception as e:
                        traceback.print_exc()
                        page.add(ft.Text(
                            "An Unknown Error occurred. Please check your internet connection and input and try again."))
                    page.update()

                page.add(ft.ElevatedButton(height=80, content=
                ft.Row(
                    # width=230,
                    controls=[
                        ft.Icon(ft.icons.START_ROUNDED, size=45),
                        ft.Column(spacing=-3, controls=[
                            ft.Text("", size=8),
                            ft.Text("Stop Recording", size=22,
                                    color=ft.colors.INVERSE_SURFACE,
                                    font_family="Gaegu"),
                            ft.Text("Click to finish recording", size=15,
                                    color="grey",
                                    font_family="Gaegu")
                        ])]
                ), on_click=specialgeneratefortranscribe))
                page.update()

                exp = ft.ExpansionPanel(
                    header=ft.ListTile(title=ft.Text("Raw Transcription")),
                )

                tile = ft.ListTile(
                    title=ft.Text(
                        "This is a rough transcript. It'll be post-processed during generation."),
                    subtitle=ft.Text("")
                )
                exp.content = tile

                panel = ft.ExpansionPanelList(
                    controls=[exp
                              ],
                    elevation=8
                )

                page.add(panel)
                page.update()

                with speech_recognition.Microphone() as mic:

                    while myContainer.hasStoppedRecording is False:
                        # transcription = ft.Text("")
                        # page.add(transcription)
                        # page.update()
                        try:
                                audio = recognizer.listen(mic)

                                text = recognizer.recognize_google(audio, language="en-US")
                                # text = text.lower()
                                # text = text[14:len(text) - 3]
                                # transcription.value = text

                                myContainer.lecturetranscription += " " + str(text)
                                exp.content = ft.ListTile(
                                    title=ft.Text(
                                        "This is a rough transcript. It'll be post-processed during generation."),
                                    subtitle=ft.Text(myContainer.lecturetranscription)
                                )
                                page.update()

                        except speech_recognition.UnknownValueError:
                            recognizer = speech_recognition.Recognizer()
                            pass

            if myContainer.uploadedaudio is None:
                def startRecordingButton(e):
                    page.remove(e.control)
                    page.update()

                    myContainer.hasStartedRecording = True
                    record_and_transcribe_audio()

                page.add(ft.ElevatedButton(height=80, content=
                ft.Row(
                    # width=230,
                    controls=[
                        ft.Icon(ft.icons.START_ROUNDED, size=45),
                        ft.Column(spacing=-3, controls=[
                            ft.Text("", size=8),
                            ft.Text("Start Recording", size=22,
                                    color=ft.colors.INVERSE_SURFACE,
                                    font_family="Gaegu"),
                            ft.Text("Click to begin recording lecture", size=15,
                                    color="grey",
                                    font_family="Gaegu")
                        ])]
                ), on_click=startRecordingButton))
                page.update()

            else:
                def generateFromUploadButton(e):
                    page.remove(e.control)
                    transcriptionprogressbar = ft.Row([ft.ProgressRing(), ft.Text("Transcribing Upload")])
                    page.add(transcriptionprogressbar)
                    page.update()

                    silence_based_conversion(convert_to_wav_mono_pcm(myContainer.uploadedaudio))
                    text = myContainer.silencebasedtranscription
                    page.add(ft.Text((str(text))))
                    page.update()

                    myContainer.audiotranscription = str(text)

                    page.remove(transcriptionprogressbar)
                    page.update()

                    myContainer.hasStoppedRecording = True
                    page.update()
                    try:
                        generatecontent(recordingmode=True)
                    except Exception as e:
                        traceback.print_exc()
                        page.add(ft.Text(
                            "An Unknown Error occurred. Please check your internet connection and input and try again."))

                    page.update()

                page.add(ft.ElevatedButton("Generate from Upload", on_click=generateFromUploadButton))
                page.update()



        else:
            if myContainer.uploadedtext is not None:
                myContainer.user_input = read_pdf(myContainer.uploadedtext)

            def generateButton(e):
                page.remove(e.control)
                myContainer.hasStoppedRecording = True
                page.update()
                try:
                    generatecontent(recordingmode=False)
                except Exception as e:
                    traceback.print_exc()
                    page.add(ft.Text(
                        "An Unknown Error occurred. Please check your internet connection and input and try again."))
                page.update()

            page.add(ft.ElevatedButton(icon=ft.icons.NAVIGATE_NEXT_OUTLINED, text="Generate", on_click=generateButton))

    def entertextbox():
        clearandaddtitle(page=page)
        page.add(ft.TextField(value="", label="Type your content here...", multiline=True, on_change=change_user_input))
        page.add(ft.ElevatedButton(icon=ft.icons.NAVIGATE_NEXT_OUTLINED, text="Generate", on_click=generation))
        page.update()

    if inputstr == "Upload Audio" or inputstr == "Upload PDF":
        uploadbeforegenerate()
    elif inputstr == "Record Lecture":
        recgeneration()
    elif inputstr == "Enter Text":
        entertextbox()


def displayoptions(myContainer, inputstr, page: ft.Page):
    clearandaddtitle(page=page)

    def changecheckbox(e):
        myContainer.summarize = e.control.value

    summarize = ft.Checkbox("Summarize (Makes Concise Notes)", on_change=changecheckbox)
    page.add(summarize)
    page.update()

    def generatenotes(e):
        myContainer.format = "Generate Notes"
        myContainer.summarize = summarize.value
        generateafteroption(myContainer=myContainer, inputstr=inputstr, page=page)

    def generateaudionotes(e):
        myContainer.format = "Generate Audio Notes"
        myContainer.summarize = summarize.value
        generateafteroption(myContainer=myContainer, inputstr=inputstr, page=page)

    def generatevideonotes(e):
        myContainer.format = "Generate Video Notes (Experimental)"
        myContainer.summarize = summarize.value
        generateafteroption(myContainer=myContainer, inputstr=inputstr, page=page)

    page.add(ft.ElevatedButton(height=80, content=
    ft.Row(
        #width=230,
        controls=[
            ft.Icon(ft.icons.EDIT_NOTE_SHARP, size=45),
            ft.Column(spacing=-3, controls=[
                ft.Text("", size=8),
                ft.Text("Text Notes", size=22,
                        color=ft.colors.INVERSE_SURFACE,
                        font_family="Gaegu"),
                ft.Text("Classic, Stylish Notes", size=15,
                        color="grey",
                        font_family="Gaegu")
            ])]
    ), on_click=generatenotes))
    page.add(ft.ElevatedButton(height=80, content=
    ft.Row(
        #width=230,
        controls=[
            ft.Icon(ft.icons.AUDIO_FILE, size=45),
            ft.Column(spacing=-3, controls=[
                ft.Text("", size=8),
                ft.Text("Audio Notes", size=22,
                        color=ft.colors.INVERSE_SURFACE,
                        font_family="Gaegu"),
                ft.Text("Audio File + Transcript", size=15,
                        color="grey",
                        font_family="Gaegu")
            ])]
    ), on_click=generateaudionotes))
    page.add(ft.ElevatedButton(height=80, content=
    ft.Row(
        #width=230,
        controls=[
            ft.Icon(ft.icons.MOVIE, size=45),
            ft.Column(spacing=-3, controls=[
                ft.Text("", size=8),
                ft.Text("Video Notes", size=22,
                        color=ft.colors.INVERSE_SURFACE,
                        font_family="Gaegu"),
                ft.Text("(Experimental Feature)", size=15,
                        color="grey",
                        font_family="Gaegu")
            ])]
    ), on_click=generatevideonotes))
    page.update()

safety_settings={
        HarmCategory.HARM_CATEGORY_HATE_SPEECH: HarmBlockThreshold.BLOCK_ONLY_HIGH,
        HarmCategory.HARM_CATEGORY_HARASSMENT: HarmBlockThreshold.BLOCK_ONLY_HIGH,
        HarmCategory.HARM_CATEGORY_UNSPECIFIED: HarmBlockThreshold.BLOCK_ONLY_HIGH,
        HarmCategory.HARM_CATEGORY_DANGEROUS_CONTENT: HarmBlockThreshold.BLOCK_ONLY_HIGH,
        HarmCategory.HARM_CATEGORY_SEXUALLY_EXPLICIT: HarmBlockThreshold.BLOCK_ONLY_HIGH
}


def internet_connection():
    try:
        response = requests.get("https://google.com", timeout=5)
        return True
    except requests.ConnectionError:
        return False



def main(page: ft.Page):
    page.window_width=600
    page.window_resizable = False
    page.window_maximizable = False


    for view in page.views[1:]:
        page.views.remove(view)
    page.controls.clear()

    loadinghome = ft.ProgressRing()
    page.add(loadinghome)


    page.update()

    if not internet_connection():

        cupertino_actions = [
        ]

        dialog = ft.CupertinoAlertDialog(
                    title=ft.Row(controls = [
                                ft.Icon(ft.icons.SIGNAL_WIFI_STATUSBAR_CONNECTED_NO_INTERNET_4_ROUNDED, color=ft.colors.ON_SURFACE_VARIANT),
                        ft.Text("Not Connected")
                                ]),
                    content=ft.Text("Please connect to the internet", font_family="Gaegu"),
                    actions=cupertino_actions,
                    modal = True
                )

        page.overlay.append(dialog)
        dialog.open = True
        page.update()

        while not internet_connection():
            page.update()

        dialog.open = False
        page.overlay.remove(dialog)


    def backtomain(e):
        for control in page.controls:
            if control._get_children() is not None:
                for child in control._get_children():
                    if str(type(child)) == "<class 'flet_core.audio.Audio'>":
                        child.pause()
        main(page)

    page.theme_mode = ft.ThemeMode.SYSTEM
    page.title = "Notecandy"


    #page.client_storage.clear()
    #page.client_storage.set("secret", encrypt("", "1234"))

    page.padding = 20
    page.fonts = {
        "Gaegu": "https://raw.githubusercontent.com/google/fonts/main/ofl/gaegu/Gaegu-Regular.ttf"
    }

    page.theme = ft.Theme(
        scrollbar_theme= ft.ScrollbarTheme(
            main_axis_margin=0
        ),
        color_scheme = ft.ColorScheme(
        primary="#3F88C5"
    ), font_family="Gaegu",
    text_theme = ft.TextTheme(
        headline_large=ft.TextStyle(color=ft.colors.BLUE, foreground=ft.Paint(
                            gradient=ft.PaintLinearGradient(
                                (0, 20),
                                (190, 20),
                                ["#E56399", "#6CBEED"],
                                tile_mode=ft.GradientTileMode.MIRROR
                            )
                        )),
        headline_medium=ft.TextStyle(color=ft.colors.BLUE, foreground=ft.Paint(
                            gradient=ft.PaintLinearGradient(
                                (0, 20),
                                (190, 20),
                                ["#E56399", "#6CBEED"],
                                tile_mode=ft.GradientTileMode.MIRROR
                            )
                        )),
        headline_small=ft.TextStyle(color=ft.colors.BLUE, weight=ft.FontWeight.BOLD, foreground=ft.Paint(
                            gradient=ft.PaintLinearGradient(
                                (0, 20),
                                (190, 20),
                                ["#E56399", "#6CBEED"],
                                tile_mode=ft.GradientTileMode.MIRROR
                            )
                        )),        # h1
        title_large=ft.TextStyle(color=ft.colors.BLUE, weight=ft.FontWeight.BOLD, foreground=ft.Paint(
                            gradient=ft.PaintLinearGradient(
                                (0, 20),
                                (190, 20),
                                ["#E56399", "#6CBEED"],
                                tile_mode=ft.GradientTileMode.MIRROR
                            )
                        )),  # h2
        title_medium=ft.TextStyle(color=ft.colors.BLUE, weight=ft.FontWeight.BOLD, foreground=ft.Paint(
                            gradient=ft.PaintLinearGradient(
                                (0, 20),
                                (190, 20),
                                ["#E56399", "#6CBEED"],
                                tile_mode=ft.GradientTileMode.MIRROR
                            )
                        )),  # h3
        title_small=ft.TextStyle(color=ft.colors.BLUE, weight=ft.FontWeight.BOLD, foreground=ft.Paint(
            gradient=ft.PaintLinearGradient(
                (0, 20),
                (190, 20),
                ["#E56399", "#6CBEED"],
                tile_mode=ft.GradientTileMode.MIRROR
            )
        )),  # not h4
        body_large=ft.TextStyle(size=20,color=ft.colors.BLUE, weight=ft.FontWeight.BOLD, foreground=ft.Paint(
            gradient=ft.PaintLinearGradient(
                (0, 20),
                (190, 20),
                ["#E56399", "#6CBEED"],
                tile_mode=ft.GradientTileMode.MIRROR
            )
        )),        # h4?

        body_medium=ft.TextStyle(size=17,font_family="Gaegu"
        ),
    )
    )

    page.appbar = ft.AppBar(
        leading=ft.IconButton(icon=ft.icons.HOME,
                                  on_click=backtomain, icon_color=ft.colors.ON_SURFACE_VARIANT),
        leading_width=40,
        bgcolor=ft.colors.SURFACE_VARIANT,
        actions=[

        ]
    )

    if page.client_storage.get("savednotes") is None:
        page.client_storage.set("savednotes", [])
    if page.client_storage.get("savednotetitles") is None:
        page.client_storage.set("savednotetitles", [])
    if page.client_storage.get("savedaudionotes") is None:
        page.client_storage.set("savedaudionotes", [])
    if page.client_storage.get("savedaudios") is None:
        page.client_storage.set("savedaudios", [])
    if page.client_storage.get("savedaudionotetitles") is None:
        page.client_storage.set("savedaudionotetitles", [])
    if page.client_storage.get("savedlecturetranscripts") is None:
        page.client_storage.set("savedlecturetranscripts", [])
    if page.client_storage.get("savedlecturetranscriptsaudio") is None:
        page.client_storage.set("savedlecturetranscriptsaudio", [])

    page.scroll = ft.ScrollMode.ALWAYS
    page.on_scroll_interval = 100





    class datacontainerobj:
        def __init__(self):
            self.audiotranscription = None
            self.lecturetranscription = ""
            self.pressed = False
            self.notestext = None
            self.audiogeneratefromlecture = None
            self.hasStoppedRecording = False
            self.hasStartedRecording = False
            self.user_input = None
            self.silencebasedtranscription = None
            self.uploadedtext = None
            self.uploadedaudio = None
            self.recordingmode = False
            self.summarize = False
            self.format = "Generate Notes"
            self.notetosave = ""
            self.audionotetosave = ""
            self.audiotosave = None

    myContainer = datacontainerobj()


    def recordlecture(e):
        myContainer.recordingmode = True
        displayoptions(myContainer=myContainer, inputstr="Record Lecture", page=page)

    def uploadaudio(e):
        myContainer.recordingmode = True
        displayoptions(myContainer=myContainer, inputstr="Upload Audio", page=page)

    def entertext(e):
        myContainer.recordingmode = False
        displayoptions(myContainer=myContainer, inputstr="Enter Text", page=page)

    def uploadpdf(e):
        myContainer.recordingmode = False
        displayoptions(myContainer=myContainer, inputstr="Upload PDF", page=page)

    class savednotecontainer:
        def __init__(self):
            self.number = 0

    mySavedNote = savednotecontainer()

    def renderaudionote(e):
        clearandaddtitle(page=page)
        progressloading =ft.ProgressRing(height=50, width=50)
        page.add(progressloading)

        mySavedNote.number = e.control.key

        unconvertedaudio = page.client_storage.get("savedaudios")[mySavedNote.number]
        decoded = base64.b64decode(unconvertedaudio.encode("ascii"))

        audio = ft.Audio(src=convert_to_wav_mono_pcm(io.BytesIO(decoded)), autoplay=False)

        def playaudio(e):
            audio.resume()
            play_button.visible = False
            pause_button.visible = True
            page.update()

            while True:
                slider.value = (float(audio.get_current_position()) / float(audio.get_duration()))
                page.update()

        def pauseaudio(e):
            audio.pause()
            pause_button.visible = False
            play_button.visible = True

        # Play button
        play_button = ft.IconButton(on_click=playaudio, icon=ft.icons.PLAY_ARROW)
        pause_button = ft.IconButton(on_click=pauseaudio, icon=ft.icons.PAUSE)
        pause_button.visible = False

        slider = ft.ProgressBar(width=int(1300.0* (page.width/1920.0)), color=page.theme.color_scheme.primary, bgcolor="#eeeeee")
        slider.value = 0

        row = ft.Row(spacing=14, controls=[play_button, pause_button, audio, slider])

        transcript = page.client_storage.get("savedlecturetranscriptsaudio")[mySavedNote.number]

        if transcript != "":
            page.add(
                ft.Text("Lecture Transcript", theme_style=ft.TextThemeStyle.HEADLINE_MEDIUM, weight=ft.FontWeight.BOLD))
            page.add(ft.Divider(thickness=2))
            page.add(ft.Text(transcript))
            page.add(ft.Text(" "))
            page.update()
        # Adding controls to the page
        page.remove(progressloading)

        page.add(row)

        page.update()

        page.add(
            ft.Text("Notes", theme_style=ft.TextThemeStyle.HEADLINE_MEDIUM, weight=ft.FontWeight.BOLD))
        page.add(ft.Divider(thickness=2))
        page.add(ft.Markdown(page.client_storage.get("savedaudionotes")[mySavedNote.number]))
        page.update()

    def rendernormalnote(e):
        clearandaddtitle(page=page)

        mySavedNote.number = e.control.key

        text = page.client_storage.get("savednotes")[mySavedNote.number]
        transcript = page.client_storage.get("savedlecturetranscripts")[mySavedNote.number]

        if transcript != "":
            page.add(
                ft.Text("Lecture Transcript", theme_style=ft.TextThemeStyle.HEADLINE_SMALL, weight=ft.FontWeight.BOLD))
            page.add(ft.Divider(thickness=2))
            page.add(ft.Text(transcript))
            page.add(ft.Text(" "))
            page.update()

        page.add(ft.Text("Notes", theme_style=ft.TextThemeStyle.HEADLINE_MEDIUM, weight=ft.FontWeight.BOLD))
        page.add(ft.Divider(thickness=2))
        page.add(ft.Markdown(text, extension_set=ft.MarkdownExtensionSet.GITHUB_WEB))
        page.update()

    def deleteaudionote(e):
        mySavedNote.number = e.control.key

        def followdelete(e):
            title = page.client_storage.get("savedaudionotetitles")
            text = page.client_storage.get("savedaudionotes")
            transcript = page.client_storage.get("savedlecturetranscriptsaudio")
            audioarray = page.client_storage.get("savedaudios")

            title = [element for i, element in enumerate(title) if i != mySavedNote.number]
            text = [element for i, element in enumerate(text) if i != mySavedNote.number]
            transcript = [element for i, element in enumerate(transcript) if i != mySavedNote.number]
            audioarray = [element for i, element in enumerate(audioarray) if i != mySavedNote.number]

            page.client_storage.set("savedaudionotetitles", title)
            page.client_storage.set("savedaudionotes", text)
            page.client_storage.set("savedlecturetranscriptsaudio", transcript)
            page.client_storage.set("savedaudios", audioarray)

            e.control.parent.open = False
            page.overlay.remove(e.control.parent)
            main(page)

        def canceldelete(e):
            e.control.parent.open = False
            page.overlay.remove(e.control.parent)

        cupertino_actions = [
            ft.CupertinoDialogAction(
                "OK",
                is_destructive_action=True,
                on_click=followdelete,
            ),
            ft.CupertinoDialogAction(
                text="Cancel",
                is_default_action=False,
                on_click=canceldelete,
            )
        ]

        page.open(
            ft.CupertinoAlertDialog(
                title=ft.Text("Delete Audio Note?"),
                content=ft.Text("This action is permanent.", font_family="Gaegu"),
                actions=cupertino_actions,
            ))

    def deletenormalnote(e):
        mySavedNote.number = e.control.key

        def followdelete(e):
            title = list(page.client_storage.get("savednotetitles"))
            text = page.client_storage.get("savednotes")
            transcript = page.client_storage.get("savedlecturetranscripts")

            title = [element for i, element in enumerate(title) if i != mySavedNote.number]
            text = [element for i, element in enumerate(text) if i != mySavedNote.number]
            transcript = [element for i, element in enumerate(transcript) if i != mySavedNote.number]

            page.client_storage.set("savednotetitles", title)
            page.client_storage.set("savednotes", text)
            page.client_storage.set("savedlecturetranscripts", transcript)

            e.control.parent.open = False
            page.overlay.remove(e.control.parent)
            main(page)

        def canceldelete(e):
            e.control.parent.open = False
            page.overlay.remove(e.control.parent)

        cupertino_actions = [
            ft.CupertinoDialogAction(
                "Yes",
                is_destructive_action=True,
                on_click=followdelete,
            ),
            ft.CupertinoDialogAction(
                text="No",
                is_default_action=False,
                on_click=canceldelete,
            ),
        ]

        alert = ft.CupertinoAlertDialog(
                title=ft.Text("Delete Note?"),
                content=ft.Text("This action is permanent.", font_family="Gaegu"),
                actions=cupertino_actions,
            )

        page.overlay.append(alert)
        alert.open = True
        page.update()

    page.remove(loadinghome)
    clearandaddtitle(page=page)

    page.add(ft.Text(""))

    page.add(ft.Row(spacing=360, controls=[
        ft.Text("Your Notes",weight=ft.FontWeight.BOLD,
                theme_style=ft.TextThemeStyle.HEADLINE_MEDIUM),

        ft.PopupMenuButton(items=
        [
            ft.PopupMenuItem(text="Record Lecture", on_click=recordlecture, icon = ft.icons.RECORD_VOICE_OVER),
            ft.PopupMenuItem(text="Enter Text", on_click=entertext, icon = ft.icons.NOTES),
            ft.PopupMenuItem(text="Upload PDF", on_click=uploadpdf, icon = ft.icons.FILE_UPLOAD),
            ft.PopupMenuItem(text="Upload Audio", on_click=uploadaudio, icon = ft.icons.SURROUND_SOUND),
        ],
             elevation=0,
            icon=ft.icons.ADD_CIRCLE, icon_color=ft.colors.ON_SURFACE_VARIANT)]))

    page.add(ft.Divider(thickness=2))



    textpanel = ft.ExpansionPanelList(
        elevation=8,
        controls=[

        ]
    )

    page.add(ft.Text("Text Notes",
                     style=ft.TextStyle(size=25, color=ft.colors.ON_SURFACE_VARIANT, weight=ft.FontWeight.BOLD)))

    if len(page.client_storage.get("savednotetitles")) == 0:
        page.add(
            ft.Row(spacing=0, controls=[
            ft.Text("Click \""),
            ft.Icon(size=15, name=ft.icons.ADD_CIRCLE, color=ft.colors.ON_SURFACE_VARIANT),
            ft.Text("\" to create a note.")]))
        page.update()
    else:
        titles = list(page.client_storage.get("savednotetitles"))
        page.update()

        for title in reversed(titles):
            number = titles.index(title)

            mytext = title[0:title.find("NoteDate")]
            exp = ft.ExpansionPanel(header=ft.ListTile(title=ft.Text(mytext)), content =
            ft.ListTile(
                title=ft.Text(title[title.find("NoteDate") + 8:len(title)]),
                subtitle=ft.Text("Click here to open note"),
            trailing=ft.IconButton(ft.icons.OPEN_IN_NEW, key=number, on_click=rendernormalnote),
                leading = ft.IconButton(ft.icons.DELETE, key=number, on_click=deletenormalnote)
        )

            )

            textpanel.controls.append(exp)

    page.add(textpanel)


    audiopanel = ft.ExpansionPanelList(
        elevation=8,
        controls=[

        ]
    )

    page.add(ft.Text(""))
    page.add(ft.Text("Audio Notes",
                     style=ft.TextStyle(size=25, color=ft.colors.ON_SURFACE_VARIANT, weight=ft.FontWeight.BOLD)))

    if len(page.client_storage.get("savedaudionotetitles")) == 0:
        page.add(
                 ft.Row(spacing=0, controls=[
                     ft.Text("Click \""),
                     ft.Icon(size=15, name=ft.icons.ADD_CIRCLE, color=ft.colors.ON_SURFACE_VARIANT),
                     ft.Text("\" to create a note.")]))
    else:
        titles = list(page.client_storage.get("savedaudionotetitles"))

        for title in reversed(titles):
            number = titles.index(title)
            mytext = title[0:title.find("NoteDate")]
            exp = ft.ExpansionPanel(header=ft.ListTile(title=ft.Text(mytext)), content=
            ft.ListTile(
                title=ft.Text(title[title.find("NoteDate") + 8:len(title)]),
                subtitle=ft.Text("Click here to open note"),
                trailing=ft.IconButton(ft.icons.OPEN_IN_NEW, key=number, on_click=renderaudionote),
                                    leading=ft.IconButton(ft.icons.DELETE, key=number, on_click=deleteaudionote)
            )

            )

            audiopanel.controls.append(exp)

    page.add(audiopanel)
    page.update()

ft.app(target=main, assets_dir="assets")






































