# Getting Started — ClassicPvP

This guide walks you through setting up your Asheron's Call client and DATs to connect to the ClassicPvP server.

---

## Server Info

| Field | Value |
|-------|-------|
| **URL** | doctide.online |
| **Port** | 9000 |
| **Name** | Classic PvP |
| **Type** | ACE |

---

## Setup Instructions

### Step 1 — Download the Client Files

Download the `.7z` file from:
**[mega.nz/folder/xi4jiKjJ#jpuTVa7CQYyNxyp-UHC_GA](https://mega.nz/folder/xi4jiKjJ#jpuTVa7CQYyNxyp-UHC_GA)**

### Step 2 — Unzip

Unzip the downloaded file using **7-Zip** (a free utility — google it if you don't have it installed).

### Step 3 — Copy Your Asheron's Call Folder

Navigate to `C:\Turbine` and make a copy of your existing `Asheron's Call` folder. Name the copy **ClassicPvP**.

Your folder structure should look like this:
```
C:\Turbine\
    Asheron's Call\     ← your original (keep this intact)
    ClassicPvP\         ← your new copy
```

### Step 4 — Replace the DAT and Client Files

Copy the DAT and client (`.exe`) files you unzipped in Step 2 and paste them into `C:\Turbine\ClassicPvP`, overwriting the existing files when prompted.

### Step 5 — Configure Thwarg Launcher

Open **Thwarg Launcher**. At the bottom, you'll see the file path to your client — by default it reads:

```
C:\Turbine\Asheron's Call\acclient.exe
```

Click the **three dots (...)** next to that path and navigate to:

```
C:\Turbine\ClassicPvP\acclient_Infiltration.exe
```

### Step 6 — Log In

You're all set. Launch the client through Thwarg Launcher and log in to **Classic PvP**.

---

## Switching Between Servers

If you also play on an End of Retail server like **Doctide**, you'll need to swap the client path in Thwarg Launcher depending on which server you want to connect to.

| Server | Client Path |
|--------|-------------|
| **Classic PvP** | `C:\Turbine\ClassicPvP\acclient_Infiltration.exe` |
| **Doctide (EoR)** | `C:\Turbine\Asheron's Call\acclient.exe` |

Click the three dots in Thwarg Launcher to switch between them.

---

*For server rules, systems, and season info, see the [Release Notes](ReleaseNotes.md).*
