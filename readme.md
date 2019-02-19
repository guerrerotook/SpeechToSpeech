# Speech to Speech translation sample

This project is a sample for working with the [Cognitive Services Speech APIs](https://azure.microsoft.com/en-gb/try/cognitive-services/) and shows how you can process speech input, translate text, and convert text to speech.

There are currently two tabs in the application to show different ways of using the APIs. See the relevant section of the README for getting started with each flow.

## Prerequisites

To run this sample you will need the following:

* A copy of the code (either by cloning the GitHub repo or downloading and extracting a ZIP of the code)
* [Visual Studio 2017](https://visualstudio.microsoft.com/vs/)
* [A Cognitive Services Speech API key]

### Getting a Cognitive Services Speech API Key

To get a Speech API key go to the  [Cognitive Services home page](https://azure.microsoft.com/en-gb/try/cognitive-services/) and click on "Speech APIs". This will guide you to getting a Speech API key (including a free trial key). Make a note of your API key and which region you deploy to as you will need this information when configuring the sample app

### Getting a Translator Text API Key

To get a Translator Text API key go to the  [Translator Text page](https://azure.microsoft.com/en-us/services/cognitive-services/translator-text-api/ ) and click on "Start Free". This will guide you to getting a Translator Text API key (including a free trial key). Make a note of your API key and which region you deploy to as you will need this information when configuring the sample app

## Speech / Translation / Speech tab

This tab takes speech input, translates it and outputs as speech.

If you haven't configured the Speech API yet then the "Speech API Config" section will be expanded. Enter the region and Speech API key from the Prerequisites section. These values will be saved for when you next run the app.

"Input source" allows you to choose whether to use input from the default microphone on your system, or a WAV file with speech in.

In the "Languages" section you specify the language details: what the original input language is, what language you want the output speech in, and the voice profile to use

Once you have entered the configuration, click "Start"; if using the microphone then speak into it. The UI will update to show the results of the speech capture and translation, and the resulting translated speech will be played.

## Text / Translation / Speech

This tab takes text input, translates it and outputs as speech.

If you haven't configured the text translation yet then the "Translation API Config" section will be expanded. Enter the Text Translation API key and the Speech API key from the Prerequisites section. These values will be saved for when you next run the app. (The Speech API key is shared between the Speech and Text tabs, so if you've already entered it on the Speech tab it will be populated here already)

In the "Languages" section you specify the language details: what the original input language is, what language you want to translate to, what voice profile to use, and what audio format to use

Once you have entered the configuration, click "Start"; if using the microphone then speak into it. The UI will update to show the results of the speech capture and translation, and the resulting translated speech will be played.
