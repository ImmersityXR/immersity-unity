using Komodo.Utilities;
using UnityEngine;

namespace Komodo.Runtime
{
    public class SocketIOServerSimulator : MonoBehaviour {

        [ShowOnly]
        public string hint = "Use the context menu on this component.";

        private int _numIncrementalClients = 0;

        private int _incrementalClientsStartIndex = 1;

        private int _GetTopIncrementalClientId() {
            return _numIncrementalClients + _incrementalClientsStartIndex - 1;
        }

        /** Test client spawning in the editor. **/
        [ContextMenu("Add Incremental Client")]
        public void AddIncrementalClient() {
            SocketIOAdapter.Instance.OnClientJoined(_GetTopIncrementalClientId() + 1);
            _numIncrementalClients += 1;
        }

        /** Test client spawning in the editor. **/
        [ContextMenu("Remove Incremental Client")]
        public void RemoveIncrementalClient() {
            SocketIOAdapter.Instance.OnOtherClientLeft(_GetTopIncrementalClientId());
            _numIncrementalClients -= 1;
        }

        private Position GeneratePosition(Entity_Type entityType, Vector3 position, Quaternion rotation) 
        {
            int clientID = _GetTopIncrementalClientId();

            Position result = new Position
            {
                clientId = clientID,
                entityId = MainClientUpdater.Instance.ComputeEntityID(clientID, entityType),
                entityType = MainClientUpdater.Instance.ComputeEntityType(entityType),
                scaleFactor = MainClientUpdater.Instance.ComputeScaleFactor(entityType),
                rot = rotation,
                pos = position,
            };

            //Debug.Log($"new Position({result.clientId}, {result.entityId}, {result.entityType}, {result.scaleFactor}, {result.rot}, {result.pos})");

            return result;
        }

        /** Test receiving updates in the editor. 
         *  This function acts as if the data is already unpacked from 
         *  raw data. 
         **/
        [ContextMenu("Receive Position Update For Head")]
        public void ReceiveCenterPositionUpdateForHead() {

            if (_GetTopIncrementalClientId() == 0) {
                AddIncrementalClient();
            }

            Position position = GeneratePosition(Entity_Type.users_head, Vector3.zero, Quaternion.AngleAxis(180f, Vector3.forward));

            ClientSpawnManager.Instance.ApplyPositionToHead(position);
        }
        
        //TODO(Brandon): Suggestion: rename this to PositionUpdate
        /**
         * Based on NetworkUpdateHandler > NetworkUpdate, but instead of 
         * sending the update out, it causes this code client to "receive" 
         * a relay update (which will apply to the top incremental user client).
         */
        private void SendSyncPoseMessage(Position pos) 
        {
            float[] arr_pos = NetworkUpdateHandler.Instance.SerializeCoordsStruct(pos);
#if UNITY_WEBGL && !UNITY_EDITOR
    //do nothing, so the compiler doesn't complain
#else
            SocketIOClientSimulator.Instance.RelayPositionUpdate(arr_pos);
#endif

        }
        
        /** Test receiving updates in the editor. 
         *  This function acts as if the data is already unpacked from 
         *  raw data. 
         **/
        [ContextMenu("Packed Position Update For Head")]
        public void PackedCenterPositionUpdateForHead() {

            if (_GetTopIncrementalClientId() == 0) {
                AddIncrementalClient();
            }

            Position position = GeneratePosition(Entity_Type.users_head, Vector3.zero, Quaternion.AngleAxis(180f, Vector3.forward));

            SendSyncPoseMessage(position);
        }

        /** Test receiving updates in the editor. 
         *  This function acts as if the data is already unpacked from 
         *  raw data. 
         **/
        [ContextMenu("Receive Position Update For Left Hand")]
        public void ReceiveCenterPositionUpdateForLeftHand() {

            if (_GetTopIncrementalClientId() == 0) {
                AddIncrementalClient();
            }

            Position position = GeneratePosition(Entity_Type.users_Lhand, Vector3.one, Quaternion.AngleAxis(180f, Vector3.forward));

            ClientSpawnManager.Instance.ApplyPositionToLeftHand(position);
        }

        /** Test receiving updates in the editor. 
         *  This function acts as if the data is already unpacked from 
         *  raw data. 
         **/
        [ContextMenu("Receive Position Update For Right Hand")]
        public void ReceiveCenterPositionUpdateForRightHand() {

            if (_GetTopIncrementalClientId() == 0) {
                AddIncrementalClient();
            }

            Position position = GeneratePosition(Entity_Type.users_Rhand, Vector3.one + Vector3.one, Quaternion.AngleAxis(180f, Vector3.forward));

            ClientSpawnManager.Instance.ApplyPositionToRightHand(position);
        }

        /** Test sending updates in the editor **/

        [ContextMenu("SendEmptyStateCatchUp")]
        public void SendEmptyStateCatchUp() {
            var socketIOAdapter = (SocketIOAdapter) FindObjectOfType(typeof(SocketIOAdapter));

            socketIOAdapter.OnReceiveStateCatchup("{\"clients\": [99, 98], \"entities\": [], \"isRecording\": false,\"scene\": null}");
        }

        //{"assets":[{"id":111420,"name":"Dragon whole","url":"https://s3.us-east-2.amazonaws.com/vrcat-assets/9bc7be11-8784-44a5-a621-0705f0e8e5dc/model.glb","isWholeObject":true,"scale":1},{"id":111452,"name":"Miller Index Planes","url":"https://s3.us-east-2.amazonaws.com/vrcat-assets/feabc4e3-1cdf-4663-b1c7-c63efe677a56/model.glb","isWholeObject":false,"scale":1},{"id":111470,"name":"Sheer Dress","url":"https://s3.us-east-2.amazonaws.com/vrcat-assets/b2dee1ca-a203-4e49-841d-fd81ce53eb1d/model.glb","isWholeObject":true,"scale":1},{"id":111478,"name":"TiltBrush BrushTests","url":"https://s3.us-east-2.amazonaws.com/vrcat-assets/fe562af4-e660-454c-b9e5-b6c57086cc12/model.glb","isWholeObject":true,"scale":1}],"build":"base/stable","course_id":1,"create_at":"2020-11-13T20:19:54.000Z","description":" This is a the demo session for our talk with TCNJ. ","end_time":"2020-11-13T19:11:00.000Z","session_id":126,"session_name":"TCNJ Demo","start_time":"2020-11-13T18:11:00.000Z","users":[{"student_id":1,"email":"admin@komodo.edu","first_name":"Admin","last_name":"Komodo"},{"student_id":2,"email":"first1@illinois.edu","first_name":"First1","last_name":"Last1"},{"student_id":5,"email":"first2@illinois.edu","first_name":"First2","last_name":"Last2"},{"student_id":10,"email":"dtamay3@illinois.edu","first_name":"First3","last_name":"Last3"},{"student_id":14,"email":"first3@illinois.edu","first_name":"Alex","last_name":"Cabada"},{"student_id":26,"email":"demo1@illinois.edu","first_name":"First4","last_name":"First5"},{"student_id":27,"email":"first5@illinois.edu","first_name":"First5","last_name":"Last5"},{"student_id":28,"email":"demo3@illinois.edu","first_name":"Demo","last_name":"3"},{"student_id":29,"email":"demo4@illinois.edu","first_name":"Demo","last_name":"4"},{"student_id":30,"email":"demo5@illinois.edu","first_name":"Demo","last_name":"5"},{"student_id":31,"email":"demo6@illinois.edu","first_name":"Demo","last_name":"6"}]}

        [ContextMenu("SendExampleStateCatchUp")]
        public void SendExampleStateCatchUp()
        {
            var socketIOAdapter = (SocketIOAdapter) FindObjectOfType(typeof(SocketIOAdapter));

            int id0 = NetworkedObjectsManager.Instance.GenerateEntityIDBase() + 0; //111420; // 
            int id1 = NetworkedObjectsManager.Instance.GenerateEntityIDBase() + 1; //111452; //
            int id2 = NetworkedObjectsManager.Instance.GenerateEntityIDBase() + 2; //111470; //
            int id3 = NetworkedObjectsManager.Instance.GenerateEntityIDBase() + 3; //111478; //

            Position pos0 = new Position(-1, id0, (int) Entity_Type.objects, 1, new Quaternion(), new Vector3(0.5f, 0.5f, 0.5f));
            Position pos1 = new Position(-1, id1, (int) Entity_Type.objects, 1, new Quaternion(), new Vector3(1.0f, 1.0f, 1.0f));
            Position pos2 = new Position(-1, id2, (int) Entity_Type.objects, 1, new Quaternion(), new Vector3(1.5f, 1.5f, 1.5f));
            Position pos3 = new Position(-1, id3, (int) Entity_Type.objects, 1, new Quaternion(), new Vector3(2.0f, 2.0f, 2.0f));

            string latest0 = JsonUtility.ToJson(pos0);
            string latest1 = JsonUtility.ToJson(pos1);
            string latest2 = JsonUtility.ToJson(pos2);
            string latest3 = JsonUtility.ToJson(pos3);

            string stateString = "{\"clients\": [99, 98, 97, 96, 95, 94], \"entities\": [ {\"id\":" + id0 + ",\"latest\": " + latest0 + ",\"render\":true,\"locked\":true}, {\"id\":" + id1 + ",\"latest\": " + latest1 + ",\"render\":true,\"locked\":true}, {\"id\":" + id2 + ",\"latest\": " + latest2 + ",\"render\":true,\"locked\":true}, {\"id\":" + id3 + ",\"latest\": " + latest3 + ",\"render\":true,\"locked\":true}], \"isRecording\": false, \"scene\": null}";

            Debug.Log(stateString);

            socketIOAdapter.OnReceiveStateCatchup(stateString);
        }


    }
}