# CurvedPhysicalUIToggleHandler

## 🇬🇧 English Documentation

### Overview

`CurvedPhysicalUIToggleHandler` is a script designed to handle VR interactions for Canvas UI elements—specifically curved interfaces—using a **"Physical Proxy"** approach for **Toggles (Checkboxes)**.

Standard Unity UI Graphic Raycasters often struggle with curved world-space canvases in VR. This script solves that by allowing you to place invisible 3D Colliders over your UI Toggle visuals. When the VR Ray Interactor clicks the physical 3D collider, this script captures the event and programmatically flips the `.isOn` state of the corresponding Canvas UI Toggle.

### Features

- **Physical Proxy Mapping:** Maps 3D Box Colliders directly to 2D UI Toggles.
- **Dual Wielding Support:** Independently tracks and processes inputs from both Left and Right VR controllers.
- **Automatic Event Triggering:** Changing the `.isOn` state via this script automatically invokes the Toggle's native `OnValueChanged` event.
- **Array-Based Indexing:** Easily link multiple colliders and toggles by matching their array indices.
- **Editor Visualization:** Draws yellow debug lines in the Scene view connecting the colliders to their target toggles for easy visual confirmation.

### How to Use

1. **Prepare the UI:** Create your World Space Canvas and UI Toggles (Checkboxes).
2. **Create Proxies:** Create empty GameObjects with `BoxCollider` components. Position and scale them so they perfectly cover the interactive areas of your UI Toggles.
3. **Attach Script:** Attach the `CurvedPhysicalUIToggleHandler` script to a manager object (e.g., the Canvas itself).
4. **Map Arrays:** \* Add your 3D Colliders to the `Toggle Box Colliders` array.
   - Add your corresponding UI Toggles to the `Canvas Toggles` array.
   - _Important:_ Ensure Index 0 in Colliders matches Index 0 in Toggles.
5. **Assign VR Settings:** Drag and drop your Left and Right `XRRayInteractor` components and assign the select `InputActionReference` for both hands.

### Example Scenario

**Use Case:** You have a Settings Menu with two checkboxes: `MuteMusic_Toggle` and `Subtitles_Toggle`.

1. You create two transparent 3D cubes: `Collider_Mute` and `Collider_Subs`. You place them right in front of the UI toggles.
2. In the Inspector for this script:
   - **Size:** Set to `2`.
   - **Index 0:** Place `Collider_Mute` in the Collider array, and `MuteMusic_Toggle` in the Toggle array.
   - **Index 1:** Place `Collider_Subs` in the Collider array, and `Subtitles_Toggle` in the Toggle array.
3. When the player points their VR laser at `Collider_Mute` and pulls the trigger, the script identifies it as Index `0`, and automatically flips the state of `MuteMusic_Toggle` (from True to False, or False to True).

---

---

## 🇮🇩 Dokumentasi Bahasa Indonesia

### Ikhtisar

`CurvedPhysicalUIToggleHandler` adalah skrip yang dirancang untuk menangani interaksi VR pada elemen Canvas UI—khususnya antarmuka yang melengkung (curved)—menggunakan pendekatan **"Physical Proxy"** khusus untuk **Toggle (Checkbox)**.

Graphic Raycaster bawaan Unity sering kali bermasalah saat mendeteksi Canvas yang melengkung di ruang VR. Skrip ini mengatasi masalah tersebut dengan memanfaatkan Collider 3D transparan yang diletakkan di atas visual UI. Ketika XR Ray Interactor menembak dan mengklik collider 3D tersebut, skrip akan menangkap interaksinya dan membalikkan status `.isOn` pada Toggle UI Canvas yang bersangkutan secara terprogram.

### Fitur Utama

- **Physical Proxy Mapping:** Menghubungkan Collider 3D fisik dengan UI Toggle 2D secara langsung.
- **Dukungan Dua Tangan (Dual Wielding):** Melacak dan memproses input dari kontroler VR Kiri dan Kanan secara independen.
- **Pemicu Event Otomatis:** Mengubah status `.isOn` melalui skrip ini akan secara otomatis memanggil event bawaan `OnValueChanged` pada Toggle.
- **Indeks Berbasis Array:** Memudahkan pemetaan banyak toggle sekaligus hanya dengan menyamakan urutan indeks pada array.
- **Visualisasi Editor:** Menampilkan garis penghubung berwarna kuning (Gizmos) di Scene view antara collider dan toggle target untuk mempermudah validasi saat mengatur posisi.

### Cara Penggunaan

1. **Siapkan UI:** Buat Canvas (World Space) dan letakkan UI Toggle (Checkbox) Anda.
2. **Buat Proxy:** Buat GameObject kosong yang memiliki komponen `BoxCollider`. Atur posisi dan ukurannya agar menutupi area toggle UI secara presisi. (Hapus komponen MeshRenderer agar tidak terlihat).
3. **Pasang Skrip:** Pasangkan skrip `CurvedPhysicalUIToggleHandler` pada objek manajer (misalnya, di Canvas itu sendiri).
4. **Petakan Array:**
   - Masukkan Collider 3D yang telah dibuat ke dalam array `Toggle Box Colliders`.
   - Masukkan komponen UI Toggle yang bersangkutan ke dalam array `Canvas Toggles`.
   - _Penting:_ Pastikan Indeks 0 pada Collider terhubung dengan Indeks 0 pada Toggle, begitu pula seterusnya.
5. **Atur Pengaturan VR:** Masukkan referensi `XRRayInteractor` Kiri dan Kanan, lalu tentukan `InputActionReference` (tombol trigger/select) untuk masing-masing tangan.

### Contoh Skenario Penggunaan

**Kasus:** Anda memiliki Menu Pengaturan dengan dua kotak centang: `Toggle_BisukanMusik` dan `Toggle_Subtitle`.

1. Anda membuat dua kubus 3D transparan bernama `Collider_Bisu` dan `Collider_Sub`. Letakkan kedua collider ini tepat di depan grafik toggle UI tersebut.
2. Pada Inspector skrip ini:
   - **Size:** Isi dengan angka `2`.
   - **Indeks 0:** Masukkan `Collider_Bisu` di array Collider, dan `Toggle_BisukanMusik` di array Toggle.
   - **Indeks 1:** Masukkan `Collider_Sub` di array Collider, dan `Toggle_Subtitle` di array Toggle.
3. Saat pemain mengarahkan laser VR ke `Collider_Bisu` dan menekan _trigger_, skrip akan mendeteksinya sebagai Indeks `0`, lalu secara otomatis mengubah status `Toggle_BisukanMusik` (dari Nyala menjadi Mati, atau sebaliknya).

---

### Inspector Variables Reference

| Variable Name        | Type                   | Description                                                                                                                |
| :------------------- | :--------------------- | :------------------------------------------------------------------------------------------------------------------------- |
| `toggleBoxColliders` | `Collider[]`           | Array of physical 3D Colliders placed over the UI. / _Array Collider 3D fisik yang menutupi UI._                           |
| `canvasToggles`      | `Toggle[]`             | Array of Canvas Toggles. Must match the order of the colliders. / _Array Toggle UI. Urutannya harus sama dengan collider._ |
| `leftRayInteractor`  | `XRRayInteractor`      | Reference to the Left Hand Ray Interactor. / _Referensi ke Ray Interactor Kiri._                                           |
| `rightRayInteractor` | `XRRayInteractor`      | Reference to the Right Hand Ray Interactor. / _Referensi ke Ray Interactor Kanan._                                         |
| `leftSelectAction`   | `InputActionReference` | VR Input action for left hand click/trigger. / _Input Action trigger untuk tangan Kiri._                                   |
| `rightSelectAction`  | `InputActionReference` | VR Input action for right hand click/trigger. / _Input Action trigger untuk tangan Kanan._                                 |
| `showDebug`          | `bool`                 | Toggles debug logs in the console. / _Menyalakan/mematikan log debug di konsol._                                           |
