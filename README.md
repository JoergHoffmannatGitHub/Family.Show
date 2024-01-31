# Family.Show

![.NET 8.0](https://img.shields.io/badge/Version-.NET%208.0-informational?style=flat&logo=dotnet)
&nbsp;
[![CI/CD](https://github.com/JoergHoffmannatGitHub/Family.Show/actions/workflows/ci-cd.yml/badge.svg)](https://github.com/JoergHoffmannatGitHub/Family.Show/actions/workflows/ci-cd.yml)
&nbsp;
[![GitHub release badge](https://badgen.net/github/release/JoergHoffmannatGitHub/Family.Show/stable)](https://github.com/JoergHoffmannatGitHub/Family.Show/releases/latest)
&nbsp;
![GitHub License](https://img.shields.io/github/license/JoergHoffmannatGitHub/Family.Show)
<!-- ![Windows badge](misc/badge-windows.svg) -->

This C# project is intend to continue [Family.Show](https://github.com/JoergHoffmannatGitHub/CodePlex_FamilyShow) originally hosted on [http://familyshow.codeplex.com/](http://web.archive.org/web/20170504160027/http://familyshow.codeplex.com/) by Vertigo.

This project is contiued as [My Family Tree](https://chronoplexsoftware.com/myfamilytree/) from Chronoplex Software.

The starting point is the latest version [4.5](https://github.com/JoergHoffmannatGitHub/CodePlex_FamilyShow/tree/main/4.0).

## Overview

For a hobby that revolves around dead people, genealogy is remarkably popular: it's the fastest growing scene in North America. And a perfect study for our first Windows Presentation Foundation (WPF) reference application.

![Alt text](./FamilyShowDetail-1.jpg?raw=true)

Family.Show is a Windows Presentation Foundation (WPF) reference application. Vertigo Software collaborated with Microsoft to build Family.Show as a tool for developers to use as a reference for building their own applications using WPF. In addition, Family.Show is intended to be a functional application that genealogy enthusiasts and everyday users can use to build their family trees and family histories.

A detailed discussion [Family.Show - A WPF Reference Application](https://github.com/JoergHoffmannatGitHub/CodePlex_FamilyShow/blob/main/Family.Show%20-%20A%20WPF%20Reference%20Application.pdf)

The aim of this project is the enhancment with actual technologies:

- Unit tests
- CI/CD
- NuGet pakages
- Installer
- Domain model architecture
- Performance
- Integrate help (XPS or CHM, translation)
- Additional formats (GEDCOM 5.5.1, GEDCOM 7, Gramps)

### Requirements

Family.Show runs on Microsoft Windows 10. To run the application on Microsoft Windows without .NET 8, you must first download and install version 8.0 of .NET 8. The framework can be downloaded at https://dotnet.microsoft.com/en-us/download/dotnet/8.0.

## Example

Family.Show contains two sample files (a GEDCOM file and a Family.Show file) which can be accessed in the "Sample Files" folder of the source directory in FamilyShow\Sample Files. These files demonstrate some of the special features in Family.Show.

When you begin your first tree, try hovering the mouse over icons. Often there is a tooltip which provides additional information. Sometimes extra options are accessible via right click menus.

## File Formats

Family.Show supports the following file formats.

### Family.Show Files (*.familyx and *.family)

This is an custom XML based file format which packages the family tree details, photos, stories and attachments in a single compressed archive.

Note that unlike a GEDCOM file, any attached files, photos etc are in packaged in the file rather than being simple resource links. This is incredibly useful for sharing trees between users, so long as they are also using Family.Show.

In Version 2 of Family.Show, the *.family file format was used. Like a GEDCOM file, photos and stories were included as resource links. All files opened in this format will be converted to the new *.familyx format.

### GEDCOM Files (*.ged)

GEDCOM is a widely used format in other family tree software. Family.Show has support for the many common fields of GEDCOM version 5.5 including:

- People (INDI [Name, Surname, Suffix) including Restriction (RESN) tags.
- Events (BIRT [Date, Place], DEAT [Date, Place], MARR [Date, Place], DIV [Date], OCCU [Description], EDUC [Description], RELI [Description])
- Citations, sources, repositories and notes.
- Photos when the GEDCOM file references a local file.

**Extended characters, other languages and GEDCOM files**

Family.Show has full support for the UTF-8 encoding format. Unfortunately most GEDCOM files are not encoded in this format so during import, Family.Show removes characters which it may not be able to display. If you use a language which has extended character sets, some information may be lost. If the GEDCOM file has been exported from another genealogy program, there may be an option to export in UTF-8 format. If UTF-8 is not available, the next best choice is ANSI which is a subset of UTF-8. Alternatively, it is sometimes possible to convert files to UTF-8 encoding using programs such as Notepad. To do this, open the *.ged in Notepad and click Save As from the File Menu. At the bottom of the save dialogue, there is an option to select Encoding. Choose UTF-8 to encode the file in UTF-8 format ensuring the file extension remains as *.ged (see Figure 1). Once your file is correctly encoded, check 'Enable UTF-8 support' for full extended character support.

**Notes**

- Restriction tags (RESN) are imported. Supported restrictions are Locked and Privacy.
- Family.Show has a dedicated field "IsLiving" for recording if a person is alive or not. GEDCOM has no such equivalent field, instead relying on DEAT and BIRT tags to provide this information. Often, DEAT tags are missing if nothing is known of the individuals death so during import, it is not possible to determine the IsLiving status. In this case, the rule used is that a person with no DEAT tag is living unless their age is greater than 90. In the few cases of people being older than this, users can simply update the record manually.
- Often approximate dates are present in GEDCOM files. On import, Family.Show will attempt to analyse dates which are not in a standard format. Dates which are about (ABT), before (BEF) or after (AFT) are supported. Sometimes quarterly information is in a GEDCOM DATE tag e.g. Jan-Feb-Mar 2009. In this case, ABT 01/01/2009 will be recorded. Given that there are so many possible ways of recording date information, inevitably, some information may be lost in transfer. 
- Family.Show does not fully support the GEDCOM standard. If you import a GEDCOM file with additional fields this information will not be transferred.

## Support

You can consult https://github.com/JoergHoffmannatGitHub/Family.Show to view more information about Family.Show.
If you believe you have discovered a bug, please report it on the [Issues](https://github.com/JoergHoffmannatGitHub/Family.Show/issues) forums.
