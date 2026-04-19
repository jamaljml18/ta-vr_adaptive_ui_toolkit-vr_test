# CurvedPhysicalUIInputFieldHandler

## 🇬🇧 English Documentation

### Overview

`CurvedPhysicalUIInputFieldHandler` is a script designed to handle VR interactions for Canvas `TMP_InputField` elements—specifically curved interfaces—using a **"Physical Proxy"** approach.

Text inputs in VR often trigger disruptive native OS keyboards (like the Android, Pico, or Meta Quest system keyboards). This script solves that problem by using an invisible 3D Box Collider as a proxy. When the VR Ray Interactor clicks the proxy, the script safely focuses the UI to show the blinking cursor and seamlessly summons a **Custom 3D VR Keyboard** (`KeyboardInputFieldManager`) instead of the system keyboard.

### Features

- **Physical Proxy Mapping:** Maps a 3D Box Collider directly to a 2D `TMP_InputField`.
- **Native OS Keyboard Prevention:** Automatically sets the input field to Read-Only and hides mobile input to prevent disruptive system keyboards from popping up, while keeping the visual caret (cursor) active.
- **Custom Keyboard Integration:** Directly links to and opens your custom VR keyboard manager.
- **Dual Wielding Support:** Independently tracks and processes inputs from both Left and Right VR controllers.
- **Editor Visualization:** Draws a magenta debug outline in the Scene view for the input collider to make positioning easy.

### How to Use

1. **Prepare the UI:** Create your World Space Canvas and add a TextMeshPro Input Field (`TMP_InputField`).
2. **Create the Proxy:** Create an empty GameObject with a `BoxCollider` component. Position and scale it so it perfectly covers the interactive area of your Input Field. (Remove the `MeshRenderer` so it remains invisible).
3. **Attach Script:** Attach the `CurvedPhysicalUIInputFieldHandler` script to a manager object (e.g., the Canvas itself or the Input Field object).
4. **Link References:** - Assign the `TMP_InputField` to the `Input Field` slot.
   - Assign your 3D proxy to the `Input Collider` slot.
   - Assign the GameObject containing your `KeyboardInputFieldManager` script to the `Custom Keyboard Manager` slot.
5. **Assign VR Settings:** Drag and drop your Left and Right `XRRayInteractor` components and assign the select `InputActionReference` (trigger button) for both hands.

### Example Scenario

**Use Case:** You have a login screen asking for the player's username via `Username_InputField`.

1. You create a transparent 3D cube named `Collider_Username` and place it exactly over the visual text box.
2. In the Inspector for this script:
   - Drop `Username_InputField` into the **Input Field** variable.
   - Drop `Collider_Username` into the **Input Collider** variable.
   - Drop your custom 3D Keyboard GameObject into the **Custom Keyboard Manager** variable.
3. When the player points their VR laser at the `Collider_Username` and pulls the trigger, the script prevents the headset's native keyboard from appearing. Instead, it makes the cursor blink inside the text box and summons your custom 3D VR keyboard right in front of the player.

---

---

## 🇮🇩 Dokumentasi Bahasa Indonesia

### Ikhtisar

`CurvedPhysicalUIInputFieldHandler` adalah skrip yang dirancang untuk menangani interaksi VR pada elemen Canvas `TMP_InputField`—khususnya antarmuka yang melengkung (curved)—menggunakan pendekatan **"Physical Proxy"**.

Input teks di VR sering kali memicu munculnya keyboard bawaan sistem operasi (OS) yang mengganggu imersi (seperti keyboard bawaan Android, Pico, atau Meta Quest). Skrip ini memecahkan masalah tersebut menggunakan Box Collider 3D transparan sebagai perantara. Ketika XR Ray Interactor mengklik collider tersebut, skrip akan memfokuskan UI untuk menampilkan kursor yang berkedip, dan secara mulus memanggil **Keyboard 3D Kustom** (`KeyboardInputFieldManager`) alih-alih keyboard sistem.

### Fitur Utama

- **Physical Proxy Mapping:** Menghubungkan Collider 3D fisik dengan elemen `TMP_InputField` 2D secara langsung.
- **Pencegahan Keyboard Bawaan OS:** Secara otomatis mengatur Input Field menjadi _Read-Only_ dan menyembunyikan input mobile untuk mencegah keyboard sistem OS muncul, namun tetap mempertahankan kursor (caret) visual yang berkedip.
- **Integrasi Keyboard Kustom:** Terhubung langsung dan membuka manajer keyboard VR 3D buatan Anda sendiri.
- **Dukungan Dua Tangan (Dual Wielding):** Melacak dan memproses input dari kontroler VR Kiri dan Kanan secara independen.
- **Visualisasi Editor:** Menampilkan garis bingkai berwarna magenta (Gizmos) di Scene view pada area collider untuk mempermudah pengaturan posisi.

### Cara Penggunaan

1. **Siapkan UI:** Buat Canvas (World Space) dan tambahkan TextMeshPro Input Field (`TMP_InputField`).
2. **Buat Proxy:** Buat GameObject kosong yang memiliki komponen `BoxCollider`. Atur posisi dan ukurannya agar menutupi area kotak input teks Anda secara presisi. (Hapus komponen `MeshRenderer` agar tidak terlihat).
3. **Pasang Skrip:** Pasangkan skrip `CurvedPhysicalUIInputFieldHandler` pada objek manajer (misalnya, di Canvas itu sendiri atau objek Input Field).
4. **Hubungkan Referensi:**
   - Masukkan `TMP_InputField` ke slot `Input Field`.
   - Masukkan proxy 3D Anda ke slot `Input Collider`.
   - Masukkan GameObject yang memiliki skrip `KeyboardInputFieldManager` ke slot `Custom Keyboard Manager`.
5. **Atur Pengaturan VR:** Masukkan referensi `XRRayInteractor` Kiri dan Kanan, lalu tentukan `InputActionReference` (tombol trigger/select) untuk masing-masing tangan.

### Contoh Skenario Penggunaan

**Kasus:** Anda memiliki layar login yang meminta nama pengguna pemain melalui `InputField_Username`.

1. Anda membuat kubus 3D transparan bernama `Collider_Username` dan meletakkannya persis menutupi kotak teks visual.
2. Pada Inspector skrip ini:
   - Tarik `InputField_Username` ke variabel **Input Field**.
   - Tarik `Collider_Username` ke variabel **Input Collider**.
   - Tarik GameObject Keyboard 3D Kustom Anda ke variabel **Custom Keyboard Manager**.
3. Saat pemain mengarahkan laser VR ke `Collider_Username` dan menekan _trigger_, skrip akan mencegah keyboard bawaan headset muncul. Sebagai gantinya, kursor akan mulai berkedip di dalam kotak teks dan keyboard VR 3D kustom Anda akan muncul tepat di hadapan pemain untuk mulai mengetik.

---

### Inspector Variables Reference

| Variable Name           | Type                        | Description                                                                                                                                 |
| :---------------------- | :-------------------------- | :------------------------------------------------------------------------------------------------------------------------------------------ |
| `inputField`            | `TMP_InputField`            | The target TextMeshPro Input Field component. / _Komponen TMP_InputField target._                                                           |
| `inputCollider`         | `BoxCollider`               | Physical 3D Collider covering the Input Field area. / _Collider 3D fisik yang menutupi area Input Field._                                   |
| `leftRayInteractor`     | `XRRayInteractor`           | Reference to the Left Hand Ray Interactor. / _Referensi ke Ray Interactor Kiri._                                                            |
| `rightRayInteractor`    | `XRRayInteractor`           | Reference to the Right Hand Ray Interactor. / _Referensi ke Ray Interactor Kanan._                                                          |
| `leftSelectAction`      | `InputActionReference`      | VR Input action for left hand click/trigger. / _Input Action trigger untuk tangan Kiri._                                                    |
| `rightSelectAction`     | `InputActionReference`      | VR Input action for right hand click/trigger. / _Input Action trigger untuk tangan Kanan._                                                  |
| `customKeyboardManager` | `KeyboardInputFieldManager` | Reference to the script handling your custom 3D VR Keyboard logic. / _Referensi ke skrip yang menangani logika Keyboard VR 3D kustom Anda._ |
