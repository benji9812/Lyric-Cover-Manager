using Spectre.Console;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lyric_Cover_Manager
{
    public static class MenuHelper
    {
        public static void ShowMenu()
        {
            AnsiConsole.Write
            (
                 new Panel
                (
                "[bold yellow]1.[/] Lägg till ny låt\n" +
                "[bold yellow]2.[/] Visa alla låtar\n" +
                "[bold yellow]3.[/] Sök låt\n" +
                "[bold yellow]4.[/] Filtrera på genre\n" +
                "[bold yellow]5.[/] Filtrera på status\n" +
                "[bold yellow]6.[/] Filtrera på språk\n" +
                "[bold yellow]7.[/] Visa de senaste övade\n" +
                "[bold yellow]8.[/] Redigera lyrics\n" +
                "[bold red]0.[/] Avsluta"
                )
                 .Header("[bold underline green]Lyric Cover Manager[/]")
                 .Border(BoxBorder.Rounded)
                 .BorderStyle(new Style(Color.Fuchsia))
            );
        }

        public static string GetInput(string prompt)
        {
            return AnsiConsole.Prompt
            (
                new TextPrompt<string>($"[bold cyan]{prompt}[/]")
                    .AllowEmpty()
            );
        }
    }
}
