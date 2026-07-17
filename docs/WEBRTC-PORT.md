# WebRTC Voice/Video Port — Integration Guide

This documents the WebRTC peer-to-peer voice/video/screen-share layer ported
from David Tamayo's KomodoSandbox prototype, and what remains to wire it up
in the Unity editor.

**Provenance:**
- Client: https://github.com/davtamay/KomodoSandbox (`webrtc.js`,
  `webrtcUnity.jslib`, `WebRTCVideoTexture.cs`, Jan–Apr 2024)
- Original signaling server: https://github.com/davtamay/RelayTesting
- The signaling server has been re-ported into the relay
  (`services/relay/` in
  [immersityxr-core](https://github.com/ImmersityXR/immersityxr-core)) as a
  dedicated `/rtc` namespace (`rtc.js`), Socket.IO 2.x, per-session rooms,
  shared-secret auth — see the relay README there.

## What is already in this repo

| File | Role |
|---|---|
| `Komodo/Assets/WebGLTemplates/KomodoWebXRFullView2020/webrtc.js` (+ `Hidden~` copy) | Mesh WebRTC: signaling client, offer/answer/ICE, mic mute, camera toggle, screen share, device switching, video-element pooling |
| `.../KomodoWebXRFullView2020/index.html` (+ `Hidden~` copy) | Hidden DOM elements webrtc.js manages (local video, debug buttons, device selects) |
| `Komodo/Assets/Packages/KomodoCore/KomodoCoreAssets/Plugins/jslib/webrtcUnity.jslib` | Unity → JS call control + per-peer video→canvas→`texImage2D` texture painting |
| `Komodo/Assets/Packages/KomodoCore/Runtime/Scripts/RuntimeSession/Network/WebRTCVideoTexture.cs` | Unity-side texture lifecycle: allocates pooled `Texture2D`s for peers, assigns to `RawImage`s |

How it connects: `webrtc.js` joins `RELAY_BASE_URL + '/rtc'` with query
parameters `userName`, `client_id`, `session_id` (from the page URL), and
`auth` (the page's `?auth=` shared secret, when the relay enforces one).
STUN/TURN servers arrive from the relay via the `rtcConfig` event — nothing
is baked into builds.

## The contract

### Unity → JS (DllImport from `webrtcUnity.jslib`)

```csharp
[DllImport("__Internal")] static extern void ConnectToWebRTC(string clientName);
[DllImport("__Internal")] static extern void CallClient(string userName);
[DllImport("__Internal")] static extern void AnswerClientOffer(string userName);
[DllImport("__Internal")] static extern void HangUpClient();
[DllImport("__Internal")] static extern void ShareScreen(int enabled);
[DllImport("__Internal")] static extern void SetMicrophone();   // toggle
[DllImport("__Internal")] static extern void SetVideo();        // toggle
[DllImport("__Internal")] static extern void ChangeVideoDevice(string deviceId);
```

### JS → Unity (SendMessage)

To the GameObject registered as `window.socketIOAdapterName` (the existing
`SocketIOAdapter` object — same mechanism `socket-funcs.jslib` already uses):

| Method to implement | Payload | Meaning |
|---|---|---|
| `ReceiveClientCall(int clientId)` | caller's client id | incoming call offer |
| `ReceiveClientCallAndAnswer(int clientId)` | caller's client id | offer that should be auto-answered (multi-peer join) |
| `ReceiveClientAnswer(int clientId)` | answerer's client id | our offer was answered |
| `ReceiveCallEnded(int clientId)` | leaver's client id | a peer left the call |
| `ReceiveCallRejected(int clientId)` | answerer's client id | our offer was declined |
| `ReceiveEmptyRoom()` | — | call is over (≤1 participant left) |
| `CallFailed(string userName)` | target name | offer could not be delivered |

To a GameObject named `WebRTCVideo`:

| Method | Payload | Meaning |
|---|---|---|
| `ReceiveDimensions(string)` | `"name,width,height"` | a peer's video dimensions are known; size the RawImage |

## What remains (in-editor work)

1. **Adapter methods** — add the seven receiver methods above to
   `SocketIOAdapter` (or a small dedicated component on the same
   GameObject) and route them to your call UI.
2. **Call UI** — accept/reject panel, in-call roster, mute/camera/screen
   buttons that invoke the DllImport functions. David's
   `ShareMediaConnection.cs`, `WebRTCReceiveCallReferences.cs`,
   `WebRTCVideoReferences.cs`, `MinMaxSharedVideo.cs` and
   `WebRTCVideo.prefab` in KomodoSandbox are the reference implementation —
   they were **not** ported because they depend on his Unity 2023 / XR
   Interaction Toolkit 3 UI stack and third-party audio libs; rebuild the
   equivalent against this project's UI instead.
3. **A `WebRTCVideo` GameObject** in the scene carrying
   `WebRTCVideoTexture` with pooled `RawImage`s for peer video tiles.
4. **Relay-side setup** — deploy a relay built with the `/rtc` namespace
   and, for off-network participants, configure a TURN server in the relay
   config (`rtc.iceServers`).

## Known characteristics (from the prototype)

- Mesh topology: every peer streams to every other peer. Tiles are capped
  at 256×192 for this reason; expect ~4–6 video participants max.
- `webxr.js` (already in this template lineage) must keep the page's
  `requestAnimationFrame` running during WebXR sessions or video textures
  freeze in VR — David's fork hijacks RAF for this; verify behavior when
  testing in-headset.
- Auto-answer for multi-peer joins is driven by the
  `newOfferAwaiting2`/`makeClientSendOffer` flow; first call is manual
  accept.
