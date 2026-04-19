# SpatialUIController

## 🇬🇧 English Documentation

### Overview

`SpatialUIController` is a Unity script designed to manage Spatial (floating) UI in VR environments. It allows UI elements to elegantly follow the player's view (camera) with customizable offsets, axis locking, and smooth trailing effects. It also comes with built-in VR Input System integration to display, hide, and instantly recenter the UI directly in front of the user.

### Features

- **Dynamic Follow Tracking:** Choose specifically which axes (X, Y, Z) the UI should track relative to the camera.
- **Smooth Movement:** Utilizes `Vector3.SmoothDamp` to create a natural, lag-like trailing effect as the player moves.
- **Auto-Offset Calculation:** Automatically calculates and maintains the initial relative distance between the camera and the UI.
- **Face Camera Toggle:** Automatically rotates the UI so it always remains perfectly readable and faces the user.
- **Native VR Input Integration:** Direct slots for `InputActionReference` to trigger showing, hiding, and recentering the UI without needing extra script logic.

### How to Use

1. **Prepare the UI:** - Create an empty GameObject named `Curved UI`.
   - Inside it, create a child GameObject named `Curved Mesh` and a Camera named `UI Camera`.
   - Attach the `SpatialUIController` script to the `Curved UI` object.
   - Attach the `CurvedUI` script to the `Curved Mesh` object.
   - Configure the `UI Camera`: Set **Projection** to **Orthographic**, **Culling Mask** to **UI only**, **Background Type** to **Solid Color** (Hex `#314D79`), and assign your prepared Render Texture to the **Output Texture** field. Remember to remove the **Audio Listener** component from this camera.
   - On your Canvas, assign the `UI Camera` to the **Render Camera** slot.
   - On the `Curved Mesh`, create **Box Colliders** and adjust their positions and sizes to match the interactive elements (buttons, toggles, sliders, scrollbars, scrollviews, dropdowns, input fields) located within the Canvas children.
   - _Note: For more detailed usage and setup, please refer to the provided `Example Scene` and `Example Scene with Hand Tracking`._
2. **Assign Camera (Optional):** Assign your main VR Camera to the `Camera Transform` field in the `SpatialUIController`. If left blank, the script will automatically find `Camera.main`.
3. **Position the UI:** Move your `Curved UI` object in the Scene View to your desired offset (e.g., slightly lower and forward from the camera). The script will automatically save this relative position on `Start`.
4. **Configure Settings:**
   - Check `Follow X`, `Follow Y`, and `Follow Z` depending on how you want the UI to move.
   - Adjust `Smooth Time` (e.g., `0.1` to `0.3`) for comfortable movement delay.
   - Check `Always Face Camera` to prevent the UI from becoming unreadable at odd angles.
5. **Assign VR Inputs:** Drag and drop your desired Input Actions into `Displays UI Input`, `Hide UI Input`, and `Recenter UI Input` to bind them to your VR controller buttons.

### Example Scenario

**Use Case:** You are building an immersive VR simulation where the user has a "Floating Tool Menu".

1. You place the menu canvas 0.5 meters in front of the player and slightly below eye level.
2. You enable `Follow X`, `Follow Y`, `Follow Z`, and `Always Face Camera`, with a `Smooth Time` of `0.15`.
3. As the player walks around the virtual room, the Tool Menu smoothly floats along with them, always staying in their lower peripheral vision.
4. If the player wants to hide the menu, they press the "B" button on their right controller (assigned to `Hide UI Input`). If they need it back directly in their line of sight, they press the "A" button (assigned to `Recenter UI Input`), which instantly snaps the menu to the center of their view.

---

## 🇮🇩 Dokumentasi Bahasa Indonesia

### Ikhtisar

`SpatialUIController` adalah skrip Unity yang dirancang untuk mengelola UI Spasial (melayang) di lingkungan VR. Skrip ini memungkinkan elemen UI untuk mengikuti pandangan pemain (kamera) dengan elegan menggunakan pengaturan offset, penguncian sumbu (axis), dan efek gerakan yang halus. Skrip ini juga dilengkapi dengan integrasi VR Input System bawaan untuk menampilkan, menyembunyikan, dan mencenterkan ulang UI tepat di depan pengguna.

### Fitur Utama

- **Pelacakan Dinamis:** Anda dapat memilih sumbu mana saja (X, Y, Z) yang akan diikuti oleh UI secara spesifik.
- **Gerakan Halus (Smooth Movement):** Menggunakan `Vector3.SmoothDamp` untuk menciptakan efek gerakan yang natural dan sedikit lambat (efek lag) saat pemain bergerak.
- **Kalkulasi Offset Otomatis:** Secara otomatis menghitung dan mempertahankan jarak relatif awal antara kamera dan UI saat game dimulai.
- **Selalu Menghadap Kamera:** Secara otomatis memutar UI agar selalu dapat dibaca dengan jelas dan menghadap ke arah pengguna.
- **Integrasi Input VR Bawaan:** Menyediakan referensi `InputActionReference` langsung untuk memicu fungsi tampil, sembunyi, dan recenter UI tanpa memerlukan tambahan logika skrip.

### Cara Penggunaan

1. **Siapkan UI:**
   - Buat GameObject kosong bernama `Curved UI`.
   - Di dalamnya, buat anak GameObject bernama `Curved Mesh` dan sebuah Camera bernama `UI Camera`.
   - Tempelkan skrip `SpatialUIController` pada objek `Curved UI`.
   - Tempelkan skrip `CurvedUI` pada objek `Curved Mesh`.
   - Atur pengaturan `UI Camera`: Ubah **Projection** menjadi **Orthographic**, **Culling Mask** hanya centang **UI**, **Background Type** menjadi **Solid Color** dengan warna `#314D79`, dan pada bagian **Output Texture**, masukkan Render Texture yang telah disiapkan. Hapus juga komponen **Audio Listener** pada kamera ini.
   - Pada bagian Canvas, atur **Render Camera** dengan memasukkan `UI Camera` tersebut.
   - Pada objek `Curved Mesh`, buat collider menggunakan komponen **Box Collider**. Sesuaikan posisi dan ukurannya dengan tempat elemen interaktif (button, toggle, slider, scrollbar, scrollview, dropdown, inputfield) yang ada pada child dari Canvas.
   - _Catatan: Untuk lebih jelasnya mengenai cara penggunaan, dapat dilihat pada scene `Example Scene` dan `Example Scene with Hand Tracking`._
2. **Masukkan Kamera (Opsional):** Masukkan kamera VR utama Anda ke kolom `Camera Transform` pada skrip `SpatialUIController`. Jika dibiarkan kosong, skrip akan otomatis mencari `Camera.main`.
3. **Posisikan UI:** Pindahkan objek `Curved UI` di Scene View ke posisi offset yang Anda inginkan (misalnya, sedikit ke bawah dan ke depan kamera). Skrip akan otomatis menyimpan posisi relatif ini pada fungsi `Start`.
4. **Atur Pengaturan:**
   - Centang `Follow X`, `Follow Y`, dan `Follow Z` sesuai dengan arah pergerakan yang Anda inginkan.
   - Sesuaikan `Smooth Time` (misal: `0.1` hingga `0.3`) untuk mendapatkan jeda gerakan yang nyaman.
   - Centang `Always Face Camera` untuk mencegah UI sulit dibaca pada sudut tertentu.
5. **Hubungkan Input VR:** Masukkan Input Action yang diinginkan ke dalam `Displays UI Input`, `Hide UI Input`, dan `Recenter UI Input` untuk menyambungkannya ke tombol kontroler VR Anda.

### Contoh Skenario Penggunaan

**Kasus:** Anda sedang membuat simulasi VR imersif di mana pengguna memiliki "Menu Alat Melayang" (Floating Tool Menu).

1. Anda meletakkan canvas menu sejauh 0,5 meter di depan pemain dan sedikit di bawah pandangan mata.
2. Anda mengaktifkan `Follow X`, `Follow Y`, `Follow Z`, dan `Always Face Camera`, dengan `Smooth Time` sebesar `0.15`.
3. Saat pemain berjalan di sekitar ruangan virtual, Menu Alat akan melayang mengikuti mereka secara halus, dan selalu berada di batas bawah pandangan mereka.
4. Jika pemain ingin menyembunyikan menu, mereka menekan tombol "B" pada kontroler kanan (yang dimasukkan pada `Hide UI Input`). Jika mereka ingin menu tersebut kembali tepat di tengah pandangan, mereka menekan tombol "A" (pada `Recenter UI Input`), yang akan langsung memindahkan menu ke depan wajah pengguna secara instan.

---

### Inspector Variables Reference

| Variable Name      | Type                   | Description                                                                                                                                    |
| :----------------- | :--------------------- | :--------------------------------------------------------------------------------------------------------------------------------------------- |
| `cameraTransform`  | `Transform`            | Camera transform that the UI will follow. / _Transform kamera yang akan diikuti oleh UI._                                                      |
| `distanceX, Y, Z`  | `float`                | Offset relative to camera's orientation. Auto-calculated on Start. / _Offset relatif terhadap orientasi kamera. Dihitung otomatis saat Start._ |
| `followX`          | `bool`                 | If enabled, UI follows the camera's X-axis. / _Jika aktif, UI mengikuti posisi kamera pada sumbu X._                                           |
| `followY`          | `bool`                 | If enabled, UI follows the camera's Y-axis. / _Jika aktif, UI mengikuti posisi kamera pada sumbu Y._                                           |
| `followZ`          | `bool`                 | If enabled, UI follows the camera's Z-axis. / _Jika aktif, UI mengikuti posisi kamera pada sumbu Z._                                           |
| `smoothTime`       | `float`                | Higher values = slower/smoother movement. / _Semakin besar nilai, semakin lambat/halus pergerakannya._                                         |
| `alwaysFaceCamera` | `bool`                 | If enabled, the UI will always rotate to face the user. / _Jika aktif, UI akan selalu berotasi menghadap kamera._                              |
| `ui`               | `GameObject`           | Target UI that will be shown or hidden. / _UI target yang akan ditampilkan atau disembunyikan._                                                |
| `displaysUIInput`  | `InputActionReference` | VR button used to display and snap the UI to the front. / _Tombol VR yang digunakan untuk menampilkan UI._                                     |
| `hideUIInput`      | `InputActionReference` | VR button used to hide the UI. / _Tombol VR yang digunakan untuk menyembunyikan UI._                                                           |
| `recenterUIInput`  | `InputActionReference` | VR button to quickly snap UI in front of user's view. / _Tombol VR untuk memastikan UI berada tepat di depan pandangan pengguna._              |
