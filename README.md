# TestLinker

[![License](https://img.shields.io/github/license/vladyslav-burylov/resharper-testlinker)](LICENSE)

TestLinker collects link data between types (i.e., production and test code) based on various mechanisms and provides various features based on that. For your convenience, TestLinker automatically takes base/derived types into account when meaningful.

## WARNING

This fork of the [TestLinker plugin](https://github.com/matkoch/resharper-testlinker) originally developed by Matthias Koch was created for personal use and does not guaranteed to be supported.

### Building Locally

- Please go though https://github.com/JetBrains/resharper-rider-plugin
- Once initial setup is done, plugin can be build using:

  ```bash
  ./gradlew :buildPlugin
  ```

## Navigation

<img src=misc/Demon_Navigate.gif />

- [Goto Related Files](https://www.jetbrains.com/help/resharper/2016.1/Navigation_and_Search__Go_to_Related_Files.html) is extended with navigation points to production/test classes.
- New shortcuts `ReSharper_GotoAllLinkedTypes` and `ReSharper_GotoLinkedTypesWithDerivedName` (assignable via keyboard options) that jumps between linked types. In case of multiple linked types, a dedicated popup menu is shown, which can also be displayed in [Find Results](https://www.jetbrains.com/help/resharper/2016.1/Reference__Windows__Find_Results_Window.html) window.

## Test Creation

<img src=misc/Demo_Create.gif />

- Create production/test class if they don't exist
- Requires at least one matching pair of test and production class in the project

## Test Execution

- Tests can be executed from their linked production code. This feature automatically integrates with the shortcuts for executing unit tests in *run*, *debug*, *profile*, and *cover* mode.

## Configuration

Link data is currently maintained via:

- **Derived names**, as with `Calculator` and `CalculatorTest`. Pre-/Postfixes can be configured in the options page.
- **Usages of TypeofAttributes**, as in `[Subject (typeof (FirstComponent), typeof(SecondComponent)]`, which are applied to test classes. This custom attribute is especially useful for integration test and can be configured through the options page.

### Rider

<img src=misc/OptionsPage-Rider.png width=600px />

### ReSharper

<img src=misc/OptionsPage.png width=600px />
