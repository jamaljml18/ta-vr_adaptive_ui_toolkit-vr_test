# CurvedPhysicalUIScrollbarHandler

## 🇬🇧 English Documentation

### Overview

`CurvedPhysicalUIScrollviewHandler` (acting as the Scrollbar Handler) is a script designed to handle VR interactions for Canvas UI elements—specifically curved interfaces—using a **"Physical Proxy"** approach for **Scrollbars** (commonly used in Scroll Views).

A physics-based interaction system maps BoxCollider proxies to UI elements (Buttons, Toggles, Sliders, Scrollbars). This ensures precise raycast detection regardless of the visual mesh curvature or texture resolution. Similar to sliders, a scrollbar requires continuous tracking. This script allows players to **press, hold, and drag** their VR laser across an invisible 3D Box Collider. The script calculates the laser's position along the collider's local X-axis in real-time and dynamically updates the corresponding UI Scrollbar's value (0.0 to 1.0), effectively scrolling the connected content.

### Features

- **Hold and Drag Interaction:** Continuously updates the scrollbar value as long as the VR trigger is held down and the laser remains on the collider.
- **Dual Wielding Safe:** Tracks the `activeInteractor` (the specific hand that started the drag) to ensure the other hand doesn't accidentally interfere or cancel the scrolling interaction.
- **Local Math Calculation:** Uses inverse transform math to ensure accurate scrolling even if the Canvas is rotated, scaled, or curved.
- **Array-Based Indexing:** Easily link multiple colliders and scrollbars by matching their array indices.
- **Editor Visualization:** Draws blue and red debug lines in the Scene view to visualize the scrollbar's track, moving direction, and bounds, making setup much easier.

### How to Use

1. **Prepare the UI:** Create your World Space Canvas, Scroll View, and UI Scrollbars.
2. **Create Proxies:** Create empty GameObjects with `BoxCollider` components. Position and stretch the collider's X-axis so it perfectly covers the interactive "track" of your UI Scrollbar.
3. **Attach Script:** Attach the `CurvedPhysicalUIScrollviewHandler` script to a manager object (e.g., the Canvas itself).
4. **Map Arrays:** \* Add your 3D Colliders to the `Scrollbar Colliders` array.
   - Add your corresponding UI Scrollbars to the `Canvas Scrollbars` array.
   - _Important:_ Ensure Index 0 in Colliders matches Index 0 in Scrollbars.
5. **Assign VR Settings:** Drag and drop your Left and Right `XRRayInteractor` components and assign the select `InputActionReference` (trigger button) for both hands.

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
3. When the player points their VR laser at the top of the `Collider_InventoryTrack` and pulls the trigger, the inventory scrolls to the top. As they hold the trigger and drag the laser downward, the Scroll View moves smoothly in real-time.

---

---

## 🇮🇩 Dokumentasi Bahasa Indonesia

### Ikhtisar

`CurvedPhysicalUIScrollviewHandler` adalah skrip yang dirancang untuk menangani interaksi VR pada elemen Canvas UI—khususnya antarmuka yang melengkung (curved)—menggunakan pendekatan **"Physical Proxy"** khusus untuk **Scrollbar** (biasanya digunakan pada Scroll View).

Ini adalah sistem interaksi berbasis fisika yang memetakan proxy BoxCollider ke elemen UI (Button, Toggle, Slider, Scrollbar). Hal ini memastikan deteksi raycast yang presisi tanpa mempedulikan kelengkungan mesh visual atau resolusi tekstur. Sama seperti slider, scrollbar membutuhkan pelacakan posisi secara terus-menerus. Skrip ini memungkinkan pemain untuk **menekan, menahan, dan menggeser (drag)** laser VR mereka di sepanjang Box Collider 3D transparan. Skrip menghitung posisi laser pada sumbu X lokal collider secara real-time dan memperbarui nilai Scrollbar UI (0.0 hingga 1.0) secara dinamis, sehingga konten UI di Scroll View akan tergulung.

### Fitur Utama

- **Interaksi Tahan dan Geser (Drag):** Memperbarui nilai scrollbar secara terus-menerus selama tombol _trigger_ VR ditahan dan laser tetap berada di atas collider.
- **Aman untuk Dua Tangan (Dual Wielding):** Melacak `activeInteractor` (tangan spesifik yang memulai _drag_) untuk memastikan tangan yang lain tidak secara tidak sengaja mengganggu atau membatalkan interaksi scroll.
- **Kalkulasi Matematika Lokal:** Menggunakan konversi ruang koordinat (_inverse transform_) untuk memastikan gulungan tetap akurat meskipun Canvas diputar, diskalakan, atau dilengkungkan.
- **Indeks Berbasis Array:** Memudahkan pemetaan banyak scrollbar sekaligus hanya dengan menyamakan urutan indeks pada array.
- **Visualisasi Editor:** Menampilkan garis penghubung berwarna biru dan merah di Scene view untuk memvisualisasikan jalur dan batas scrollbar, sehingga mempermudah proses penataan.

### Cara Penggunaan

1. **Siapkan UI:** Buat Canvas (World Space), Scroll View, dan komponen UI Scrollbar Anda.
2. **Buat Proxy:** Buat GameObject kosong yang memiliki komponen `BoxCollider`. Atur posisi dan rentangkan sumbu X collider tersebut agar menutupi seluruh "jalur" (track) dari Scrollbar UI Anda secara presisi.
3. **Pasang Skrip:** Pasangkan skrip `CurvedPhysicalUIScrollviewHandler` pada objek manajer (misalnya, di Canvas itu sendiri).
4. **Petakan Array:**
   - Masukkan Collider 3D yang telah direntangkan ke dalam array `Scrollbar Colliders`.
   - Masukkan komponen UI Scrollbar yang bersangkutan ke dalam array `Canvas Scrollbars`.
   - _Penting:_ Pastikan Indeks 0 pada Collider terhubung dengan Indeks 0 pada Scrollbar.
5. **Atur Pengaturan VR:** Masukkan referensi `XRRayInteractor` Kiri dan Kanan, lalu tentukan `InputActionReference` (tombol trigger/select) untuk masing-masing tangan.

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
3. Saat pemain mengarahkan laser VR mereka ke ujung atas `Collider_JalurInventaris` dan menekan _trigger_, inventaris akan bergulir ke paling atas. Saat mereka menahan _trigger_ dan menggeser laser ke bawah, isi menu Scroll View akan bergulir ke bawah secara mulus dan real-time.

---

### Inspector Variables Reference

| Variable Name        | Type                   | Description                                                                                                                                  |
| :------------------- | :--------------------- | :------------------------------------------------------------------------------------------------------------------------------------------- |
| `scrollbarColliders` | `BoxCollider[]`        | Array of 3D Box Colliders acting as the scrollbar tracks. / _Array Box Collider 3D yang bertindak sebagai jalur scrollbar._                  |
| `canvasScrollbars`   | `Scrollbar[]`          | Array of Canvas Scrollbars. Must match the order of the colliders. / _Array Scrollbar UI. Urutannya harus sama dengan collider._             |
| `leftRayInteractor`  | `XRRayInteractor`      | Reference to the Left Hand Ray Interactor. / _Referensi ke Ray Interactor Kiri._                                                             |
| `rightRayInteractor` | `XRRayInteractor`      | Reference to the Right Hand Ray Interactor. / _Referensi ke Ray Interactor Kanan._                                                           |
| `leftSelectAction`   | `InputActionReference` | VR Input action for left hand click/trigger. / _Input Action trigger untuk tangan Kiri._                                                     |
| `rightSelectAction`  | `InputActionReference` | VR Input action for right hand click/trigger. / _Input Action trigger untuk tangan Kanan._                                                   |
| `showDebug`          | `bool`                 | Toggles debug logs in the console (useful to see value changes). / _Menyalakan log debug di konsol (berguna untuk melihat perubahan nilai)._ |
