# CurvedPhysicalUISliderDragHandler

## 🇬🇧 English Documentation

### Overview

`CurvedPhysicalUISliderDragHandler` is a script designed to handle VR interactions for Canvas UI elements—specifically curved interfaces—using a **"Physical Proxy"** approach for **Sliders**.

A physics-based interaction system maps BoxCollider proxies to UI elements (Buttons, Toggles, Sliders, Scrollbars). This ensures precise raycast detection regardless of the visual mesh curvature or texture resolution. Unlike simple buttons or toggles that only require a single click, a slider requires continuous tracking. This script allows players to **press, hold, and drag** their VR laser across an invisible 3D Box Collider. The script calculates the laser's position along the collider's local X-axis in real-time and updates the corresponding UI Slider's value dynamically.

### Features

- **Hold and Drag Interaction:** Continuously updates the slider value as long as the VR trigger is held down and the laser remains on the collider.
- **Dual Wielding Safe:** Tracks the `activeInteractor` (the specific hand that started the drag) to ensure the other hand doesn't accidentally interfere or cancel the interaction.
- **Local Math Calculation:** Uses inverse transform math to ensure accurate sliding even if the Canvas is rotated, scaled, or curved.
- **Array-Based Indexing:** Easily link multiple colliders and sliders by matching their array indices.
- **Editor Visualization:** Draws magenta and red debug lines in the Scene view to visualize the slider's track and bounds, making setup much easier.

### How to Use

1. **Prepare the UI:** Create your World Space Canvas and UI Sliders.
2. **Create Proxies:** Create empty GameObjects with `BoxCollider` components. Position and stretch the collider's X-axis so it perfectly covers the interactive "track" of your UI Slider.
3. **Attach Script:** Attach the `CurvedPhysicalUISliderDragHandler` script to a manager object (e.g., the Canvas itself).
4. **Map Arrays:** \* Add your 3D Colliders to the `Slider Colliders` array.
   - Add your corresponding UI Sliders to the `Canvas Sliders` array.
   - _Important:_ Ensure Index 0 in Colliders matches Index 0 in Sliders.
5. **Assign VR Settings:** Drag and drop your Left and Right `XRRayInteractor` components and assign the select `InputActionReference` (trigger button) for both hands.

### Slider Orientation Guide

The system calculates values based on the Collider's **Local X-Axis**. To define the sliding direction (whether your slider is horizontal, vertical, or inverted), apply the following rotations to the Collider GameObject:

| Slide Direction             | Rotation (X, Y, Z) | Description         |
| :-------------------------- | :----------------- | :------------------ |
| **Left to Right** (Default) | `(0, 0, 0)`        | Standard Horizontal |
| **Right to Left**           | `(0, 180, 0)`      | Inverted Horizontal |
| **Bottom to Top**           | `(0, 0, 90)`       | Standard Vertical   |
| **Top to Bottom**           | `(0, 0, -90)`      | Inverted Vertical   |

> **Note:** Ensure the Collider's `Size X` matches the visual length of the UI track. The script uses this Size X to map the 0 to 1 value.

### Example Scenario

**Use Case:** You have an Audio Settings Menu with a `Volume_Slider`.

1. You create a transparent 3D cube named `Collider_VolumeTrack`. You stretch its width (X-axis) to cover the entire length of the volume slider visually.
2. In the Inspector for this script:
   - **Size:** Set to `1`.
   - **Index 0:** Place `Collider_VolumeTrack` in the Collider array, and `Volume_Slider` in the Slider array.
3. When the player points their left VR laser at the start of the `Collider_VolumeTrack` and pulls the trigger, the volume drops to minimum. As they hold the trigger and drag the laser to the right, the slider moves smoothly in real-time, increasing the volume.

---

---

## 🇮🇩 Dokumentasi Bahasa Indonesia

### Ikhtisar

`CurvedPhysicalUISliderDragHandler` adalah skrip yang dirancang untuk menangani interaksi VR pada elemen Canvas UI—khususnya antarmuka yang melengkung (curved)—menggunakan pendekatan **"Physical Proxy"** khusus untuk **Slider**.

Ini adalah sistem interaksi berbasis fisika yang memetakan proxy BoxCollider ke elemen UI (Button, Toggle, Slider, Scrollbar). Hal ini memastikan deteksi raycast yang presisi tanpa mempedulikan kelengkungan mesh visual atau resolusi tekstur. Berbeda dengan tombol atau toggle sederhana yang hanya membutuhkan satu klik, slider membutuhkan pelacakan posisi secara terus-menerus. Skrip ini memungkinkan pemain untuk **menekan, menahan, dan menggeser (drag)** laser VR mereka di sepanjang Box Collider 3D transparan. Skrip menghitung posisi laser pada sumbu X lokal collider secara real-time dan memperbarui nilai Slider UI secara dinamis.

### Fitur Utama

- **Interaksi Tahan dan Geser (Drag):** Memperbarui nilai slider secara terus-menerus selama tombol _trigger_ VR ditahan dan laser tetap berada di atas collider.
- **Aman untuk Dua Tangan (Dual Wielding):** Melacak `activeInteractor` (tangan spesifik yang memulai _drag_) untuk memastikan tangan yang lain tidak secara tidak sengaja mengganggu atau membatalkan interaksi.
- **Kalkulasi Matematika Lokal:** Menggunakan konversi ruang koordinat (_inverse transform_) untuk memastikan pergeseran tetap akurat meskipun Canvas diputar, diskalakan, atau dilengkungkan.
- **Indeks Berbasis Array:** Memudahkan pemetaan banyak slider sekaligus hanya dengan menyamakan urutan indeks pada array.
- **Visualisasi Editor:** Menampilkan garis penghubung berwarna magenta dan merah di Scene view untuk memvisualisasikan jalur dan batas slider, sehingga mempermudah proses penataan.

### Cara Penggunaan

1. **Siapkan UI:** Buat Canvas (World Space) dan letakkan komponen UI Slider Anda.
2. **Buat Proxy:** Buat GameObject kosong yang memiliki komponen `BoxCollider`. Atur posisi dan rentangkan sumbu X collider tersebut agar menutupi seluruh "jalur" (track) dari Slider UI Anda secara presisi.
3. **Pasang Skrip:** Pasangkan skrip `CurvedPhysicalUISliderDragHandler` pada objek manajer (misalnya, di Canvas itu sendiri).
4. **Petakan Array:**
   - Masukkan Collider 3D yang telah direntangkan ke dalam array `Slider Colliders`.
   - Masukkan komponen UI Slider yang bersangkutan ke dalam array `Canvas Sliders`.
   - _Penting:_ Pastikan Indeks 0 pada Collider terhubung dengan Indeks 0 pada Slider.
5. **Atur Pengaturan VR:** Masukkan referensi `XRRayInteractor` Kiri dan Kanan, lalu tentukan `InputActionReference` (tombol trigger/select) untuk masing-masing tangan.

### Panduan Orientasi Slider

Sistem menghitung nilai pergeseran berdasarkan **Sumbu X Lokal (Local X-Axis)** dari Collider. Untuk menentukan arah geseran (apakah slider Anda horizontal, vertikal, atau terbalik), terapkan rotasi berikut pada GameObject Collider Anda di Inspector:

| Arah Geseran               | Rotasi (X, Y, Z) | Keterangan          |
| :------------------------- | :--------------- | :------------------ |
| **Kiri ke Kanan** (Bawaan) | `(0, 0, 0)`      | Horizontal Standar  |
| **Kanan ke Kiri**          | `(0, 180, 0)`    | Horizontal Terbalik |
| **Bawah ke Atas**          | `(0, 0, 90)`     | Vertikal Standar    |
| **Atas ke Bawah**          | `(0, 0, -90)`    | Vertikal Terbalik   |

> **Catatan:** Pastikan nilai `Size X` pada Collider sama persis dengan panjang jalur UI secara visual. Skrip ini menggunakan nilai Size X tersebut untuk memetakan rentang nilai 0 hingga 1.

### Contoh Skenario Penggunaan

**Kasus:** Anda memiliki Menu Pengaturan Audio dengan `Slider_Volume`.

1. Anda membuat kubus 3D transparan bernama `Collider_JalurVolume`. Anda merentangkan lebarnya (sumbu X) agar menutupi seluruh panjang visual slider volume.
2. Pada Inspector skrip ini:
   - **Size:** Isi dengan angka `1`.
   - **Indeks 0:** Masukkan `Collider_JalurVolume` di array Collider, dan `Slider_Volume` di array Slider.
3. Saat pemain mengarahkan laser VR kiri mereka ke ujung kiri `Collider_JalurVolume` dan menekan _trigger_, volume akan turun ke minimum. Saat mereka menahan _trigger_ dan menggeser laser ke kanan, slider akan bergerak mulus secara real-time, menaikkan volume.

---

### Inspector Variables Reference

| Variable Name        | Type                   | Description                                                                                                                                  |
| :------------------- | :--------------------- | :------------------------------------------------------------------------------------------------------------------------------------------- |
| `sliderColliders`    | `BoxCollider[]`        | Array of 3D Box Colliders acting as the slider tracks. / _Array Box Collider 3D yang bertindak sebagai jalur slider._                        |
| `canvasSliders`      | `Slider[]`             | Array of Canvas Sliders. Must match the order of the colliders. / _Array Slider UI. Urutannya harus sama dengan collider._                   |
| `leftRayInteractor`  | `XRRayInteractor`      | Reference to the Left Hand Ray Interactor. / _Referensi ke Ray Interactor Kiri._                                                             |
| `rightRayInteractor` | `XRRayInteractor`      | Reference to the Right Hand Ray Interactor. / _Referensi ke Ray Interactor Kanan._                                                           |
| `leftSelectAction`   | `InputActionReference` | VR Input action for left hand click/trigger. / _Input Action trigger untuk tangan Kiri._                                                     |
| `rightSelectAction`  | `InputActionReference` | VR Input action for right hand click/trigger. / _Input Action trigger untuk tangan Kanan._                                                   |
| `showDebug`          | `bool`                 | Toggles debug logs in the console (useful to see value changes). / _Menyalakan log debug di konsol (berguna untuk melihat perubahan nilai)._ |
