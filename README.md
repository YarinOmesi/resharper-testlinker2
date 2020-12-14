# Test Linker 2

[![License](https://img.shields.io/github/license/vladyslav-burylov/resharper-testlinker2)](LICENSE)

Test Linker 2 collects link data between types (i.e., production and test code)
based on various mechanisms and provides various features based on that.
For your convenience, Test Linker 2 automatically takes base/derived types into
account when meaningful.

## Features

- Adds hotkey for quick switching between tests/code
- Synchronizes test/production classes renames

<img src="misc/Demo.gif"  alt="demo video"/>

## Configuration

Link data is currently maintained via:

- **Derived names**, as with `Calculator` and `CalculatorTest`. Pre-/Postfixes can be configured in the options page.
- **Usages of TypeofAttributes**, as in `[Subject (typeof (FirstComponent), typeof(SecondComponent)]`, which are applied to test classes. This custom attribute is especially useful for integration test and can be configured through the options page.

<img src="misc/OptionsPage.png" alt="options page screenshot"/>

## Building Locally

- Please go though https://github.com/JetBrains/resharper-rider-plugin
- Once initial setup is done, plugin can be built using:

  ```bash
  ./gradlew :buildPlugin
  ```

- Other useful scripts:
  - `./gradlew :rdgen` - regenerate data contract
  - `./gradlew :runIde` - run plugins inside sandbox (IDE version specified by `gradle.properties`)
