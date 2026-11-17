# mSzczepienia

**Bridging the gap between 21st-century digital healthcare and 19th-century paper booklets.**

This repository contains the source code for **mSzczepienia**, a comprehensive system designed to digitize, verify, and manage historical vaccination records. It was developed as a Bachelor's Thesis project at the **Warsaw University of Technology** (Faculty of Mathematics and Information Science).

## The Problem

Despite the widespread digitalization of healthcare in Poland (e-prescriptions, the *mObywatel* app), the management of vaccination history - especially legacy childhood vaccines and travel medicine (the "Yellow Booklet") - remains heavily reliant on physical paper. Paper booklets are prone to loss, damage, and lack automated reminders, creating a "single point of failure" for patients and a verification nightmare for border controls.

## Our Solution: The "Human-in-the-loop" Approach

**mSzczepienia** doesn't just scan documents; it creates a verifiable chain of trust.
We know that medical handwriting is notoriously difficult for AI to read perfectly. Therefore, we implemented a **Human-in-the-loop (HITL)** architecture:

1. **Snap:** A citizen takes a photo of their paper booklet using our mobile app.
2. **Extract:** A multimodal LLM (`gpt-5-mini` on Azure OpenAI) reads the photo and returns structured entries (vaccine names, batch numbers, dates) as JSON.
3. **Verify:** A qualified healthcare worker reviews the AI's output alongside the original photo in a split-screen web interface.
4. **Certify:** Once approved, the record becomes a permanent, digital certificate in the patient's history.

### Why an LLM instead of classic OCR?

We compared two extractors: a custom-trained Azure AI Document Intelligence model and a multimodal LLM. Classic OCR transcribes characters, so a smudged or hurried entry comes out as a misspelled string. The LLM reads the entry in context: it recognizes vaccine names from a partial or misspelled scrawl and can sometimes infer which vaccine was meant when the name is barely legible. It recognized vaccine names noticeably better, so it became the final choice. Its output is never trusted blindly: every entry still goes through fuzzy matching against the official vaccine list and a healthcare worker's review.

---

## Key Features

### For Citizens (Mobile App - Flutter)

* **Digital History:** View all your verified vaccinations in one clean, Material Design interface.
* **OCR Uploads:** Easily upload photos of your old paper booklets for digitization.
* **Smart Reminders:** Get push notifications for upcoming booster doses or when your submission gets verified.

### For Healthcare Workers (Web App - Blazor)

* **Verification Dashboard:** A specialized split-screen UI to compare original photos with AI-extracted data.
* **Fuzzy Matching:** The system suggests official vaccine names even if the OCR misread the handwriting (e.g., *Pfiizer* → *Pfizer*).
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
* **AI & OCR:** `gpt-5-mini` on `Azure OpenAI` turns booklet photos into structured JSON entries. A custom `Azure AI Document Intelligence` model was evaluated as an alternative and is kept in the code (`AzureOcrService`).
* **Storage:** `Azure Blob Storage` with short-lived Shared Access Signatures (SAS) for secure medical image handling.
* **Deployment:** Dockerized backend hosted on `Azure Container Apps`.

---

## Project Structure

| Project | Description |
| :--- | :--- |
| `mSzczepienia.Api` | ASP.NET Core Web API: controllers, services, EF Core repositories and migrations, CSV seed data, a background reminder worker. |
| `mSzczepienia.Client` | Blazor WebAssembly app for healthcare workers and administrators (MudBlazor). |
| `mSzczepienia.Mobile` | Flutter app for citizens. |
| `mSzczepienia.Shared` | DTOs and enums shared by the API and the web client. |
| `mSzczepienia.Tests.Unit` | Unit tests (xUnit, NSubstitute, FluentAssertions). |
| `mSzczepienia.Tests.Integration` | API integration tests on `WebApplicationFactory` with an in-memory database and mocked external services. |
| `mSzczepienia.Tests.Client` | Blazor component tests (bUnit). |

---

## Getting Started (Local Development)

### Prerequisites

* [.NET 8 SDK](https://dotnet.microsoft.com/download)
* [Docker](https://www.docker.com/) (for SQL Server, or the whole backend)
* [Flutter SDK](https://docs.flutter.dev/get-started/install) (for the mobile app)
* Your own cloud resources; no keys are committed to this repository:
  * a **Firebase** project with Email/Password authentication (and Cloud Messaging for push notifications),
  * an **Azure OpenAI** deployment of a vision-capable chat model,
  * an **Azure Storage** account for the booklet photos.

### 1. Clone the repository

```bash
git clone https://github.com/p10tr13/mSzczepienia.git
cd mSzczepienia
```

### 2. Configure the API

Settings are read from `mSzczepienia.Api/appsettings*.json` and can be overridden with environment variables (`Section__Key`) or [user secrets](https://learn.microsoft.com/aspnet/core/security/app-secrets):

| Setting | What to put there |
| :--- | :--- |
| `ConnectionStrings:DefaultConnection` | SQL Server connection string. The default points at the local Docker container below. |
| `Firebase:ProjectId` | Your Firebase project ID. |
| `Firebase:ServiceAccountKeyPath` | Path to your Firebase service account key (JSON), used to send push notifications. |
| `AzureOpenAI:Endpoint`, `AzureOpenAI:ApiKey`, `AzureOpenAI:DeploymentName` | Your Azure OpenAI resource and the name of a vision-capable model deployment (`gpt-5-mini` by default). |
| `BlobStorage:ConnectionString`, `BlobStorage:ContainerName` | Your Azure Storage account and container for uploaded photos. |

Database migrations and seed data (diseases, vaccines, facilities) are applied automatically when the API starts.

### 3. Run the backend

Start only SQL Server and run the API from your IDE or the CLI:

```bash
docker compose up -d db
dotnet run --project mSzczepienia.Api
```

Or run the API and the database together with Docker Compose. It reads the cloud settings from environment variables, for example from a `.env` file next to `docker-compose.yml`:

```bash
DB_CONNECTION_STRING="Server=db,1433;Database=mSzczepieniaDB;User Id=sa;Password=YourStrong!Passw0rd;TrustServerCertificate=True"
BLOB_CONNECTION_STRING=...
AZURE_OPENAI_ENDPOINT=...
AZURE_OPENAI_APIKEY=...
AZURE_OPENAI_DEPLOYMENT_NAME=...
```

```bash
docker compose up --build
```

The API is then available at `http://localhost:8080`.

### 4. Run the web client

Put your Firebase **Web API key** in `mSzczepienia.Client/wwwroot/appsettings.Development.json` (`Firebase:ApiKey`) and check that `ApiUrl` points at your API, then:

```bash
dotnet run --project mSzczepienia.Client
```

### 5. Run the mobile app

Replace `mSzczepienia.Mobile/mszczepienia/android/app/google-services.json` with the file from your Firebase project (or run `flutterfire configure`), set `baseUrl` in `lib/core/network/dio_client.dart` to your API (`http://10.0.2.2:8080/api` reaches a local API from the Android emulator), then:

```bash
cd mSzczepienia.Mobile/mszczepienia
flutter pub get
flutter run
```

### Running the tests

```bash
dotnet test mSzczepienia.sln
```

All 512 tests (108 unit, 403 integration, 1 component) run without any cloud resources: the integration tests use an in-memory database, mocked OCR and storage services, and Firebase is not initialized in the `Testing` environment.

---

## Authors

Developed as a Bachelor's Thesis at Warsaw University of Technology (MiNI WUT) by:

* **Piotr Iśtok**
* **Piotr Jacak**
---

## License

[MIT](LICENSE)
