#! "netcoreapp2.0"
#r "nuget:NetStandard.Library,2.0.0"
#r "nuget:Kurukuru,1.0.0"

using Kurukuru;
using System.Threading;

Spinner.Start("Processing...", () =>
{
    Thread.Sleep(1000 * 3);

    // MEMO: If you want to show as failed, throw a exception here.
    // throw new Exception("Something went wrong!");
});

Spinner.Start("Stage 1...", spinner =>
{
    Thread.Sleep(1000 * 3);
    spinner.Text = "Stage 2...";
    Thread.Sleep(1000 * 3);
    spinner.Fail("Something went wrong!");
});