# AbySalto - Tehnički zadatak – Junior Developer
REST API za upravljanje narudžbama restorana, izgrađen na ASP.NET Core 9.0.

## Tehnologije

- **ASP.NET Core 9.0** - Web framework
- **Entity Framework Core 9.0** - ORM za pristup bazi podataka
- **SQL Server** - Baza podataka
- **FluentValidation** - Validacija ulaznih podataka
- **IMemoryCache** - Caching
- **Swagger** - Dokumentacija i testiranje API-a (alternativno, aplikacija dolazi s definiranim HTTP zahtjevima u datoteci `AbySalto.Junior/AbySalto.Junior.http` koji se mogu pokrenuti direktno iz Visual Studia za testiranje.)
- **NUnit + Moq** - Unit i integracijski testovi

## Arhitektura

Projekt koristi slojevitu arhitekturu:
```
Controller → IService → Service → IRepository → Repository → Baza podataka
```

- **Controller** - Prima HTTP zahtjeve, prosljeđuje ih servisu
- **Service** - Poslovna logika, caching, logging
- **Repository** - Pristup bazi podataka putem Entity Frameworka
- **Middleware** - Globalno centralizirano hvatanje iznimki
- **DTOs** - Odvajanje API modela od domenskih modela
- **FluentValidation** - Validacija ulaznih podataka

## Pokretanje aplikacije

### Preduvjeti

- .NET 9.0 SDK
- SQL Server
- Visual Studio 2022

### Koraci

1. Kloniraj repozitorij:
```bash
git clone https://github.com/jgasi/abysalto-junior-task.git
```

2. Postavi connection string u `AbySalto.Junior/appsettings.Development.json` prema svom lokalnom SQL Serveru:
```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=VAŠE_IME_SERVERA;Database=AbySalto;Trusted_Connection=True;TrustServerCertificate=True;"
  }
}
```

> Baza podataka **AbySalto** će se automatski kreirati pri prvom pokretanju aplikacije — nije potrebno ručno kreirati bazu niti pokretati migracije.

3. Pokreni aplikaciju bez debuggera s **Ctrl+F5** u Visual Studiu (pokretanje bez debuggera kako bi centralizirani middleware za iznimke mogao raditi bez prekida).

4. Swagger UI se otvara automatski na:
```
https://localhost:7056
```

## API Endpointi

| Method | Endpoint | Opis |
|--------|----------|------|
| `GET` | `/api/restaurant/orders` | Dohvati sve narudžbe |
| `GET` | `/api/restaurant/orders?sortByTotal=true` | Dohvati narudžbe sortirane po ukupnom iznosu |
| `GET` | `/api/restaurant/orders/{id}` | Dohvati narudžbu po ID-u |
| `POST` | `/api/restaurant/orders` | Kreiraj novu narudžbu |
| `PATCH` | `/api/restaurant/orders/{id}/status` | Promijeni status narudžbe |

### Statusi narudžbe

| Vrijednost | Opis |
|------------|------|
| `0` | Pending (na čekanju) |
| `1` | InPreparation (u pripremi) |
| `2` | Completed (završena) |

### Primjer kreiranja narudžbe (POST)
```json
{
  "customerName": "Ivan Horvat",
  "paymentMethod": "Cash",
  "deliveryAddress": "Ilica 1, Zagreb",
  "contactNumber": "0911234567",
  "note": "Bez luka",
  "currency": "EUR",
  "items": [
    {
      "name": "Pizza Margherita",
      "quantity": 2,
      "price": 8.50
    },
    {
      "name": "Cola",
      "quantity": 1,
      "price": 2.00
    }
  ]
}
```

### Primjer promjene statusa (PATCH)
```json
{
  "status": 1
}
```

## Testovi

Projekt sadrži unit i integracijske testove u projektu `AbySalto.Junior.Tests`.

Pokretanje testova:

View -> Test Explorer -> Run All Tests In View

### Unit testovi (OrderServiceTests)
Testiraju Service sloj s mockiranim dependencyima:
- Kreiranje narudžbe
- Dohvat narudžbe po ID-u
- Bacanje iznimke za nepostojeći ID
- Zabrana promjene statusa završene narudžbe
- Promjena statusa narudžbe

### Integracijski testovi (OrdersControllerTests)
Testiraju Controller + Service + Repository + InMemory bazu zajedno:
- Kreiranje narudžbe
- Dohvat svih narudžbi
- Bacanje iznimke za nepostojeći ID
- Sortiranje narudžbi po ukupnom iznosu
- Promjena statusa narudžbe
