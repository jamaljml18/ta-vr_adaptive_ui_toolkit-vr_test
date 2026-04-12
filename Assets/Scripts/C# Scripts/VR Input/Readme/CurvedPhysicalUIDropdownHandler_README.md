# CurvedPhysicalUIDropdownHandler

## 🇬🇧 English Documentation

### Overview

`CurvedPhysicalUIDropdownHandler` is a script designed to handle VR interactions for Canvas `TMP_Dropdown` elements—specifically curved interfaces—using a **"Physical Proxy"** approach.

Standard dropdowns in VR can be notoriously difficult to interact with because they spawn a dynamic list (Template) at runtime. This script solves that elegantly using a **Two-Collider System**. Instead of placing a collider on every single dropdown item, you place one collider on the main header, and _one large collider_ over the entire expanded list area. The script mathematically calculates which option the player clicked based on where the laser hits the local Y-axis of the list collider.

### Features

- **Two-Part Physical Proxy:** Uses a `headerCollider` to open/close the dropdown, and a `listCollider` to select items.
- **Math-Based Index Calculation:** Highly optimized. It uses inverse transform math on the Y-axis to figure out exactly which item was clicked without needing dozens of individual colliders.
- **Smart Collider Toggling:** Automatically disables the `listCollider` when the dropdown is closed so it doesn't invisibly block your other UI elements from VR raycasts.
- **Dual Wielding Support:** Independently tracks and processes inputs from both Left and Right VR controllers.
- **Editor Visualization:** Draws a yellow debug box for the Header and a cyan debug box for the List in the Scene view to ensure perfect alignment.

### How to Use

1. **Prepare the UI:** Create your World Space Canvas and add a `TMP_Dropdown`. Populate it with your desired options (e.g., Low, Medium, High).
2. **Create the Header Proxy:** Create an empty GameObject with a `BoxCollider`. Size it to perfectly cover the main, always-visible Dropdown button.
3. **Create the List Proxy:** Create another empty GameObject with a `BoxCollider`. Size it to perfectly cover the **expanded list (Template)** area that appears when the dropdown opens. _(Tip: Temporarily enable the Dropdown's Template in the hierarchy to measure this accurately, then disable it)._
4. **Attach Script:** Attach the `CurvedPhysicalUIDropdownHandler` script to a manager object (e.g., the Canvas itself or the Dropdown parent).
5. **Map References:** \* Assign your `TMP_Dropdown`.
   - Assign the `Header Collider` and `List Collider` you just created.
6. **Assign VR Settings:** Drag and drop your Left and Right `XRRayInteractor` components and assign the select `InputActionReference` (trigger button) for both hands.

### Example Scenario

**Use Case:** You have a Difficulty Dropdown with 3 options: `Easy`, `Normal`, and `Hard`.

1. You create `Collider_Header` (small box over the main button) and `Collider_DropdownList` (a tall box covering where the 3 items will appear).
2. When the player points their VR laser at `Collider_Header` and clicks, the script calls `dropdown.Show()` and activates `Collider_DropdownList`.
3. The player points their laser at the bottom third of the expanded list and clicks.
4. The script calculates the hit position on the Y-axis, determines it belongs to Index `2`, selects `Hard`, closes the dropdown, and hides the list collider again.

---

---

## 🇮🇩 Dokumentasi Bahasa Indonesia

### Ikhtisar

`CurvedPhysicalUIDropdownHandler` adalah skrip yang dirancang untuk menangani interaksi VR pada elemen Canvas `TMP_Dropdown`—khususnya antarmuka yang melengkung (curved)—menggunakan pendekatan **"Physical Proxy"**.

Dropdown standar di VR sering kali sangat sulit diinteraksikan karena mereka memunculkan daftar dinamis (Template) saat runtime. Skrip ini mengatasinya secara elegan menggunakan **Sistem Dua-Collider**. Alih-alih menempatkan collider pada setiap item dropdown, Anda cukup menempatkan satu collider pada tombol utama (header), dan _satu collider besar_ yang menutupi seluruh area daftar yang terbuka. Skrip ini secara matematis menghitung opsi mana yang diklik pemain berdasarkan posisi jatuhnya laser pada sumbu Y lokal dari list collider.

### Fitur Utama

- **Proxy Fisik Dua Bagian:** Menggunakan `headerCollider` untuk membuka/menutup dropdown, dan `listCollider` untuk memilih item.
- **Kalkulasi Indeks Berbasis Matematika:** Sangat optimal. Menggunakan kalkulasi _inverse transform_ pada sumbu Y untuk mengetahui secara pasti item mana yang diklik tanpa memerlukan puluhan collider terpisah.
- **Smart Collider Toggling:** Secara otomatis menonaktifkan `listCollider` saat dropdown ditutup sehingga tidak menghalangi elemen UI lain dari tembakan raycast VR.
- **Dukungan Dua Tangan (Dual Wielding):** Melacak dan memproses input dari kontroler VR Kiri dan Kanan secara independen.
- **Visualisasi Editor:** Menampilkan kotak debug kuning untuk Header dan kotak cyan untuk List di Scene view guna memastikan presisi letak collider.

### Cara Penggunaan

1. **Siapkan UI:** Buat Canvas (World Space) dan tambahkan `TMP_Dropdown`. Isi dengan opsi yang Anda inginkan (misal: Rendah, Sedang, Tinggi).
2. **Buat Proxy Header:** Buat GameObject kosong dengan komponen `BoxCollider`. Atur ukurannya agar pas menutupi tombol utama Dropdown yang selalu terlihat.
3. **Buat Proxy List:** Buat GameObject kosong lain dengan `BoxCollider`. Atur ukurannya agar pas menutupi area **daftar yang terbuka (Template)** saat dropdown ditekan. _(Tips: Aktifkan sementara objek Template milik Dropdown di Hierarchy untuk mengukur ukurannya secara akurat, lalu nonaktifkan kembali)._
4. **Pasang Skrip:** Pasangkan skrip `CurvedPhysicalUIDropdownHandler` pada objek manajer (misalnya, di Canvas itu sendiri atau induk Dropdown).
5. **Hubungkan Referensi:** \* Masukkan komponen `TMP_Dropdown` Anda.
   - Masukkan `Header Collider` dan `List Collider` yang baru saja Anda buat.
6. **Atur Pengaturan VR:** Masukkan referensi `XRRayInteractor` Kiri dan Kanan, lalu tentukan `InputActionReference` (tombol trigger/select) untuk masing-masing tangan.

### Contoh Skenario Penggunaan

**Kasus:** Anda memiliki Dropdown Tingkat Kesulitan dengan 3 opsi: `Mudah`, `Normal`, dan `Sulit`.

1. Anda membuat `Collider_Header` (kotak kecil di atas tombol utama) dan `Collider_DaftarDropdown` (kotak tinggi yang menutupi area tempat 3 item tersebut akan muncul).
2. Saat pemain mengarahkan laser VR ke `Collider_Header` dan mengklik, skrip memanggil `dropdown.Show()` dan mengaktifkan `Collider_DaftarDropdown`.
3. Pemain mengarahkan laser ke sepertiga bagian bawah dari daftar yang terbuka lalu mengklik.
4. Skrip menghitung posisi klik pada sumbu Y, menentukan bahwa posisi tersebut milik Indeks `2`, memilih opsi `Sulit`, menutup dropdown, dan menyembunyikan list collider kembali.

---

### Inspector Variables Reference

| Variable Name        | Type                   | Description                                                                                                                                            |
| :------------------- | :--------------------- | :----------------------------------------------------------------------------------------------------------------------------------------------------- |
| `dropdown`           | `TMP_Dropdown`         | The target TextMeshPro Dropdown component. / _Komponen TMP_Dropdown target._                                                                           |
| `headerCollider`     | `BoxCollider`          | Physical collider covering the main dropdown button. / _Collider fisik yang menutupi tombol utama dropdown._                                           |
| `listCollider`       | `BoxCollider`          | Large physical collider covering the expanded dropdown template/list area. / _Collider besar yang menutupi seluruh area daftar dropdown saat terbuka._ |
| `leftRayInteractor`  | `XRRayInteractor`      | Reference to the Left Hand Ray Interactor. / _Referensi ke Ray Interactor Kiri._                                                                       |
| `rightRayInteractor` | `XRRayInteractor`      | Reference to the Right Hand Ray Interactor. / _Referensi ke Ray Interactor Kanan._                                                                     |
| `leftSelectAction`   | `InputActionReference` | VR Input action for left hand click/trigger. / _Input Action trigger untuk tangan Kiri._                                                               |
| `rightSelectAction`  | `InputActionReference` | VR Input action for right hand click/trigger. / _Input Action trigger untuk tangan Kanan._                                                             |
| `showDebug`          | `bool`                 | Toggles debug logs in the console. / _Menyalakan/mematikan log debug di konsol._                                                                       |
