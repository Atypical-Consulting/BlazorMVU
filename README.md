# BlazorMVU
A Model-View-Update (MVU) pattern implementation for Blazor.

---

[![Atypical-Consulting - BlazorMVU](https://img.shields.io/static/v1?label=Atypical-Consulting&message=BlazorMVU&color=blue&logo=github)](https://github.com/Atypical-Consulting/BlazorMVU "Go to GitHub repo")
[![License: BSD-3-Clause](https://img.shields.io/badge/License-BSD--3--Clause-blue.svg)](https://opensource.org/licenses/BSD-3-Clause)
[![stars - BlazorMVU](https://img.shields.io/github/stars/Atypical-Consulting/BlazorMVU?style=social)](https://github.com/Atypical-Consulting/BlazorMVU)
[![forks - BlazorMVU](https://img.shields.io/github/forks/Atypical-Consulting/BlazorMVU?style=social)](https://github.com/Atypical-Consulting/BlazorMVU)

[![GitHub tag](https://img.shields.io/github/tag/Atypical-Consulting/BlazorMVU?include_prereleases=&sort=semver&color=blue)](https://github.com/Atypical-Consulting/BlazorMVU/releases/)
[![issues - BlazorMVU](https://img.shields.io/github/issues/Atypical-Consulting/BlazorMVU)](https://github.com/Atypical-Consulting/BlazorMVU/issues)
[![GitHub pull requests](https://img.shields.io/github/issues-pr/Atypical-Consulting/BlazorMVU)](https://github.com/Atypical-Consulting/BlazorMVU/pulls)
[![GitHub contributors](https://img.shields.io/github/contributors/Atypical-Consulting/BlazorMVU)](https://github.com/Atypical-Consulting/BlazorMVU/graphs/contributors)
[![GitHub last commit](https://img.shields.io/github/last-commit/Atypical-Consulting/BlazorMVU)](https://github.com/Atypical-Consulting/BlazorMVU/commits/master)

---

## 📝 Table of Contents

* [Introduction](#-introduction)
* [Features](#-features)
* [Installation](#-installation)
* [Usage](#-usage)
* [Running the Tests](#-running-the-tests)
* [Contributing](#-contributing)
* [License](#-license)
* [Contact](#-contact)
* [Acknowledgements](#-acknowledgements)
* [Contributors](#-contributors)

## 📖 Introduction

BlazorMvu is a library that implements the Model-View-Update (MVU) pattern for Blazor. It provides a structured way to organize your Blazor components and manage their state, making your code more understandable and easier to maintain.

## 📌 Features

* MVU pattern implementation for Blazor
* Demo components showcasing the usage of the library
* Unit tests using BUnit

## 📥 Installation

Clone the repository and build the project:

```bash
git clone https://github.com/Atypical-Consulting/BlazorMvu.git
cd BlazorMvu
dotnet build
```

## 📚 Usage

To use the BlazorMvu library, inherit from the `MvuComponent<TModel, TMsg>` class in your Blazor component. Define your model and messages, and implement the Init and Update methods.

## 🚀 Running the Tests

Tests are located in the `BlazorMvu.Tests project`. You can run them using the .NET Core CLI:

```bash
dotnet test
```

## 🤝 Contributing

Contributions are welcome! Please read the [CONTRIBUTION GUIDELINES](https://github.com/Atypical-Consulting/BlazorMVU/blob/main/CONTRIBUTING.md) first.

## 📜 License

This project is licensed under the terms of the MIT license. If you use this library in your project, please consider adding a link to this repository in your project's README.

This project is maintained by [Atypical Consulting](https://www.atypical.consulting/). If you need help with this project, please contact us from this repository by opening an issue.

## 📬 Contact

You can contact us by opening an issue on this repository.

## 🙌 Acknowledgements

* [All Contributors](../../contributors)
* [Atypical Consulting](https://www.atypical.consulting/)

## ✨ Contributors

[![Contributors](https://contrib.rocks/image?repo=Atypical-Consulting/BlazorMVU)](http://contrib.rocks)