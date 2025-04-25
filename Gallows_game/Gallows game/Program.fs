open System
open System.IO
open System.Text.Json
open System.Diagnostics
open System.Collections.Generic
open System.Threading // Fontos, hogy hozzáadjuk ezt a nevet, hogy a Sleep függvény működjön.

// Típusok
type Language = | Hungarian | English
type Difficulty = | Easy | Medium | Hard
type Stats = {
    GamesPlayed: int
    GamesWon: int
    FastestWinSeconds: float
    AverageWinSeconds: float // Átlagos nyerési idő
}

let defaultStats = { GamesPlayed = 0; GamesWon = 0; FastestWinSeconds = Double.MaxValue; AverageWinSeconds = 0.0 }
let statsFile = "stats.json"

let loadStats () =
    if File.Exists(statsFile) then
        JsonSerializer.Deserialize<Stats>(File.ReadAllText(statsFile))
    else defaultStats

let saveStats stats =
    File.WriteAllText(statsFile, JsonSerializer.Serialize(stats))

let loadWords lang difficulty =
    let file =
        let langPart = match lang with Hungarian -> "hu" | English -> "en"
        let diffPart = match difficulty with Easy -> "easy" | Medium -> "medium" | Hard -> "hard"
        sprintf "words_%s_%s.txt" langPart diffPart
    printfn "Trying to load: %s" file
    if not (File.Exists(file)) then
        failwithf "❌ Szólista nem található: %s" file
    let lines = File.ReadAllLines(file) |> Array.filter (fun s -> s.Trim().Length > 0)
    let wordDefinitionPairs = 
        [for i in 0..(lines.Length - 1) .. 1 do
            if i + 1 < lines.Length then
                yield lines.[i], lines.[i + 1]]  // A párok: szó és definíció
    Array.ofList wordDefinitionPairs  // Itt alakítjuk át listává a tömböt

let pickWord (wordDefinitionPairs: (string * string) array) =
    let rnd = Random()
    let word, definition = wordDefinitionPairs.[rnd.Next(wordDefinitionPairs.Length)]
    word.ToUpperInvariant(), definition

let printColored (text: string) color =
    let old = Console.ForegroundColor
    Console.ForegroundColor <- color
    Console.Write(text)
    Console.ForegroundColor <- old

let drawHangman errors =
    let stages = [| 
        "  ---------  \n  |         | \n            | \n            | \n            | \n            | \n            | ";
        "  ---------  \n  |         | \n  O         | \n            | \n            | \n            | \n            | ";
        "  ---------  \n  |         | \n  O         | \n  |         | \n            | \n            | \n            | ";
        "  ---------  \n  |         | \n  O         | \n /|         | \n            | \n            | \n            | ";
        "  ---------  \n  |         | \n  O         | \n /|\\        | \n            | \n            | \n            | ";
        "  ---------  \n  |         | \n  O         | \n /|\\        | \n /          | \n            | \n            | ";
        "  ---------  \n  |         | \n  O         | \n /|\\        | \n / \\        | \n            | \n            | ";
    |]
    Console.Clear()
    printfn "%s" stages.[min errors (stages.Length - 1)]

let drawLetterStatus (guessed: Set<char>) =
    let all = ['A'..'Z']
    for c in all do
        if guessed.Contains(c) then
            printColored (sprintf "%c " c) ConsoleColor.Red
        else
            printColored (sprintf "%c " c) ConsoleColor.Green
    printfn ""

let T lang key =
    let hu: IDictionary<string, string> = dict [
        "menu", "Főmenü"; "new", "Új játék"; "rules", "Szabályok"; "exit", "Kilépés";
        "difficulty", "Nehézség"; "guess", "Tippelj egy betűt vagy szót:"; 
        "win", "Nyertél!"; "lose", "Vesztettél! A szó az volt: "; "again", "Nyomj Entert a folytatáshoz.";
        "welcome", "Üdvözöljük az akasztófa játékunkban!"; "stats", "Statisztikák"; "choose", "Válassz a menü pontok közül";
    ]
    let en: IDictionary<string, string> = dict [
        "menu", "Main Menu"; "new", "New Game"; "rules", "Rules"; "exit", "Exit";
        "difficulty", "Difficulty"; "guess", "Guess a letter or the full word:"; 
        "win", "You won!"; "lose", "You lost! The word was: "; "again", "Press Enter to continue.";
        "welcome", "Welcome to our Gallows game!"; "stats", "Statistics"; "choose", "Choose an option from the menu";
    ]
    if lang = Hungarian then hu.[key] else en.[key]

let chooseLanguage () =
    printfn "1. Magyar"
    printfn "2. English"
    match Console.ReadLine() with
    | "1" -> Hungarian
    | _ -> English

let chooseDifficulty lang =
    printfn "%s" (T lang "difficulty")
    printfn "1. Easy  2. Medium  3. Hard"
    match Console.ReadLine() with
    | "1" -> Easy | "3" -> Hard | _ -> Medium

let showStats stats lang =
    Console.Clear()
    printfn "%s" (T lang "stats")
    printfn "Games Played: %d" stats.GamesPlayed
    printfn "Games Won: %d" stats.GamesWon
    printfn "Fastest Win: %.1f seconds" stats.FastestWinSeconds
    printfn "Average Win: %.1f seconds" stats.AverageWinSeconds
    printfn "%s" (T lang "again")
    Console.ReadLine() |> ignore

let showDefinitionAndWait definition =
    Console.Clear()
    printfn "%s" definition
    // 3 másodperc várakozás
    Thread.Sleep(3000)  // 3 másodperc várakozás
    Console.Clear()

let playGame lang stats =
    let difficulty = chooseDifficulty lang
    let wordDefinitionPairs = loadWords lang difficulty
    if wordDefinitionPairs.Length = 0 then printfn "No words available."; stats else
    let word, definition = pickWord wordDefinitionPairs
    let revealed = Array.create word.Length '_' |> Array.toList
    let stopwatch: Stopwatch = Stopwatch.StartNew()

    // Megjelenítjük a szó definícióját és várunk 3 másodpercet
    showDefinitionAndWait definition

    let rec loop guessed state errors =
        Console.Clear()
        drawHangman errors
        drawLetterStatus guessed
        printfn "\n%s" (String.Join(" ", state |> List.map string))
        printf "%s " (T lang "guess")
        let input = Console.ReadLine().ToUpperInvariant()
        let newState, newErrors, guessed =
            if input.Length = 1 then
                let ch = input.[0]
                if guessed.Contains ch then state, errors, guessed
                elif word.Contains(string ch) then
                    let state' =
                        [for i in 0..word.Length-1 -> if word.[i] = ch then ch else state.[i]]
                    state', errors, guessed.Add ch
                else state, errors + 1, guessed.Add ch
            elif input = word then
                word.ToCharArray() |> Array.toList, errors, guessed
            else state, errors + 1, guessed
        if newState |> List.contains '_' && newErrors < 8 then loop guessed newState newErrors
        else
            let elapsed = stopwatch.Elapsed.TotalSeconds
            let updatedStats = {
                GamesPlayed = stats.GamesPlayed + 1
                GamesWon = if newState |> List.contains '_' then stats.GamesWon else stats.GamesWon + 1
                FastestWinSeconds = if not (newState |> List.contains '_') && elapsed < stats.FastestWinSeconds then elapsed else stats.FastestWinSeconds
                AverageWinSeconds = (stats.AverageWinSeconds * float stats.GamesPlayed + elapsed) / float (stats.GamesPlayed + 1)
            }
            if newState |> List.contains '_' then
                drawHangman newErrors; printfn "%s%s" (T lang "lose") word
            else
                printfn "%s" (T lang "win")
            printfn "Time: %.1fs" elapsed
            printfn "%s" (T lang "again")
            Console.ReadLine() |> ignore
            updatedStats
    loop Set.empty revealed 0

let rec menu lang stats =
    Console.Clear()
    printfn "==== %s ====" (T lang "menu")
    printfn "%s" (T lang "choose")
    printfn "1. %s\n2. %s\n3. %s\n4. %s" (T lang "new") (T lang "rules") (T lang "stats") (T lang "exit")
    match Console.ReadLine() with
    | "1" -> let updated = playGame lang stats in saveStats updated; menu lang updated
    | "2" -> printfn "A játék célja kitalálni a szót betűnként vagy egészben. 8 hibalehetőséged van. (Nyomj egy entert a folyatáshoz) / The goal of the game is to guess the word as a letter or as a whole, you have 8 chances to make a mistake. (Press Enter to continue) "; Console.ReadLine() |> ignore; menu lang stats
    | "3" -> showStats stats lang; menu lang stats
    | "4" -> () // Kilépés
    | _ -> menu lang stats

[<EntryPoint>]
let main _ =
    Console.OutputEncoding <- System.Text.Encoding.UTF8
    let lang = chooseLanguage()
    // Üdvözlő üzenet megjelenítése 3 másodpercig
    printfn "%s" (T lang "welcome")  
    Thread.Sleep(3000) // 3 másodperces várakozás
    let stats = loadStats()
    menu lang stats
    0
