## 📺 YoutubeDownload — Blazor Application (.NET 8)

**YoutubeDownload** is a .NET application built with **Blazor (.NET 8)** for viewing and downloading YouTube video and audio streams using [YoutubeExplode](https://github.com/Tyrrrz/YoutubeExplode) and [FFmpeg](https://ffmpeg.org/).

The project currently runs as a **Blazor-based application**, providing a modern, component-based UI and serving as the foundation for future **desktop integration using .NET MAUI (Blazor Hybrid)**.

---

### 🎯 Current Features

The application allows users to:

- Enter a YouTube video URL
- Retrieve video and audio stream information
- Select the desired format and quality
- Download video or audio using FFmpeg

The UI is fully built with **Blazor components**, enabling reuse across Web and Desktop platforms.

---

### 🧱 Technologies Used

- .NET 8
- Blazor
- Razor Components
- YoutubeExplode
- FFmpeg

---

### ✅ Requirements

Before running the project, ensure you have the following installed:

- [.NET SDK 8.0](https://dotnet.microsoft.com/en-us/download/dotnet/8.0)
- FFmpeg installed and available in your system `PATH`

Verify FFmpeg installation:

```bash
ffmpeg -version
```

---

### 🚀 Running the Application

From the project root directory:
```bash
dotnet build
dotnet run
```

Then open your browser and navigate to:
```bash
https://localhost:5001
```

or
```bash
https://localhost:5001
```
>⚠️ The port may vary depending on your local environment configuration.

---

### 🧭 Project Direction & Next Steps
The project is currently focused on the Blazor UI layer, with the following planned improvements:

- Integration with .NET MAUI (Blazor Hybrid) for a native desktop application
- Clean Architecture separation:
- Core (Domain & Application)
- Infrastructure
- UI (Blazor / MAUI)
- Native filesystem access via MAUI
- Improved download progress tracking
- Background downloads
- UI/UX refinements
- Better error handling and logging

The goal is to reuse the same Blazor components and business logic across Web and Desktop environments.

---

### ⚠️ Notes

YouTube may block requests depending on IP address or internal platform changes.

If you encounter a `403 Forbidden` error, check:

- Your YoutubeExplode version
- Network configuration
- Possible IP or rate-limiting restrictions

---

📄 License

This project is licensed under the LGPL License.
See the [LICENSE](https://github.com/tiago-saldanha/YoutubeDownload/blob/master/License.txt) file for more details.

---

👨‍💻 Developed by

[Tiago Ávila Saldanha](https://github.com/tiago-saldanha)














