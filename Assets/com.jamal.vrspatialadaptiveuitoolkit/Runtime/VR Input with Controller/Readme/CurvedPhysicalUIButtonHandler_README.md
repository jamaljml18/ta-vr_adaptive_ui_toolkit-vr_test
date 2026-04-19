# CurvedPhysicalUIButtonHandler

## 🇬🇧 English Documentation

### Overview

`CurvedPhysicalUIButtonHandler` is a script designed to handle VR interactions for Canvas UI elements—specifically curved interfaces—using a **"Physical Proxy"** approach.

Standard Unity UI Graphic Raycasters often struggle with curved world-space canvases in VR. This script solves that by allowing you to place invisible 3D Colliders over your UI visuals. When the VR Ray Interactor clicks the physical 3D collider, this script captures the event and programmatically triggers the corresponding Canvas UI Button.

### Features

- **Physical Proxy Mapping:** Maps 3D Box Colliders directly to 2D UI Buttons.
- **Dual Wielding Support:** Independently tracks and processes inputs from both Left and Right VR controllers.
- **Array-Based Indexing:** Easily link multiple colliders and buttons by matching their array indices.
- **Editor Visualization:** Draws debug lines in the Scene view connecting the colliders to their target buttons for easy visual confirmation.

### How to Use

1. **Prepare the UI:** Create your World Space Canvas and UI Buttons.
2. **Create Proxies:** Create empty GameObjects with `BoxCollider` components. Position and scale them so they perfectly cover the interactive areas of your UI Buttons.
3. **Attach Script:** Attach the `CurvedPhysicalUIButtonHandler` script to a manager object (e.g., the Canvas itself).
4. **Map Arrays:** \* Add your 3D Colliders to the `Button Box Colliders` array.
   - Add your corresponding UI Buttons to the `Canvas Buttons` array.
   - _Important:_ Ensure Index 0 in Colliders matches Index 0 in Buttons.
5. **Assign VR Settings:** Drag and drop your Left and Right `XRRayInteractor` components and assign the select `InputActionReference` for both hands.

### Example Scenario

**Use Case:** You have a Main Menu with two buttons: `StartGame_Btn` and `Quit_Btn`.

1. You create two transparent 3D cubes: `Collider_Start` and `Collider_Quit`. You place them right in front of the UI buttons.
2. In the Inspector for this script:
   - **Size:** Set to `2`.
   - **Index 0:** Place `Collider_Start` in the Collider array, and `StartGame_Btn` in the Button array.
   - **Index 1:** Place `Collider_Quit` in the Collider array, and `Quit_Btn` in the Button array.
3. When the player points their VR laser at `Collider_Start` and pulls the trigger, the script identifies it as Index `0`, and automatically invokes the `onClick()` event of `StartGame_Btn`.

---

---

## 🇮🇩 Dokumentasi Bahasa Indonesia

### Ikhtisar

`CurvedPhysicalUIButtonHandler` adalah skrip yang dirancang untuk menangani interaksi VR pada elemen Canvas UI—khususnya antarmuka yang melengkung (curved)—menggunakan pendekatan **"Physical Proxy"**.

Graphic Raycaster bawaan Unity sering kali bermasalah saat mendeteksi Canvas yang melengkung di ruang VR. Skrip ini mengatasi masalah tersebut dengan memanfaatkan Collider 3D transparan yang diletakkan di atas visual UI. Ketika XR Ray Interactor menembak dan mengklik collider 3D tersebut, skrip akan menangkap interaksinya dan memicu jalannya fungsi (event onClick) pada Tombol UI Canvas yang bersangkutan.

### Fitur Utama

- **Physical Proxy Mapping:** Menghubungkan Collider 3D fisik dengan UI Button 2D secara langsung.
- **Dukungan Dua Tangan (Dual Wielding):** Melacak dan memproses input dari kontroler VR Kiri dan Kanan secara independen.
- **Indeks Berbasis Array:** Memudahkan pemetaan banyak tombol sekaligus hanya dengan menyamakan urutan indeks pada array.
- **Visualisasi Editor:** Menampilkan garis penghubung (Gizmos) di Scene view antara collider dan tombol target untuk mempermudah validasi saat mengatur posisi.

### Cara Penggunaan

1. **Siapkan UI:** Buat Canvas (World Space) dan letakkan UI Button Anda.
2. **Buat Proxy:** Buat GameObject kosong yang memiliki komponen `BoxCollider`. Atur posisi dan ukurannya agar menutupi area tombol UI secara presisi. (Hapus komponen MeshRenderer agar tidak terlihat).
3. **Pasang Skrip:** Pasangkan skrip `CurvedPhysicalUIButtonHandler` pada objek manajer (misalnya, di Canvas itu sendiri).
4. **Petakan Array:**
   - Masukkan Collider 3D yang telah dibuat ke dalam array `Button Box Colliders`.
   - Masukkan komponen UI Button yang bersangkutan ke dalam array `Canvas Buttons`.
   - _Penting:_ Pastikan Indeks 0 pada Collider terhubung dengan Indeks 0 pada Button, begitu pula seterusnya.
5. **Atur Pengaturan VR:** Masukkan referensi `XRRayInteractor` Kiri dan Kanan, lalu tentukan `InputActionReference` (tombol trigger/select) untuk masing-masing tangan.

### Contoh Skenario Penggunaan

**Kasus:** Anda memiliki Menu Utama dengan dua tombol: `Tombol_Mulai` dan `Tombol_Keluar`.

1. Anda membuat dua kubus 3D transparan bernama `Collider_Mulai` dan `Collider_Keluar`. Letakkan kedua collider ini tepat di depan grafik tombol UI tersebut.
2. Pada Inspector skrip ini:
   - **Size:** Isi dengan angka `2`.
   - **Indeks 0:** Masukkan `Collider_Mulai` di array Collider, dan `Tombol_Mulai` di array Button.
   - **Indeks 1:** Masukkan `Collider_Keluar` di array Collider, dan `Tombol_Keluar` di array Button.
3. Saat pemain mengarahkan laser VR ke `Collider_Mulai` dan menekan _trigger_, skrip akan mendeteksinya sebagai Indeks `0`, lalu secara otomatis menjalankan fungsi `onClick()` yang ada pada `Tombol_Mulai`.

---

### Inspector Variables Reference

| Variable Name        | Type                   | Description                                                                                                                |
| :------------------- | :--------------------- | :------------------------------------------------------------------------------------------------------------------------- |
| `buttonBoxColliders` | `Collider[]`           | Array of physical 3D Colliders placed over the UI. / _Array Collider 3D fisik yang menutupi UI._                           |
| `canvasButtons`      | `Button[]`             | Array of Canvas Buttons. Must match the order of the colliders. / _Array Tombol UI. Urutannya harus sama dengan collider._ |
| `leftRayInteractor`  | `XRRayInteractor`      | Reference to the Left Hand Ray Interactor. / _Referensi ke Ray Interactor Kiri._                                           |
| `rightRayInteractor` | `XRRayInteractor`      | Reference to the Right Hand Ray Interactor. / _Referensi ke Ray Interactor Kanan._                                         |
| `leftSelectAction`   | `InputActionReference` | VR Input action for left hand click/trigger. / _Input Action trigger untuk tangan Kiri._                                   |
| `rightSelectAction`  | `InputActionReference` | VR Input action for right hand click/trigger. / _Input Action trigger untuk tangan Kanan._                                 |
| `showDebug`          | `bool`                 | Toggles debug logs in the console. / _Menyalakan/mematikan log debug di konsol._                                           |
