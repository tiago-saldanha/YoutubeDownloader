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
- **Intelligent Caching (NEW):**
  - **Video Metadata Cache:** Cached for **60 minutes** to avoid redundant requests.
  - **Stream Manifest Cache:** Cached for **10 minutes** to balance freshness and performance.
  - Significantly reduces calls to YouTube, improving performance and avoiding rate limits.

---

### 🧱 Architecture & Technologies

- **.NET 9** (Latest features)
- **MAUI Blazor Hybrid:** For the native Windows desktop experience.
- **Blazor Web App:** For the server-side/client-side web experience.
- **Razor Class Library (SharedUI):** A single project containing all views and logic shared between Web and Desktop.
- **YoutubeExplode:** Core library for YouTube metadata and stream retrieval.
- **FFmpeg:** Backend engine for media conversion and muxing.
- **MudBlazor:** Component library for a polished Material Design UI.
- **IMemoryCache:** In-memory caching with TTL for optimized request handling.

---

### ✅ Requirements

Before running the project, ensure you have:

- [.NET SDK 9.0](https://dotnet.microsoft.com/download/dotnet/9.0)
- **Visual Studio 2022** with the ".NET Multi-platform App UI development" workload.
- **FFmpeg:**  
  - For **Web**: Must be in the server's `PATH`.  
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

Set `YoutubeDownloader.Web` as your startup project:
```bash
# Via CLI
dotnet run --project YoutubeDownloader.Blazor
```

#### Example

<img width="1222" height="895" alt="image" src="https://github.com/user-attachments/assets/df7b6d93-607f-491c-9fa6-737db742f30a" />

---

### 🏗 Project Structure
- Core/Domain: Interfaces, ViewModels, and business rules.
- Infrastructure: FFmpeg service implementation, YoutubeExplode integration, caching, and HTTP handling.
- SharedUI: All `.razor` components and CSS shared across platforms.
- Desktop: MAUI entry point and native service implementations (`IDeviceService`).
- Blazor: Web entry point and API Controllers for web-based downloads.
- 

---

### ⚠️ Notes
- YouTube may apply rate limits or temporary blocks based on request patterns.
- The application mitigates this using caching, but high request volumes may still trigger limits.
- If you encounter 403 Forbidden or RequestLimitExceeded:
    - Keep `YoutubeExplode` updated to the latest version.
    - Avoid excessive parallel requests.
    - Ensure a stable internet connection (especially for Desktop mode).

---

### 📄 License 
This project is licensed under the LGPL License. See the [LICENSE](https://github.com/tiago-saldanha/YoutubeDownloader/blob/master/License.txt) file for details.



