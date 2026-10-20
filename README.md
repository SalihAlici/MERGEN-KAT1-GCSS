# MERGEN-KAT1-GCSS

Ground Control Station Software for model satellite and CubeSat missions. It decodes telemetry received from the satellite over a serial port and displays it in charts, a data table, a map and a 3D model. It logs data to CSV, sends commands to the satellite and records the camera feed.

Written in C# with Windows Forms (.NET Framework 4.7.2). The user interface is in Turkish.

![TEKNOFEST 2025 Category 1 version](docs/teknofest2025_kat1.png)
*TEKNOFEST 2025 Category 1 version. Values shown are sample data.*

## Versions

The same codebase was adapted to the requirements of different competitions in separate branches. The `master` branch contains the first interface design. The completed versions are:

| Branch | Mission | Telemetry packet |
|---|---|---|
| [`KAT1-YİST`](../../tree/KAT1-Y%C4%B0ST) | TEKNOFEST 2025 Model Satellite Competition, Category 1 | 22 fields, `*`-separated |
| [`AZ`](../../tree/AZ) | CubeSat competition in Azerbaijan | 15 fields, `,`-separated |
| [`T3`](../../tree/T3) | T3 Foundation satellite program (MERSAT) | 16 fields, `,`-separated |

## Common features

- **Serial connection (9600 baud):**
  - Reading and processing run on separate threads, so the interface stays responsive.
  - Packets start with `$` and end with `#`. Corrupt or incomplete packets are discarded; the buffer is cleared if it overflows.
- **Charts and logging:** telemetry is plotted over time. Every packet is added to the data table and appended to a CSV file on the desktop.
- **Map:** the satellite position and its track are drawn with GMap.NET; invalid coordinates are ignored.
- **Command panel:**
  - Send a message to the satellite (`C` + message).
  - Lock / unlock commands (`B` / `A`).
- **Camera:** live video with automatic MP4 recording to the desktop (AForge / Accord).

## Differences between versions

### TEKNOFEST 2025, Category 1 (`KAT1-YİST`)
- **Telemetry:**
  - Packet number, satellite status, error code, transmission time.
  - Payload and carrier pressure and altitude, altitude difference, descent rate.
  - Temperature, battery voltage, GPS, pitch / roll / yaw, RHRH, IoT S1–S2, team number.
- **Charts:** 8 time charts. Log file `TELEMETRİ.csv`. OpenStreetMap map.
- **3D simulation:** the satellite model rotates with the received angles (OpenTK); the model changes after separation (5th bit of the error code).
- **ARAS (fault detection) indicator:** one light per error-code bit. Faulty bits turn red with an audible alert.

```
$packet*status*error_code*time*pressure1*pressure2*altitude1*altitude2*altitude_diff*descent_rate*temperature*battery*
 gps_lat*gps_lon*gps_alt*pitch*roll*yaw*RHRH*iot_s1*iot_s2*team_no#
```

### Azerbaijan CubeSat (`AZ`)
- **Telemetry:**
  - Team ID, packet number, time.
  - Pressure, relative altitude, descent rate, battery voltage.
  - GPS, pitch / roll / yaw, separation status, encrypted message.
- **Charts:** 4 charts (relative altitude, pressure, descent rate, battery). Log file `MERGEN.csv`. Google Hybrid map.
- **Extra commands:** **EEPROM** (`E`) and **OFFSET** (`R`).
- **Camera:** connected cameras are listed and one can be selected.

![Azerbaijan CubeSat version](docs/azerbaycan_cubesat.png)

```
$team_id,packet,time,pressure,relative_altitude,descent_rate,battery,gps_lat,gps_lon,gps_alt,pitch,roll,yaw,separation,encrypted_message#
```

### T3 Foundation, MERSAT (`T3`)
- **Telemetry:**
  - Packet number, satellite status, error code, transmission time.
  - Pressure, altitude, descent rate, temperature, battery voltage.
  - GPS, pitch / roll / yaw, team number.
- **Charts:** 5 charts. Log file `TELEMETRİ.csv`. Google Hybrid map.
- **3D simulation:** rewritten with HelixToolkit (WPF), with smoothed angle transitions.
- **ARAS indicator.**
- **Extra commands:** **EEPROM** (`E`) and **RESET** (`R`).
- **Camera:** connected cameras are listed and one can be selected.

![T3 Foundation MERSAT version](docs/t3_mersat.png)

```
$packet,status,error_code,time,pressure,altitude,descent_rate,temperature,battery,gps_lat,gps_lon,gps_alt,pitch,roll,yaw,team_no#
```

## Files (in the completed versions)

| File | Contents |
|---|---|
| `Form1.cs` | Main window, port, camera and command buttons |
| `Data.cs` | Serial reading, packet framing, processing queue, sending commands |
| `TelemetryData.cs` | Packet parser |
| `Chart.cs` | Charts |
| `DataGridViewHandler.cs` | Data table and CSV logging |
| `Map.cs` | Map and track |
| `_3DSimulation.cs` | 3D satellite model |
| `Aras.cs` | Error-code indicator |
| `Camera.cs` | Camera feed and recording |

## Build and run

**Requirements:**
- Windows 10 / 11
- Visual Studio 2019 or 2022 with the ".NET desktop development" workload (including .NET Framework 4.7.2)
- Internet connection for the map
- USB camera for video (optional)

**Steps:**
1. Clone the repository and check out the version you need:
   ```
   git clone https://github.com/SalihAlici/MERGEN-KAT1-GCSS.git
   cd MERGEN-KAT1-GCSS
   git checkout T3          # or KAT1-YİST, AZ
   ```
2. Open `MERGEN-KAT1-GCSS.sln` in Visual Studio.
3. Restore NuGet packages: right-click the solution → *Restore NuGet Packages* (they may also be restored on the first build).
4. Build and run (F5).
5. Select the payload port from the list and press the connect button. The port opens at 9600 baud.
6. To use the camera, select it and press the camera button. Recordings are saved to the desktop as MP4.

Telemetry is logged to a CSV file on the desktop.

## Libraries

GMap.NET (map), OpenTK and HelixToolkit (3D), AForge / Accord.Video (camera and recording), System.Windows.Forms.DataVisualization (charts), CircularProgressBar.

## Author

[SalihAlici](https://github.com/SalihAlici)
