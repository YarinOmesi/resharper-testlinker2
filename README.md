# TestLinker

[![License](https://img.shields.io/github/license/matkoch/testlinker.svg?style=flat-square&logo=data%3Aimage%2Fpng%3Bbase64%2CiVBORw0KGgoAAAANSUhEUgAAAEAAAABACAYAAACqaXHeAAAAAXNSR0IArs4c6QAAAARnQU1BAACxjwv8YQUAAAAJcEhZcwAAHYcAAB2HAY%2Fl8WUAAAAZdEVYdFNvZnR3YXJlAHBhaW50Lm5ldCA0LjAuMTCtCgrAAAADB0lEQVR4XtWagXETMRREUwIlUAIlUAodQAl0AJ1AB9BB6AA6gA6MduKbkX%2BevKecNk525jHO3l%2Fp686xlJC70%2Bl0C942vjV%2Bn9FreVQbBc0wWujfRpW8Z78JaIb53hhJ1ygTA80w9PQ36duBMjHQHPCuoQZfutSjeqU1PAJN4E3j2pN7aVKv6pnWcgGawNfGa5N6prVcgGZBn8yvVXZXQbOgPXokXaPMNZwoc41D%2FaHZ8b7hpBrKjnCizIjD%2FaHZ8aPR6%2BeZXqqh7Agnyow43B%2BaZz40qnQ36a6rlsYgnChDLOkPzTN1z%2B9PafU0N3OAcaIMsaQ%2FNBufG1X9JyrtDMr0Y4xwokxlWX%2BPjAYdemhPrWeDvYcPJ8r0LO3v4oszNfivQQuTp2u9qJGKE2V6lvZ38UVj9q3t3oqEE2U2lvfXF4t6qPjTqDUV1fRyhw8nymws768vfOr2NtqOqFY4UUZE%2BusL6VDRX7%2FGzOHDiTIi0t9WMPsUKzNPx4kysf62gmuHir3sPXw4USbWny485ZOc2PsJ7VTro%2F3pwp5DxV7qHq2xa41TrY%2F2J7PfJkaHir3UwwdtU061PtqfTP0CUaYm2v3LxCtoDI2lMWk8p1of7Y8K0jhRJgaaYZwoE0P%2FpFUndZqtP6T4BE2zC5qtP6T4BE2zC5qtPyRN8OvhZUQae3ZBtT7anyb49PA6Ivp5wKnWR%2FvbJkncZXr6wokysf62CXRCWjmJxhqd2JwoE%2BuvTqS37JGJlB39GLzhRJmN5f31gz8XTpSJgWYYJ8rEQDOME2VioBnGiTIx0AzjRJkYaIZxokwMNMM4USYGmmGcKBMDzTBOlImBZhgnysRAM4wTZWKgGcaJMjHQDONEmRhohnGiTAw0wzhRJgaaYZwoEwPNME6UiYFmGCfKxEAzjBNlYqAZxokyMdAMoL%2FO%2BNi4bzjpT1e%2BNFb8V7gFzUXMLHqk%2BM1A8wArFj1S5GagOUly0SMtuxloTnJrUU%2B7QXOSW4t62g2ak9xa1NNu0Jzk1qKednK6%2Bw9roIB8keT%2F3QAAAABJRU5ErkJggg%3D%3D)](https://github.com/matkoch/TestLinker/blob/master/LICENSE)

TestLinker collects link data between types (i.e., production and test code) based on various mechanisms and provides various features based on that. For your convenience, TestLinker automatically takes base/derived types into account when meaningful.

## WARNING

This fork of the [TestLinker plugin](https://github.com/matkoch/resharper-testlinker) originally developed by Matthias Koch was created for personal use and does not guaranteed to be supported.

### Building Locally

- Please go though https://github.com/JetBrains/resharper-rider-plugin
- Once initial setup is done, plugin can be build using:

  ```bash
  ./gradlew buildPlugin
  ```

## Navigation

<img src=https://raw.githubusercontent.com/matkoch/TestLinker/master/misc/Demon_Navigate.gif />

- [Goto Related Files](https://www.jetbrains.com/help/resharper/2016.1/Navigation_and_Search__Go_to_Related_Files.html) is extended with navigation points to production/test classes.
- New shortcuts `ReSharper_GotoAllLinkedTypes` and `ReSharper_GotoLinkedTypesWithDerivedName` (assignable via keyboard options) that jumps between linked types. In case of multiple linked types, a dedicated popup menu is shown, which can also be displayed in [Find Results](https://www.jetbrains.com/help/resharper/2016.1/Reference__Windows__Find_Results_Window.html) window.

## Test Creation

<img src=https://raw.githubusercontent.com/matkoch/TestLinker/master/misc/Demo_Create.gif />

- Create production/test class if they don't exist
- Requires at least one matching pair of test and production class in the project

## Test Execution

- Tests can be executed from their linked production code. This feature automatically integrates with the shortcuts for executing unit tests in *run*, *debug*, *profile*, and *cover* mode.

## Configuration

Link data is currently maintained via:
- **Derived names**, as with `Calculator` and `CalculatorTest`. Pre-/Postfixes can be configured in the options page.
- **Usages of TypeofAttributes**, as in `[Subject (typeof (FirstComponent), typeof(SecondComponent)]`, which are applied to test classes. This custom attribute is especially useful for integration test and can be configured through the options page.

<img src=https://raw.githubusercontent.com/matkoch/TestLinker/master/misc/OptionsPage.png width=600px />
