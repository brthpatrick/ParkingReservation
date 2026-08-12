# Rendszerterv – Parkolóhely-foglalás Backend

## Áttekintés

A rendszer egy parkolóhely-foglalási backend API, amely lehetővé teszi parkolóhelyek nyilvántartását, foglalási kérések kezelését (ütközés-ellenőrzéssel), lekérdezést és lemondást.

## Architektúra

A megoldás Clean Architecture elvek szerint, rétegzett felépítéssel készült:

    ParkingReservation.Api            <- REST kontrollerek, Swagger, DI konfiguráció
    ParkingReservation.Infrastructure <- EF Core DbContext, repository megvalósítás, seed
    ParkingReservation.Application    <- Üzleti logika (ReservationService), interfészek, DTO-k
    ParkingReservation.Domain         <- Entitások (ParkingSpot, Reservation), enumok
    ParkingReservation.Tests          <- Unit tesztek

A függőségi irány: **Api → Infrastructure → Application → Domain**. A Domain réteg semmilyen külső csomagtól nem függ, az Application réteg csak a Domain-től függ és nem ismeri az EF Core-t (a perzisztenciát az IParkingReservationRepository interfész absztrahálja).

## Adatmodell

**ParkingSpot**
- Id (int, PK)
- Code (string, egyedi, pl. "A1")
- IsActive (bool)
- Type (enum: Standard, Disabled, ElectricCharging)

**Reservation**
- Id (int, PK)
- ParkingSpotId (int, FK -> ParkingSpot)
- RequesterName (string)
- StartTime, EndTime (DateTime)
- Status (enum: Confirmed, Cancelled)
- CreatedAt (DateTime)

## Üzleti logika

A foglalás létrehozásának lépései (ReservationService.CreateReservationAsync):
1. Alap validáció: EndTime > StartTime
2. A parkolóhely létezik és aktív
3. Típus-specifikus szabályok (lásd lentebb)
4. Ütközés-ellenőrzés: nincs másik Confirmed státuszú foglalás a helyen, amely időben átfed (existing.Start < new.End AND existing.End > new.Start)
5. Ha minden feltétel teljesül, a foglalás létrejön Confirmed státusszal

Lemondás (CancelReservationAsync): a foglalás státusza Cancelled-re vált, nem törlődik fizikailag (auditálhatóság miatt).

## Parkolóhely típusok (opcionális bővítés)

A parkolóhelyek nem egyformák, ezt a Type mező tükrözi, és ez ténylegesen befolyásolja a foglalási logikát:

- **Standard**: nincs extra szabály, bárki foglalhatja
- **Disabled** (mozgáskorlátozott hely): csak akkor foglalható, ha a kérésben a HasDisabilityPermit mező true - egyéb esetben a rendszer elutasítja
- **ElectricCharging** (elektromos töltős hely): egy foglalás időtartama nem haladhatja meg a 4 órát, hogy a hely gyakrabban felszabaduljon más elektromos autók számára

## Adatbázis és induló állapot

MS SQL Server, EF Core Code-First migrációval. Induláskor (Program.cs) a rendszer automatikusan lefuttatja a migrációt (Database.Migrate()), majd seedeli az adatbázist 5 alap parkolóhellyel (A1-A3: Standard, B1: Disabled, B2: ElectricCharging), ha még nincs adat.

## Futtatás

A teljes rendszer (API + SQL Server) egy paranccsal indítható Docker Compose-szal:

    docker-compose up --build

Az API a 8080-as porton, a Swagger UI a /swagger útvonalon érhető el.

## Tesztelés

A ParkingReservation.Tests projekt xUnit + Moq + FluentAssertions alapú unit teszteket tartalmaz a ReservationService-hez, lefedve a validációs szabályokat, az ütközés-ellenőrzést és a lemondási logikát.

## Teljesítmény megfontolások

Nincs konkrét terhelési célszám megadva. Az ütközés-ellenőrzés adatbázis szinten (SQL lekérdezéssel) történik, nem memóriában betöltött adatokon, hogy skálázódjon nagyobb foglalás-számmal is. A ParkingSpots.Code mezőn egyedi index van, ami gyors keresést tesz lehetővé kód alapján.