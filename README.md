# BlazorMVU [![Sparkline](https://stars.medv.io/Atypical-Consulting/BlazorMVU.svg)](https://stars.medv.io/Atypical-Consulting/BlazorMVU)
A Model-View-Update (MVU) pattern implementation for Blazor.
---

[![Atypical-Consulting - BlazorMVU](https://img.shields.io/static/v1?label=Atypical-Consulting&message=BlazorMVU&color=blue&logo=github)](https://github.com/Atypical-Consulting/BlazorMVU "Go to GitHub repo")
[![License: MIT](https://img.shields.io/badge/License-MIT-blue.svg)](https://opensource.org/licenses/MIT)
[![.NET 10](https://img.shields.io/badge/.NET-10.0-purple?logo=dotnet)](https://dotnet.microsoft.com/en-us/download/dotnet/10.0)
[![stars - BlazorMVU](https://img.shields.io/github/stars/Atypical-Consulting/BlazorMVU?style=social)](https://github.com/Atypical-Consulting/BlazorMVU)
[![forks - BlazorMVU](https://img.shields.io/github/forks/Atypical-Consulting/BlazorMVU?style=social)](https://github.com/Atypical-Consulting/BlazorMVU)

[![GitHub tag](https://img.shields.io/github/tag/Atypical-Consulting/BlazorMVU?include_prereleases=&sort=semver&color=blue)](https://github.com/Atypical-Consulting/BlazorMVU/releases/)
[![issues - BlazorMVU](https://img.shields.io/github/issues/Atypical-Consulting/BlazorMVU)](https://github.com/Atypical-Consulting/BlazorMVU/issues)
[![GitHub pull requests](https://img.shields.io/github/issues-pr/Atypical-Consulting/BlazorMVU)](https://github.com/Atypical-Consulting/BlazorMVU/pulls)
[![GitHub contributors](https://img.shields.io/github/contributors/Atypical-Consulting/BlazorMVU)](https://github.com/Atypical-Consulting/BlazorMVU/graphs/contributors)
[![GitHub last commit](https://img.shields.io/github/last-commit/Atypical-Consulting/BlazorMVU)](https://github.com/Atypical-Consulting/BlazorMVU/commits/master)

---

## Table of Contents

* [Introduction](#introduction)
* [The Problem](#the-problem)
* [The Solution](#the-solution)
* [Features](#features)
* [Tech Stack](#tech-stack)
* [Roadmap](#roadmap)
* [Installation](#installation)
* [Usage](#usage)
* [Architecture](#architecture)
* [Project Structure](#project-structure)
* [Running the Tests](#running-the-tests)
* [Stats](#stats)
* [Contributing](#contributing)
* [License](#license)
* [Contact](#contact)
* [Acknowledgements](#acknowledgements)

## Introduction

**BlazorMVU** is a library that implements the Model-View-Update (MVU) pattern for Blazor. It provides a structured way to organize your Blazor components and manage their state, making your code more understandable and easier to maintain.

![Drag Racing](./assets/blazormvu.png)

Demo: This Blazor project is deployed on [GitHub Pages](https://atypical-consulting.github.io/BlazorMVU/)


## The Problem

The Elm architecture, or Model-View-Update (MVU), is a simple yet powerful pattern for structuring applications. It has gained popularity due to its simplicity, maintainability, and robustness. However, despite its advantages, the Elm architecture has not been widely adopted in the Blazor community.

Blazor, as a framework, is flexible and allows for various design patterns to be implemented, including MVU. However, there hasn't been a straightforward way to implement the Elm architecture in Blazor — leaving developers to manage complex component state with ad-hoc patterns that are hard to test and reason about.

## The Solution

**BlazorMVU** brings the benefits of the Elm architecture to the Blazor community. By providing a library that implements the MVU pattern, we make it easier for developers to structure their Blazor applications in a way that is easy to understand, maintain, and test.

By reducing the complexity associated with state management and UI updates, developers can focus more on the business logic of their applications, leading to more robust and reliable software.

## Features

### Core Features
* **MVU Pattern Implementation** - Full Model-View-Update architecture for Blazor
* **SimpleMvuComponent** - Lightweight base class for simple components
* **MvuComponent** - Full-featured base class with advanced capabilities

### Advanced Features
* **Commands (Cmd<TMsg>)** - Declarative side effects handling
  - `Cmd.OfTask` - Async operations that return messages
  - `Cmd.OfMsg` - Immediate message dispatch
  - `Cmd.Batch` - Combine multiple commands
  - `Cmd.Delay` - Delayed message dispatch
* **Subscriptions (Sub<TMsg>)** - External event listeners
  - `Sub.Timer` - Interval-based updates
  - `Sub.Timeout` - One-time delayed messages
  - `Sub.Custom` - Custom subscription logic
* **Middleware** - Dispatch pipeline interceptors
  - Logging middleware
  - Timing middleware
  - Debounce/Throttle middleware
  - Error handling middleware
* **Time-Travel Debugging** - Navigate through state history
* **State Persistence** - localStorage/sessionStorage integration
* **MvuResult<T>** - Functional result type for error handling

### Demo Components
* Counter, Text Reverser, Password Form, Todo List
* Fetch with error handling
* Stopwatch with subscriptions
* Shopping Cart with commands and async
* Parent-Child communication patterns

### Testing
* Unit tests using BUnit and xUnit v3
* Shouldly assertions

## Tech Stack

| Layer | Technology |
|-------|-----------|
| Runtime | .NET 9.0 (SDK 10.0) |
| UI Framework | Blazor (Razor Components) |
| Language | C# 12 |
| Testing | xUnit v3 + bUnit + Shouldly |
| Build | Nuke Build |

## Roadmap

BlazorMVU is actively maintained and targeting **.NET 10** (already running on SDK 10.0.x). Upcoming improvements:

- **NuGet Package** — Publish `Atypical-Consulting.BlazorMVU` to NuGet.org for easy installation
- **Source Generator** — Auto-generate boilerplate for `Update` dispatch tables
- **DevTools Integration** — Browser extension for time-travel debugging visualization
- **Blazor United** — Full support for Blazor Web App (SSR + interactive) hybrid rendering modes
- **Performance Benchmarks** — BenchmarkDotNet suite comparing MVU vs standard Blazor component patterns

> **Want to contribute?** Pick any roadmap item and open a PR. See [CONTRIBUTING.md](./CONTRIBUTING.md) for guidelines.

## Stats

![Alt](https://repobeats.axiom.co/api/embed/26eb4f8ccaa87d22a857066599225c9f542a4070.svg "Repobeats analytics image")

## Installation

Clone the repository and build the project:

```bash
git clone https://github.com/Atypical-Consulting/BlazorMvu.git
cd BlazorMvu
dotnet build
```

### Using the Template

Install the template:

```bash
dotnet new install BlazorMVU.Templates
```

Create a new project:

```bash
dotnet new blazormvu -n MyApp
cd MyApp
dotnet run
```

## Usage

### Basic Usage

For simple components, inherit from `SimpleMvuComponent<TModel, TMsg>`:

```csharp
@inherits BlazorMVU.SimpleMvuComponent<int, MvuCounter.Msg>

<div class="grid">
  <button @onclick="@(() => Dispatch(new Msg.Decrement()))">-</button>
  <input type="text" value="@State" disabled />
  <button @onclick="@(() => Dispatch(new Msg.Increment()))">+</button>
</div>

@code {
  // Messages using discriminated unions
  public abstract record Msg
  {
    public record Increment : Msg;
    public record Decrement : Msg;
  }

  protected override int Init() => 0;

  protected override int Update(Msg msg, int model)
    => msg switch
    {
      Msg.Increment => model + 1,
      Msg.Decrement => model - 1,
      _ => model
    };
}
```

### Advanced Usage with Commands

For components that need side effects, inherit from `MvuComponent<TModel, TMsg>`:

```csharp
@inherits BlazorMVU.MvuComponent<FetchExample.Model, FetchExample.Msg>

@code {
  public record Model(string? Data, bool IsLoading, string? Error);

  public abstract record Msg
  {
    public record FetchData : Msg;
    public record DataReceived(MvuResult<string> Result) : Msg;
  }

  // Return both model and command
  protected override (Model, Cmd<Msg>) InitWithCmd()
    => (new Model(null, true, null), Cmd.OfMsg<Msg>(new Msg.FetchData()));

  protected override (Model, Cmd<Msg>) UpdateWithCmd(Msg msg, Model model)
    => msg switch
    {
      Msg.FetchData => (
        model with { IsLoading = true },
        Cmd.OfTask<Msg>(async ct => {
          var result = await FetchDataAsync(ct);
          return new Msg.DataReceived(result);
        })),

      Msg.DataReceived received => (
        received.Result.IsSuccess
          ? model with { Data = received.Result.Value, IsLoading = false }
          : model with { Error = received.Result.Error?.Message, IsLoading = false },
        Cmd.None<Msg>()),

      _ => (model, Cmd.None<Msg>())
    };
}
```

### Using Subscriptions

Override the `Subscriptions` method to react to external events:

```csharp
protected override Sub<Msg> Subscriptions(Model model)
{
  if (model.IsRunning)
  {
    // Tick every 100ms while running
    return Sub.Timer<Msg>(
      TimeSpan.FromMilliseconds(100),
      now => new Msg.Tick(now),
      "timer-id");
  }
  return Sub.None<Msg>();
}
```

### Using Middleware

Add middleware in `OnInitialized`:

```csharp
protected override void OnInitialized()
{
  UseMiddleware(
    Middleware.ConsoleLogger<Model, Msg>(),
    Middleware.Timing<Model, Msg>((msg, elapsed) =>
      Console.WriteLine($"{msg} took {elapsed.TotalMilliseconds}ms"))
  );

  base.OnInitialized();
}
```

### Time-Travel Debugging

Enable time-travel debugging via parameter:

```razor
<MyComponent EnableTimeTravel="true" TimeTravelMaxHistory="50" />
```

Then access the debugger:

```csharp
// Go back in history
Debugger?.GoBack();
RestoreFromDebugger();

// Go forward
Debugger?.GoForward();
RestoreFromDebugger();
```

## Architecture

The MVU (Model-View-Update) pattern creates a unidirectional data flow:

```
                    ┌─────────────────────────────────────┐
                    │                                     │
                    ▼                                     │
              ┌───────────┐                               │
              │   Model    │  (immutable state)            │
              └─────┬─────┘                               │
                    │                                     │
                    ▼                                     │
              ┌───────────┐                               │
              │   View     │  (Blazor Razor component)     │
              └─────┬─────┘                               │
                    │ user interaction                     │
                    ▼                                     │
              ┌───────────┐                               │
              │  Message   │  (discriminated union)        │
              └─────┬─────┘                               │
                    │                                     │
                    ▼                                     │
              ┌───────────┐     ┌───────────┐             │
              │  Update    │────▶│    Cmd     │─── side ───┘
              │ (pure fn)  │     │ (effects)  │   effects
              └───────────┘     └───────────┘  return Msg
```

- **Model** -- An immutable record representing component state
- **View** -- A Blazor Razor component that renders the model
- **Message** -- A discriminated union (abstract record) describing what happened
- **Update** -- A pure function: `(Msg, Model) -> (Model, Cmd<Msg>)`
- **Cmd** -- Declarative side effects (async, delay, batch) that produce new messages

### Project Structure

```
BlazorMVU/
├── src/
│   ├── BlazorMVU.Core/          # Library: base components, commands, subscriptions
│   │   ├── Cmd.cs               # Command types (OfTask, OfMsg, Batch, Delay)
│   │   ├── Sub.cs               # Subscription types (Timer, Timeout, Custom)
│   │   ├── Middleware.cs         # Dispatch pipeline interceptors
│   │   ├── MvuComponent.cs      # Base component classes
│   │   ├── MvuResult.cs         # Functional result type
│   │   ├── StatePersistence.cs   # localStorage/sessionStorage integration
│   │   └── TimeTravel.cs        # Time-travel debugging support
│   ├── BlazorMVU.Demo/          # Demo Blazor WebAssembly app
│   │   ├── Components/          # Example MVU components (Counter, Todo, etc.)
│   │   ├── Pages/               # Demo pages
│   │   └── wwwroot/             # Static assets
│   └── BlazorMVU.Tests/         # Unit tests (xUnit v3 + bUnit)
├── build/                       # Nuke build scripts
├── assets/                      # Documentation images
└── BlazorMVU.sln               # Solution file
```

## Running the Tests

Tests are located in the `BlazorMvu.Tests project`. You can run them using the .NET Core CLI:

```bash
dotnet test
```

## Contributing

Contributions are welcome! Please read the [CONTRIBUTION GUIDELINES](https://github.com/Atypical-Consulting/BlazorMVU/blob/main/CONTRIBUTING.md) first.

## License

This project is licensed under the terms of the MIT license. If you use this library in your project, please consider adding a link to this repository in your project's README.

This project is maintained by [Atypical Consulting](https://www.atypical.consulting/). If you need help with this project, please contact us from this repository by opening an issue.

## Contact

You can contact us by opening an issue on this repository.

## Acknowledgements

* [All Contributors](../../contributors)
* [Atypical Consulting](https://www.atypical.consulting/)

## Contributors

[![Contributors](https://contrib.rocks/image?repo=Atypical-Consulting/BlazorMVU)](https://github.com/Atypical-Consulting/BlazorMVU/graphs/contributors)

---

Built with care by [Atypical Consulting](https://atypical.garry-ai.cloud) — opinionated, production-grade open source.
