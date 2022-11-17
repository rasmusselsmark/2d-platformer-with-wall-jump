/* Makes the camera follow the player
 * Attached the script to the camera
 * Add the player to follow in the 'player field' in the GUI
 */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollower : MonoBehaviour
{
    [SerializeField] Transform player;

    void Update()
    {
        transform.position = new Vector3(player.position.x, transform.position.y, transform.position.z);
        if (player.position.y > 5)
        {
            transform.position = new Vector3(transform.position.x, player.position.y - 5, transform.position.z);
        }
        if (player.position.y < -3)
        {
            transform.position = new Vector3(transform.position.x, player.position.y + 3, transform.position.z);
        }
    }
}

