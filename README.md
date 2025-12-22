# VR Spatial Adaptive UI Toolkit

**VR Spatial Adaptive UI Toolkit** is a comprehensive solution for Unity VR designed to bridge the gap between immersive 3D visuals and reliable user interaction. It enables developers to project dynamic UI onto complex 3D surfaces—such as curved screens—while maintaining precise input detection through a physics-based proxy system. Beyond interaction, the toolkit enhances user experience with intelligent positioning features, including smooth camera-follow behaviors and instant auto-focus capabilities, ensuring interfaces remain accessible and comfortable in a spatial environment.

## 🌟 Key Features

### 1. Advanced Render Texture & Mesh Support

Supports UI Canvas projection onto complex 3D meshes (e.g., curved displays created in Blender).

- **Architecture:** Utilizes _Screen Space - Camera_ render mode with a dedicated UI Camera outputting to a _Render Texture_.
- **Versatility:** Supports both script-generated setups and imported 3D mesh presets.

### 2. Spatial Input Mapping (Physical Proxy)

A physics-based interaction system that maps `BoxCollider` proxies to UI elements (Buttons, Toggles, Sliders, Scrollbars). This ensures precise raycast detection regardless of the visual mesh curvature or texture resolution.

**Slider & Scrollbar Orientation Guide:**
The system calculates values based on the Collider's **Local X-Axis**. To define the sliding direction, apply the following rotations to the Collider GameObject:

| Slide Direction             | Rotation (x, y, z) | Description         |
| :-------------------------- | :----------------: | :------------------ |
| **Left to Right** (Default) |    `(0, 0, 0)`     | Standard Horizontal |
| **Right to Left**           |   `(0, 180, 0)`    | Inverted Horizontal |
| **Bottom to Top**           |    `(0, 0, 90)`    | Standard Vertical   |
| **Top to Bottom**           |   `(0, 0, -90)`    | Inverted Vertical   |

> **Note:** Ensure the Collider's `Size X` matches the length of the UI track.

### 3. Smooth Adaptive Follow

Enables the UI to float and follow the user's headset movement with adjustable smoothing.

- Designed to reduce motion sickness by avoiding rigid locking to the camera.
- Follow speed and damping parameters are fully customizable.

### 4. Static Management & Auto-Focus

Provides robust control for UI elements when not in "Follow Mode":

- **Show/Hide System:** Easily toggle UI visibility.
- **Auto-Focus (Re-center):** Instantly snaps and orients the UI directly in front of the user's current view, ideal for summoning menus or alerts.
