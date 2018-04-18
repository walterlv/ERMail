[English][en]|[日本語][jp]|[简体中文][zh-chs]|[繁體中文][zh-cht]
-|-|-|-

[en]: /README.md
[jp]: /README.jp.md
[zh-chs]: /README.zh-chs.md
[zh-cht]: /README.zh-cht.md

# Assemble Mailing

Assemble Mailing is an e-mail client that can help you to assemble related emails into an aggregated view so that there is no need for you to click open them one by one.

## Getting started

### Run the program (UWP)

1. Run
1. Enter your mail info and then click "Sign in". *We'll not store user's password directly, it is managed by the operating system.*
1. Select a mail folder (INBOX is selected by default).
1. Select a mail from the mail list.
1. Wait and view your mail content, then you can click the Assemble Button on the app's bottom-right corner.
1. Then our classification algorithm will run with full-screen logs. *The algorithm is under developing so that it will result in nothing.*

## How to contribute

### Requirement

- Visual Studio 2017 (with version 15.3 or later)
    - .NET Standard 2.0
    - .NET Core 2.0
    - C# 7.2
    - NuGet (4.3 or later)
- UWP (C#) Windows SDK 10.0.16299.0
    - for building AssembleMailing.Universal
- [Avalonia](https://github.com/AvaloniaUI/Avalonia) [Visual Studio Extension](https://marketplace.visualstudio.com/items?itemName=AvaloniaTeam.AvaloniaforVisualStudio)
    - for designing AssembleMailing.Desktop

### Build and run

1. Ensure that your Windows device has developer mode enabled.
    - Goto Settings -> Update & Security -> For developers -> Developer mode
1. Switch your startup project from `AssembleMailing.Desktop` to `AssembleMailing.Universal`
    - It's not necessary if you'll contribute to Avalonia version.
1. Make sure your target device is `Local Machine` not a `Simulator`.
    - Goto project properties -> Debug -> Local Machine.

### Project structure

+ **.vscode** *If you debug this project using VSCode, this folder contains the build info and debug info.*
+ **docs** *Stores documentation of this project.*
    - **assets** *Images or other assets that is used by the documentation.*
+ **src** *The main source code.*
    - **AssembleMailing.Core** *The main logic of this project. All the code here is cross-platform.*
    - **AssembleMailing.Desktop** *The startup project targeting Avalonia UI Framework so that it could be cross-platform.*
    - **AssembleMailing.Universal** *Windows 10 Specified startup project (UWP).*
    - You can add your own UI Framework here, but it should support .NET Standard 2.0.

### Roadmap

This is a project mostly for studying and experimenting new technology we're learning. But we'll also publish the product with these new technologies to increase efficiency and create value for each user.

1. [x] Basic UI
    - [x] for UWP
    - [ ] for Avalonia
    - *for Xamarin*
1. [x] Fetch mails
    - [x] from the remote server
    - [x] using local cache
1. [ ] Classify all emails via Machine Learning
    - [ ] NaiveBayesClassifier
    - [ ] other available machine learning technology
1. [ ] Word segmentation algorithm
    - *for English*
    - [ ] for 中文
    - *for other languages*
1. [ ] Publish
    - [ ] prepare UI assets for UWP
    - [ ] prepare UI assets for Avalonia
    - [ ] improve stability
1. *Introduce some other black technology*
1. *Analysis feedback data and improve them*

## Preview of the UI

![UWP Client](/docs/assets/2018-04-15-19-15-57.png)  
▲ UWP Client

![Avalonia Client](/docs/assets/2018-04-15-19-18-15.png)  
▲ Avalonia Client
