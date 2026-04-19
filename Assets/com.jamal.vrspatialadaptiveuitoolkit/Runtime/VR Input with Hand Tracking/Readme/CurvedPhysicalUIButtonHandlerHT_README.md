# CurvedPhysicalUIButtonHandlerHT

## 🇬🇧 English Documentation

### Overview

`CurvedPhysicalUIButtonHandlerHT` is a script designed to handle VR interactions for Canvas UI elements—specifically curved interfaces—using a **"Physical Proxy"** approach, optimized for **Hand Tracking**.

Standard Unity UI Graphic Raycasters often struggle with curved world-space canvases in VR. This script solves that by allowing you to place invisible 3D Colliders over your UI visuals. When the user points their XR Ray Interactor using their bare hands and performs a **Pinch Gesture**, this script captures the event and programmatically triggers the corresponding Canvas UI Button. It is built to be fully compatible with **XR Interaction Toolkit (XRI) 3.0+**.

### Features

- **Hand Tracking Optimized:** Native support for bare-hand pinch gestures instead of physical controller buttons.
- **XRI 3.0+ Ready:** Utilizes the updated Input System architecture (dynamically enabling actions) required by newer Unity XR toolkits.
- **Physical Proxy Mapping:** Maps 3D Box Colliders directly to 2D UI Buttons.
- **Dual Wielding Support:** Independently tracks and processes pinch inputs from both Left and Right hands.
- **Array-Based Indexing:** Easily link multiple colliders and buttons by matching their array indices.
- **Editor Visualization:** Draws debug lines in the Scene view connecting the colliders to their target buttons for easy visual confirmation.

### How to Use

1. **Prepare the UI:** Create your World Space Canvas and UI Buttons.
2. **Create Proxies:** Create empty GameObjects with `BoxCollider` components. Position and scale them so they perfectly cover the interactive areas of your UI Buttons. (Remove MeshRenderer so they are invisible).
3. **Attach Script:** Attach the `CurvedPhysicalUIButtonHandlerHT` script to a manager object (e.g., the Canvas itself).
4. **Map Arrays:** - Add your 3D Colliders to the `Button Box Colliders` array.
   - Add your corresponding UI Buttons to the `Canvas Buttons` array.
   - _Important:_ Ensure Index 0 in Colliders matches Index 0 in Buttons.
5. **Assign VR Settings:** - Drag and drop your Left and Right `XRRayInteractor` components.
   - For `Left Select Action` and `Right Select Action`, assign the specific pinch/select actions from your XRI Input Assets (e.g., `XRI LeftHand Interaction/Select` and `XRI RightHand Interaction/Select`).

### Example Scenario

**Use Case:** You have a Main Menu with two buttons: `StartGame_Btn` and `Quit_Btn`.

1. You create two transparent 3D cubes: `Collider_Start` and `Collider_Quit`. You place them right in front of the UI buttons.
2. In the Inspector for this script:
   - **Size:** Set to `2`.
   - **Index 0:** Place `Collider_Start` in the Collider array, and `StartGame_Btn` in the Button array.
   - **Index 1:** Place `Collider_Quit` in the Collider array, and `Quit_Btn` in the Button array.
3. When the player points their bare hand VR laser at `Collider_Start` and **performs a pinch gesture**, the script identifies it as Index `0`, and automatically invokes the `onClick()` event of `StartGame_Btn`.

---

---

## 🇮🇩 Dokumentasi Bahasa Indonesia

### Ikhtisar

`CurvedPhysicalUIButtonHandlerHT` adalah skrip yang dirancang untuk menangani interaksi VR pada elemen Canvas UI—khususnya antarmuka yang melengkung (curved)—menggunakan pendekatan **"Physical Proxy"**, dan secara khusus dioptimalkan untuk **Hand Tracking (Pelacakan Tangan)**.

Graphic Raycaster bawaan Unity sering kali bermasalah saat mendeteksi Canvas yang melengkung di ruang VR. Skrip ini mengatasi masalah tersebut dengan memanfaatkan Collider 3D transparan yang diletakkan di atas visual UI. Ketika pengguna mengarahkan laser dari tangan kosong mereka dan melakukan **Gestur Mencubit (Pinch)**, skrip akan menangkap interaksinya dan memicu event `onClick` pada Tombol UI Canvas yang bersangkutan. Skrip ini dirancang agar kompatibel penuh dengan **XR Interaction Toolkit (XRI) 3.0+**.

### Fitur Utama

- **Optimasi Hand Tracking:** Dukungan bawaan untuk membaca gestur cubitan tangan kosong, bukan sekadar pelatuk kontroler fisik.
- **Kompatibel XRI 3.0+:** Menggunakan arsitektur Input System terbaru (mengaktifkan action secara dinamis) yang diwajibkan oleh versi Unity XR modern.
- **Physical Proxy Mapping:** Menghubungkan Collider 3D fisik dengan UI Button 2D secara langsung.
- **Dukungan Dua Tangan:** Melacak dan memproses gestur cubitan dari tangan Kiri dan Kanan secara independen.
- **Indeks Berbasis Array:** Memudahkan pemetaan banyak tombol sekaligus hanya dengan menyamakan urutan indeks pada array.
- **Visualisasi Editor:** Menampilkan garis penghubung (Gizmos) warna cyan di Scene view antara collider dan tombol target untuk mempermudah validasi.

### Cara Penggunaan

1. **Siapkan UI:** Buat Canvas (World Space) dan letakkan UI Button Anda.
2. **Buat Proxy:** Buat GameObject kosong yang memiliki komponen `BoxCollider`. Atur posisi dan ukurannya agar menutupi area tombol UI secara presisi. (Hapus komponen MeshRenderer agar tidak terlihat).
3. **Pasang Skrip:** Pasangkan skrip `CurvedPhysicalUIButtonHandlerHT` pada objek manajer (misalnya, di Canvas itu sendiri).
4. **Petakan Array:**
   - Masukkan Collider 3D yang telah dibuat ke dalam array `Button Box Colliders`.
   - Masukkan komponen UI Button yang bersangkutan ke dalam array `Canvas Buttons`.
   - _Penting:_ Pastikan Indeks 0 pada Collider terhubung dengan Indeks 0 pada Button, begitu pula seterusnya.
5. **Atur Pengaturan VR:** - Masukkan referensi `XRRayInteractor` Kiri dan Kanan.
   - Pada kolom `Left Select Action` dan `Right Select Action`, pastikan Anda memilih Input Action yang membaca gestur cubitan, misalnya: `XRI LeftHand Interaction/Select` dan `XRI RightHand Interaction/Select`.

### Contoh Skenario Penggunaan

**Kasus:** Anda memiliki Menu Utama dengan dua tombol: `Tombol_Mulai` dan `Tombol_Keluar`.

1. Anda membuat dua kubus 3D transparan bernama `Collider_Mulai` dan `Collider_Keluar`. Letakkan kedua collider ini tepat di depan grafik tombol UI tersebut.
2. Pada Inspector skrip ini:
   - **Size:** Isi dengan angka `2`.
   - **Indeks 0:** Masukkan `Collider_Mulai` di array Collider, dan `Tombol_Mulai` di array Button.
   - **Indeks 1:** Masukkan `Collider_Keluar` di array Collider, dan `Tombol_Keluar` di array Button.
3. Saat pemain mengarahkan laser dari tangan kosong mereka ke `Collider_Mulai` dan **melakukan gestur mencubit (pinch)**, skrip akan mendeteksinya sebagai Indeks `0`, lalu secara otomatis menjalankan fungsi `onClick()` yang ada pada `Tombol_Mulai`.

---

### Inspector Variables Reference

| Variable Name        | Type                   | Description                                                                                                                   |
| :------------------- | :--------------------- | :---------------------------------------------------------------------------------------------------------------------------- |
| `buttonBoxColliders` | `Collider[]`           | Array of physical 3D Colliders placed over the UI. / _Array Collider 3D fisik yang menutupi UI._                              |
| `canvasButtons`      | `Button[]`             | Array of Canvas Buttons. Must match the order of the colliders. / _Array Tombol UI. Urutannya harus sama dengan collider._    |
| `leftRayInteractor`  | `XRRayInteractor`      | Reference to the Left Hand Ray Interactor. / _Referensi ke Ray Interactor Kiri._                                              |
| `rightRayInteractor` | `XRRayInteractor`      | Reference to the Right Hand Ray Interactor. / _Referensi ke Ray Interactor Kanan._                                            |
| `leftSelectAction`   | `InputActionReference` | VR Input action for left hand pinch gesture (e.g., XRI LeftHand Interaction/Select). / _Input Action gestur cubitan Kiri._    |
| `rightSelectAction`  | `InputActionReference` | VR Input action for right hand pinch gesture (e.g., XRI RightHand Interaction/Select). / _Input Action gestur cubitan Kanan._ |
| `showDebug`          | `bool`                 | Toggles debug logs in the console. / _Menyalakan/mematikan log debug di konsol._                                              |
