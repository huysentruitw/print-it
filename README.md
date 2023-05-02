# print-it

Windows service for printing PDF files to a local or network printer in the background.

## Usage instructions

1. Download the [latest release](https://github.com/huysentruitw/print-it/releases/latest)
2. Extract the package to f.e. C:\print-it
3. Create print-it as a Windows service from an elevated command line: `sc create "PrintIt" binPath="C:\print-it\PrintIt.ServiceHost.exe" start=auto`
4. Start the service from the command line: `sc start PrintIt`
5. Check if the service is listening on port 7000 by running: `netstat -a | find ":7000"`

## API

By default, _PrintIt.ServiceHost_ is listening on http://localhost:7000. The endpoint is configurable in _appsettings.json_.

#### [GET] /printers/list

List all available printers on the system.

#### [GET] /printers/paperSources?printerPath=\\\\REMOTE_PC_NAME\\PRINTER-NAME

List all paper sources(trays) on the printer with the UNC-path `\\REMOTE_PC_NAME\PRINTER-NAME`.

#### [GET] /printers/paperSizes?printerPath=\\\\REMOTE_PC_NAME\\PRINTER-NAME

List all paper sizes on the printer with the UNC-path `\\REMOTE_PC_NAME\PRINTER-NAME`.

#### [POST] /printers/install?printerPath=\\\\REMOTE_PC_NAME\\PRINTER-NAME

Install the network printer with the UNC-path `\\REMOTE_PC_NAME\PRINTER-NAME`. 

#### [POST] /print/from-pdf

To print a PDF on a given printer, post a multipart form to this end-point with the following fields:

Field Name   | Required           | Content
------------ | ------------------ | ---------
PdfFile      | :heavy_check_mark: | The PDF file to print (Content-type: application/pdf)
PrinterPath  | :heavy_check_mark: | The UNC-path of the printer to send the PDF to
PageRange    |                    | An optional page range string (f.e. "1-5", "1, 3", "1, 4-8", "2-", "-5")
Copies       |                    | An optional number of copies (defaults to 1)
PaperSource  |                    | An optional name of the page source. See GET for valid page sources
PaperSize    |                    | An optional name of the page size. See GET for valid page sizes

## PDFium

This project uses the [PDFium library](https://pdfium.googlesource.com/) for rendering the PDF file which is licensed under Apache 2.0, see [LICENSE](pdfium-binary/LICENSE).

The version included in this repository under the folder `pdfium-binary` was taken from https://github.com/bblanchon/pdfium-binaries/releases/tag/chromium/4194.

