# GTFS to XLSX (and vice versa)

Converts a [GTFS Static][gtfs-static] feed file into a Microsoft Excel workbook for convenient manual editing. Also converts an Excel workbook back to a GTFS Static feed file.

[gtfs-static]: https://developers.google.com/transit/gtfs

## Quickstart

Download the [latest release][releases] for your operating system and architecture.

Convert to Microsoft Excel workbook

```bash
GtfsXlsxCli --from "MyFeed.zip" --to "MyFeed.xlsx"
```

Convert to GTFS Static feed file

```bash
GtfsXlsxCli --from "MyFeed.xlsx" --to "MyFeed.zip"
```

Get description of all command line arguments

```bash
GtfsXlsxCli --help
```

[releases]: https://github.com/thzinc/gtfs-xlsx/releases

## Building

The project can be loaded with VSCode or Visual Studio for local development.

To build and package for each supported OS/architecture:

```bash
make package
```

## Code of Conduct

We are committed to fostering an open and welcoming environment. Please read our [code of conduct](CODE_OF_CONDUCT.md) before participating in or contributing to this project.

## Contributing

We welcome contributions and collaboration on this project. Please read our [contributor's guide](CONTRIBUTING.md) to understand how best to work with us.

## License and Authors

[![Daniel James logo](https://secure.gravatar.com/avatar/eaeac922b9f3cc9fd18cb9629b9e79f6.png?size=16) Daniel James](https://github.com/thzinc)

[![license](https://img.shields.io/github/license/thzinc/gtfs-xlsx.svg)](https://github.com/thzinc/gtfs-xlsx/blob/master/LICENSE)
[![GitHub contributors](https://img.shields.io/github/contributors/thzinc/gtfs-xlsx.svg)](https://github.com/thzinc/gtfs-xlsx/graphs/contributors)

This software is made available by Daniel James under the MIT license.