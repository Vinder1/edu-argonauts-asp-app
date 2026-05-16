using System.Text.RegularExpressions;
using Microsoft.AspNetCore.Mvc;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddOpenApi();

var app = builder.Build();

var words = File.ReadAllLines("obscene.txt");
words = [.. words.OrderByDescending(w => w.Length)];
var pattern = $@"\b({string.Join("|", words)})\b";
var regex = new Regex(pattern, RegexOptions.IgnoreCase | RegexOptions.Compiled);

app.MapPost("/", ([FromBody] Message message) =>
{
    return regex.Replace(message.Text, match => new string('*', match.Length));
});

app.Run();

record Message(string Text);
