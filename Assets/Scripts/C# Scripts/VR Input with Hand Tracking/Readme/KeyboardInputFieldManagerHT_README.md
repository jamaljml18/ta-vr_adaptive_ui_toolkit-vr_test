# KeyboardInputFieldManagerHT

## 🇬🇧 English Documentation

### Overview

`KeyboardInputFieldManagerHT` is a script designed to act as the core brain for a **Custom 3D VR Keyboard**, specifically optimized for **Hand Tracking**.

When developing VR applications, relying on the headset's native OS keyboard (like the Meta Quest or Pico system keyboard) can break immersion, block UI elements, or fail entirely when using curved world-space canvases. This script solves that by allowing you to build your own physical 3D keyboard inside the game world that is operated by bare hands. It maps invisible 3D Box Colliders (acting as physical keys) to typing actions triggered by a **Pinch Gesture**, pushing text directly into a target `TMP_InputField`. It is built to be fully compatible with **XR Interaction Toolkit (XRI) 3.0+**.

### Features

- **Hand Tracking Optimized:** Native support for bare-hand pinch gestures instead of physical controller triggers.
- **XRI 3.0+ Ready:** Utilizes the updated Input System architecture (dynamically enabling actions) required by newer Unity XR toolkits.
- **Physical Proxy Keys:** Maps 3D Colliders directly to specific keyboard functions (Characters, Space, Backspace, Enter, Tab, etc.).
- **Dual Wielding Support:** Players can type naturally using pinch gestures with both Left and Right hands simultaneously.
- **Smart Modifiers:** Includes built-in logic for `Shift` and `CapsLock`, including translating standard number keys into symbols (e.g., `1` to `!`) based on standard US Keyboard layouts.
- **Caret (Cursor) Management:** Forces the Unity EventSystem to keep focus on the Input Field so the blinking cursor remains visible while typing.
- **Multi-line Support:** Automatically detects if the target Input Field is single-line (triggers submit/close on Enter) or multi-line (adds a line break on Enter).

### How to Use

1. **Prepare the Keyboard Visuals:** Create a 3D model representing a keyboard. Group all keys under a single parent GameObject.
2. **Create Key Proxies:** Add a `BoxCollider` to every single key on your keyboard.
3. **Attach Script:** Attach the `KeyboardInputFieldManagerHT` script to a manager object (or the keyboard parent).
4. **Map the Keys:** - Open the `Keyboard Keys` array in the Inspector.
   - For every key, assign its `Collider`.
   - Select its `Function` (e.g., `Character`, `Backspace`, `Space`).
   - If the function is `Character`, type the corresponding letter or symbol in the `Character Value` field (use lowercase).
5. **Assign VR Settings:** - Assign the keyboard parent GameObject to the `Keyboard Visual Container`.
   - Drag and drop your Left and Right `XRRayInteractor` components.
   - For `Left Select Action` and `Right Select Action`, assign the specific pinch/select actions (e.g., `XRI LeftHand Interaction/Select`).

### Example Scenario

**Use Case:** A player needs to type their name using Hand Tracking.

1. You build a 3D QWERTY keyboard and map all the colliders using this script.
2. The player points their bare-hand laser at the text box and **pinches**. The custom 3D keyboard appears.
3. The player uses their left hand to **pinch** the `S` key collider, and their right hand to **pinch** the `A` key collider.
4. The script catches the hand raycast hits, identifies the mapped characters, handles the string insertion, and instantly updates the `TMP_InputField` to show "sa".
5. The player **pinches** the `Enter` key collider. The script fires the submit event and safely hides the keyboard visuals.

---

---

## 🇮🇩 Dokumentasi Bahasa Indonesia

### Ikhtisar

`KeyboardInputFieldManagerHT` adalah skrip yang dirancang untuk bertindak sebagai otak utama dari **Keyboard VR 3D Kustom** yang dioptimalkan khusus untuk **Hand Tracking (Pelacakan Tangan)**.

Saat mengembangkan aplikasi VR, mengandalkan keyboard OS bawaan headset dapat merusak imersi atau gagal berfungsi pada Canvas ruang dunia yang melengkung. Skrip ini memungkinkan Anda membangun keyboard 3D fisik sendiri yang dioperasikan dengan tangan kosong. Skrip ini memetakan Box Collider 3D ke fungsi pengetikan yang dipicu oleh **Gestur Mencubit (Pinch)**, lalu memasukkan teks secara langsung ke dalam `TMP_InputField` target. Skrip ini dirancang agar kompatibel penuh dengan **XR Interaction Toolkit (XRI) 3.0+**.

### Fitur Utama

- **Optimasi Hand Tracking:** Dukungan bawaan untuk membaca gestur cubitan tangan kosong, bukan sekadar pelatuk kontroler fisik.
- **Kompatibel XRI 3.0+:** Menggunakan arsitektur Input System terbaru (mengaktifkan action secara dinamis) yang diwajibkan oleh versi Unity XR modern.
- **Tombol Proxy Fisik:** Memetakan Collider 3D langsung ke fungsi keyboard spesifik (Karakter, Spasi, Backspace, Enter, Tab, dll.).
- **Dukungan Dua Tangan:** Pemain dapat mengetik secara natural menggunakan gestur mencubit dari tangan Kiri dan Kanan secara bersamaan.
- **Modifier Cerdas:** Dilengkapi logika bawaan untuk `Shift` dan `CapsLock`, termasuk menerjemahkan tombol angka standar menjadi simbol berdasarkan tata letak Keyboard AS.
- **Manajemen Kursor (Caret):** Memaksa Unity EventSystem untuk tetap fokus pada Input Field sehingga kursor tetap terlihat saat mengetik.
- **Dukungan Multi-baris:** Mendeteksi secara otomatis apakah Input Field target adalah satu baris atau multi-baris.

### Cara Penggunaan

1. **Siapkan Visual Keyboard:** Buat model 3D keyboard. Masukkan semua tombol ke dalam satu GameObject induk (parent).
2. **Buat Proxy Tombol:** Tambahkan `BoxCollider` pada setiap tombol di keyboard Anda.
3. **Pasang Skrip:** Pasangkan skrip `KeyboardInputFieldManagerHT` pada objek manajer.
4. **Petakan Tombol:** - Buka array `Keyboard Keys` di Inspector.
   - Masukkan `Collider` untuk setiap tombol.
   - Pilih `Function`-nya (misal: `Character`, `Backspace`, `Space`).
   - Jika fungsinya `Character`, isi kolom `Character Value` dengan huruf/simbol yang sesuai (huruf kecil).
5. **Hubungkan Referensi:** - Masukkan GameObject induk keyboard ke `Keyboard Visual Container`.
   - Masukkan `XRRayInteractor` Kiri dan Kanan.
   - Pada kolom `Left Select Action` dan `Right Select Action`, pilih Input Action yang membaca gestur cubitan (misalnya: `XRI LeftHand Interaction/Select`).

---

### Inspector Variables Reference

| Variable Name             | Type                   | Description                                                                                                                                      |
| :------------------------ | :--------------------- | :----------------------------------------------------------------------------------------------------------------------------------------------- |
| `keyboardVisualContainer` | `GameObject`           | The parent object containing all keyboard visuals. Toggled on/off automatically. / _Objek induk visual keyboard. Dinyalakan/dimatikan otomatis._ |
| `leftRayInteractor`       | `XRRayInteractor`      | Reference to the Left Hand Ray Interactor. / _Referensi ke Ray Interactor Kiri._                                                                 |
| `rightRayInteractor`      | `XRRayInteractor`      | Reference to the Right Hand Ray Interactor. / _Referensi ke Ray Interactor Kanan._                                                               |
| `leftSelectAction`        | `InputActionReference` | VR Input action for left hand pinch gesture (e.g., XRI LeftHand Interaction/Select). / _Input Action gestur cubitan tangan Kiri._                |
| `rightSelectAction`       | `InputActionReference` | VR Input action for right hand pinch gesture (e.g., XRI RightHand Interaction/Select). / _Input Action gestur cubitan tangan Kanan._             |
| `keyboardKeys`            | `List<KeyDefinition>`  | Mapping of physical colliders to functions and characters. / _Pemetaan collider fisik ke fungsi tombol dan karakternya._                         |
