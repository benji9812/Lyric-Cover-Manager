using Spectre.Console;
using System;
using System.Collections.Generic;

namespace Lyric_Cover_Manager
{
    public static class MenuHelper
    {
        public static string ShowMenu()
        {
            AnsiConsole.Clear();

            var gklavArt = @"
[bold yellow]
      _   _
     / `-' \
    |  .-.  |
    |  | |  |
    |  | |  |
     \ | | / 
      `' `'    
[/]";

            var hype = "[blink bold lime]★ COVER BANGERS ★[/]";
            var subtitle = "[bold fuchsia]♫ Let's Rock! ♫[/]";
            var logga = $"{gklavArt}\n{hype}\n{subtitle}";

            var mainPanel = new Panel(logga)
                .Border(BoxBorder.Heavy)
                .Padding(2, 2)
                .Expand()
                .Header("[bold underline fuchsia]Lyric Cover Manager[/]");

            // Beräkna top-padding för vertikal centrering (för stor konsol)
            int topPad = Math.Max(0, (Console.WindowHeight / 2) - 8); // -8: halva ASCII-höjden

            var centered = new Padder(mainPanel, new Padding(0, topPad, 0, 0));
            AnsiConsole.Write(centered);

            var choices = new List<string>
    {
        "[green]Lägg till ny låt[/]",
        "[yellow]Visa alla låtar[/]",
        "[blue]Sök låt[/]",
        "[green]Filtrera på genre[/]",
        "[green]Filtrera på status[/]",
        "[green]Filtrera på språk[/]",
        "[magenta]Visa de senaste övade[/]",
        "[cyan]Redigera lyrics[/]",
        "[red]Ta bort låt[/]",
        "[bold underline italic red]Avsluta[/]"
    };

            var choiceText = AnsiConsole.Prompt(
                new SelectionPrompt<string>()
                    .Title("[bold underline lime]Välj ett menyval[/]")
                    .PageSize(12)
                    .AddChoices(choices));

            int menuNum = choices.FindIndex(x => x == choiceText);
            return menuNum switch
            {
                0 => "1",
                1 => "2",
                2 => "3",
                3 => "4",
                4 => "5",
                5 => "6",
                6 => "7",
                7 => "8",
                8 => "9",
                9 => "0",
                _ => "0"
            };
        }

        public static string GetInput(string prompt)
        {
            return AnsiConsole.Prompt(
                new TextPrompt<string>($"[bold cyan]{prompt}[/]")
                    .AllowEmpty()
            );
        }
        public static void Pause(string msg = "Tryck Enter för att fortsätta...")
        {
            AnsiConsole.MarkupLine($"[grey]{msg}[/]");
            Console.ReadLine();
        }
    }
}
