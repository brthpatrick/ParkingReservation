# API-leírás – Parkolóhely-foglalás Backend

Base URL (lokálisan, Docker Compose-ból indítva): http://localhost:8080

Interaktív dokumentáció és tesztelés: http://localhost:8080/swagger

## Parkolóhelyek

### GET /api/ParkingSpots

Visszaadja az összes parkolóhelyet.

**Válasz (200 OK):**
```json
[
  { "id": 1, "code": "A1", "isActive": true, "type": "Standard" },
  { "id": 4, "code": "B1", "isActive": true, "type": "Disabled" },
  { "id": 5, "code": "B2", "isActive": true, "type": "ElectricCharging" }
]
```

### GET /api/ParkingSpots/{id}/reservations

Visszaadja egy adott parkolóhely összes foglalását (Confirmed és Cancelled státuszút is), időrendben.

**Válasz (200 OK):**
```json
[
  {
    "id": 1,
    "parkingSpotId": 1,
    "parkingSpotCode": "A1",
    "requesterName": "Teszt Elek",
    "startTime": "2026-08-20T10:00:00",
    "endTime": "2026-08-20T12:00:00",
    "status": "Confirmed"
  }
]
```

**Válasz (404 Not Found):** ha a megadott id-jű parkolóhely nem létezik.

## Foglalások

### POST /api/Reservations

Új foglalás létrehozása. A rendszer ellenőrzi az időintervallum érvényességét, a parkolóhely létezését/aktív státuszát, a típus-specifikus szabályokat, és hogy nincs-e időbeli átfedés egy már meglévő aktív foglalással.

**Kérés body:**
```json
{
  "parkingSpotId": 1,
  "requesterName": "Teszt Elek",
  "startTime": "2026-08-20T10:00:00",
  "endTime": "2026-08-20T12:00:00",
  "hasDisabilityPermit": false
}
```

A hasDisabilityPermit mező csak Disabled típusú parkolóhelynél releváns, egyéb esetben figyelmen kívül van hagyva.

**Válasz (201 Created):**
```json
{
  "id": 1,
  "parkingSpotId": 1,
  "parkingSpotCode": "A1",
  "requesterName": "Teszt Elek",
  "startTime": "2026-08-20T10:00:00",
  "endTime": "2026-08-20T12:00:00",
  "status": "Confirmed"
}
```

**Válasz (400 Bad Request):** ha a kérés érvénytelen. Lehetséges okok:
- "A záró időpontnak a kezdő időpont után kell lennie."
- "A megadott parkolóhely nem létezik."
- "A megadott parkolóhely jelenleg nem foglalható."
- "Ez a parkolóhely mozgáskorlátozottak számára van fenntartva, érvényes igazolvány szükséges a foglaláshoz."
- "Elektromos töltős parkolóhely egyszerre maximum 4 órára foglalható, a nagyobb kihasználtság érdekében."
- "A parkolóhely a megadott időszakban már foglalt."

### DELETE /api/Reservations/{id}

Egy foglalás lemondása. A foglalás nem törlődik fizikailag, hanem Cancelled státuszt kap.

**Válasz (204 No Content):** sikeres lemondás esetén.

**Válasz (400 Bad Request):**
- "A foglalás nem található."
- "A foglalás már le van mondva."