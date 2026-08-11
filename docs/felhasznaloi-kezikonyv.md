# Felhasználói kézikönyv – Parkolóhely-foglalás Backend

## A rendszer elindítása

Előfeltétel: Docker Desktop telepítve és fut.

1. Klónozd le a repót
2. Nyiss egy terminált a repó gyökerében (ahol a docker-compose.yml található)
3. Futtasd a következő parancsot:

    docker-compose up --build

4. Várd meg, amíg megjelenik a "Now listening on: http://[::]:8080" sor - ekkor a rendszer készen áll
5. A rendszer induláskor automatikusan létrehozza az adatbázist, lefuttatja a migrációt, és feltölti 5 alap parkolóhellyel (A1, A2, A3, B1, B2)

## Az API kipróbálása

Nyisd meg a böngészőben: http://localhost:8080/swagger

Itt egy interaktív felület jelenik meg, ahol minden végpont kipróbálható:

1. Kattints a kívánt végpontra a listában, hogy kinyíljon
2. Kattints a "Try it out" gombra
3. Ha szükséges, töltsd ki a mezőket / a kérés body-t
4. Kattints "Execute"
5. Az eredmény (státuszkód + válasz) alul jelenik meg

## Tipikus felhasználási forgatókönyv

**1. Parkolóhelyek listázása**

GET /api/ParkingSpots - megmutatja az elérhető parkolóhelyeket (kód és aktív állapot).

**2. Foglalás létrehozása**

POST /api/Reservations - add meg, melyik parkolóhelyre (parkingSpotId), ki (requesterName) és milyen időszakra (startTime, endTime) szeretne foglalni. Ha a helyszín szabad az adott időszakban, a foglalás létrejön, és visszakapod az azonosítóját (id).

Ha a helyszín már foglalt abban az időszakban, a rendszer 400-as hibát ad, és leírja az okot.

**3. Egy parkolóhely foglalásainak megtekintése**

GET /api/ParkingSpots/{id}/reservations - megmutatja egy adott parkolóhely összes foglalását (lemondottakat is), időrendben.

**4. Foglalás lemondása**

DELETE /api/Reservations/{id} - a megadott azonosítójú foglalást lemondja. A lemondott foglalás a helyet szabaddá teszi más foglalások számára ugyanabban az időszakban.

## A rendszer leállítása

A terminálban, ahol fut: Ctrl+C, majd futtasd:

    docker-compose down

Ez leállítja és eltávolítja a konténereket. Az adatbázis tartalma nem marad meg a következő induláskor (friss seed adatokkal indul újra), mivel nincs named volume beállítva az adatok perzisztálására.
