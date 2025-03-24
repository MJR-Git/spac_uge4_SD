# Resultater

# Kodekvalitet

## Kode orginitation

## Kode kommentare

Godt beskrevet i metoderne  
Kunne eventuelt skrive dokument kommentare, burte komme hvis der bliver skvevet /// lige over en metode.   
Kunne ligene noget som dette:
``` c#
/// <summary>
///  Initiates the download of PDFs from a list of URLs, ensuring resilience and concurrency control.
/// </summary>
/// <param name="urlList">List of urls to be downloaded</param>
/// <param name="downloadPath">Where the files shuld be plased</param>
/// <returns></returns>
public async Task DownloadPdfsAsync(List<Models.PdfUrl> urlList, string downloadPath)
{...}
```

## Fejl hånternig

Sten og glashus  
Der er ikke nogen direkte fejlhåntering. Der er forbyggende håntering af hvis der ikke bliver gevet en *excelPath* og *outputPath* med en specifik besked. Det samme bliver ikke gjordt for NumberOfRows da den ikke kaster en fejl hvis den ikke bliver funet men sat til 0. File not found bliver dog ikke grebet og hånteret fra **ExcelParser**.  
Generelt burde disse exptions gribes og deres messages blive skrevet til en log.

# Testdækning og forbedringer

# Forbedringer

## Models

### PdfUrl

equals er ikke overskrevet så simple ```.equals()``` virker ikke hvis man har det 2 ens elementer der komme 2 forksellige steder fra. Der bude komme en muglighed for at autogenerere basis verssionen hvis *equals* bliver skrevet på en linge i klassen.  

## Utilities

### ExcelParser

Hvor der bliver spunget rows over blev der brugt 500, dette springer de første 500 rows over istedet for kun den første hvor hedere er.
