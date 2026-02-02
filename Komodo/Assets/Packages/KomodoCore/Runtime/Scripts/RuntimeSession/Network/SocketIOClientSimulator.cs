using UnityEngine;
using Komodo.Utilities;
using System.Collections.Generic;

namespace Komodo.Runtime
{
    public class SocketIOClientSimulator : SingletonComponent<SocketIOClientSimulator>
    {
        public static SocketIOClientSimulator Instance
        {
            get { return (SocketIOClientSimulator) _Instance; }
            set { _Instance = value; }
        }

        public bool isVerbose = false;

        public bool doLogClientEvents = true;

        public bool doLogCustomInteractions = true;

        public bool doLogPositionEvents = false;

        public bool setSocketIOAdapterNameFails;

        public bool openSyncConnectionFails;

        public bool openChatConnectionFails;

        public bool setSyncEventListenersFails;

        public bool setChatEventListenersFails;

        public bool joinSyncSessionFails;

        public bool joinChatSessionFails;

        public bool leaveSyncSessionFails;

        public bool leaveChatSessionFails;

        public bool sendStateCatchUpRequestFails;

        public bool enableVRButtonFails;

        public bool closeSyncConnectionFails;

        public bool closeChatConnectionFails;

        public bool doLogMessageEvents = false;

        public int clientId;

        public int sessionId;

        public int isTeacher;

        public string sessionDetails = @"{""assets"":[{""id"":111550,""name"":""GraceGremer"",""url"":""https://s3.us-east-2.amazonaws.com/vrcat-assets/8f5fef97-a735-4c2e-8d28-fc3badfe09a3/model.glb"",""isWholeObject"":true,""scale"":1},{""id"":111576,""name"":""GarmentSetup1"",""url"":""https://s3.us-east-2.amazonaws.com/vrcat-assets/1843c37c-dc40-4520-91cb-ad1cdc70d72e/model.glb"",""isWholeObject"":false,""scale"":1},{""id"":111577,""name"":""GarmentSetup2"",""url"":""https://s3.us-east-2.amazonaws.com/vrcat-assets/cb3464eb-f96e-48c6-b774-e4dfbdc8ab78/model.glb"",""isWholeObject"":false,""scale"":1},{""id"":111578,""name"":""GarmentSetup3"",""url"":""https://s3.us-east-2.amazonaws.com/vrcat-assets/e8567627-ade7-493e-a017-e8b1b61e71af/model.glb"",""isWholeObject"":false,""scale"":1},{""id"":111579,""name"":""GarmentSetup4"",""url"":""https://s3.us-east-2.amazonaws.com/vrcat-assets/9a133a69-c8ba-4cf8-9539-a9a2b2827226/model.glb"",""isWholeObject"":false,""scale"":1},{""id"":111580,""name"":""GarmentSetup5"",""url"":""https://s3.us-east-2.amazonaws.com/vrcat-assets/0529709a-803e-4f61-86d1-092dabf0c2cb/model.glb"",""isWholeObject"":false,""scale"":1},{""id"":111589,""name"":""ShanenHaigler"",""url"":""https://s3.us-east-2.amazonaws.com/vrcat-assets/7bb5d74d-39f3-4b3d-aa56-4708b62bda95/model.glb"",""isWholeObject"":true,""scale"":1},{""id"":111597,""name"":""Garment O'Donnell"",""url"":""https://s3.us-east-2.amazonaws.com/vrcat-assets/ee63a354-b427-4ca3-a2a0-6e230efabe55/model.glb"",""isWholeObject"":true,""scale"":1},{""id"":111602,""name"":""GraceGremer"",""url"":""https://s3.us-east-2.amazonaws.com/vrcat-assets/14bacabd-7f09-4120-b420-ccfa09b23e03/model.glb"",""isWholeObject"":true,""scale"":1},{""id"":111604,""name"":""CarleeIhde"",""url"":""https://s3.us-east-2.amazonaws.com/vrcat-assets/de622ac8-eabd-4d3a-ab8f-ab56ee415813/model.glb"",""isWholeObject"":true,""scale"":1},{""id"":111605,""name"":""CarleeI"",""url"":""https://s3.us-east-2.amazonaws.com/vrcat-assets/29394a64-0e31-4824-86f8-133297f9f84c/model.glb"",""isWholeObject"":true,""scale"":1},{""id"":111607,""name"":""G-SarahMiranda"",""url"":""https://s3.us-east-2.amazonaws.com/vrcat-assets/f3f1442c-f9cc-454b-8b32-8f3597d79272/model.glb"",""isWholeObject"":true,""scale"":1},{""id"":111608,""name"":""SarahMirandaMood"",""url"":""https://s3.us-east-2.amazonaws.com/vrcat-assets/0b5036d8-37f0-4c32-b65a-7030cef4718d/model.glb"",""isWholeObject"":true,""scale"":1},{""id"":111609,""name"":""G-ShanenHaigleer"",""url"":""https://s3.us-east-2.amazonaws.com/vrcat-assets/5539b18d-ed67-48cb-8a87-164fc161319a/model.glb"",""isWholeObject"":true,""scale"":1}],""app_and_build"":""/test/Brandon-develop-2021-10-19-15xx/"",""course_id"":3,""create_at"":""2021-03-26T01:00:58.000Z"",""description"":""(No description added)"",""end_time"":""2021-03-31T19:03:00.000Z"",""session_id"":141,""session_name"":""SP21 - Critique Group C"",""start_time"":""2021-03-31T18:03:00.000Z"",""users"":[]}";

        private float[] _arrayPointer;

        private int _relayUpdateSize;

        private int _posCursor;

        public string InstantiationManagerName = "InstantiationManager";

        public string NetworkManagerName = "NetworkManager";

        private ClientSpawnManager _ClientSpawnManager;

        private NetworkUpdateHandler _NetworkUpdateHandler;

        public void Awake()
        {
            //used to set our managers alive state to true to detect if it exist within scene
            var initManager = Instance;
        }

        public void Start()
        {
            var instMgr = GameObject.Find(InstantiationManagerName);
            if (!instMgr)
            {
                throw new System.Exception($"You must have a GameObject named {InstantiationManagerName} in your scene.");
            }
            _ClientSpawnManager = instMgr.GetComponent<ClientSpawnManager>();

            if (!_ClientSpawnManager)
            {
                throw new System.Exception($"{InstantiationManagerName} must have a ClientSpawnManager component.");
            }

            var netMgr = GameObject.Find(NetworkManagerName);
            if (!netMgr)
            {
                throw new System.Exception($"You must have a GameObject named {NetworkManagerName} in your scene.");
            }
            _NetworkUpdateHandler = netMgr.GetComponent<NetworkUpdateHandler>();

            if (!_NetworkUpdateHandler)
            {
                throw new System.Exception($"{NetworkManagerName} must have a NetworkUpdateHandler component.");
            }
        }

        private void DebugLog (string message) {
            Debug.Log($"[SocketSim] {message}");
        }

        public void GameInstanceSendMessage(string who, string what, string data)
        {
            if (isVerbose) DebugLog($"GameInstanceSendMessage({who}, {what}, {data})");
        }

        public void Emit(string name, string data)
        {
            if (isVerbose) DebugLog($"Emit({name}, {data})");
        }

        public void OnReceiveStateCatchUp(string jsonStringifiedData)
        {
            if (isVerbose) DebugLog($"received state sync event: {jsonStringifiedData}");

            Debug.LogError("Need to call SocketIOAdapter.OnReceiveStateCatchup(jsonStringifiedData); here");
        }

        public int SendStateCatchUpRequest()
        {
            if (isVerbose) DebugLog("SendStateCatchUpRequest");
            Emit("state", "{ session_id: session_id, client_id: client_id }");

            return sendStateCatchUpRequestFails ? SocketIOJSLib.FAILURE : SocketIOJSLib.SUCCESS;
        }

        public void OnJoined(int clientId)
        {
            if (doLogClientEvents) DebugLog($"OnJoined({clientId})");
            ClientSpawnManager.Instance.AddNewClient(clientId);
        }

        public void OnDisconnected(int clientId)
        {
            if (doLogClientEvents) DebugLog($"OnDisconnected({clientId})");
            ClientSpawnManager.Instance.RemoveClient(clientId);
        }

        public void OnMicText(string jsonStringifiedData)
        {
            DebugLog("OnMicText");
            _ClientSpawnManager.OnReceiveSpeechToTextSnippet(jsonStringifiedData);
        }

        public int SetChatEventListeners()
        {
            DebugLog("SetChatEventListeners");
            //todo(Brandon): call OnMicText with data

            return setChatEventListenersFails ? SocketIOJSLib.FAILURE : SocketIOJSLib.SUCCESS;
        }

        public void OnDraw(float[] data)
        {
            DebugLog($"OnDraw({data})");
        }

        public void InitReceiveDraw(float[] arrayPointer, int size)
        {
            DebugLog("InitReceiveDraw");
            // int drawCursor = 0;
            //todo(Brandon): call OnDraw with data and pass in drawCursor also
        }

        public void SendDraw(float[] arrayPointer, int size)
        {
            DebugLog("SendDraw");
            Emit("draw", arrayPointer.ToString());
        }

        public int GetClientIdFromBrowser()
        {
            if (doLogClientEvents) DebugLog("GetClientIdFromBrowser -- returning user-set value");
            return clientId;
        }

        public int GetSessionIdFromBrowser()
        {
            DebugLog("GetSessionIdFromBrowser -- returning user-set value");
            return sessionId;
        }

        public int GetIsTeacherFlagFromBrowser()
        {
            DebugLog("GetIsTeacherFlagFromBrowser -- returning user-set value");
            return isTeacher;
        }

        public void SocketIOSendPosition(float[] array, int size)
        {
            if (doLogPositionEvents) DebugLog("SocketIOSendPosition");
            Emit("update", array.ToString());
        }

        public void SocketIOSendInteraction(int[] array, int size)
        {
            if (doLogCustomInteractions) DebugLog($"SocketIOSendInteraction({array.ToString()}, {size})");
            Emit("interact", array.ToString());
        }

        /**
            InitSocketIOReceivePosition: function(arrayPointer, size) {
                if (window.sync) {
                    var posCursor = 0;

                    // NOTE(rob):
                    // we use "arrayPointer >> 2" to change the pointer location on the module heap
                    // when interpreted as float32 values ("HEAPF32[]"). 
                    // for example, an original arrayPointer value (which is a pointer to a 
                    // position on the module heap) of 400 would right shift to 100 
                    // which would be the correct corresponding index on the heap
                    // for elements of 32-bit size.

                    window.sync.on('relayUpdate', function(data) {
                        if (data.length + posCursor > size) {
                            posCursor = 0;
                        }
                        for (var i = 0; i < data.length; i++) {
                            HEAPF32[(arrayPointer >> 2) + posCursor + i] = data[i];
                        }
                        posCursor += data.length;
                    });
                }
            },
        */
        public void InitSocketIOReceivePosition(float[] arrayPointer, int size)
        {
            _arrayPointer = arrayPointer;
            
            DebugLog($"InitSocketIOReceivePosition({_arrayPointer}, {size})");

            _relayUpdateSize = size;
            //  var posCursor = 0;
            //todo(Brandon): call OnRelayUpdate, passing in data, and updating posCursor
        }

        /** 
         * See the body of InitSocketIOReceivePosition for the relayUpdate 
         * event listener.
         */
        public void RelayPositionUpdate(float[] data) 
        {

            if (doLogPositionEvents)
            {
                string dataString = string.Join(" ", data);

                DebugLog($"RelayUpdate({(dataString != "" ? dataString : "null")})");
            }

            if (data.Length + _posCursor > _relayUpdateSize) {
                _posCursor = 0;
            }

            for (var i = 0; i < data.Length; i++) {
                _arrayPointer[_posCursor + i] = data[i];
            }

            _posCursor += data.Length;
        }

        public void OnInteractionUpdate(float[] data)
        {
            if (doLogCustomInteractions) DebugLog($"OnInteractionUpdate({data.ToString()})");
        }

        public void InitSocketIOReceiveInteraction(int[] arrayPointer, int size)
        {
            DebugLog("InitSocketIOReceiveInteraction");
            // var intCursor = 0;
            //todo(Brandon): call OnInteractionUpdate, passing in data, and updating intCursor
        }

        public void ToggleCapture(int operation, int session_id)
        {
            if (operation == 0)
            {
                Emit("start_recording", session_id.ToString());
            }
            else
            {
                Emit("end_recording", session_id.ToString());
            }
        }
        /**
            GetSessionDetails: function() {
                if (window.details) {
                    var serializedDetails = JSON.stringify(window.details);
                    if (serializedDetails) {
                        var bufferSize = lengthBytesUTF8(serializedDetails) + 1;
                        var buffer = _malloc(bufferSize);
                        stringToUTF8(serializedDetails, buffer, bufferSize);
                        return buffer;
                    } else {
                        console.log("Unable to serialize details: " + window.details)
                        var bufferSize = lengthBytesUTF8("{}") + 1;
                        var buffer = _malloc(bufferSize);
                        stringToUTF8("", buffer, bufferSize);
                        return buffer;
                    }
                } else {
                    // var bufferSize = lengthBytesUTF8("{details:{}}") + 1;
                    // var buffer = _malloc(bufferSize);
                    // stringToUTF8("", buffer, bufferSize);
                    // return buffer;
                    return null;
                }
            },
        **/
        public static string GetSessionDetails ()
        {
            //TODO -- extend this with a public boolean to account for multiple code paths above.
            Instance.DebugLog($"GetSessionDetails()");

            return Instance.sessionDetails;
        }

        public void BrowserEmitMessage (string type, string message)
        {
            if (!doLogMessageEvents)
            {
                return;
            }

            DebugLog($"BrowserEmitMessage({type}, {message})");
        }

        public int CloseSyncConnection () {
            DebugLog("CloseSyncConnection");

            return closeSyncConnectionFails ? SocketIOJSLib.FAILURE : SocketIOJSLib.SUCCESS;
        }

        public int CloseChatConnection () {
            DebugLog("CloseChatConnection");

            return closeChatConnectionFails ? SocketIOJSLib.FAILURE : SocketIOJSLib.SUCCESS;
        }

        public int SetSyncEventListeners () {
            DebugLog("SetSyncEventListeners()");

            return setSyncEventListenersFails ? SocketIOJSLib.FAILURE : SocketIOJSLib.SUCCESS;
        }

        public int OpenSyncConnection() {
            DebugLog("OpenSyncConnection()");

            return openSyncConnectionFails ? SocketIOJSLib.FAILURE : SocketIOJSLib.SUCCESS;
        }

        public int OpenChatConnection() {
            DebugLog("OpenChatConnection()");

            return openChatConnectionFails ? SocketIOJSLib.FAILURE : SocketIOJSLib.SUCCESS;
        }

        public int EnableVRButton () {
            DebugLog("EnableVRButton()");

            return enableVRButtonFails ? SocketIOJSLib.FAILURE : SocketIOJSLib.SUCCESS;
        }

        public int JoinSyncSession () {
            DebugLog("JoinSyncSession()");

            return joinSyncSessionFails ? SocketIOJSLib.FAILURE : SocketIOJSLib.SUCCESS;
        }

        public int JoinChatSession () {
            DebugLog("JoinChatSession()");

            return joinChatSessionFails ? SocketIOJSLib.FAILURE : SocketIOJSLib.SUCCESS;
        }

        public int LeaveSyncSession () {
            DebugLog("LeaveSyncSession()");

            return leaveSyncSessionFails ? SocketIOJSLib.FAILURE : SocketIOJSLib.SUCCESS;
        }

        public int LeaveChatSession () {
            DebugLog("LeaveChatSession()");

            return leaveChatSessionFails ? SocketIOJSLib.FAILURE : SocketIOJSLib.SUCCESS;
        }

        public string SetSocketIOAdapterName (string name) {
            DebugLog($"window.socketIOAdapterName = {name}");

            if (setSocketIOAdapterNameFails)
            {
                return "INCORRECT_NAME";
            }
            return SocketIOAdapter.Instance.gameObject.name;
        }

        [ContextMenu("Ping Example")]
        public void PingExample () {
            SocketIOAdapter.Instance.OnPing();
        }

        [ContextMenu("Pong Example")]
        public void PongExample () {
            SocketIOAdapter.Instance.OnPong(56789);
        }
    }
}