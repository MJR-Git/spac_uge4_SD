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
Der er ikke nogen direkte fejlhåntering. Der er forbyggende håntering af hvis der ikke bliver gevet en *excelPath* og *outputPath* med en specifik besked. Det samme bliver ikke gjordt for NumberOfRows da den ikke kaster en fejl hvis den ikke bliver funet men sat til 0, den håntere heller ikke hvis de er sat højere end der er rejker. File not found bliver dog ikke grebet og hånteret fra **ExcelParser**.  
Generelt burde disse exptions gribes og deres messages blive skrevet til en log.

# Testdækning og forbedringer

# Forbedringer

Overvej at bruge ```IEnumarable<T>``` de steder hvor en liste bliver brugt i et foreach loop ex: 
``` c#
public async Task DownloadPdfsAsync(IEnumerable<Models.PdfUrl> urlList, string downloadPath)
```
Det gør det muglit at bruge mage forskællige tybe samlinger

I did [project tree diagram](../README.md/#project-structure) bliver det vist fint som planetext men som formateret markdown kommer de på en linge, der er en måde at se fromateret makdown i vscode. Dette kan løses ved at putte 2 mellemrum efter hvær linge eller bruge html lige skift ```</br>``` ex på begge måder:
spac_uge4_SD/  
├── .gitignore  
├── README.md</br>
├── CshScript/</br>

## Models

### PdfUrl

equals er ikke overskrevet så simple ```.equals()``` virker ikke hvis man har det 2 ens elementer der komme 2 forksellige steder fra. Der bude komme en muglighed for at autogenerere basis verssionen hvis *equals* bliver skrevet på en linge i klassen.  

## Utilities

### ExcelParser
I exelparser burde der værre en måde hvor nullable verdier bliver sat til *null*

Hvor der bliver spunget rows over blev der brugt 500, dette springer de første 500 rows over istedet for kun den første hvor hedere er.

### PdfDownloader
I DownloadPdf laver null cheks hvor Url og AlternativeUrl vil aldrig værre null på den måde de blev instatieret da ```.ToString()``` retunrere ```""``` fra et tomt input.