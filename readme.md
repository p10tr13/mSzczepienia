# mSzczepienia

**Bridging the gap between 21st-century digital healthcare and 19th-century paper booklets.**

This repository contains the source code for **mSzczepienia**, a comprehensive system designed to digitize, verify, and manage historical vaccination records. It was developed as a Bachelor's Thesis project at the **Warsaw University of Technology** (Faculty of Mathematics and Information Science).

## The Problem

Despite the widespread digitalization of healthcare in Poland (e-prescriptions, the *mObywatel* app), the management of vaccination history - especially legacy childhood vaccines and travel medicine (the "Yellow Booklet") - remains heavily reliant on physical paper. Paper booklets are prone to loss, damage, and lack automated reminders, creating a "single point of failure" for patients and a verification nightmare for border controls.

## Our Solution: The "Human-in-the-loop" Approach

**mSzczepienia** doesn't just scan documents; it creates a verifiable chain of trust.
We know that medical handwriting is notoriously difficult for AI to read perfectly. Therefore, we implemented a **Human-in-the-loop (HITL)** architecture:

1. **Snap:** A citizen takes a photo of their paper booklet using our mobile app.
2. **Extract:** Azure AI Vision extracts text (dates, batch numbers, vaccine names) using OCR.
3. **Verify:** A qualified healthcare worker reviews the AI's output alongside the original photo in a split-screen web interface.
4. **Certify:** Once approved, the record becomes a permanent, digital certificate in the patient's history.

---

## Key Features

### For Citizens (Mobile App - Flutter)

* **Digital History:** View all your verified vaccinations in one clean, Material Design interface.
* **OCR Uploads:** Easily upload photos of your old paper booklets for digitization.
* **Smart Reminders:** Get push notifications for upcoming booster doses or when your submission gets verified.

### For Healthcare Workers (Web App - Blazor)

* **Verification Dashboard:** A specialized split-screen UI to compare original photos with AI-extracted data.
* **Fuzzy Matching:** The system suggests official vaccine names even if the OCR misread the handwriting (e.g., *Pfiizer*  *Pfizer*).
* **Direct Entry:** Add new vaccinations directly to a patient's profile immediately after administering a shot.

### For Administrators (Web App - Blazor)

* **Global Dictionaries:** Manage the official list of EU-approved vaccines (seeded from EMA data).
* **Facility Management:** Register medical facilities and manage personnel accounts.
* **Analytics:** Monitor system usage and OCR rejection rates via interactive charts.

---

## Tech Stack & Architecture

The system is built on a modern, cloud-native **N-Layer architecture**:

* **Backend:** `.NET 8` (Web API) following Domain-Driven Design principles.
* **Mobile Client:** `Flutter` (Dart), targeted for Android 15.0+.
* **Web Client:** `Blazor WebAssembly` with MudBlazor components.
* **Database:** `Azure SQL Database` accessed via Entity Framework Core.
* **Identity Management:** `Firebase Authentication` (JWT based stateless auth).
* **AI & OCR:** `Azure AI Vision` for document text extraction.
* **Storage:** `Azure Blob Storage` with short-lived Shared Access Signatures (SAS) for secure medical image handling.
* **Deployment:** Dockerized backend hosted on `Azure Container Apps`.

---

## Getting Started (Local Development)

To run this project locally, you will need:

* [.NET 8 SDK](https://dotnet.microsoft.com/download)
* [Flutter SDK](https://flutter.dev/docs/get-started/install) (for the mobile app)
* A SQL Server instance (local or cloud)
* API Keys for **Firebase** and **Azure AI Vision**.

### Quick Start

1. Clone the repository:

2. Configure your `appsettings.Development.json` in the API project with your Database Connection String and Azure/Firebase keys.
3. Apply database migrations:
```bash
dotnet ef database update
```


4. Run the API and Web Client via Visual Studio / VS Code.
5. Run the Flutter app on an emulator or physical device:
```bash
cd mSzczepienia.Mobile
flutter run
```

---

## Authors

Developed as a Bachelor's Thesis at Warsaw University of Technology (MiNI WUT) by:

* **Piotr Iśtok**
* **Piotr Jacak**