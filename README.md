# GetDownloadedDumbass (GDD)

Un téléchargeur de médias moderne en C# WPF, propulsé par `yt-dlp`.

---

## Fonctionnalités

- Téléchargement de vidéos et audio depuis plus de 1000 sites (YouTube, TikTok, SoundCloud, Twitch...)
- Choix du format de sortie : `mp4`, `mkv`, `mp3`, `flac`, `wav`, `m4a`
- Choix de la qualité : `best`, `1080p`, `720p`, `480p`, `360p`
- File d'attente multi-URLs
- Historique des téléchargements sauvegardé en JSON
- Dossier de sortie configurable
- Possibilité d'arrêter un téléchargement en cours
- Interface graphique moderne avec palette rouge/orange

---

## Prérequis

- [.NET 10 SDK](https://dotnet.microsoft.com/download)
- [yt-dlp](https://github.com/yt-dlp/yt-dlp/releases) — télécharger `yt-dlp.exe`
- [ffmpeg](https://ffmpeg.org/download.html) — télécharger `ffmpeg.exe`

---

## Installation

1. Cloner le dépôt :
```bash
git clone https://github.com/ton-user/GetDownloadedDumbass.git
cd GetDownloadedDumbass
```

2. Placer les binaires dans le dossier `Assets/` :
```
Assets/
├── yt-dlp.exe
└── ffmpeg.exe
```

> Ces fichiers ne sont pas inclus dans le dépôt et doivent être ajoutés manuellement.

3. Restaurer les dépendances et lancer :
```bash
dotnet run
```

---

## Utilisation

1. Coller une URL dans le champ en haut
2. Choisir le format et la qualité
3. Cliquer sur **+ ADD TO QUEUE**
4. Répéter pour ajouter plusieurs URLs
5. Choisir le dossier de sortie via **Browse**
6. Cliquer sur **START DOWNLOAD**
7. Suivre la progression dans la section **STATUS**
8. L'historique des téléchargements s'affiche automatiquement en bas

---

## Stack technique

| Composant | Technologie |
|-----------|-------------|
| Interface graphique | WPF (.NET 10) |
| Téléchargement | yt-dlp (via `Process`) |
| Conversion audio/vidéo | ffmpeg |
| Sérialisation JSON | Newtonsoft.Json |
| Pattern architectural | MVVM |

---

## Structure du projet

```
GDD/
├── Assets/             # yt-dlp.exe et ffmpeg.exe (non versionnés)
├── Models/
│   ├── AppSettings.cs  # Modèle des paramètres
│   └── DownloadItem.cs # Modèle d'un téléchargement
├── Services/
│   ├── DownloaderService.cs  # Appel yt-dlp
│   ├── HistoryService.cs     # Lecture/écriture historique JSON
│   └── SettingsService.cs    # Lecture/écriture options JSON
├── ViewModels/
│   └── MainViewModel.cs      # Logique MVVM centrale
├── Views/
│   └── MainWindow.xaml       # Interface graphique
└── README.md
```

---

## Fichiers ignorés (non versionnés)

Les fichiers suivants sont exclus du dépôt git :

- `Assets/yt-dlp.exe` — binaire trop lourd
- `Assets/ffmpeg.exe` — binaire trop lourd
- `options.json` — paramètres locaux
- `history.json` — historique local
- `bin/`, `obj/` — fichiers de build

---

## Sites supportés

yt-dlp supporte plus de 1000 sites, dont notamment :
YouTube, TikTok, Twitter/X, Instagram, Twitch, Vimeo, SoundCloud, Bandcamp, Dailymotion, Reddit, Facebook, et bien d'autres.

Liste complète : `yt-dlp --list-extractors`