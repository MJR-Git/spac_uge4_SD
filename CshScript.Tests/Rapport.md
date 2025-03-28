# Kørning af testsne
Run ```dotnet restore``` then ```dotnet test``` in the tearminal at the project root


# Kodekvalitet

## Kode orginitation
Fint organiceret, jeg ville nok have klat Models mappen Entitys men begge er valide

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
  
Der er ikke nogen direkte fejlhåntering. Der er forbyggende håntering af hvis der ikke bliver gevet en *excelPath* og *outputPath* med en specifik besked. Det samme bliver ikke gjordt for NumberOfRows da den ikke kaster en fejl hvis den ikke bliver funet men sat til 0. File not found bliver dog ikke grebet og hånteret fra **ExcelParser**.  
Generelt burde disse exptions gribes og deres messages blive skrevet til en log.  

Der hvor der er catch bloks bliverder græbet den generele Exeption istedet for at girbe enden grupper af exeptions, så som IOExeption, eller specifike, så som FileNotFoundException.


# Testdækning
Dækker alt untalen catch blocken i TryDownloadPdf i PdfDownloader og Program filen.  
Ender med 82% for hele projektet og 99% af Utilites.  


# Forbedringer

Overvej at bruge ```IEnumarable<T>``` de steder hvor en liste bliver brugt i et foreach loop ex: 
``` c#
public async Task DownloadPdfsAsync(IEnumerable<Models.PdfUrl> urlList, string downloadPath)
```
Hvis lingen ovenover ville ikke ændre programmet men gørre det muglit at bruge mage forskællige tybe samlinger som input, ex. hvis man ville værre sikker på at der ikke var nogen dubletter kunne man burge et ```Set<T>``` istedet for en ```List<T>```. Dette gør det også mugligt at burge yield keywordet for at retunere en salming inde fra en for/foreach løkke ex:  
``` c#
public IEnumarable<foo> bar(ICollection someCollection){
    foreach(element in someCollection){
        yield return element.name
    }
}
```
Dette er dog ikke rigig relevant for dette pogram da alle foreach løgger der retunere et nyt set af elementer bliver kørt inde i et ```Parallel.ForEach``` hvor det ikke er mugligt at burge yield keywordet.

I dit [project tree diagram](../README.md/#project-structure) bliver det vist fint som planetext men som formateret markdown kommer de på en linge, der er en måde at se fromateret makdown i vscode. Dette kan løses ved at putte 2 mellemrum efter hvær linge eller bruge html lige skift ```</br>``` ex på begge måder:
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
I DownloadPdf laver null cheks hvor Url og AlternativeUrl vil aldrig værre null på den måde de blev instatieret da ```.ToString()``` retunrere ```""``` fra et tomt input. Dette kan ses ved at kørre **DownloadOnlyPdfs** tæsten som debug med et brackepoint her:
``` c#
if (url.Url != null)
{
    /* Brakepoint */ if (await TryDownloadPdf(url.Url, pdfPath, client, pipeline)) 
    {
        url.Downloaded = true;
        return;
    }
}
```
Og kigge efter den *PdfUrl* med Brnummer: BR50968