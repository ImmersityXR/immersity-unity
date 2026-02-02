using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

namespace Komodo.Runtime
{
    public class UsernamesListController : MonoBehaviour
    {
        [SerializeField] private SessionDataTemplate sessionData;
        
        public Transform listParent;
        public GameObject usernameText;

        Dictionary<string, GameObject> userIDToUsernameText = new Dictionary<string, GameObject>();

        public void AddListItem(int userID)
        {
            string username = sessionData.GetPlayerNameFromClientID(userID);
            if (!userIDToUsernameText.ContainsKey(username))
            {
                var listItem = Instantiate(usernameText, listParent, false);
                userIDToUsernameText.Add(username, listItem);

                var listItemText = listItem.GetComponentInChildren<Text>(true);

                userIDToUsernameText[username] = listItem;

                listItemText.text = username;
            }
            else
                Debug.Log("CLIENT LABEL + " + username + " Already exist");
        }

        public void RemoveListItem(string userID)
        {
            RemoveListItemAsync(userID);
        }

        private async void RemoveListItemAsync(string userID)
        {
            if (userIDToUsernameText.ContainsKey(userID))
            {
                while (userIDToUsernameText[userID] == null)
                    await Task.Delay(1);

                Destroy(userIDToUsernameText[userID]);
                userIDToUsernameText.Remove(userID);

            }
            else
                Debug.Log("Client Does not exist");

        }

    }
}
