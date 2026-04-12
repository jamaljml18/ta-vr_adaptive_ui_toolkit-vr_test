# CurvedPhysicalUIInputFieldHandlerHT

## 🇬🇧 English Documentation

### Overview

`CurvedPhysicalUIInputFieldHandlerHT` is a script designed to handle VR interactions for Canvas `TMP_InputField` elements—specifically curved interfaces—using a **"Physical Proxy"** approach, optimized for **Hand Tracking**.

Standard Unity input fields in VR often trigger the native OS keyboard (like the Meta Quest or Pico default keyboard) when clicked. This can break immersion, especially in custom 3D environments. This script solves that by placing an invisible 3D Collider over your Input Field. When the user points their XR Ray Interactor using their bare hands and performs a **Pinch Gesture**, the script safely focuses the UI and triggers your own Custom 3D Keyboard instead, while forcefully suppressing the native OS keyboard. It is built to be fully compatible with **XR Interaction Toolkit (XRI) 3.0+**.

### Features

- **Hand Tracking Optimized:** Native support for bare-hand pinch gestures instead of physical controller triggers.
- **XRI 3.0+ Ready:** Utilizes the updated Input System architecture (dynamically enabling actions) required by newer Unity XR toolkits.
- **Native OS Keyboard Suppression:** Automatically applies the `readOnly = true` hack to prevent the headset's default 2D keyboard from popping up and ruining immersion, while keeping the input field visually active (blinking cursor).
- **Custom Keyboard Integration:** Directly links to your own `KeyboardInputFieldManager` to spawn a custom, immersive 3D keyboard in the VR world.
- **Dual Wielding Support:** Independently tracks and processes pinch inputs from both Left and Right hands.
- **Editor Visualization:** Draws a magenta debug box in the Scene view to help you perfectly align the proxy collider.

### How to Use

1. **Prepare the UI:** Create your World Space Canvas and add a `TMP_InputField`.
2. **Create the Proxy:** Create an empty GameObject with a `BoxCollider`. Position and scale it so it perfectly covers the interactive text area of your Input Field.
3. **Attach Script:** Attach the `CurvedPhysicalUIInputFieldHandlerHT` script to a manager object (e.g., the Canvas itself or the Input Field).
4. **Map References:** - Assign the `TMP_InputField` to the _Input Field_ slot.
   - Assign the `BoxCollider` to the _Input Collider_ slot.
   - Assign your custom 3D keyboard manager to the _Custom Keyboard Manager_ slot.
5. **Assign VR Settings:** - Drag and drop your Left and Right `XRRayInteractor` components.
   - For `Left Select Action` and `Right Select Action`, assign the specific pinch/select actions (e.g., `XRI LeftHand Interaction/Select`).

### Example Scenario

**Use Case:** You have a "Login Screen" where the player needs to enter their Username.

1. You create a transparent 3D cube called `Collider_Username` and place it over the Username text box.
2. The player points their bare-hand VR laser at the Username box and **performs a pinch gesture**.
3. The script detects the pinch, sets the Input Field to focused (so the cursor starts blinking), ensures the headset's native keyboard stays hidden, and tells your custom 3D holographic keyboard to appear in front of the player.

---

---

## 🇮🇩 Dokumentasi Bahasa Indonesia

### Ikhtisar

`CurvedPhysicalUIInputFieldHandlerHT` adalah skrip yang dirancang untuk menangani interaksi VR pada elemen Canvas `TMP_InputField`—khususnya antarmuka yang melengkung (curved)—menggunakan pendekatan **"Physical Proxy"** yang dioptimalkan untuk **Hand Tracking (Pelacakan Tangan)**.

Input field standar Unity di VR sering kali memicu munculnya keyboard bawaan sistem operasi (seperti keyboard default Meta Quest atau Pico) saat diklik. Hal ini bisa merusak imersi pemain. Skrip ini mengatasinya dengan menempatkan Collider 3D di atas Input Field. Ketika pengguna mengarahkan laser dari tangan kosong mereka dan melakukan **Gestur Mencubit (Pinch)**, skrip akan memfokuskan UI dan memunculkan Keyboard 3D Kustom buatan Anda sendiri, sekaligus memblokir kemunculan keyboard bawaan OS. Skrip ini dirancang agar kompatibel penuh dengan **XR Interaction Toolkit (XRI) 3.0+**.

### Fitur Utama

- **Optimasi Hand Tracking:** Dukungan bawaan untuk membaca gestur cubitan tangan kosong, bukan sekadar pelatuk kontroler fisik.
- **Kompatibel XRI 3.0+:** Menggunakan arsitektur Input System terbaru (mengaktifkan action secara dinamis) yang diwajibkan oleh versi Unity XR modern.
- **Pemblokir Keyboard Bawaan OS:** Secara otomatis menerapkan trik `readOnly = true` untuk mencegah keyboard 2D bawaan headset muncul, namun tetap menjaga agar kolom input terlihat aktif (kursor tetap berkedip).
- **Integrasi Keyboard Kustom:** Terhubung langsung dengan `KeyboardInputFieldManager` Anda untuk memunculkan keyboard 3D yang imersif di dalam dunia VR.
- **Dukungan Dua Tangan:** Melacak dan memproses gestur cubitan dari tangan Kiri dan Kanan secara independen.
- **Visualisasi Editor:** Menampilkan kotak debug berwarna magenta di Scene view untuk membantu Anda mencocokkan ukuran collider dengan tepat.

### Cara Penggunaan

1. **Siapkan UI:** Buat Canvas (World Space) dan tambahkan `TMP_InputField`.
2. **Buat Proxy:** Buat GameObject kosong dengan komponen `BoxCollider`. Atur posisi dan ukurannya agar menutupi area teks pada Input Field Anda.
3. **Pasang Skrip:** Pasangkan skrip `CurvedPhysicalUIInputFieldHandlerHT` pada objek manajer (misalnya, di Canvas itu sendiri atau pada Input Field).
4. **Hubungkan Referensi:** - Masukkan komponen `TMP_InputField` ke kolom _Input Field_.
   - Masukkan `BoxCollider` yang baru dibuat ke kolom _Input Collider_.
   - Masukkan manager keyboard 3D Anda ke kolom _Custom Keyboard Manager_.
5. **Atur Pengaturan VR:** - Masukkan referensi `XRRayInteractor` Kiri dan Kanan.
   - Pada kolom `Left Select Action` dan `Right Select Action`, pilih Input Action yang membaca gestur cubitan (misalnya: `XRI LeftHand Interaction/Select`).

### Contoh Skenario Penggunaan

**Kasus:** Anda memiliki "Layar Login" di mana pemain harus memasukkan Username.

1. Anda membuat kubus 3D transparan bernama `Collider_Username` dan meletakkannya di atas kotak teks Username.
2. Pemain mengarahkan laser dari tangan kosong mereka ke kotak Username lalu **melakukan gestur mencubit**.
3. Skrip mendeteksi cubitan tersebut, memfokuskan Input Field (kursor mulai berkedip), memastikan keyboard bawaan headset tidak muncul, lalu memerintahkan keyboard 3D holografik kustom Anda untuk muncul di hadapan pemain.

---

### Inspector Variables Reference

| Variable Name           | Type                        | Description                                                                                                                                                   |
| :---------------------- | :-------------------------- | :------------------------------------------------------------------------------------------------------------------------------------------------------------ |
| `inputField`            | `TMP_InputField`            | The target TextMeshPro Input Field component. / _Komponen TMP_InputField target._                                                                             |
| `inputCollider`         | `BoxCollider`               | Physical collider covering the text input area. / _Collider fisik yang menutupi area teks input._                                                             |
| `leftRayInteractor`     | `XRRayInteractor`           | Reference to the Left Hand Ray Interactor. / _Referensi ke Ray Interactor Kiri._                                                                              |
| `rightRayInteractor`    | `XRRayInteractor`           | Reference to the Right Hand Ray Interactor. / _Referensi ke Ray Interactor Kanan._                                                                            |
| `leftSelectAction`      | `InputActionReference`      | VR Input action for left hand pinch gesture (e.g., XRI LeftHand Interaction/Select). / _Input Action gestur cubitan Kiri._                                    |
| `rightSelectAction`     | `InputActionReference`      | VR Input action for right hand pinch gesture (e.g., XRI RightHand Interaction/Select). / _Input Action gestur cubitan Kanan._                                 |
| `customKeyboardManager` | `KeyboardInputFieldManager` | Reference to your script that handles spawning/managing the custom 3D VR keyboard. / _Referensi ke skrip yang mengatur pemanggilan keyboard 3D kustom di VR._ |
