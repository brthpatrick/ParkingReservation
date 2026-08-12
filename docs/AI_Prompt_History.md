# AI Prompt History – Parkolóhely-foglalás backend

## Megjegyzés a dokumentumról

A fejlesztés során Claude-ot használtam segítőként, elsősorban tervezési javaslatok adására és kódvázlatok generálására. A tényleges munka viszont nem másolás volt: minden generált fájlt magam hoztam létre (mappastruktúra, fájlok létrehozása PowerShell-ből/Visual Studio-ból), lefuttattam, teszteltem, és ahol hibát vagy hiányosságot találtam, azt vissza kellett jeleznem és javíttatnom, vagy saját magamnak kellett kijavítanom. Az architektúrával, technológiai döntésekkel és a beadási követelményekkel kapcsolatos végső döntések is tőlem származnak, tehát az AI csak javaslatokat adott, a választás és a jóváhagyás minden esetben az én feladatom volt.

---

## 1. Követelményelemzés

**Prompt:**
"Itt a Parkolóhely-foglalás backend házi feladat kiírása. Elemezd a teljes dokumentumot, és válaszd szét a kötelező funkciókat, opcionális funkciókat, technikai és beadási követelményeket."

**Saját közreműködés:** Végigolvastam a kiírást, és az AI által készített listát összevetettem az eredeti dokumentummal, hogy semmi ne maradjon ki vagy legyen félreértelmezve.

---

## 2. Technológiai stack és architektúra tervezése

**Prompt:**
"Van tapasztalatom C#, ASP.NET Core, Entity Framework Core és SQL Server területén. Ajánlj egy stacket a parkolóhely-foglalási backendhez, ami reálisan megvalósítható 3-4 óra alatt, Dockerrel futtatható, és nem overengineered."

**Saját közreműködés:** A javaslatot (C#/.NET + EF Core + MS SQL Server, réteges architektúra) a saját tapasztalatom alapján fogadtam el, kifejezetten azért, mert ezekkel a technológiákkal már dolgoztam korábban. Elutasítottam a Repository Pattern / CQRS bevezetését, mert feleslegesen bonyolította volna a projektet ehhez a mérethez.

---

## 3. Adatmodell és foglalási logika tervezése

**Prompt:**
"Tervezd meg az adatmodellt: parkolóhely, foglalás, kérelmező, kezdő/záró időpont. Legyen megoldás az időintervallum-átfedések kiszűrésére (teljes átfedés, részleges átfedés, egymás után következő foglalások stb.)."

**Saját közreműködés:** Az átfedés-ellenőrző logikát végigkövettem eset-eset alapján (teljes átfedés, részleges átfedés, azonos kezdő/záró idő, nulla hosszúságú intervallum), és külön rákérdeztem, mi történik konkurens kérések esetén, mert ezt hiányosnak találtam az első verzióban.

---

## 4. Implementáció – Domain és Application réteg

**Promptok:**
- "Hozzuk létre a Domain réteget: entitások, alap validációs szabályok."
- "Hozzuk létre az Application réteget: interfészek, DTO-k, ReservationService a foglalási üzleti logikával."

**Saját közreműködés:** Minden fájlt magam hoztam létre Visual Studio-ban, lefordítottam, és a build hibákat magam javítottam ki vagy jeleztem vissza, ha a generált kód nem illeszkedett a projekt aktuális állapotához (pl. névtér-eltérések, hiányzó using-ok).

---

## 5. Implementáció – Infrastructure és Api réteg

**Promptok:**
- "Implementáljuk az Infrastructure réteget: EF Core DbContext, repository, seed adatok."
- "Hozzuk létre az Api projektet: kontrollerek, Swagger konfiguráció, appsettings."

**Saját közreműködés:** A migrációkat és a seedelést magam futtattam le és ellenőriztem SQL Server ellen. A Swagger felületen manuálisan teszteltem végig az endpointokat, mielőtt továbbmentünk volna a következő fázisra.

---

## 6. Docker környezet

**Prompt:**
"Állítsuk össze a Dockerfile-t és a docker-compose.yml-t úgy, hogy egy `docker compose up --build` paranccsal induljon az API és az MS SQL adatbázis is, migrációval és seeddel együtt."

**Saját közreműködés:** A `docker compose up --build` parancsot ténylegesen lefuttattam, és amikor első nekifutásra a konténer nem találta el az adatbázist (connection string / hálózati probléma), ezt visszajeleztem, és közösen javítottuk ki, amíg éles környezetben is stabilan futott a migráció, a seed és az ütközés-ellenőrzés.

---

## 7. Tesztelés

**Prompt:**
"Írjunk unit teszteket a ReservationService üzleti logikájára xUnit, Moq és FluentAssertions használatával – fedjük le a sikeres foglalást, ütköző foglalást, hibás időintervallumot, nem létező parkolóhelyet stb."

**Saját közreműködés:** A teszteket lefuttattam Visual Studio-ban, ellenőriztem, hogy valóban 8/8 zölden fut, és átgondoltam, hogy a lefedett esetek (sikeres foglalás, ütköző foglalás, hibás intervallum, nem létező parkolóhely stb.) valóban lefedik-e a kiírásban kért teszteseteket.

---

## Összegzés

Az AI-t segítőként használtam a projekt minden fázisában: tervezési javaslatokat kértem, kódvázlatokat generáltattam. Ugyanakkor minden fájlt magam hoztam létre és futtattam, a hibákat (build hibák, Docker hálózati probléma) magam vettem észre és javíttattam vagy javítottam ki, és minden érdemi technikai és architekturális döntést én hagytam jóvá, a saját tapasztalatom alapján.
