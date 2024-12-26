using System.Collections.Immutable;
using BlazorMVU.Demo.Components;

namespace BlazorMVU.Demo.Pages;

public partial class Index
{
    private const string CodeUrl =
        "https://github.com/Atypical-Consulting/BlazorMVU/blob/main/src/BlazorMVU.Demo/Components/";

    private readonly ImmutableList<MvuTodoList.Todo> _todos =
    [
        new("Programming", false),
        new("Cooking", true),
        new("Cleaning", false)
    ];
}