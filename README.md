## 📺 YoutubeDownloader — Multiplatform Application (.NET 9)

**YoutubeDownloader** is a cross-platform solution built with **Blazor Hybrid** and **.NET 9**. It offers a unified experience across **Web (Blazor)** and **Desktop (Windows via .NET MAUI)**, allowing users to view and download YouTube content using [YoutubeExplode](https://github.com/Tyrrrz/YoutubeExplode) and [FFmpeg](https://ffmpeg.org/).

The project uses a **Shared UI** strategy, where 100% of the interface and business logic is reused between the web and the native desktop application.

---

### 🎯 Key Features

- **Hybrid UI:** Modern, responsive interface built with Blazor components and **MudBlazor**.
- **Dual Mode Execution:**
  - **Web:** Downloads are processed on the server and sent to the browser.
  - **Desktop (MAUI):** Downloads are processed locally on the user's machine for maximum performance and privacy.
- **Smart Stream Selection:** Retrieve video and audio metadata to select specific formats, resolutions, and codecs.
- **Local Processing:** Integrated FFmpeg for merging high-quality video and audio streams.

---

### 🧱 Architecture & Technologies

- **.NET 9** (Latest LTS/Current features)
- **MAUI Blazor Hybrid:** For the native Windows desktop experience.
- **Blazor Web App:** For the server-side/client-side web experience.
- **Razor Class Library (SharedUI):** A single project containing all views and logic shared between Web and Desktop.
- **YoutubeExplode:** Core library for YouTube metadata and stream retrieval.
- **FFmpeg:** Backend engine for media conversion and muxing.
- **MudBlazor:** Component library for a polished Material Design UI.

---

### ✅ Requirements

Before running the project, ensure you have:

- [.NET SDK 9.0](https://dotnet.microsoft.com/download/dotnet/9.0)
- **Visual Studio 2022** with the ".NET Multi-platform App UI development" workload.
- **FFmpeg:** - For **Web**: Must be in the server's `PATH`.
  - For **Desktop**: Included in the `Infrastructure/lib` folder (configured as "Copy if newer").

---

### 🚀 Running the Application

#### 1. Desktop Application (MAUI)
Set `YoutubeDownloader.Desktop` as your startup project and run:
```bash
# Via CLI
dotnet build YoutubeDownloader.Desktop -c Debug
dotnet run --project YoutubeDownloader.Desktop
```

#### 2. Web Application
Set `YoutubeDownloader.Blazor` as your startup project:
```bash
# Via CLI
dotnet run --project YoutubeDownloader.Blazor
```

Access via: `https://localhost:7299`

---

### Example
<img width="1222" height="895" alt="image" src="https://github.com/user-attachments/assets/df7b6d93-607f-491c-9fa6-737db742f30a" />

---

### 🏗 Project Structure

- Core/Domain: Interfaces, ViewModels, and business rules.
- Infrastructure: FFmpeg service implementation, YoutubeExplode integration, and caching.
- SharedUI: All `.razor` components and CSS shared across platforms.
- Desktop: MAUI entry point and native service implementations (`IDeviceService`).
- Blazor: Web entry point and API Controllers for web-based downloads.

---

### ⚠️ Notes
YouTube may block requests based on IP or platform changes. If you encounter `403 Forbidden`:

- Keep `YoutubeExplode` updated to the latest version.
- In the Desktop app, ensure your local internet connection is stable.

---

### 📄 License

This project is licensed under the `LGPL License`.
See the [LICENSE](https://github.com/tiago-saldanha/YoutubeDownloader/blob/master/License.txt) file for details.
