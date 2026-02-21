## 📺 YoutubeDownload - ASP.NET Core MVC

Web application built with ASP.NET Core MVC (.NET 8) for viewing and downloading YouTube video and audio streams using the [YoutubeExplode](https://github.com/Tyrrrz/YoutubeExplode) e o [FFmpeg](https://ffmpeg.org/).

The project currently provides a simple web interface where users can:

- Enter a YouTube video URL
- List available video and audio streams
- Download the selected format

> ⚠️ At the moment, the project is focused only on the Web (MVC) application.
> API support, Docker, and desktop applications may be added in future versions.
---

### 🧱 Technologies Used

- .NET 8
- ASP.NET Core MVC
- Razor Views
- YoutubeExplode
- FFmpeg

---

### ✅ Requirements

Before running the project, make sure you have installed:

- [.NET SDK 8.0](https://dotnet.microsoft.com/en-us/download/dotnet/8.0)
- FFmpeg installed on your machine and available in your system `PATH`

To verify that FFmpeg is correctly installed:

```bash
ffmpeg -version
```

---

### 🚀 Running the Application

From the project root directory, run:

`dotnet build`
`dotnet run`

After starting the application, open your browser and navigate to:

https://localhost:5001

or

http://localhost:5000

(The port may vary depending on your local configuration.)

---

### 📂 Project Structure

YoutubeDownload.Web
 ├── Controllers
 ├── Models
 ├── Views
 ├── Services
 └── wwwroot
 
- Responsibility Separation
- Controllers → Handle HTTP requests
- Models → ViewModels used by the UI
- Views → Razor-based UI layer
- Services → Integration with YoutubeExplode and FFmpeg

---

### 🔮 Future Improvements

- Separate REST API
- Clean Architecture (Domain / Application / Infrastructure layers)
- Download history tracking
- UI/UX improvements
- Improved error handling and logging

--

### ⚠️ Notes

YouTube may block requests depending on IP address or internal platform changes.
If you encounter a 403 Forbidden error, check:
Your YoutubeExplode version
Network configuration
Possible IP restrictions

---

### 📄 License

This project is licensed under the LGPL License **LGPL**.
See the [LICENSE](License.txt) file for more details.

--- 

### 👨‍💻 Desenvolvido por [Tiago Ávila Saldanha](https://github.com/tiago-saldanha) (YoutubeDownload)

---
