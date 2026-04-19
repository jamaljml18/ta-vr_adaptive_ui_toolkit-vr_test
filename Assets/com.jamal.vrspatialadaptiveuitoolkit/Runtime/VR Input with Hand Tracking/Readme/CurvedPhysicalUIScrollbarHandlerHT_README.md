# CurvedPhysicalUIScrollbarHandlerHT

## 🇬🇧 English Documentation

### Overview

`CurvedPhysicalUIScrollbarHandlerHT` is a script designed to handle VR interactions for Canvas UI elements—specifically curved interfaces—using a **"Physical Proxy"** approach for **Scrollbars**, completely optimized for **Hand Tracking (Auto-Hover)**.

Unlike physical controllers where a user presses a trigger to "grab" a scrollbar, bare-hand pinch gestures can sometimes be tiring for continuous scrolling. This script uses an **Auto-Hover** architecture. You do not need to pinch or click. By simply pointing your hand laser at the invisible 3D Box Collider and moving your hand along its track, the script calculates your laser's position on the local X-axis in real-time and instantly updates the UI Scrollbar (0.0 to 1.0), effectively scrolling the content with just a gaze/point.

### Features

- **Pinch-Free Auto-Hover Interaction:** The scrollbar updates continuously the moment the VR laser touches the collider. No holding, grabbing, or pinching required.
- **Hand Tracking Optimized:** Built specifically for effortless bare-hand navigation in XR environments.
- **Real-Time Update Loop:** Constantly polls the `TryGetCurrent3DRaycastHit` of both Interactors every frame for seamless, instant responsiveness.
- **Local Math Calculation:** Uses inverse transform math to ensure accurate scrolling even if the Canvas is rotated, scaled, or curved.
- **Array-Based Indexing:** Easily link multiple colliders and scrollbars by matching their array indices.
- **Editor Visualization:** Draws blue and red debug lines in the Scene view to visualize the scrollbar's track, moving direction, and bounds, making setup much easier.

### How to Use

1. **Prepare the UI:** Create your World Space Canvas, Scroll View, and UI Scrollbars.
2. **Create Proxies:** Create empty GameObjects with `BoxCollider` components. Position and stretch the collider's X-axis so it perfectly covers the interactive "track" of your UI Scrollbar.
3. **Attach Script:** Attach the `CurvedPhysicalUIScrollbarHandlerHT` script to a manager object (e.g., the Canvas itself).
4. **Map Arrays:** - Add your 3D Colliders to the `Scrollbar Colliders` array.
   - Add your corresponding UI Scrollbars to the `Canvas Scrollbars` array.
   - _Important:_ Ensure Index 0 in Colliders matches Index 0 in Scrollbars.
5. **Assign VR Settings:** Drag and drop your Left and Right `XRRayInteractor` components. (Notice: Input Action References are no longer needed for this Auto-Hover version).

### Scrollbar Orientation Guide

The system calculates values based on the Collider's **Local X-Axis**. UI Scrollbars are often vertical. To define the sliding direction, apply the following rotations to the Collider GameObject in the Inspector:

| Slide Direction             | Rotation (X, Y, Z) | Description                                               |
| :-------------------------- | :----------------- | :-------------------------------------------------------- |
| **Left to Right** (Default) | `(0, 0, 0)`        | Standard Horizontal                                       |
| **Right to Left**           | `(0, 180, 0)`      | Inverted Horizontal                                       |
| **Bottom to Top**           | `(0, 0, 90)`       | Standard Vertical                                         |
| **Top to Bottom**           | `(0, 0, -90)`      | Inverted Vertical (Most common for reading top-to-bottom) |

> **Note:** Ensure the Collider's `Size X` matches the visual length of the UI scrollbar track. The script uses this Size X to map the 0.0 to 1.0 value.

### Example Scenario

**Use Case:** You have an Inventory Menu with a vertical `Inventory_Scrollbar`.

1. You create a transparent 3D cube named `Collider_InventoryTrack`. You stretch its width (X-axis) to cover the entire length of the scrollbar visually. Because it is a vertical scrollbar, you rotate the `Collider_InventoryTrack` by `(0, 0, -90)`.
2. In the Inspector for this script:
   - **Size:** Set to `1`.
   - **Index 0:** Place `Collider_InventoryTrack` in the Collider array, and `Inventory_Scrollbar` in the Scrollbar array.
3. When the player points their VR laser at the top of the `Collider_InventoryTrack`, the inventory instantly snaps to the top. As they simply move their hand downward (without pinching), the Scroll View moves smoothly in real-time.

---

---

## 🇮🇩 Dokumentasi Bahasa Indonesia

### Ikhtisar

`CurvedPhysicalUIScrollbarHandlerHT` adalah skrip yang dirancang untuk menangani interaksi VR pada elemen Canvas UI—khususnya antarmuka yang melengkung (curved)—menggunakan pendekatan **"Physical Proxy"** khusus untuk **Scrollbar**, dan dioptimalkan sepenuhnya untuk **Hand Tracking (Auto-Hover)**.

Berbeda dengan kontroler fisik di mana pengguna menekan pelatuk untuk "menggenggam" scrollbar, gestur mencubit (pinch) secara terus-menerus dengan tangan kosong terkadang bisa melelahkan. Skrip ini menggunakan arsitektur **Auto-Hover**. Anda tidak perlu mencubit atau mengklik. Hanya dengan mengarahkan laser dari tangan Anda ke Box Collider 3D dan menggerakkan tangan di sepanjang jalurnya, skrip akan menghitung posisi laser pada sumbu X lokal secara real-time dan langsung memperbarui Scrollbar UI (0.0 hingga 1.0). Konten akan bergulir hanya dengan sorotan tangan.

### Fitur Utama

- **Interaksi Auto-Hover Tanpa Cubit:** Scrollbar diperbarui secara terus-menerus tepat saat laser VR menyentuh collider. Tidak perlu menahan, menggenggam, atau mencubit.
- **Optimasi Hand Tracking:** Dibangun khusus untuk navigasi tangan kosong yang ringan dan tanpa beban di lingkungan XR.
- **Loop Pembaruan Real-Time:** Secara konstan menanyakan `TryGetCurrent3DRaycastHit` dari kedua Interactor setiap _frame_ untuk responsivitas instan.
- **Kalkulasi Matematika Lokal:** Menggunakan konversi ruang koordinat (_inverse transform_) untuk memastikan gulungan tetap akurat meskipun Canvas diputar, diskalakan, atau dilengkungkan.
- **Indeks Berbasis Array:** Memudahkan pemetaan banyak scrollbar sekaligus hanya dengan menyamakan urutan indeks pada array.
- **Visualisasi Editor:** Menampilkan garis penghubung berwarna biru dan merah di Scene view untuk memvisualisasikan jalur dan batas scrollbar, sehingga mempermudah proses penataan.

### Cara Penggunaan

1. **Siapkan UI:** Buat Canvas (World Space), Scroll View, dan komponen UI Scrollbar Anda.
2. **Buat Proxy:** Buat GameObject kosong yang memiliki komponen `BoxCollider`. Atur posisi dan rentangkan sumbu X collider tersebut agar menutupi seluruh "jalur" (track) dari Scrollbar UI Anda secara presisi.
3. **Pasang Skrip:** Pasangkan skrip `CurvedPhysicalUIScrollbarHandlerHT` pada objek manajer (misalnya, di Canvas itu sendiri).
4. **Petakan Array:**
   - Masukkan Collider 3D yang telah direntangkan ke dalam array `Scrollbar Colliders`.
   - Masukkan komponen UI Scrollbar yang bersangkutan ke dalam array `Canvas Scrollbars`.
   - _Penting:_ Pastikan Indeks 0 pada Collider terhubung dengan Indeks 0 pada Scrollbar.
5. **Atur Pengaturan VR:** Masukkan referensi `XRRayInteractor` Kiri dan Kanan. (Perhatian: Referensi Input Action tidak lagi diperlukan untuk versi Auto-Hover ini).

### Panduan Orientasi Scrollbar

Sistem menghitung nilai pergeseran berdasarkan **Sumbu X Lokal (Local X-Axis)** dari Collider. Karena Scrollbar UI sering kali berbentuk vertikal, terapkan rotasi berikut pada GameObject Collider Anda di Inspector untuk menyesuaikan arah geseran:

| Arah Geseran               | Rotasi (X, Y, Z) | Keterangan                                                       |
| :------------------------- | :--------------- | :--------------------------------------------------------------- |
| **Kiri ke Kanan** (Bawaan) | `(0, 0, 0)`      | Horizontal Standar                                               |
| **Kanan ke Kiri**          | `(0, 180, 0)`    | Horizontal Terbalik                                              |
| **Bawah ke Atas**          | `(0, 0, 90)`     | Vertikal Standar                                                 |
| **Atas ke Bawah**          | `(0, 0, -90)`    | Vertikal Terbalik (Paling umum untuk membaca dari atas ke bawah) |

> **Catatan:** Pastikan nilai `Size X` pada Collider sama persis dengan panjang jalur UI secara visual. Skrip ini menggunakan nilai Size X tersebut untuk memetakan rentang nilai 0.0 hingga 1.0.

### Contoh Skenario Penggunaan

**Kasus:** Anda memiliki Menu Inventaris dengan `Scrollbar_Inventaris` vertikal.

1. Anda membuat kubus 3D transparan bernama `Collider_JalurInventaris`. Anda merentangkan lebarnya (sumbu X) agar menutupi seluruh panjang visual scrollbar. Karena ini scrollbar vertikal, Anda memutar `Collider_JalurInventaris` sebesar `(0, 0, -90)`.
2. Pada Inspector skrip ini:
   - **Size:** Isi dengan angka `1`.
   - **Indeks 0:** Masukkan `Collider_JalurInventaris` di array Collider, dan `Scrollbar_Inventaris` di array Scrollbar.
3. Saat pemain mengarahkan laser VR mereka ke ujung atas `Collider_JalurInventaris`, inventaris langsung melompat ke paling atas. Saat mereka cukup menggeser tangan ke bawah (tanpa perlu mencubit), isi menu Scroll View akan bergulir ke bawah secara mulus dan real-time.

---

### Inspector Variables Reference

| Variable Name        | Type              | Description                                                                                                                                  |
| :------------------- | :---------------- | :------------------------------------------------------------------------------------------------------------------------------------------- |
| `scrollbarColliders` | `BoxCollider[]`   | Array of 3D Box Colliders acting as the scrollbar tracks. / _Array Box Collider 3D yang bertindak sebagai jalur scrollbar._                  |
| `canvasScrollbars`   | `Scrollbar[]`     | Array of Canvas Scrollbars. Must match the order of the colliders. / _Array Scrollbar UI. Urutannya harus sama dengan collider._             |
| `leftRayInteractor`  | `XRRayInteractor` | Reference to the Left Hand Ray Interactor. / _Referensi ke Ray Interactor Kiri._                                                             |
| `rightRayInteractor` | `XRRayInteractor` | Reference to the Right Hand Ray Interactor. / _Referensi ke Ray Interactor Kanan._                                                           |
| `showDebug`          | `bool`            | Toggles debug logs in the console (useful to see value changes). / _Menyalakan log debug di konsol (berguna untuk melihat perubahan nilai)._ |
