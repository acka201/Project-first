# Gallows Game

## Description

Gallows is a classic word-guessing game where the objective is to guess the hidden word. The game allows the player to guess letters or try to guess the entire word, while having a limited number of mistakes allowed.

This version supports both Hungarian and English languages and can be played at various difficulty levels: Easy, Medium, and Hard. The program follows the traditional Hangman game rules but is extended with new features such as saving and loading statistics.

### Motivation

The goal of this project is to create an interactive and fun game that enhances vocabulary and problem-solving skills. The game supports multiple languages and difficulty levels, so everyone can find the right challenge.

## Development Environment

- **Development Language**: F#
- **Libraries Used**: 
    - `System.IO` for file handling
    - `System.Text.Json` for handling JSON files (storing statistics)
    - `System.Diagnostics` for measuring game time
    - `System.Threading` for managing game time pauses
- **Development Environment**: Visual Studio or any F# supporting IDE
- **Features**: 
    - Support for two languages (Hungarian and English)
    - Multiple difficulty levels
    - Word list loaded from files
    - Game time measurement
    - Saving and loading statistics from a JSON file
    - Displaying the Hangman figure on incorrect guesses

## Running the Project

1. Clone the repository to your local machine:
    ```bash
    git clone https://github.com/acka201/Project-first.git
    ```

2. Open the project in an F# supporting IDE (e.g., Visual Studio).

3. To start the game, run the `Akasztófa` project.

4. Upon starting, the game will present you with the following menu options:
    - **New Game**: Start a new game, where you can choose the difficulty and language.
    - **Rules**: View the game rules.
    - **Statistics**: View your game statistics.
    - **Exit**: Exit the program.

### Screenshot of the Game

See pictures.  
*(Example of the game screen)*

## Development Plan

In the future, we plan to add the following features:

- **Web Version**: Make the game available in the browser, hosted on GitHub Pages or another free hosting service.
- **Additional Languages**: Expand the language support to make the game available to more users.
- **More Statistical Features**: Provide more detailed game statistics, such as fastest response times.
- **UI Improvements**: Further enhance the user experience and interface design.

## License

This project is licensed under the **MIT License**. See the `LICENSE` file for more details.

How to Use
To run the project, ensure that the necessary files, such as words_en_easy.txt, words_hu_easy.txt, etc., are available when you run the program.

If you encounter any issues, double-check that the files are correctly formatted and located in the appropriate directory.

Screenshots
You can replace the "See pictures" placeholder with actual screenshots once you have them available. If no screenshot is available, feel free to leave it as is and add it later.
