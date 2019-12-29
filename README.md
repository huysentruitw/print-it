# print-it

Windows service for printing PDF files to a local or network printer in the background.

This project uses the [PDFium library](https://pdfium.googlesource.com/) for rendering the PDF file which is licensed under Apache 2.0, see [LICENSE](pdfium-binary/LICENSE).

### API

By default, _PrintIt.ServiceHost_ is listening on http://localhost:7500. The endpoint is configurable in _appsettings.json_.

#### [GET] /api/printers/list

List all available printers on the system.

#### [POST] /api/printers/install?printerPath=\\\\REMOTE_PC_NAME\\PRINTER-NAME

Install the network printer with the UNC-path `\\REMOTE_PC_NAME\PRINTER-NAME`. 
