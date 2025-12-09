# 🛡️ Safety Equipment Control System

A modern Desktop Application for managing safety equipment inventory, developed with **C#** and **Windows Forms**.

## 📸 Project Overview
This project modernizes the traditional Windows Forms interface by implementing a custom **"Flat Design"** system programmatically (Apple-inspired UI). It moves beyond simple file storage by implementing a robust **Enterprise Data Layer** using **Microsoft SQL Server** and **Entity Framework Core**.

## ✨ Key Features
- **Modern UI/UX:** Custom flat design with Emoji iconography, clean layout, and responsive anchors.
- **Enterprise Database:** Data persistence managed via **SQL Server** (CRUD operations).
- **ORM Integration:** Uses **Entity Framework Core** for efficient database interaction.
- **Smart Logic:** Automatic status calculation (Good/Not Good) based on specific equipment validity rules.
- **Access Control:** Secure Login screen with input validation.
- **Search & Filter:** Real-time filtering capability.

## 🚀 How to Run this Project
To run this application on your machine, you need to set up the local database first:

1. **Clone** this repository to your computer.
2. **Setup Database:**
   - Open the file `database_setup.sql` included in this repository.
   - Copy the script and run it in **SQL Server Management Studio (SSMS)**.
   - This will create the `SafetyDB` database and the necessary tables automatically.
3. **Run the App:**
   - Open `Safety Equipment Control.sln` in Visual Studio.
   - Press **F5** to build and run.

## 🛠️ Tech Stack
- **Language:** C#
- **Framework:** .NET Framework 4.7.2 (Windows Forms)
- **Database:** Microsoft SQL Server Express
- **ORM:** Entity Framework Core 3.1
- **Architecture:** N-Tier style (UI, Logic, Data)
