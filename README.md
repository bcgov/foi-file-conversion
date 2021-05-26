# FOI File Conversion

## Introduction
FOI-File-Conversion is a backend process to convert Excel (.xls, .xlsx) and iCalendar(.ics) files to pdf so that it can be loaded to Intella for deduplication and export to pdf. Current Intella software is having issue while converting these problematic file type to pdf for further processing in AXIS. This is has been tackled using this FOI-File-Conversion background process.

This process include a File watcher, a processor for excel files and a processor for iCalendar files. File watcher will keep on watching the Shared LAN for problematic file types and the respective processor pick up the files based on the file type and convert to pdfs.

Note : As part of the development of FOI File conversion component, a third party tool/SDK called "File Formats" from Syncfusion has been utilized. This tool comes up with a paid license which need to be procured seperately in order to further utilize this code base with other projects.

# File Watcher and Conversion Flow

[Here is the File Watcher and Conversion Flow]()

# Architecture

Bring the architecture diagram here


# Execution & Deployments

Details on deployment and code run
## Prerequisites and System Requirements for Local execution

## Local deployment steps

# Code Repo/Solution structure

# Additional Information

Regarding Suncfusion licensing and unit test
## License
Syncfusion license details
## Unit test

Unit test details

