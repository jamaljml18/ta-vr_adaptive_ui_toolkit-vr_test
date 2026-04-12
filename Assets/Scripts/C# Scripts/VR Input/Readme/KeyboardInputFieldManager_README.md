# KeyboardInputFieldManager

## 🇬🇧 English Documentation

### Overview

`KeyboardInputFieldManager` is a script designed to act as the core brain for a **Custom 3D VR Keyboard**.

When developing VR applications, relying on the headset's native OS keyboard (like the Meta Quest or Pico system keyboard) can break immersion, block UI elements, or fail entirely when using curved world-space canvases. This script solves that by allowing you to build your own physical 3D keyboard inside the game world. It maps invisible 3D Box Colliders (acting as physical keys) to typing actions, pushing text directly into a target `TMP_InputField`.

### Features

- **Physical Proxy Keys:** Maps 3D Colliders directly to specific keyboard functions (Characters, Space, Backspace, Enter, Tab, etc.).
- **Dual Wielding Support:** Players can type naturally using both Left and Right VR controllers simultaneously.
- **Smart Modifiers:** Includes built-in logic for `Shift` and `CapsLock`, including translating standard number keys into symbols (e.g., `1` to `!`) based on standard US Keyboard layouts.
- **Caret (Cursor) Management:** Forces the Unity EventSystem to keep focus on the Input Field so the blinking cursor remains visible while typing.
- **Multi-line Support:** Automatically detects if the target Input Field is single-line (triggers submit/close on Enter) or multi-line (adds a line break on Enter).

### How to Use

1. **Prepare the Keyboard Visuals:** Create a 3D model or a Canvas-based UI representing a keyboard. Group them all under a single parent GameObject.
2. **Create Key Proxies:** Add a `BoxCollider` to every single key on your keyboard.
3. **Attach Script:** Attach the `KeyboardInputFieldManager` script to a manager object (or the keyboard parent).
4. **Map the Keys:** - Open the `Keyboard Keys` array in the Inspector.
   - For every key, assign its `Collider`.
   - Select its `Function` (e.g., `Character`, `Backspace`, `Space`).
   - If the function is `Character`, type the corresponding letter or symbol in the `Character Value` field (use lowercase).
5. **Assign References:** - Assign the keyboard parent GameObject to the `Keyboard Visual Container`.
   - Drag and drop your Left and Right `XRRayInteractor` and their `InputActionReference` (trigger buttons).

### Example Scenario

**Use Case:** A player needs to type their name into a high-score board.

1. You build a 3D QWERTY keyboard and map all the colliders using this script.
2. You use the `CurvedPhysicalUIInputFieldHandler` on your text box, linking it to this `KeyboardInputFieldManager`.
3. The player points their laser at the text box and clicks. The custom 3D keyboard appears.
4. The player uses their left hand to click the `S` key collider, and their right hand to click the `A` key collider.
5. The script catches the raycast hits, identifies the mapped characters, handles the string insertion at the current caret position, and instantly updates the `TMP_InputField` to show "sa".
6. The player clicks the `Enter` key collider. The script fires the submit event and safely hides the keyboard visuals.

---

---

## 🇮🇩 Dokumentasi Bahasa Indonesia

### Ikhtisar

`KeyboardInputFieldManager` adalah skrip yang dirancang untuk bertindak sebagai otak utama dari **Keyboard VR 3D Kustom**.

Saat mengembangkan aplikasi VR, mengandalkan keyboard OS bawaan headset (seperti keyboard sistem Meta Quest atau Pico) dapat merusak imersi, menghalangi elemen UI, atau gagal berfungsi pada Canvas ruang dunia yang melengkung. Skrip ini mengatasi masalah tersebut dengan memungkinkan Anda membangun keyboard 3D fisik Anda sendiri di dalam dunia game. Skrip ini memetakan Box Collider 3D (yang bertindak sebagai tombol fisik) ke fungsi pengetikan, lalu memasukkan teks secara langsung ke dalam `TMP_InputField` target.

### Fitur Utama

- **Tombol Proxy Fisik:** Memetakan Collider 3D langsung ke fungsi keyboard spesifik (Karakter, Spasi, Backspace, Enter, Tab, dll.).
- **Dukungan Dua Tangan (Dual Wielding):** Pemain dapat mengetik secara natural menggunakan kontroler VR Kiri dan Kanan secara bersamaan.
- **Modifier Cerdas:** Dilengkapi logika bawaan untuk `Shift` dan `CapsLock`, termasuk menerjemahkan tombol angka standar menjadi simbol (misalnya, `1` menjadi `!`) berdasarkan tata letak Keyboard AS standar.
- **Manajemen Kursor (Caret):** Memaksa Unity EventSystem untuk tetap fokus pada Input Field sehingga kursor yang berkedip tetap terlihat saat pemain mengetik.
- **Dukungan Multi-baris:** Mendeteksi secara otomatis apakah Input Field target adalah satu baris (memicu submit/tutup saat Enter) atau multi-baris (menambahkan baris baru saat Enter).

### Cara Penggunaan

1. **Siapkan Visual Keyboard:** Buat model 3D atau UI berbasis Canvas yang merepresentasikan sebuah keyboard. Masukkan semuanya ke dalam satu GameObject induk (parent).
2. **Buat Proxy Tombol:** Tambahkan `BoxCollider` pada setiap tombol di keyboard Anda.
3. **Pasang Skrip:** Pasangkan skrip `KeyboardInputFieldManager` pada objek manajer (atau di induk keyboard).
4. **Petakan Tombol:** - Buka array `Keyboard Keys` di Inspector.
   - Untuk setiap tombol, masukkan `Collider`-nya.
   - Pilih `Function`-nya (misal: `Character`, `Backspace`, `Space`).
   - Jika fungsinya adalah `Character`, ketik huruf atau simbol yang sesuai di kolom `Character Value` (gunakan huruf kecil).
5. **Hubungkan Referensi:** - Masukkan GameObject induk keyboard ke `Keyboard Visual Container`.
   - Tarik dan masukkan `XRRayInteractor` Kiri dan Kanan Anda beserta `InputActionReference` (tombol trigger) masing-masing.

### Contoh Skenario Penggunaan

**Kasus:** Pemain harus mengetikkan nama mereka ke dalam papan skor tertinggi.

1. Anda membuat keyboard QWERTY 3D dan memetakan semua collider menggunakan skrip ini.
2. Anda menggunakan `CurvedPhysicalUIInputFieldHandler` pada kotak teks Anda, dan menghubungkannya dengan `KeyboardInputFieldManager` ini.
3. Pemain mengarahkan lasernya ke kotak teks dan mengklik. Keyboard 3D kustom akan muncul.
4. Pemain menggunakan tangan kiri untuk mengklik collider tombol `S`, dan tangan kanan untuk mengklik collider tombol `A`.
5. Skrip menangkap benturan raycast, mengidentifikasi karakter yang dipetakan, menyisipkan teks pada posisi kursor saat ini, dan langsung memperbarui `TMP_InputField` untuk menampilkan "sa".
6. Pemain mengklik collider tombol `Enter`. Skrip akan memicu event _submit_ dan menyembunyikan visual keyboard secara otomatis.

---

### Inspector Variables Reference

| Variable Name             | Type                   | Description                                                                                                                                                          |
| :------------------------ | :--------------------- | :------------------------------------------------------------------------------------------------------------------------------------------------------------------- |
| `keyboardVisualContainer` | `GameObject`           | The parent object containing all keyboard visuals. Toggled on/off automatically. / _Objek induk yang berisi semua visual keyboard. Dinyalakan/dimatikan otomatis._   |
| `leftRayInteractor`       | `XRRayInteractor`      | Reference to the Left Hand Ray Interactor. / _Referensi ke Ray Interactor Kiri._                                                                                     |
| `rightRayInteractor`      | `XRRayInteractor`      | Reference to the Right Hand Ray Interactor. / _Referensi ke Ray Interactor Kanan._                                                                                   |
| `leftSelectAction`        | `InputActionReference` | VR Input action for left hand click/trigger. / _Input Action trigger untuk tangan Kiri._                                                                             |
| `rightSelectAction`       | `InputActionReference` | VR Input action for right hand click/trigger. / _Input Action trigger untuk tangan Kanan._                                                                           |
| `keyboardKeys`            | `List<KeyDefinition>`  | The list mapping every physical collider to its key function and string character. / _Daftar yang memetakan setiap collider fisik ke fungsi tombol dan karakternya._ |
