using Unity.VisualScripting;
using UnityEngine;



//The game Singleton. Here, you can store references to objects that need to be globally accessible
public class Game : MonoBehaviour
{
    public static Game I { get; private set; }
    private void Awake()
    {
        // If there is an instance, and it's not me, delete myself.

        if (I != null && I != this)
        {
            Destroy(this);
        }
        else
        {
            I = this;
        }
    }
}
